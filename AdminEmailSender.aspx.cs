using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AdminEmailSender : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PermissionManager.RedirectIfNoPermission(this, PermissionManager.PageCodes.AdminEmailSender))
        {
            return;
        }

        if (!Page.IsPostBack)
        {
            ClearMessage();
            ClearForm();
            BindSenderList();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ClearMessage();
        BindSenderList();
    }

    protected void btnResetSearch_Click(object sender, EventArgs e)
    {
        ClearMessage();
        txtSearchSender.Text = "";
        chkShowInactive.Checked = false;
        BindSenderList();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        ClearMessage();
        AddSender();
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        ClearMessage();
        UpdateSender();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearMessage();
        ClearForm();
    }

    protected void btnTest_Click(object sender, EventArgs e)
    {
        ClearMessage();
        TestSender();
    }

    protected void gvSender_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ClearMessage();

        int rowIndex;
        if (!int.TryParse(Convert.ToString(e.CommandArgument), out rowIndex) ||
            rowIndex < 0 ||
            rowIndex >= gvSender.DataKeys.Count)
        {
            ShowError("Selected sender row is invalid");
            return;
        }

        DataKey key = gvSender.DataKeys[rowIndex];
        if (e.CommandName == "EditSender")
        {
            LoadRowToForm(key);
        }
        else if (e.CommandName == "SoftDeleteSender")
        {
            SoftDeleteSender(key);
        }
    }

    private void BindSenderList()
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT SENDER_CODE,
                   COALESCE(SENDER_NAME, '') AS SENDER_NAME,
                   SENDER_EMAIL,
                   APP_PASSWORD,
                   ACTIVE_STATUS,
                   CREATED_DATE,
                   UPDATED_DATE
              FROM ITPROD.SHCUMAILS
             WHERE 1 = 1";

        Dictionary<string, object> param = new Dictionary<string, object>();
        string search = (txtSearchSender.Text ?? "").Trim().ToUpper();
        if (!string.IsNullOrEmpty(search))
        {
            sql += @"
               AND (UPPER(SENDER_NAME) LIKE @SEARCH
                    OR UPPER(SENDER_EMAIL) LIKE @SEARCH)";
            param.Add("@SEARCH", "%" + search + "%");
        }

        if (!chkShowInactive.Checked)
        {
            sql += " AND ACTIVE_STATUS = 'Y'";
        }

        sql += " ORDER BY SENDER_NAME, SENDER_EMAIL";

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        gvSender.DataSource = dt;
        gvSender.DataBind();
    }

    private void AddSender()
    {
        string senderName;
        string senderEmail;
        string appPassword;

        if (!ValidateInput(out senderName, out senderEmail, out appPassword))
        {
            return;
        }

        if (SenderExists(senderEmail))
        {
            ShowError("Sender Email already exists");
            return;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            INSERT INTO ITPROD.SHCUMAILS
                (SENDER_CODE, SENDER_NAME, SENDER_EMAIL, APP_PASSWORD, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
            VALUES
                (@SENDER_CODE, @SENDER_NAME, @SENDER_EMAIL, @APP_PASSWORD, @ACTIVE_STATUS, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @UPDATED_USER)";

        db.InsertData(sql, BuildSenderParams(senderName, senderEmail, appPassword));

        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        ShowSuccess("Add Email Sender success");
        ClearForm();
        BindSenderList();
    }

    private void UpdateSender()
    {
        if (hdMode.Value != "EDIT")
        {
            ShowError("Please select sender from grid before update");
            return;
        }

        string senderName;
        string senderEmail;
        string appPassword;

        if (!ValidateInput(out senderName, out senderEmail, out appPassword))
        {
            return;
        }

        string oldSenderEmail = hdSenderEmail.Value.Trim();
        if (!string.Equals(senderEmail, oldSenderEmail, StringComparison.OrdinalIgnoreCase))
        {
            ShowError("Cannot change Sender Email. Please deactivate old sender and add a new sender.");
            return;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            UPDATE ITPROD.SHCUMAILS
               SET SENDER_NAME = @SENDER_NAME,
                   SENDER_EMAIL = @SENDER_EMAIL,
                   APP_PASSWORD = @APP_PASSWORD,
                   ACTIVE_STATUS = @ACTIVE_STATUS,
                   UPDATED_DATE = CURRENT_TIMESTAMP,
                   UPDATED_USER = @UPDATED_USER
             WHERE SENDER_CODE = @SENDER_CODE";

        int rows = db.ExecuteNonQuery(sql, BuildSenderParams(senderName, senderEmail, appPassword));
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (rows == 0)
        {
            ShowError("Sender not found for update");
            return;
        }

        ShowSuccess("Update Email Sender success");
        ClearForm();
        BindSenderList();
    }

    private void SoftDeleteSender(DataKey key)
    {
        string senderEmail = Convert.ToString(key.Values["SENDER_EMAIL"]).Trim();

        dbConnect db = new dbConnect();
        string sql = @"
            UPDATE ITPROD.SHCUMAILS
               SET ACTIVE_STATUS = 'N',
                   UPDATED_DATE = CURRENT_TIMESTAMP,
                   UPDATED_USER = @UPDATED_USER
             WHERE SENDER_CODE = @SENDER_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SENDER_CODE", senderEmail);
        param.Add("@UPDATED_USER", PermissionManager.GetCurrentPersonCode(this));

        int rows = db.ExecuteNonQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (rows == 0)
        {
            ShowError("Sender not found for deactivate");
            return;
        }

        ShowSuccess("Deactivate Email Sender success");
        ClearForm();
        BindSenderList();
    }

    private void TestSender()
    {
        string senderName;
        string senderEmail;
        string appPassword;

        if (!ValidateInput(out senderName, out senderEmail, out appPassword))
        {
            return;
        }

        string testToEmail = txtTestToEmail.Text.Trim();
        if (!IsValidEmail(testToEmail))
        {
            ShowError("Please enter valid Test TO email");
            return;
        }

        string errorMessage;
        EmailSender emailSender = new EmailSender();
        bool success = emailSender.SendTestEmail(senderEmail, appPassword, testToEmail, out errorMessage);
        if (!success)
        {
            ShowError("Test Email failed : " + errorMessage);
            return;
        }

        ShowSuccess("Test Email success. Please check mailbox " + testToEmail);
    }

    private bool ValidateInput(out string senderName, out string senderEmail, out string appPassword)
    {
        senderName = (txtSenderName.Text ?? "").Trim();
        senderEmail = (txtSenderEmail.Text ?? "").Trim();
        appPassword = NormalizeAppPassword(txtAppPassword.Text);

        if (string.IsNullOrWhiteSpace(senderName))
        {
            ShowError("Please enter Sender Name");
            return false;
        }

        if (!IsValidEmail(senderEmail))
        {
            ShowError("Please enter valid Sender Email");
            return false;
        }

        if (string.IsNullOrWhiteSpace(appPassword))
        {
            ShowError("Please enter App Password");
            return false;
        }

        return true;
    }

    private bool SenderExists(string senderEmail)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT COUNT(*) AS CNT
              FROM ITPROD.SHCUMAILS
             WHERE SENDER_CODE = @SENDER_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SENDER_CODE", senderEmail);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError || dt.Rows.Count == 0)
        {
            return false;
        }

        return Convert.ToInt32(dt.Rows[0]["CNT"]) > 0;
    }

    private Dictionary<string, object> BuildSenderParams(string senderName, string senderEmail, string appPassword)
    {
        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SENDER_CODE", senderEmail);
        param.Add("@SENDER_NAME", senderName);
        param.Add("@SENDER_EMAIL", senderEmail);
        param.Add("@APP_PASSWORD", appPassword);
        param.Add("@ACTIVE_STATUS", chkActive.Checked ? "Y" : "N");
        param.Add("@UPDATED_USER", PermissionManager.GetCurrentPersonCode(this));
        return param;
    }

    private void LoadRowToForm(DataKey key)
    {
        string senderEmail = Convert.ToString(key.Values["SENDER_EMAIL"]).Trim();
        string appPassword = GetSenderAppPassword(senderEmail);
        if (lbError.Visible)
        {
            return;
        }

        hdMode.Value = "EDIT";
        hdSenderEmail.Value = senderEmail;
        txtSenderName.Text = Convert.ToString(key.Values["SENDER_NAME"]).Trim();
        txtSenderEmail.Text = senderEmail;
        txtAppPassword.Text = appPassword;
        chkActive.Checked = Convert.ToString(key.Values["ACTIVE_STATUS"]).Trim() == "Y";

        txtSenderEmail.Enabled = false;
        btnAdd.Enabled = false;
        btnUpdate.Enabled = true;
        pnlForm.Visible = true;
    }

    private string GetSenderAppPassword(string senderEmail)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT APP_PASSWORD
              FROM ITPROD.SHCUMAILS
             WHERE SENDER_CODE = @SENDER_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SENDER_CODE", senderEmail);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return "";
        }

        if (dt.Rows.Count == 0)
        {
            ShowError("Sender not found");
            return "";
        }

        return Convert.ToString(dt.Rows[0]["APP_PASSWORD"]).Trim();
    }

    private void ClearForm()
    {
        pnlForm.Visible = true;
        hdMode.Value = "";
        hdSenderEmail.Value = "";
        txtSenderName.Text = "";
        txtSenderEmail.Text = "";
        txtAppPassword.Text = "";
        txtTestToEmail.Text = "";
        chkActive.Checked = true;

        txtSenderEmail.Enabled = true;
        btnAdd.Enabled = true;
        btnUpdate.Enabled = false;
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

    private string NormalizeAppPassword(string appPassword)
    {
        if (string.IsNullOrWhiteSpace(appPassword))
        {
            return "";
        }

        return string.Join("", appPassword.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
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
