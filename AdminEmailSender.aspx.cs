using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using System.Web.UI;

public partial class AdminEmailSender : Page
{
    private const string AuthCookieName = "SCE066_TOKEN";
    private const string SenderCode = "CUSTOMER_INVOICE";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PermissionManager.RedirectIfNoPermission(this, PermissionManager.PageCodes.AdminEmailSender))
        {
            return;
        }

        if (!Page.IsPostBack)
        {
            ClearMessage();
            LoadSender();
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        ClearMessage();
        SaveSender();
    }

    protected void btnReload_Click(object sender, EventArgs e)
    {
        ClearMessage();
        LoadSender();
    }

    protected void btnTest_Click(object sender, EventArgs e)
    {
        ClearMessage();
        TestSender();
    }

    private void LoadSender()
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT SENDER_CODE, SENDER_EMAIL, APP_PASSWORD, ACTIVE_STATUS
              FROM ITPROD.SHCUMAILS
             WHERE SENDER_CODE = @SENDER_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SENDER_CODE", SenderCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        hdSenderCode.Value = SenderCode;
        if (dt.Rows.Count == 0)
        {
            txtSenderEmail.Text = "siripong.j@patayafood.com";
            txtAppPassword.Text = "";
            chkActive.Checked = true;
            return;
        }

        txtSenderEmail.Text = Convert.ToString(dt.Rows[0]["SENDER_EMAIL"]).Trim();
        txtAppPassword.Text = Convert.ToString(dt.Rows[0]["APP_PASSWORD"]).Trim();
        chkActive.Checked = Convert.ToString(dt.Rows[0]["ACTIVE_STATUS"]).Trim() == "Y";
    }

    private void SaveSender()
    {
        string senderEmail = txtSenderEmail.Text.Trim();
        string appPassword = txtAppPassword.Text.Trim();

        if (!IsValidEmail(senderEmail))
        {
            ShowError("กรุณากรอก Sender Email ให้ถูกต้อง");
            return;
        }

        if (string.IsNullOrWhiteSpace(appPassword))
        {
            ShowError("กรุณากรอก App Password");
            return;
        }

        if (SenderExists())
        {
            UpdateSender(senderEmail, appPassword);
        }
        else
        {
            InsertSender(senderEmail, appPassword);
        }
    }

    private void TestSender()
    {
        string senderEmail = txtSenderEmail.Text.Trim();
        string appPassword = txtAppPassword.Text.Trim();

        if (!IsValidEmail(senderEmail))
        {
            ShowError("กรุณากรอก Sender Email ให้ถูกต้อง");
            return;
        }

        if (string.IsNullOrWhiteSpace(appPassword))
        {
            ShowError("กรุณากรอก App Password");
            return;
        }

        string errorMessage;
        EmailSender emailSender = new EmailSender();
        bool success = emailSender.SendTestEmail(senderEmail, appPassword, out errorMessage);
        if (!success)
        {
            ShowError("ส่ง Test Email ไม่สำเร็จ : " + errorMessage);
            return;
        }

        ShowSuccess("ส่ง Test Email สำเร็จ กรุณาตรวจสอบ mailbox " + senderEmail);
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            MailAddress address = new MailAddress(email);
            return string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private bool SenderExists()
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT COUNT(*) AS CNT
              FROM ITPROD.SHCUMAILS
             WHERE SENDER_CODE = @SENDER_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SENDER_CODE", SenderCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError || dt.Rows.Count == 0)
        {
            return false;
        }

        return Convert.ToInt32(dt.Rows[0]["CNT"]) > 0;
    }

    private void InsertSender(string senderEmail, string appPassword)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            INSERT INTO ITPROD.SHCUMAILS
                (SENDER_CODE, SENDER_EMAIL, APP_PASSWORD, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
            VALUES
                (@SENDER_CODE, @SENDER_EMAIL, @APP_PASSWORD, @ACTIVE_STATUS, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @UPDATED_USER)";

        Dictionary<string, object> param = BuildSenderParams(senderEmail, appPassword);
        db.InsertData(sql, param);

        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        ShowSuccess("เพิ่ม Email Sender สำเร็จ");
    }

    private void UpdateSender(string senderEmail, string appPassword)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            UPDATE ITPROD.SHCUMAILS
               SET SENDER_EMAIL = @SENDER_EMAIL,
                   APP_PASSWORD = @APP_PASSWORD,
                   ACTIVE_STATUS = @ACTIVE_STATUS,
                   UPDATED_DATE = CURRENT_TIMESTAMP,
                   UPDATED_USER = @UPDATED_USER
             WHERE SENDER_CODE = @SENDER_CODE";

        Dictionary<string, object> param = BuildSenderParams(senderEmail, appPassword);
        int rows = db.ExecuteNonQuery(sql, param);

        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (rows == 0)
        {
            ShowError("ไม่พบ Email Sender สำหรับแก้ไข");
            return;
        }

        ShowSuccess("บันทึก Email Sender สำเร็จ");
    }

    private Dictionary<string, object> BuildSenderParams(string senderEmail, string appPassword)
    {
        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SENDER_CODE", SenderCode);
        param.Add("@SENDER_EMAIL", senderEmail);
        param.Add("@APP_PASSWORD", appPassword);
        param.Add("@ACTIVE_STATUS", chkActive.Checked ? "Y" : "N");
        param.Add("@UPDATED_USER", GetCurrentPersonCode());
        return param;
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
