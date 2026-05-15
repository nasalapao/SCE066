using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AdminEmailLog : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PermissionManager.RedirectIfNoPermission(this, PermissionManager.PageCodes.AdminEmailLog))
        {
            return;
        }

        if (!Page.IsPostBack)
        {
            ClearMessage();
            BindEmailLog();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ClearMessage();
        pnlDetail.Visible = false;
        BindEmailLog();
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        ClearMessage();
        txtKeyword.Text = "";
        txtDateFrom.Text = "";
        txtDateTo.Text = "";
        ddlStatus.SelectedValue = "";
        txtMailKind.Text = "";
        pnlDetail.Visible = false;
        BindEmailLog();
    }

    protected void gvEmailLog_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ClearMessage();

        if (e.CommandName != "ViewLog")
        {
            return;
        }

        int rowIndex;
        if (!int.TryParse(Convert.ToString(e.CommandArgument), out rowIndex) ||
            rowIndex < 0 ||
            rowIndex >= gvEmailLog.DataKeys.Count)
        {
            ShowError("Selected email log row is invalid");
            return;
        }

        int logId = Convert.ToInt32(gvEmailLog.DataKeys[rowIndex].Value);
        LoadEmailLogDetail(logId);
    }

    private void BindEmailLog()
    {
        DateTime dateFrom;
        DateTime dateTo;
        if (!ValidateDateFilters(out dateFrom, out dateTo))
        {
            return;
        }

        dbConnect db = new dbConnect();
        Dictionary<string, object> param = new Dictionary<string, object>();
        string sql = @"
            SELECT LOG_ID,
                   MAIL_KIND,
                   INVOICE_NO,
                   CUSTOMER_CODE,
                   TO_RECIPIENTS,
                   CC_RECIPIENTS,
                   SUBJECT_TEXT,
                   STATUS,
                   ERROR_MSG,
                   SENT_USER,
                   SENT_TIMESTAMP
              FROM ITPROD.SHCUEMAILL
             WHERE 1 = 1";

        string keyword = (txtKeyword.Text ?? "").Trim().ToUpper();
        if (!string.IsNullOrEmpty(keyword))
        {
            sql += @"
               AND (UPPER(INVOICE_NO) LIKE @KEYWORD_INVOICE
                    OR UPPER(CUSTOMER_CODE) LIKE @KEYWORD_CUSTOMER
                    OR UPPER(TO_RECIPIENTS) LIKE @KEYWORD_TO
                    OR UPPER(CC_RECIPIENTS) LIKE @KEYWORD_CC
                    OR UPPER(SUBJECT_TEXT) LIKE @KEYWORD_SUBJECT
                    OR UPPER(STATUS) LIKE @KEYWORD_STATUS
                    OR UPPER(ERROR_MSG) LIKE @KEYWORD_ERROR
                    OR UPPER(SENT_USER) LIKE @KEYWORD_USER)";
            string keywordParam = "%" + keyword + "%";
            param.Add("@KEYWORD_INVOICE", keywordParam);
            param.Add("@KEYWORD_CUSTOMER", keywordParam);
            param.Add("@KEYWORD_TO", keywordParam);
            param.Add("@KEYWORD_CC", keywordParam);
            param.Add("@KEYWORD_SUBJECT", keywordParam);
            param.Add("@KEYWORD_STATUS", keywordParam);
            param.Add("@KEYWORD_ERROR", keywordParam);
            param.Add("@KEYWORD_USER", keywordParam);
        }

        string status = (ddlStatus.SelectedValue ?? "").Trim().ToUpper();
        if (!string.IsNullOrEmpty(status))
        {
            sql += " AND UPPER(STATUS) = @STATUS";
            param.Add("@STATUS", status);
        }

        string mailKind = (txtMailKind.Text ?? "").Trim().ToUpper();
        if (!string.IsNullOrEmpty(mailKind))
        {
            sql += " AND UPPER(MAIL_KIND) LIKE @MAIL_KIND";
            param.Add("@MAIL_KIND", "%" + mailKind + "%");
        }

        if (dateFrom != DateTime.MinValue)
        {
            sql += " AND SENT_TIMESTAMP >= @DATE_FROM";
            param.Add("@DATE_FROM", dateFrom);
        }

        if (dateTo != DateTime.MinValue)
        {
            sql += " AND SENT_TIMESTAMP < @DATE_TO";
            param.Add("@DATE_TO", dateTo.AddDays(1));
        }

        sql += @"
             ORDER BY SENT_TIMESTAMP DESC, LOG_ID DESC
             FETCH FIRST 200 ROWS ONLY";

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        gvEmailLog.DataSource = dt;
        gvEmailLog.DataBind();
    }

    private void LoadEmailLogDetail(int logId)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT LOG_ID,
                   MAIL_KIND,
                   INVOICE_NO,
                   CUSTOMER_CODE,
                   TO_RECIPIENTS,
                   CC_RECIPIENTS,
                   SUBJECT_TEXT,
                   BODY_TEXT,
                   STATUS,
                   ERROR_MSG,
                   SENT_USER,
                   SENT_TIMESTAMP
              FROM ITPROD.SHCUEMAILL
             WHERE LOG_ID = @LOG_ID";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@LOG_ID", logId);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (dt.Rows.Count == 0)
        {
            ShowError("Email log not found");
            return;
        }

        DataRow row = dt.Rows[0];
        litLogId.Text = Encode(row["LOG_ID"]);
        litSentTime.Text = FormatDateTime(row["SENT_TIMESTAMP"]);
        litMailKind.Text = Encode(row["MAIL_KIND"]);
        litStatus.Text = Encode(row["STATUS"]);
        litInvoiceNo.Text = Encode(row["INVOICE_NO"]);
        litCustomerCode.Text = Encode(row["CUSTOMER_CODE"]);
        litToRecipients.Text = Encode(row["TO_RECIPIENTS"]);
        litCcRecipients.Text = Encode(row["CC_RECIPIENTS"]);
        litSubject.Text = Encode(row["SUBJECT_TEXT"]);
        litSentUser.Text = Encode(row["SENT_USER"]);
        litErrorMessage.Text = Encode(row["ERROR_MSG"]);

        string body = Convert.ToString(row["BODY_TEXT"]);
        frameBody.Attributes["srcdoc"] = BuildPreviewDocument(body);

        pnlDetail.Visible = true;
        ScriptManager.RegisterStartupScript(this, GetType(), "OpenEmailLogModal", "openEmailLogModal();", true);
    }

    private string BuildPreviewDocument(string body)
    {
        string bodyHtml = string.IsNullOrWhiteSpace(body)
            ? "<span class=\"empty-text\">No body text</span>"
            : body;

        return "<!DOCTYPE html><html><head><meta charset=\"utf-8\" />" +
               "<style>body{margin:0;padding:18px;font-family:Arial,Helvetica,sans-serif;font-size:14px;line-height:1.6;color:#222;overflow-wrap:anywhere}.empty-text{color:#5b6b7a}</style>" +
               "</head><body>" + bodyHtml + "</body></html>";
    }

    private bool ValidateDateFilters(out DateTime dateFrom, out DateTime dateTo)
    {
        dateFrom = DateTime.MinValue;
        dateTo = DateTime.MinValue;

        string dateFromText = (txtDateFrom.Text ?? "").Trim();
        if (!string.IsNullOrEmpty(dateFromText) && !DateTime.TryParse(dateFromText, out dateFrom))
        {
            ShowError("Date From is invalid");
            return false;
        }

        string dateToText = (txtDateTo.Text ?? "").Trim();
        if (!string.IsNullOrEmpty(dateToText) && !DateTime.TryParse(dateToText, out dateTo))
        {
            ShowError("Date To is invalid");
            return false;
        }

        if (dateFrom != DateTime.MinValue && dateTo != DateTime.MinValue && dateFrom.Date > dateTo.Date)
        {
            ShowError("Date From must be before Date To");
            return false;
        }

        return true;
    }

    private string FormatDateTime(object value)
    {
        if (value == null || value == DBNull.Value)
        {
            return "";
        }

        DateTime dateValue;
        if (!DateTime.TryParse(Convert.ToString(value), out dateValue))
        {
            return Encode(value);
        }

        return Server.HtmlEncode(dateValue.ToString("dd/MM/yyyy HH:mm:ss"));
    }

    private string Encode(object value)
    {
        return Server.HtmlEncode(Convert.ToString(value).Trim());
    }

    private void ClearMessage()
    {
        lbError.Visible = false;
        lbError.Text = "";
    }

    private void ShowError(string message)
    {
        lbError.Text = Server.HtmlEncode(message);
        lbError.Visible = true;
    }
}
