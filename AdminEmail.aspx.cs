using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AdminEmail : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PermissionManager.RedirectIfNoPermission(this, PermissionManager.PageCodes.AdminEmail))
        {
            return;
        }

        if (!Page.IsPostBack)
        {
            ClearMessage();
            BindSenderDropDown();
            ClearForm();
            BindEmailList();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ClearMessage();
        BindEmailList();
    }

    protected void btnResetSearch_Click(object sender, EventArgs e)
    {
        ClearMessage();
        txtSearchCustomer.Text = "";
        chkShowInactive.Checked = false;
        BindEmailList();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        ClearMessage();
        AddEmail();
    }

    protected void btnShowAdd_Click(object sender, EventArgs e)
    {
        ClearMessage();
        BindSenderDropDown();
        ClearForm();
        pnlForm.Visible = true;
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        ClearMessage();
        UpdateEmail();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearMessage();
        ClearForm();
    }

    protected void gvEmail_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ClearMessage();

        int rowIndex;
        if (!int.TryParse(Convert.ToString(e.CommandArgument), out rowIndex))
        {
            ShowError("ไม่พบแถวข้อมูลที่เลือก");
            return;
        }

        if (rowIndex < 0 || rowIndex >= gvEmail.DataKeys.Count)
        {
            ShowError("แถวข้อมูลที่เลือกไม่ถูกต้อง");
            return;
        }

        DataKey key = gvEmail.DataKeys[rowIndex];

        if (e.CommandName == "EditEmail")
        {
            LoadRowToForm(key);
        }
        else if (e.CommandName == "SoftDeleteEmail")
        {
            SoftDeleteEmail(key);
        }
    }

    private void BindSenderDropDown()
    {
        ddlSender.Items.Clear();
        ddlSender.Items.Add(new ListItem("-- Select Sender --", ""));

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT COALESCE(SENDER_NAME, '') AS SENDER_NAME,
                   SENDER_EMAIL
              FROM ITPROD.SHCUMAILS
             WHERE ACTIVE_STATUS = 'Y'
             ORDER BY SENDER_NAME, SENDER_EMAIL";

        DataTable dt = db.ExecuteQuery(sql);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        foreach (DataRow row in dt.Rows)
        {
            string senderName = Convert.ToString(row["SENDER_NAME"]).Trim();
            string senderEmail = Convert.ToString(row["SENDER_EMAIL"]).Trim();
            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                continue;
            }

            string text = string.IsNullOrWhiteSpace(senderName)
                ? senderEmail
                : senderName + " <" + senderEmail + ">";
            ddlSender.Items.Add(new ListItem(text, senderEmail));
        }
    }

    private void BindEmailList()
    {
        dbConnect db = new dbConnect();

        string sql = @"
            SELECT COALESCE(SENDER_NAME, '') AS SENDER_NAME,
                   COALESCE(SENDER_EMAIL, '') AS SENDER_EMAIL,
                   CASE
                       WHEN COALESCE(TRIM(SENDER_NAME), '') = '' THEN COALESCE(TRIM(SENDER_EMAIL), '')
                       ELSE TRIM(SENDER_NAME) || ' <' || TRIM(SENDER_EMAIL) || '>'
                   END AS SENDER_DISPLAY,
                   CUSTOMER_CODE,
                   RECIPIENT_TYPE,
                   EMAIL_SEQ,
                   EMAIL_ADDRESS,
                   ACTIVE_STATUS,
                   CREATED_DATE,
                   UPDATED_DATE
            FROM ITPROD.SHCUMAILD
            WHERE 1 = 1";

        Dictionary<string, object> param = new Dictionary<string, object>();

        string customerCode = NormalizeCustomerCode(txtSearchCustomer.Text);
        if (!string.IsNullOrEmpty(customerCode))
        {
            sql += " AND CUSTOMER_CODE = @CUSTOMER_CODE";
            param.Add("@CUSTOMER_CODE", customerCode);
        }

        if (!chkShowInactive.Checked)
        {
            sql += " AND ACTIVE_STATUS = 'Y'";
        }

        sql += " ORDER BY CUSTOMER_CODE, RECIPIENT_TYPE DESC, EMAIL_SEQ";

        DataTable dt = db.ExecuteQuery(sql, param);

        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        gvEmail.DataSource = dt;
        gvEmail.DataBind();
    }

    private void AddEmail()
    {
        string customerCode;
        string recipientType;
        string emailAddress;
        SenderInfo selectedSender;

        if (!ValidateInput(out customerCode, out recipientType, out emailAddress, out selectedSender))
        {
            return;
        }

        SenderInfo customerSender;
        if (!TryGetCustomerSender(customerCode, out customerSender))
        {
            return;
        }

        if (customerSender.HasSender &&
            !string.Equals(customerSender.SenderEmail, selectedSender.SenderEmail, StringComparison.OrdinalIgnoreCase))
        {
            ShowError("Customer นี้มี Sender อยู่แล้ว หากต้องการเปลี่ยน Sender ให้ Edit email เดิมเพื่อ update ทั้ง Customer");
            return;
        }

        if (IsDuplicateEmail(customerCode, recipientType, emailAddress, "", "", ""))
        {
            ShowError("Email นี้มีอยู่แล้วใน customer และ type เดียวกัน");
            return;
        }

        int emailSeq = GetNextEmailSeq(customerCode, recipientType);

        dbConnect db = new dbConnect();
        string sql = @"
            INSERT INTO ITPROD.SHCUMAILD
                (SENDER_NAME, SENDER_EMAIL, CUSTOMER_CODE, RECIPIENT_TYPE, EMAIL_SEQ, EMAIL_ADDRESS, ACTIVE_STATUS, CREATED_DATE)
            VALUES
                (@SENDER_NAME, @SENDER_EMAIL, @CUSTOMER_CODE, @RECIPIENT_TYPE, @EMAIL_SEQ, @EMAIL_ADDRESS, @ACTIVE_STATUS, CURRENT_TIMESTAMP)";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SENDER_NAME", selectedSender.SenderName);
        param.Add("@SENDER_EMAIL", selectedSender.SenderEmail);
        param.Add("@CUSTOMER_CODE", customerCode);
        param.Add("@RECIPIENT_TYPE", recipientType);
        param.Add("@EMAIL_SEQ", emailSeq);
        param.Add("@EMAIL_ADDRESS", emailAddress);
        param.Add("@ACTIVE_STATUS", chkActive.Checked ? "Y" : "N");

        db.InsertData(sql, param);

        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (!customerSender.HasSender)
        {
            UpdateCustomerSender(customerCode, selectedSender);
        }

        ShowSuccess("เพิ่ม Email สำเร็จ");
        ClearForm();
        txtSearchCustomer.Text = customerCode;
        BindEmailList();
    }

    private void UpdateEmail()
    {
        if (string.IsNullOrEmpty(hdMode.Value) || hdMode.Value != "EDIT")
        {
            ShowError("กรุณาเลือกข้อมูลจากตารางก่อนกด Update");
            return;
        }

        string customerCode;
        string recipientType;
        string emailAddress;
        SenderInfo selectedSender;

        if (!ValidateInput(out customerCode, out recipientType, out emailAddress, out selectedSender))
        {
            return;
        }

        string oldCustomerCode = hdCustomerCode.Value;
        string oldRecipientType = hdRecipientType.Value;
        string oldEmailSeq = hdEmailSeq.Value;

        if (customerCode != oldCustomerCode || recipientType != oldRecipientType)
        {
            ShowError("ไม่สามารถแก้ Customer Code หรือ Recipient Type ของรายการเดิมได้");
            return;
        }

        if (IsDuplicateEmail(customerCode, recipientType, emailAddress, oldCustomerCode, oldRecipientType, oldEmailSeq))
        {
            ShowError("Email นี้มีอยู่แล้วใน customer และ type เดียวกัน");
            return;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            UPDATE ITPROD.SHCUMAILD
               SET EMAIL_ADDRESS = @EMAIL_ADDRESS,
                   ACTIVE_STATUS = @ACTIVE_STATUS,
                   UPDATED_DATE = CURRENT_TIMESTAMP
             WHERE CUSTOMER_CODE = @CUSTOMER_CODE
               AND RECIPIENT_TYPE = @RECIPIENT_TYPE
               AND EMAIL_SEQ = @EMAIL_SEQ";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@EMAIL_ADDRESS", emailAddress);
        param.Add("@ACTIVE_STATUS", chkActive.Checked ? "Y" : "N");
        param.Add("@CUSTOMER_CODE", oldCustomerCode);
        param.Add("@RECIPIENT_TYPE", oldRecipientType);
        param.Add("@EMAIL_SEQ", Convert.ToInt32(oldEmailSeq));

        int rows = db.ExecuteNonQuery(sql, param);

        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (rows == 0)
        {
            ShowError("ไม่พบข้อมูลสำหรับแก้ไข");
            return;
        }

        if (!string.Equals(hdSenderEmail.Value.Trim(), selectedSender.SenderEmail, StringComparison.OrdinalIgnoreCase))
        {
            if (!UpdateCustomerSender(customerCode, selectedSender))
            {
                return;
            }
        }

        ShowSuccess("แก้ไข Email สำเร็จ");
        ClearForm();
        BindEmailList();
    }

    private bool UpdateCustomerSender(string customerCode, SenderInfo sender)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            UPDATE ITPROD.SHCUMAILD
               SET SENDER_NAME = @SENDER_NAME,
                   SENDER_EMAIL = @SENDER_EMAIL,
                   UPDATED_DATE = CURRENT_TIMESTAMP
             WHERE CUSTOMER_CODE = @CUSTOMER_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SENDER_NAME", sender.SenderName);
        param.Add("@SENDER_EMAIL", sender.SenderEmail);
        param.Add("@CUSTOMER_CODE", customerCode);

        db.ExecuteNonQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return false;
        }

        return true;
    }

    private void SoftDeleteEmail(DataKey key)
    {
        string customerCode = Convert.ToString(key.Values["CUSTOMER_CODE"]).Trim();
        string recipientType = Convert.ToString(key.Values["RECIPIENT_TYPE"]).Trim();
        int emailSeq = Convert.ToInt32(key.Values["EMAIL_SEQ"]);

        dbConnect db = new dbConnect();
        string sql = @"
            UPDATE ITPROD.SHCUMAILD
               SET ACTIVE_STATUS = 'N',
                   UPDATED_DATE = CURRENT_TIMESTAMP
             WHERE CUSTOMER_CODE = @CUSTOMER_CODE
               AND RECIPIENT_TYPE = @RECIPIENT_TYPE
               AND EMAIL_SEQ = @EMAIL_SEQ";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@CUSTOMER_CODE", customerCode);
        param.Add("@RECIPIENT_TYPE", recipientType);
        param.Add("@EMAIL_SEQ", emailSeq);

        int rows = db.ExecuteNonQuery(sql, param);

        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (rows == 0)
        {
            ShowError("ไม่พบข้อมูลสำหรับปิดใช้งาน");
            return;
        }

        ShowSuccess("ปิดใช้งาน Email สำเร็จ");
        ClearForm();
        BindEmailList();
    }

    private bool ValidateInput(out string customerCode, out string recipientType, out string emailAddress, out SenderInfo selectedSender)
    {
        customerCode = NormalizeCustomerCode(txtCustomerCode.Text);
        recipientType = ddlRecipientType.SelectedValue.Trim().ToUpper();
        emailAddress = txtEmailAddress.Text.Trim();
        selectedSender = GetSelectedSender();

        if (string.IsNullOrEmpty(customerCode))
        {
            ShowError("กรุณากรอก Customer Code");
            return false;
        }

        if (!selectedSender.HasSender)
        {
            ShowError("กรุณาเลือก Sender");
            return false;
        }

        if (recipientType != "TO" && recipientType != "CC" && recipientType != "NOTICE")
        {
            ShowError("Recipient Type ต้องเป็น TO, CC หรือ NOTICE");
            return false;
        }

        if (string.IsNullOrEmpty(emailAddress))
        {
            ShowError("กรุณากรอก Email Address");
            return false;
        }

        if (!IsValidEmail(emailAddress))
        {
            ShowError("รูปแบบ Email Address ไม่ถูกต้อง");
            return false;
        }

        return true;
    }

    private SenderInfo GetSelectedSender()
    {
        SenderInfo sender = new SenderInfo();
        sender.SenderEmail = (ddlSender.SelectedValue ?? "").Trim();

        ListItem item = ddlSender.SelectedItem;
        if (item != null && !string.IsNullOrWhiteSpace(sender.SenderEmail))
        {
            string text = item.Text.Trim();
            int emailStart = text.LastIndexOf(" <", StringComparison.Ordinal);
            sender.SenderName = emailStart > 0 ? text.Substring(0, emailStart).Trim() : text;
        }

        return sender;
    }

    private bool TryGetCustomerSender(string customerCode, out SenderInfo sender)
    {
        sender = new SenderInfo();

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT DISTINCT COALESCE(TRIM(SENDER_NAME), '') AS SENDER_NAME,
                            COALESCE(TRIM(SENDER_EMAIL), '') AS SENDER_EMAIL
              FROM ITPROD.SHCUMAILD
             WHERE CUSTOMER_CODE = @CUSTOMER_CODE
               AND COALESCE(TRIM(SENDER_EMAIL), '') <> ''";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@CUSTOMER_CODE", customerCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return false;
        }

        if (dt.Rows.Count == 0)
        {
            return true;
        }

        if (dt.Rows.Count > 1)
        {
            ShowError("Customer นี้มี Sender มากกว่า 1 คน กรุณาแก้ข้อมูลให้เหลือ Sender เดียว");
            return false;
        }

        sender.SenderName = Convert.ToString(dt.Rows[0]["SENDER_NAME"]).Trim();
        sender.SenderEmail = Convert.ToString(dt.Rows[0]["SENDER_EMAIL"]).Trim();
        return true;
    }

    private bool IsDuplicateEmail(string customerCode, string recipientType, string emailAddress, string oldCustomerCode, string oldRecipientType, string oldEmailSeq)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT COUNT(*) AS CNT
            FROM ITPROD.SHCUMAILD
            WHERE CUSTOMER_CODE = @CUSTOMER_CODE
              AND RECIPIENT_TYPE = @RECIPIENT_TYPE
              AND UPPER(EMAIL_ADDRESS) = @EMAIL_ADDRESS";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@CUSTOMER_CODE", customerCode);
        param.Add("@RECIPIENT_TYPE", recipientType);
        param.Add("@EMAIL_ADDRESS", emailAddress.ToUpper());

        if (!string.IsNullOrEmpty(oldCustomerCode) && !string.IsNullOrEmpty(oldRecipientType) && !string.IsNullOrEmpty(oldEmailSeq))
        {
            sql += @"
              AND NOT (CUSTOMER_CODE = @OLD_CUSTOMER_CODE
                   AND RECIPIENT_TYPE = @OLD_RECIPIENT_TYPE
                   AND EMAIL_SEQ = @OLD_EMAIL_SEQ)";
            param.Add("@OLD_CUSTOMER_CODE", oldCustomerCode);
            param.Add("@OLD_RECIPIENT_TYPE", oldRecipientType);
            param.Add("@OLD_EMAIL_SEQ", Convert.ToInt32(oldEmailSeq));
        }

        DataTable dt = db.ExecuteQuery(sql, param);

        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return true;
        }

        if (dt.Rows.Count == 0)
        {
            return false;
        }

        int count = Convert.ToInt32(dt.Rows[0]["CNT"]);
        return count > 0;
    }

    private int GetNextEmailSeq(string customerCode, string recipientType)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT COALESCE(MAX(EMAIL_SEQ), 0) + 1 AS NEXT_SEQ
            FROM ITPROD.SHCUMAILD
            WHERE CUSTOMER_CODE = @CUSTOMER_CODE
              AND RECIPIENT_TYPE = @RECIPIENT_TYPE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@CUSTOMER_CODE", customerCode);
        param.Add("@RECIPIENT_TYPE", recipientType);

        DataTable dt = db.ExecuteQuery(sql, param);

        if (db.isError || dt.Rows.Count == 0)
        {
            return 1;
        }

        return Convert.ToInt32(dt.Rows[0]["NEXT_SEQ"]);
    }

    private void LoadRowToForm(DataKey key)
    {
        string customerCode = Convert.ToString(key.Values["CUSTOMER_CODE"]).Trim();
        string recipientType = Convert.ToString(key.Values["RECIPIENT_TYPE"]).Trim();
        string emailSeq = Convert.ToString(key.Values["EMAIL_SEQ"]).Trim();
        string senderEmail = Convert.ToString(key.Values["SENDER_EMAIL"]).Trim();

        BindSenderDropDown();
        EnsureSenderListItem(Convert.ToString(key.Values["SENDER_NAME"]).Trim(), senderEmail);

        hdMode.Value = "EDIT";
        hdCustomerCode.Value = customerCode;
        hdRecipientType.Value = recipientType;
        hdEmailSeq.Value = emailSeq;
        hdSenderEmail.Value = senderEmail;

        txtCustomerCode.Text = customerCode;
        ddlSender.SelectedValue = senderEmail;
        ddlRecipientType.SelectedValue = recipientType;
        txtEmailAddress.Text = Convert.ToString(key.Values["EMAIL_ADDRESS"]).Trim();
        chkActive.Checked = Convert.ToString(key.Values["ACTIVE_STATUS"]).Trim() == "Y";

        txtCustomerCode.Enabled = false;
        ddlRecipientType.Enabled = false;
        btnAdd.Enabled = false;
        btnUpdate.Enabled = true;
        pnlForm.Visible = true;
    }

    private void EnsureSenderListItem(string senderName, string senderEmail)
    {
        if (string.IsNullOrWhiteSpace(senderEmail))
        {
            return;
        }

        if (ddlSender.Items.FindByValue(senderEmail) != null)
        {
            return;
        }

        string text = string.IsNullOrWhiteSpace(senderName)
            ? senderEmail
            : senderName + " <" + senderEmail + ">";
        ddlSender.Items.Add(new ListItem(text, senderEmail));
    }

    private void ClearForm()
    {
        pnlForm.Visible = false;
        hdMode.Value = "";
        hdCustomerCode.Value = "";
        hdRecipientType.Value = "";
        hdEmailSeq.Value = "";
        hdSenderEmail.Value = "";

        txtCustomerCode.Text = "";
        if (ddlSender.Items.Count > 0)
        {
            ddlSender.SelectedIndex = 0;
        }
        ddlRecipientType.SelectedValue = "TO";
        txtEmailAddress.Text = "";
        chkActive.Checked = true;

        txtCustomerCode.Enabled = true;
        ddlRecipientType.Enabled = true;
        btnAdd.Enabled = true;
        btnUpdate.Enabled = false;
    }

    private string NormalizeCustomerCode(string value)
    {
        return (value ?? "").Trim().ToUpper();
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            MailAddress address = new MailAddress(email);
            return address.Address == email;
        }
        catch
        {
            return false;
        }
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

    private class SenderInfo
    {
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }

        public bool HasSender
        {
            get { return !string.IsNullOrWhiteSpace(SenderEmail); }
        }
    }
}
