using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Web;
using System.Web.UI;

public partial class AdminEmailTemplate : Page
{
    private const string AuthCookieName = "SCE066_TOKEN";
    private const string TemplateCode = "CUSTOMER_INVOICE";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ClearMessage();
            LoadTemplate();
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        ClearMessage();
        SaveTemplate();
    }

    protected void btnReload_Click(object sender, EventArgs e)
    {
        ClearMessage();
        LoadTemplate();
    }

    private void LoadTemplate()
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT TEMPLATE_CODE, SUBJECT_TEMPLATE, BODY_TEMPLATE, ACTIVE_STATUS
              FROM ITPROD.SHCUMAILT
             WHERE TEMPLATE_CODE = @TEMPLATE_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@TEMPLATE_CODE", TemplateCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (dt.Rows.Count == 0)
        {
            txtSubject.Text = "Invoice {INVOICE_NO} - {CUSTOMER_NAME}";
            txtBody.Text = "<p>Dear Customer,</p>\r\n<p>Please find invoice document for {INVOICE_NO}.</p>\r\n<p>Customer: {CUSTOMER_CODE} {CUSTOMER_NAME}</p>";
            chkActive.Checked = true;
            return;
        }

        txtSubject.Text = Convert.ToString(dt.Rows[0]["SUBJECT_TEMPLATE"]);
        txtBody.Text = Convert.ToString(dt.Rows[0]["BODY_TEMPLATE"]);
        chkActive.Checked = Convert.ToString(dt.Rows[0]["ACTIVE_STATUS"]).Trim() == "Y";
    }

    private void SaveTemplate()
    {
        string subject = txtSubject.Text.Trim();
        string body = GetUnvalidatedText(txtBody.UniqueID).Trim();

        if (string.IsNullOrEmpty(subject))
        {
            ShowError("กรุณากรอก Subject");
            return;
        }

        if (string.IsNullOrEmpty(body))
        {
            ShowError("กรุณากรอก Body");
            return;
        }

        if (TemplateExists())
        {
            UpdateTemplate(subject, body);
        }
        else
        {
            InsertTemplate(subject, body);
        }
    }

    private string GetUnvalidatedText(string key)
    {
        if (Request.Unvalidated == null || Request.Unvalidated.Form == null)
        {
            return "";
        }

        return Request.Unvalidated.Form[key] ?? "";
    }

    private bool TemplateExists()
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT COUNT(*) AS CNT
              FROM ITPROD.SHCUMAILT
             WHERE TEMPLATE_CODE = @TEMPLATE_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@TEMPLATE_CODE", TemplateCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError || dt.Rows.Count == 0)
        {
            return false;
        }

        return Convert.ToInt32(dt.Rows[0]["CNT"]) > 0;
    }

    private void InsertTemplate(string subject, string body)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            INSERT INTO ITPROD.SHCUMAILT
                (TEMPLATE_CODE, SUBJECT_TEMPLATE, BODY_TEMPLATE, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
            VALUES
                (@TEMPLATE_CODE, @SUBJECT_TEMPLATE, @BODY_TEMPLATE, @ACTIVE_STATUS, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @UPDATED_USER)";

        Dictionary<string, object> param = BuildTemplateParams(subject, body);
        db.InsertData(sql, param);

        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        ShowSuccess("เพิ่ม Template สำเร็จ");
    }

    private void UpdateTemplate(string subject, string body)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            UPDATE ITPROD.SHCUMAILT
               SET SUBJECT_TEMPLATE = @SUBJECT_TEMPLATE,
                   BODY_TEMPLATE = @BODY_TEMPLATE,
                   ACTIVE_STATUS = @ACTIVE_STATUS,
                   UPDATED_DATE = CURRENT_TIMESTAMP,
                   UPDATED_USER = @UPDATED_USER
             WHERE TEMPLATE_CODE = @TEMPLATE_CODE";

        Dictionary<string, object> param = BuildTemplateParams(subject, body);
        int rows = db.ExecuteNonQuery(sql, param);

        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (rows == 0)
        {
            ShowError("ไม่พบ Template สำหรับแก้ไข");
            return;
        }

        ShowSuccess("บันทึก Template สำเร็จ");
    }

    private Dictionary<string, object> BuildTemplateParams(string subject, string body)
    {
        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@TEMPLATE_CODE", TemplateCode);
        param.Add("@SUBJECT_TEMPLATE", subject);
        param.Add("@BODY_TEMPLATE", body);
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
