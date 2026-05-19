using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CustomerEmailSend : Page
{
    private const string AuthCookieName = "SCE066_TOKEN";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PermissionManager.RedirectIfNoPermission(this, PermissionManager.PageCodes.CustomerEmail))
        {
            return;
        }

        if (!Page.IsPostBack)
        {
            ClearMessage();
            if (!string.IsNullOrWhiteSpace(Request.QueryString["VIEWINV"]))
            {
                OpenInvoiceFile(Request.QueryString["VIEWINV"]);
                return;
            }

            txtInvoiceNo.Text = (Request.QueryString["INVNO"] ?? "").Trim();
            SetDefaultDateRange();
            ddlStatus.SelectedValue = "PENDING";
            BindInvoiceList();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ClearMessage();
        BindInvoiceList();
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        ClearMessage();
        txtInvoiceNo.Text = "";
        txtCustomerCode.Text = "";
        SetDefaultDateRange();
        ddlStatus.SelectedValue = "PENDING";
        chkHasCustomerEmail.Checked = true;
        BindInvoiceList();
    }

    private void SetDefaultDateRange()
    {
        DateTime today = DateTime.Today;
        DateTime firstDay = new DateTime(today.Year, today.Month, 1);
        DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

        txtDateFrom.Text = firstDay.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
        txtDateTo.Text = lastDay.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
    }

    protected void gvInvoice_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ClearMessage();

        if (e.CommandName != "SendCustomerMail")
        {
            return;
        }

        if (!CanSendCustomerMail())
        {
            ShowError("You do not have permission to send customer email.");
            return;
        }

        int rowIndex;
        if (!int.TryParse(Convert.ToString(e.CommandArgument), out rowIndex) || rowIndex < 0 || rowIndex >= gvInvoice.DataKeys.Count)
        {
            ShowError("แถวข้อมูลที่เลือกไม่ถูกต้อง");
            return;
        }

        DataKey key = gvInvoice.DataKeys[rowIndex];
        string invoiceNo = Convert.ToString(key.Values["SHIVNO"]).Trim();
        string customerCode = Convert.ToString(key.Values["SHCUNO"]).Trim();
        string customerName = Convert.ToString(key.Values["CUSTOMER_NAME"]).Trim();
        string sentUser = GetCurrentPersonCode();

        EmailSender service = new EmailSender();
        string errorMessage;
        bool success = service.SendCustomerInvoice(invoiceNo, customerCode, customerName, sentUser, out errorMessage);

        if (!success)
        {
            ShowError("ส่ง Email ลูกค้าไม่สำเร็จ : " + errorMessage);
            BindInvoiceList();
            return;
        }

        ShowSuccess("ส่ง Email ลูกค้าสำเร็จ Invoice No: " + invoiceNo);
        BindInvoiceList();
    }

    protected void gvInvoice_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow)
        {
            return;
        }

        Button btnSendMail = e.Row.FindControl("btnSendMail") as Button;
        if (btnSendMail == null)
        {
            return;
        }

        if (!CanSendCustomerMail())
        {
            btnSendMail.Visible = false;
            return;
        }

        object countValue = DataBinder.Eval(e.Row.DataItem, "SHMLCT");
        int sendCount = 0;
        int.TryParse(Convert.ToString(countValue), out sendCount);

        if (sendCount > 0)
        {
            btnSendMail.OnClientClick = "return confirm('Invoice นี้เคยส่ง Email ลูกค้าแล้ว ต้องการส่งซ้ำใช่หรือไม่?');";
        }
    }

    protected string GetInvoiceFileUrl(object invoiceNo)
    {
        return "~/CustomerEmailSend.aspx?VIEWINV=" + HttpUtility.UrlEncode(Convert.ToString(invoiceNo).Trim());
    }

    private void OpenInvoiceFile(string invoiceNo)
    {
        invoiceNo = (invoiceNo ?? "").Trim();
        if (string.IsNullOrEmpty(invoiceNo))
        {
            ShowError("ไม่พบ Invoice No สำหรับเปิดไฟล์");
            return;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT SLFNAM, SLFEXT
              FROM ITPROD.SHDOCL
             WHERE SLCONO = 100
               AND SLDIVI = 'PFT'
               AND SLIVNO = @SLIVNO
               AND SLDATU = '8'
               AND SLREVI = (
                    SELECT MAX(SLREVI)
                      FROM ITPROD.SHDOCL
                     WHERE SLCONO = 100
                       AND SLDIVI = 'PFT'
                       AND SLIVNO = @SLIVNO_MAX
                       AND SLDATU = '8'
               )";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SLIVNO", invoiceNo);
        param.Add("@SLIVNO_MAX", invoiceNo);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (dt.Rows.Count == 0)
        {
            ShowError("ไม่พบไฟล์ INV สำหรับ Invoice " + invoiceNo);
            return;
        }

        string fileLocation = Convert.ToString(dt.Rows[0]["SLFNAM"]).Trim();
        string fileExt = Convert.ToString(dt.Rows[0]["SLFEXT"]).Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(fileLocation))
        {
            ShowError("ไม่พบ path ไฟล์ INV สำหรับ Invoice " + invoiceNo);
            return;
        }

        try
        {
            WebClient client = new WebClient();
            byte[] fileBuffer = client.DownloadData(fileLocation);
            if (fileBuffer == null)
            {
                ShowError("ไม่สามารถอ่านไฟล์ INV สำหรับ Invoice " + invoiceNo);
                return;
            }

            Response.Clear();
            Response.ContentType = GetContentType(fileExt);
            Response.AppendHeader("content-disposition", "inline; filename=Invoice" + fileExt);
            Response.BinaryWrite(fileBuffer);
            Response.End();
        }
        catch (Exception ex)
        {
            ShowError("เปิดไฟล์ INV ไม่สำเร็จ : " + ex.Message);
        }
    }

    private string GetContentType(string fileExt)
    {
        if (fileExt == ".pdf")
        {
            return "application/pdf";
        }
        if (fileExt == ".xls")
        {
            return "application/vnd.ms-excel";
        }
        if (fileExt == ".xlsx")
        {
            return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
        if (fileExt == ".doc" || fileExt == ".docx")
        {
            return "application/ms-word";
        }

        return "application/octet-stream";
    }

    private void BindInvoiceList()
    {
        dbConnect db = new dbConnect();
        Dictionary<string, object> param = new Dictionary<string, object>();

        string sql = @"
            SELECT shivno,
                   trim(shcuno) AS shcuno,
                   coalesce(okcunm, '') AS customer_name,
                   trim(shcuno) || '-' || coalesce(okcunm, '') AS customer_display,
                   rtrim(char(invdoc.invdate)) AS invoice_upload_date,
                   coalesce(shmlst, '') AS shmlst,
                   shmlts,
                   coalesce(shmlus, '') AS shmlus,
                   coalesce(shmlct, 0) AS shmlct,
                   CASE WHEN coalesce(shmlct, 0) > 0 THEN 'Sent' ELSE 'Pending' END AS mail_status_text
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
               AND shdivi = 'PFT'";

        string invoiceNo = txtInvoiceNo.Text.Trim();
        if (!string.IsNullOrEmpty(invoiceNo))
        {
            sql += " AND shivno = @SHIVNO";
            param.Add("@SHIVNO", invoiceNo);
        }

        string customerCode = txtCustomerCode.Text.Trim().ToUpper();
        if (!string.IsNullOrEmpty(customerCode))
        {
            sql += " AND shcuno = @SHCUNO";
            param.Add("@SHCUNO", customerCode);
        }

        if (chkHasCustomerEmail.Checked)
        {
            sql += @"
               AND EXISTS (
                    SELECT 1
                     FROM ITPROD.SHCUMAILD mail
                     WHERE trim(mail.CUSTOMER_CODE) = trim(shcuno)
                       AND trim(mail.RECIPIENT_TYPE) = 'TO'
                       AND coalesce(trim(mail.SENDER_EMAIL), '') <> ''
                       AND mail.ACTIVE_STATUS = 'Y'
               )";
        }

        int dateFrom;
        if (!TryParseOptionalDateValue(txtDateFrom.Text, out dateFrom))
        {
            return;
        }
        if (dateFrom > 0)
        {
            sql += " AND invdoc.invdate >= @DATEFROM";
            param.Add("@DATEFROM", dateFrom.ToString(CultureInfo.InvariantCulture));
        }

        int dateTo;
        if (!TryParseOptionalDateValue(txtDateTo.Text, out dateTo))
        {
            return;
        }
        if (dateTo > 0)
        {
            sql += " AND invdoc.invdate <= @DATETO";
            param.Add("@DATETO", dateTo.ToString(CultureInfo.InvariantCulture));
        }

        if (ddlStatus.SelectedValue == "PENDING")
        {
            sql += " AND coalesce(shmlct, 0) = 0";
        }
        else if (ddlStatus.SelectedValue == "SENT")
        {
            sql += " AND coalesce(shmlct, 0) > 0";
        }

        sql += " ORDER BY invdoc.invdate DESC, shivno";

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        AddLastUserDisplay(dt);
        gvInvoice.Columns[8].Visible = CanSendCustomerMail();
        gvInvoice.DataSource = dt;
        gvInvoice.DataBind();
    }

    private bool CanSendCustomerMail()
    {
        return PermissionManager.GetPermissionGroup(GetCurrentPersonCode()) == "SSS";
    }

    private void AddLastUserDisplay(DataTable dt)
    {
        if (!dt.Columns.Contains("SHMLUS_DISPLAY"))
        {
            dt.Columns.Add("SHMLUS_DISPLAY", typeof(string));
        }

        Dictionary<string, string> displayCache = new Dictionary<string, string>();

        foreach (DataRow row in dt.Rows)
        {
            string personCode = Convert.ToString(row["SHMLUS"]).Trim();
            if (string.IsNullOrEmpty(personCode))
            {
                row["SHMLUS_DISPLAY"] = "";
                continue;
            }

            if (!displayCache.ContainsKey(personCode))
            {
                displayCache[personCode] = GetUserDisplayText(personCode);
            }

            row["SHMLUS_DISPLAY"] = displayCache[personCode];
        }
    }

    private string GetUserDisplayText(string personCode)
    {
        personCode = (personCode ?? "").Trim();
        if (string.IsNullOrEmpty(personCode))
        {
            return "";
        }

        string escapedPersonCode = personCode.Replace("'", "''");
        string query = string.Format(@"
            SELECT TOP 1
                   ISNULL(FnameT, '') AS FnameT,
                   ISNULL(LnameT, '') AS LnameT
              FROM [HRIS].[dbo].[PersonDetail]
             WHERE RTRIM(LTRIM(PersonCode)) = '{0}'", escapedPersonCode);

        dbConnectSQL db = new dbConnectSQL(7);
        DataTable dt = db.ExecuteQuery(query);
        if (db.isError || dt.Rows.Count == 0)
        {
            return personCode;
        }

        string firstName = Convert.ToString(dt.Rows[0]["FnameT"]).Trim();
        string lastName = Convert.ToString(dt.Rows[0]["LnameT"]).Trim();
        string fullName = (firstName + " " + lastName).Trim();

        if (string.IsNullOrEmpty(fullName))
        {
            return personCode;
        }

        return fullName + " (" + personCode + ")";
    }

    private bool TryParseOptionalDateValue(string value, out int dateValue)
    {
        dateValue = 0;
        value = (value ?? "").Trim();

        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        DateTime parsedDate;
        string[] formats = new string[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy" };
        if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            dateValue = BuildDbDateValue(parsedDate.Year, parsedDate.Month, parsedDate.Day);
            return true;
        }

        if (value.Length == 8 && int.TryParse(value, out dateValue))
        {
            dateValue = NormalizeDbDateValue(dateValue);
            return true;
        }

        ShowError("รูปแบบวันที่ต้องเป็น dd/MM/yyyy หรือ yyyyMMdd");
        return false;
    }

    private int BuildDbDateValue(int year, int month, int day)
    {
        if (year > 2400)
        {
            year -= 543;
        }

        return (year * 10000) + (month * 100) + day;
    }

    private int NormalizeDbDateValue(int dateValue)
    {
        int year = dateValue / 10000;
        int monthDay = dateValue % 10000;

        if (year > 2400)
        {
            year -= 543;
        }

        return (year * 10000) + monthDay;
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
}
