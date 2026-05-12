using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Security.Claims;
using System.Web;
using System.Web.UI;

public partial class InvoiceAutoReminder : Page
{
    private const string AuthCookieName = "SCE066_TOKEN";
    private const int DefaultDays = 3;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ClearMessage();
            int days = GetDaysFromRequest();
            txtDays.Text = days.ToString(CultureInfo.InvariantCulture);
            RunReminder(days);
        }
    }

    protected void btnRun_Click(object sender, EventArgs e)
    {
        int days;
        if (!TryParseDays(txtDays.Text, out days))
        {
            ShowError("Days ต้องเป็นตัวเลขมากกว่า 0");
            return;
        }

        Response.Redirect("~/InvoiceAutoReminder.aspx?DAYS=" + days.ToString(CultureInfo.InvariantCulture), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void RunReminder(int days)
    {
        DataTable dt = GetOverdueInvoices(days);
        if (dt == null)
        {
            RegisterCloseScript();
            return;
        }

        gvInvoice.DataSource = dt;
        gvInvoice.DataBind();

        if (dt.Rows.Count == 0)
        {
            ShowSuccess("ไม่พบ Invoice ที่ค้างส่ง Email เกิน " + days.ToString(CultureInfo.InvariantCulture) + " วัน");
            RegisterCloseScript();
            return;
        }

        EmailSender service = new EmailSender();
        string errorMessage;
        int sentCount;
        bool success = service.SendOverdueInvoiceReminder(dt, days, GetCurrentPersonCode(), out errorMessage, out sentCount);
        if (!success)
        {
            ShowError("ส่ง Email Reminder ไม่สำเร็จ : " + errorMessage);
            RegisterCloseScript();
            return;
        }

        ShowSuccess("ส่ง Email Reminder สำเร็จ จำนวน " + sentCount.ToString(CultureInfo.InvariantCulture) + " รายการ");
        RegisterCloseScript();
    }

    private DataTable GetOverdueInvoices(int days)
    {
        DateTime thresholdDate = DateTime.Today.AddDays(days * -1);
        int thresholdDbDate = BuildDbDateValue(thresholdDate.Year, thresholdDate.Month, thresholdDate.Day);

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT shivno,
                   trim(shcuno) AS shcuno,
                   coalesce(okcunm, '') AS customer_name,
                   trim(shcuno) || '-' || coalesce(okcunm, '') AS customer_display,
                   invdoc.invdate AS invoice_upload_date,
                   coalesce(shmlct, 0) AS shmlct
              FROM itprod.shdoch
              INNER JOIN (
                    SELECT slcono, sldivi, slivno, max(sldate) AS invdate
                      FROM itprod.shdocl
                     WHERE sldatu = '8'
                     GROUP BY slcono, sldivi, slivno
              ) invdoc
                ON invdoc.slcono = shcono
               AND invdoc.sldivi = shdivi
               AND invdoc.slivno = shivno
              LEFT JOIN mvxcdtprod.ocusma
                ON okcono = shcono
               AND okcuno = shcuno
             WHERE shcono = 100
               AND shdivi = 'PFT'
               AND invdoc.invdate <= @THRESHOLD_DATE
               AND coalesce(shmlct, 0) = 0
               AND EXISTS (
                    SELECT 1
                      FROM ITPROD.SHCUMAILD mail
                     WHERE trim(mail.CUSTOMER_CODE) = trim(shcuno)
                       AND trim(mail.RECIPIENT_TYPE) = 'TO'
                       AND coalesce(trim(mail.EMAIL_ADDRESS), '') <> ''
                       AND trim(mail.ACTIVE_STATUS) = 'Y'
               )
             ORDER BY invdoc.invdate, shivno";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@THRESHOLD_DATE", thresholdDbDate.ToString(CultureInfo.InvariantCulture));

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return null;
        }

        AddReminderColumns(dt);
        return dt;
    }

    private void AddReminderColumns(DataTable dt)
    {
        if (!dt.Columns.Contains("INVOICE_UPLOAD_DATE_DISPLAY"))
        {
            dt.Columns.Add("INVOICE_UPLOAD_DATE_DISPLAY", typeof(string));
        }

        if (!dt.Columns.Contains("OVERDUE_DAYS"))
        {
            dt.Columns.Add("OVERDUE_DAYS", typeof(int));
        }

        if (!dt.Columns.Contains("MANAGE_URL"))
        {
            dt.Columns.Add("MANAGE_URL", typeof(string));
        }

        foreach (DataRow row in dt.Rows)
        {
            int uploadDateValue = 0;
            int.TryParse(Convert.ToString(row["INVOICE_UPLOAD_DATE"]).Trim(), out uploadDateValue);
            DateTime uploadDate;
            if (TryParseDbDate(uploadDateValue, out uploadDate))
            {
                row["INVOICE_UPLOAD_DATE_DISPLAY"] = uploadDate.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                row["OVERDUE_DAYS"] = (DateTime.Today - uploadDate.Date).Days;
            }
            else
            {
                row["INVOICE_UPLOAD_DATE_DISPLAY"] = Convert.ToString(row["INVOICE_UPLOAD_DATE"]).Trim();
                row["OVERDUE_DAYS"] = 0;
            }

            string invoiceNo = Convert.ToString(row["SHIVNO"]).Trim();
            row["MANAGE_URL"] = host.BuildUrl("~/CustomerEmailSend.aspx?INVNO=" + HttpUtility.UrlEncode(invoiceNo));
        }
    }

    private int GetDaysFromRequest()
    {
        int days;
        if (TryParseDays(Request.QueryString["DAYS"], out days))
        {
            return days;
        }

        return DefaultDays;
    }

    private bool TryParseDays(string value, out int days)
    {
        days = 0;
        if (!int.TryParse((value ?? "").Trim(), out days))
        {
            return false;
        }

        return days > 0;
    }

    private int BuildDbDateValue(int year, int month, int day)
    {
        if (year > 2400)
        {
            year -= 543;
        }

        return (year * 10000) + (month * 100) + day;
    }

    private bool TryParseDbDate(int dateValue, out DateTime date)
    {
        date = DateTime.MinValue;
        int year = dateValue / 10000;
        int month = (dateValue % 10000) / 100;
        int day = dateValue % 100;

        if (year > 2400)
        {
            year -= 543;
        }

        if (year <= 0 || month <= 0 || day <= 0)
        {
            return false;
        }

        try
        {
            date = new DateTime(year, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GetCurrentPersonCode()
    {
        HttpCookie cookie = Request.Cookies[AuthCookieName];
        if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
        {
            return "";
        }

        ClaimsPrincipal principal = JwtHelper.ValidateToken(cookie.Value);
        if (principal == null)
        {
            return "";
        }

        Claim claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return claim == null ? "" : claim.Value;
    }

    private void ClearMessage()
    {
        lbError.Text = "";
        lbError.Visible = false;
        lbSuccess.Text = "";
        lbSuccess.Visible = false;
        litCloseScript.Text = "";
    }

    private void ShowError(string message)
    {
        lbError.Text = message;
        lbError.Visible = true;
        lbSuccess.Text = "";
        lbSuccess.Visible = false;
    }

    private void ShowSuccess(string message)
    {
        lbSuccess.Text = message;
        lbSuccess.Visible = true;
        lbError.Text = "";
        lbError.Visible = false;
    }

    private void RegisterCloseScript()
    {
        litCloseScript.Text = @"
<script type=""text/javascript"">
    setTimeout(function () {
        var stopAt = new Date().getTime() + 10000;
        var timer = setInterval(function () {
            try {
                window.open('', '_self');
            } catch (e) {
            }

            try {
                window.close();
            } catch (e) {
            }

            try {
                window.top.close();
            } catch (e) {
            }

            if (new Date().getTime() >= stopAt) {
                clearInterval(timer);
            }
        }, 500);
    }, 10000);
</script>";
    }
}
