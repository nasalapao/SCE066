using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Data;              // เผื่อมีการคืนค่า DataTable
                                // ไม่ต้อง using IBM.Data.DB2.iSeries อีก

public class EmailSender
{
    private const string CustomerInvoiceTemplateCode = "CUSTOMER_INVOICE";
    private const string TestCcEmail = "nasalapao@gmail.com";

    /* ---------- 1. SMTP CONFIG ---------- */
    private readonly string smtpHost = "172.16.1.51";
    private readonly int smtpPort = 25;
    private readonly string fromEmail = "woa-system@it.patayafood.com";
    private readonly string fromPwd = "";          // ถ้าไม่ใช้ AUTH ปล่อยว่างได้

    /* ---------- 2. FOOTER ---------- */
    private readonly string footer =
        "<hr style=\"border:0;border-top:1px dashed #bbb\" />" +
        "<small style=\"color:#666\">This e-mail was sent automatically by Customer Email Send</small>";

    private bool Send(string fromAddress, string toCsv, string ccCsv, string subject, string bodyHtml, out string errorMessage)
    {
        errorMessage = "";

        try
        {
            if (string.IsNullOrWhiteSpace(fromAddress))
            {
                throw new InvalidOperationException("Sender email is empty");
            }

            using (var smtp = new SmtpClient(smtpHost, smtpPort))
            {
                // ตั้งค่า SMTP credentials ถ้ามี
                if (!string.IsNullOrEmpty(fromPwd))
                    smtp.Credentials = new System.Net.NetworkCredential(fromEmail, fromPwd);

                // สร้าง MailMessage
                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(fromAddress.Trim());
                    msg.Subject = subject ?? "";
                    msg.Body = BuildHtmlMessage(bodyHtml);
                    msg.IsBodyHtml = true;
                    msg.BodyEncoding = Encoding.UTF8;
                    msg.SubjectEncoding = Encoding.UTF8;

                    // ✅ ถ้าเป็น dev → ส่งให้เฉพาะ test
                    if (host.IsDev)
                    {
                        msg.To.Add("siripong.j@patayafood.com");
                        AddRecipientIfMissing(msg.CC, TestCcEmail);
                    }
                    else
                    {
                        // Production: ส่งให้ผู้รับจริง
                        AddRecipients(msg.To, toCsv);
                        AddRecipients(msg.CC, ccCsv);
                        AddRecipientIfMissing(msg.CC, TestCcEmail);

                        // ✅ BCC หาตัวเองทุกฉบับ (production เท่านั้น)
                        msg.Bcc.Add("siripong.j@patayafood.com");
                    }

                    // ตรวจสอบว่ามีผู้รับหรือไม่
                    if (msg.To.Count == 0 && msg.Bcc.Count == 0)
                    {
                        throw new InvalidOperationException("No recipients specified");
                    }

                    // ส่งเมล
                    smtp.Send(msg);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;

            System.Diagnostics.Debug.WriteLine(string.Format("Email send failed: {0}", ex.Message));
            return false;
        }
    }

    private void AddRecipients(MailAddressCollection collection, string recipientCsv)
    {
        if (string.IsNullOrWhiteSpace(recipientCsv))
        {
            return;
        }

        foreach (var email in recipientCsv.Split(';'))
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                collection.Add(email.Trim());
            }
        }
    }

    private void AddRecipientIfMissing(MailAddressCollection collection, string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        string normalizedEmail = email.Trim();
        foreach (MailAddress address in collection)
        {
            if (string.Equals(address.Address, normalizedEmail, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }

        collection.Add(normalizedEmail);
    }

    private string BuildHtmlMessage(string bodyHtml)
    {
        return "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head>" +
               "<body style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.6; color: #222;\">" +
               (bodyHtml ?? "") +
               footer +
               "</body></html>";
    }

    public bool SendUploadNotice(string invoiceNo, string customerCode, string customerName, string manageUrl, string sentUser, out string errorMessage)
    {
        errorMessage = "";
        string recipients = GetRecipientCsv("ALL", "NOTICE");

        if (string.IsNullOrWhiteSpace(recipients))
        {
            errorMessage = "ไม่พบ Email NOTICE สำหรับ CUSTOMER_CODE = ALL";
            WriteSceLog("NOTICE", invoiceNo, customerCode, "", "", "FAILED", errorMessage, sentUser);
            return false;
        }

        string subject = "Invoice document uploaded: " + invoiceNo;
        string body = "<p>Invoice document has been uploaded.</p>" +
                      "<p><b>Invoice No:</b> " + Encode(invoiceNo) + "<br />" +
                      "<b>Customer:</b> " + Encode(customerCode) + " " + Encode(customerName) + "</p>" +
                      "<p><a href=\"" + Encode(manageUrl) + "\">Open customer email management</a></p>";

        bool success = Send(fromEmail, recipients, "", subject, body, out errorMessage);
        WriteSceLog("NOTICE", invoiceNo, customerCode, recipients, "", subject, body, success ? "SUCCESS" : "FAILED", errorMessage, sentUser);
        return success;
    }

    public bool SendCustomerInvoice(string invoiceNo, string customerCode, string customerName, string sentUser, out string errorMessage)
    {
        errorMessage = "";
        string toRecipients = GetRecipientCsv(customerCode, "TO");
        string ccRecipients = GetRecipientCsv("ALL", "CC");

        if (string.IsNullOrWhiteSpace(toRecipients))
        {
            errorMessage = "ไม่พบ Email TO สำหรับ Customer " + customerCode;
            WriteSceLog("CUSTOMER", invoiceNo, customerCode, toRecipients, ccRecipients, "FAILED", errorMessage, sentUser);
            return false;
        }

        EmailTemplate template = GetCustomerInvoiceTemplate();
        if (template == null)
        {
            errorMessage = "ไม่พบ active template CUSTOMER_INVOICE";
            WriteSceLog("CUSTOMER", invoiceNo, customerCode, toRecipients, ccRecipients, "FAILED", errorMessage, sentUser);
            return false;
        }

        string subject = ApplyTemplateText(template.Subject, invoiceNo, customerCode, customerName);
        string body = ApplyTemplateBody(template.Body, invoiceNo, customerCode, customerName);

        bool success = Send(fromEmail, toRecipients, ccRecipients, subject, body, out errorMessage);
        WriteSceLog("CUSTOMER", invoiceNo, customerCode, toRecipients, ccRecipients, subject, body, success ? "SUCCESS" : "FAILED", errorMessage, sentUser);

        if (success)
        {
            MarkCustomerEmailSent(invoiceNo, sentUser);
        }

        return success;
    }

    private string GetRecipientCsv(string customerCode, string recipientType)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT EMAIL_ADDRESS
              FROM ITPROD.SHCUMAILD
             WHERE CUSTOMER_CODE = @CUSTOMER_CODE
               AND RECIPIENT_TYPE = @RECIPIENT_TYPE
               AND ACTIVE_STATUS = 'Y'
             ORDER BY EMAIL_SEQ";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@CUSTOMER_CODE", customerCode);
        param.Add("@RECIPIENT_TYPE", recipientType);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError || dt.Rows.Count == 0)
        {
            return "";
        }

        List<string> emails = new List<string>();
        foreach (DataRow row in dt.Rows)
        {
            string email = Convert.ToString(row["EMAIL_ADDRESS"]).Trim();
            if (!string.IsNullOrEmpty(email))
            {
                emails.Add(email);
            }
        }

        return string.Join(";", emails.ToArray());
    }

    private EmailTemplate GetCustomerInvoiceTemplate()
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT TEMPLATE_CODE, SUBJECT_TEMPLATE, BODY_TEMPLATE
              FROM ITPROD.SHCUMAILT
             WHERE TEMPLATE_CODE = @TEMPLATE_CODE
               AND ACTIVE_STATUS = 'Y'";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@TEMPLATE_CODE", CustomerInvoiceTemplateCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError || dt.Rows.Count == 0)
        {
            return null;
        }

        return new EmailTemplate
        {
            Subject = Convert.ToString(dt.Rows[0]["SUBJECT_TEMPLATE"]),
            Body = Convert.ToString(dt.Rows[0]["BODY_TEMPLATE"])
        };
    }

    private void MarkCustomerEmailSent(string invoiceNo, string sentUser)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            UPDATE ITPROD.SHDOCH
               SET SHMLST = 'S',
                   SHMLTS = CURRENT_TIMESTAMP,
                   SHMLUS = @SHMLUS,
                   SHMLCT = COALESCE(SHMLCT, 0) + 1
             WHERE SHCONO = 100
               AND SHDIVI = 'PFT'
               AND SHIVNO = @SHIVNO";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SHMLUS", sentUser ?? "");
        param.Add("@SHIVNO", invoiceNo);
        db.ExecuteNonQuery(sql, param);
    }

    private void WriteSceLog(string mailKind, string invoiceNo, string customerCode, string toRecipients, string ccRecipients, string status, string errorMessage, string sentUser)
    {
        WriteSceLog(mailKind, invoiceNo, customerCode, toRecipients, ccRecipients, "", "", status, errorMessage, sentUser);
    }

    private void WriteSceLog(string mailKind, string invoiceNo, string customerCode, string toRecipients, string ccRecipients, string subject, string body, string status, string errorMessage, string sentUser)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            INSERT INTO ITPROD.SHCUEMAILL
                (MAIL_KIND, INVOICE_NO, CUSTOMER_CODE, TO_RECIPIENTS, CC_RECIPIENTS, SUBJECT_TEXT, BODY_TEXT, STATUS, ERROR_MSG, SENT_USER, SENT_TIMESTAMP)
            VALUES
                (@MAIL_KIND, @INVOICE_NO, @CUSTOMER_CODE, @TO_RECIPIENTS, @CC_RECIPIENTS, @SUBJECT_TEXT, @BODY_TEXT, @STATUS, @ERROR_MSG, @SENT_USER, CURRENT_TIMESTAMP)";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@MAIL_KIND", mailKind ?? "");
        param.Add("@INVOICE_NO", invoiceNo ?? "");
        param.Add("@CUSTOMER_CODE", customerCode ?? "");
        param.Add("@TO_RECIPIENTS", toRecipients ?? "");
        param.Add("@CC_RECIPIENTS", ccRecipients ?? "");
        param.Add("@SUBJECT_TEXT", subject ?? "");
        param.Add("@BODY_TEXT", body ?? "");
        param.Add("@STATUS", status ?? "");
        param.Add("@ERROR_MSG", errorMessage ?? "");
        param.Add("@SENT_USER", sentUser ?? "");
        db.InsertData(sql, param);
    }

    private string ApplyTemplateText(string value, string invoiceNo, string customerCode, string customerName)
    {
        string result = value ?? "";
        result = result.Replace("{INVOICE_NO}", invoiceNo ?? "");
        result = result.Replace("{CUSTOMER_CODE}", customerCode ?? "");
        result = result.Replace("{CUSTOMER_NAME}", customerName ?? "");
        result = result.Replace("{SEND_DATE}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        return result;
    }

    private string ApplyTemplateBody(string value, string invoiceNo, string customerCode, string customerName)
    {
        string template = value ?? "";

        if (!LooksLikeHtml(template))
        {
            template = Encode(template)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", "<br />\r\n");
        }

        string result = template;
        result = result.Replace("{INVOICE_NO}", Encode(invoiceNo));
        result = result.Replace("{CUSTOMER_CODE}", Encode(customerCode));
        result = result.Replace("{CUSTOMER_NAME}", Encode(customerName));
        result = result.Replace("{SEND_DATE}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        return result;
    }

    private bool LooksLikeHtml(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string lowerValue = value.ToLowerInvariant();
        return lowerValue.Contains("<br") ||
               lowerValue.Contains("<p") ||
               lowerValue.Contains("<div") ||
               lowerValue.Contains("<table") ||
               lowerValue.Contains("<span") ||
               lowerValue.Contains("<strong") ||
               lowerValue.Contains("<b") ||
               lowerValue.Contains("<ul") ||
               lowerValue.Contains("<ol") ||
               lowerValue.Contains("<li");
    }

    private string Encode(string value)
    {
        return System.Web.HttpUtility.HtmlEncode(value ?? "");
    }

    private string GetEmailByPersonCode(string personCode)
    {
        DataTable dt = GetPersonDetail(personCode);
        if (dt.Rows.Count == 0)
        {
            return "";
        }

        return Convert.ToString(dt.Rows[0]["Email"]).Trim();
    }

    private DataTable GetPersonDetail(string personCode)
    {
        personCode = (personCode ?? "").Trim();
        if (string.IsNullOrEmpty(personCode))
        {
            return new DataTable();
        }

        string escapedPersonCode = personCode.Replace("'", "''");
        string query = string.Format(@"
            SELECT TOP 1
                   ISNULL(FnameT, '') AS FnameT,
                   ISNULL(LnameT, '') AS LnameT,
                   ISNULL(Email, '') AS Email
              FROM [HRIS].[dbo].[PersonDetail]
             WHERE RTRIM(LTRIM(PersonCode)) = '{0}'", escapedPersonCode);

        dbConnectSQL db = new dbConnectSQL(7);
        DataTable dt = db.ExecuteQuery(query);
        return db.isError ? new DataTable() : dt;
    }

    private class EmailTemplate
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

}
