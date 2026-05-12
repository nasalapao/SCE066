using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Data;              // เผื่อมีการคืนค่า DataTable
using System.IO;
                                // ไม่ต้อง using IBM.Data.DB2.iSeries อีก

public class EmailSender
{
    private const string CustomerInvoiceTemplateCode = "CUSTOMER_INVOICE";
    private const string TestCcEmail = "nasalapao@gmail.com";

    /* ---------- 1. SMTP CONFIG ---------- */
    private readonly string smtpHost = "smtp.office365.com";
    private readonly int smtpPort = 587;
    private readonly bool smtpEnableSsl = true;
    private readonly string smtpTargetName = "STARTTLS/smtp.office365.com";
    private readonly string legacySmtpHost = "172.16.1.51";
    private readonly int legacySmtpPort = 25;
    private readonly string legacyFromEmail = " @it.patayafood.com";

    /* ---------- 2. FOOTER ---------- */
    private readonly string footer =
        "<hr style=\"border:0;border-top:1px dashed #bbb\" />" +
        "<small style=\"color:#666\">This e-mail was sent automatically by Customer Email Send</small>";

    private bool Send(string senderEmail, string toCsv, string ccCsv, string subject, string bodyHtml, out string errorMessage)
    {
        return Send(senderEmail, toCsv, ccCsv, subject, bodyHtml, null, out errorMessage);
    }

    private bool Send(string senderEmail, string toCsv, string ccCsv, string subject, string bodyHtml, List<string> attachmentPaths, out string errorMessage)
    {
        errorMessage = "";

        try
        {
            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                throw new InvalidOperationException("Sender email is empty");
            }

            SenderConfig senderConfig = GetActiveSenderConfig(senderEmail, out errorMessage);
            if (senderConfig == null)
            {
                return false;
            }

            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | (SecurityProtocolType)3072;

            using (var smtp = new SmtpClient(smtpHost, smtpPort))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(senderConfig.SenderEmail, senderConfig.AppPassword);
                smtp.EnableSsl = smtpEnableSsl;

                if (!string.IsNullOrWhiteSpace(smtpTargetName))
                {
                    smtp.TargetName = smtpTargetName;
                }

                // สร้าง MailMessage
                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(senderConfig.SenderEmail);
                    msg.Subject = subject ?? "";
                    msg.Body = BuildHtmlMessage(bodyHtml);
                    msg.IsBodyHtml = true;
                    msg.BodyEncoding = Encoding.UTF8;
                    msg.SubjectEncoding = Encoding.UTF8;
                    AddAttachments(msg, attachmentPaths);

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
            errorMessage = BuildExceptionMessage(ex);

            System.Diagnostics.Debug.WriteLine(string.Format("Email send failed: {0}", errorMessage));
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

    private void AddAttachments(MailMessage message, List<string> attachmentPaths)
    {
        if (attachmentPaths == null)
        {
            return;
        }

        foreach (string attachmentPath in attachmentPaths)
        {
            if (!string.IsNullOrWhiteSpace(attachmentPath))
            {
                message.Attachments.Add(new Attachment(attachmentPath.Trim()));
            }
        }
    }

    private string BuildHtmlMessage(string bodyHtml)
    {
        return "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head>" +
               "<body style=\"font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 1.6; color: #222;\">" +
               (bodyHtml ?? "") +
               footer +
               "</body></html>";
    }

    private string BuildExceptionMessage(Exception ex)
    {
        List<string> messages = new List<string>();
        Exception current = ex;
        while (current != null)
        {
            if (!string.IsNullOrWhiteSpace(current.Message))
            {
                messages.Add(current.Message);
            }

            current = current.InnerException;
        }

        string message = string.Join(" | ", messages.ToArray());
        if (message.IndexOf("5.7.139", StringComparison.OrdinalIgnoreCase) >= 0 ||
            message.IndexOf("535", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            message += " | ตรวจสอบ App Password ของบัญชี sender, การเปิด SMTP AUTH, และสถานะ MFA/App password ใน Microsoft 365";
        }

        return message;
    }

    private bool SendByLegacySmtp(string toCsv, string ccCsv, string subject, string bodyHtml, out string errorMessage)
    {
        errorMessage = "";

        try
        {
            using (var smtp = new SmtpClient(legacySmtpHost, legacySmtpPort))
            {
                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(legacyFromEmail);
                    msg.Subject = subject ?? "";
                    msg.Body = BuildHtmlMessage(bodyHtml);
                    msg.IsBodyHtml = true;
                    msg.BodyEncoding = Encoding.UTF8;
                    msg.SubjectEncoding = Encoding.UTF8;

                    if (host.IsDev)
                    {
                        msg.To.Add("siripong.j@patayafood.com");
                    }
                    else
                    {
                        AddRecipients(msg.To, toCsv);
                        AddRecipients(msg.CC, ccCsv);
                    }

                    if (msg.To.Count == 0 && msg.Bcc.Count == 0)
                    {
                        throw new InvalidOperationException("No recipients specified");
                    }

                    smtp.Send(msg);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = BuildExceptionMessage(ex);
            System.Diagnostics.Debug.WriteLine(string.Format("Legacy email send failed: {0}", errorMessage));
            return false;
        }
    }

    private SenderConfig GetActiveSenderConfig(string senderEmailValue, out string errorMessage)
    {
        errorMessage = "";

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT SENDER_EMAIL, APP_PASSWORD
              FROM ITPROD.SHCUMAILS
             WHERE TRIM(SENDER_EMAIL) = @SENDER_EMAIL
               AND ACTIVE_STATUS = 'Y'";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SENDER_EMAIL", (senderEmailValue ?? "").Trim());

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            errorMessage = db.ErrorMessage;
            return null;
        }

        if (dt.Rows.Count == 0)
        {
            errorMessage = "ไม่พบ active email sender " + senderEmailValue;
            return null;
        }

        if (dt.Rows.Count > 1)
        {
            errorMessage = "พบ active email sender ซ้ำกัน " + senderEmailValue;
            return null;
        }

        string senderEmail = Convert.ToString(dt.Rows[0]["SENDER_EMAIL"]).Trim();
        string appPassword = NormalizeAppPassword(Convert.ToString(dt.Rows[0]["APP_PASSWORD"]));

        if (string.IsNullOrWhiteSpace(senderEmail))
        {
            errorMessage = "SMTP sender email is empty";
            return null;
        }

        if (string.IsNullOrWhiteSpace(appPassword))
        {
            errorMessage = "SMTP App Password is empty";
            return null;
        }

        return new SenderConfig
        {
            SenderEmail = senderEmail,
            AppPassword = appPassword
        };
    }

    public bool SendTestEmail(string senderEmail, string appPassword, string testToEmail, out string errorMessage)
    {
        errorMessage = "";

        try
        {
            senderEmail = (senderEmail ?? "").Trim();
            appPassword = NormalizeAppPassword(appPassword);
            testToEmail = (testToEmail ?? "").Trim();

            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                throw new InvalidOperationException("SMTP sender email is empty");
            }

            if (string.IsNullOrWhiteSpace(appPassword))
            {
                throw new InvalidOperationException("SMTP App Password is empty");
            }

            if (string.IsNullOrWhiteSpace(testToEmail))
            {
                throw new InvalidOperationException("Test TO email is empty");
            }

            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | (SecurityProtocolType)3072;

            using (var smtp = new SmtpClient(smtpHost, smtpPort))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(senderEmail, appPassword);
                smtp.EnableSsl = smtpEnableSsl;

                if (!string.IsNullOrWhiteSpace(smtpTargetName))
                {
                    smtp.TargetName = smtpTargetName;
                }

                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(senderEmail);
                    msg.To.Add(testToEmail);
                    msg.Subject = "  SMTP sender test";
                    msg.Body = BuildHtmlMessage("<p>This is a test email from   Customer Email Sender.</p>");
                    msg.IsBodyHtml = true;
                    msg.BodyEncoding = Encoding.UTF8;
                    msg.SubjectEncoding = Encoding.UTF8;

                    smtp.Send(msg);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = BuildExceptionMessage(ex);
            System.Diagnostics.Debug.WriteLine(string.Format("Email sender test failed: {0}", errorMessage));
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

    public bool SendUploadNotice(string invoiceNo, string customerCode, string customerName, string manageUrl, string sentUser, out string errorMessage)
    {
        errorMessage = "";
        string recipients = host.IsDev ? "siripong.j@patayafood.com" : GetRecipientCsv("ALL", "NOTICE");

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

        bool success = SendByLegacySmtp(recipients, "", subject, body, out errorMessage);
        WriteSceLog("NOTICE", invoiceNo, customerCode, recipients, "", subject, body, success ? "SUCCESS" : "FAILED", errorMessage, sentUser);
        return success;
    }

    public bool SendDeleteNotice(string invoiceNo, string customerCode, string customerName, string docType, string revision, string filePath, string sentUser, out string errorMessage)
    {
        errorMessage = "";
        string recipients = host.IsDev ? "siripong.j@patayafood.com" : GetRecipientCsv("ALL", "NOTICE");

        if (string.IsNullOrWhiteSpace(recipients))
        {
            errorMessage = "ไม่พบ Email NOTICE สำหรับ CUSTOMER_CODE = ALL";
            WriteSceLog("NOTICE_DELETE", invoiceNo, customerCode, "", "", "FAILED", errorMessage, sentUser);
            return false;
        }

        string subject = "Invoice document deleted: " + invoiceNo;
        string body = "<p>Invoice document has been deleted.</p>" +
                      "<p><b>Invoice No:</b> " + Encode(invoiceNo) + "<br />" +
                      "<b>Customer:</b> " + Encode(customerCode) + " " + Encode(customerName) + "<br />" +
                      "<b>Doc Type:</b> " + Encode(docType) + "<br />" +
                      "<b>Revision:</b> " + Encode(revision) + "<br />" +
                      "<b>File:</b> " + Encode(filePath) + "</p>";

        bool success = SendByLegacySmtp(recipients, "", subject, body, out errorMessage);
        WriteSceLog("NOTICE_DELETE", invoiceNo, customerCode, recipients, "", subject, body, success ? "SUCCESS" : "FAILED", errorMessage, sentUser);
        return success;
    }

    public bool SendCustomerInvoice(string invoiceNo, string customerCode, string customerName, string sentUser, out string errorMessage)
    {
        errorMessage = "";
        SenderConfig customerSender = GetCustomerInvoiceSender(customerCode, out errorMessage);
        if (customerSender == null)
        {
            WriteSceLog("CUSTOMER", invoiceNo, customerCode, "", "", "FAILED", errorMessage, sentUser);
            return false;
        }

        string toRecipients = GetRecipientCsv(customerCode, "TO", customerSender.SenderEmail);
        string ccRecipients = GetRecipientCsv("ALL", "CC", customerSender.SenderEmail);

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
        List<string> attachmentPaths = GetInvoiceAttachmentPaths(invoiceNo, out errorMessage);
        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            WriteSceLog("CUSTOMER", invoiceNo, customerCode, toRecipients, ccRecipients, subject, body, "FAILED", errorMessage, sentUser);
            return false;
        }

        bool success = Send(customerSender.SenderEmail, toRecipients, ccRecipients, subject, body, attachmentPaths, out errorMessage);
        WriteSceLog("CUSTOMER", invoiceNo, customerCode, toRecipients, ccRecipients, subject, body, success ? "SUCCESS" : "FAILED", errorMessage, sentUser);

        if (success)
        {
            MarkCustomerEmailSent(invoiceNo, sentUser);
        }

        return success;
    }

    private SenderConfig GetCustomerInvoiceSender(string customerCode, out string errorMessage)
    {
        errorMessage = "";

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT DISTINCT COALESCE(TRIM(SENDER_NAME), '') AS SENDER_NAME,
                            COALESCE(TRIM(SENDER_EMAIL), '') AS SENDER_EMAIL
              FROM ITPROD.SHCUMAILD
             WHERE TRIM(CUSTOMER_CODE) = @CUSTOMER_CODE
               AND COALESCE(TRIM(SENDER_EMAIL), '') <> ''";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@CUSTOMER_CODE", (customerCode ?? "").Trim());

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            errorMessage = db.ErrorMessage;
            return null;
        }

        if (dt.Rows.Count == 0)
        {
            errorMessage = "ไม่พบ Sender สำหรับ Customer " + customerCode;
            return null;
        }

        if (dt.Rows.Count > 1)
        {
            errorMessage = "Customer " + customerCode + " มี Sender มากกว่า 1 คน กรุณาแก้ไขที่ AdminEmail.aspx";
            return null;
        }

        string senderEmail = Convert.ToString(dt.Rows[0]["SENDER_EMAIL"]).Trim();
        if (string.IsNullOrWhiteSpace(senderEmail))
        {
            errorMessage = "Sender Email is empty for Customer " + customerCode;
            return null;
        }

        SenderConfig activeSender = GetActiveSenderConfig(senderEmail, out errorMessage);
        if (activeSender == null)
        {
            return null;
        }

        activeSender.SenderName = Convert.ToString(dt.Rows[0]["SENDER_NAME"]).Trim();
        return activeSender;
    }

    private string GetRecipientCsv(string customerCode, string recipientType)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT TRIM(EMAIL_ADDRESS) AS EMAIL_ADDRESS
              FROM ITPROD.SHCUMAILD
             WHERE TRIM(CUSTOMER_CODE) = @CUSTOMER_CODE
               AND TRIM(RECIPIENT_TYPE) = @RECIPIENT_TYPE
               AND TRIM(ACTIVE_STATUS) = 'Y'
             ORDER BY EMAIL_SEQ";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@CUSTOMER_CODE", (customerCode ?? "").Trim());
        param.Add("@RECIPIENT_TYPE", (recipientType ?? "").Trim());

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

    private string GetRecipientCsv(string customerCode, string recipientType, string senderEmail)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT TRIM(EMAIL_ADDRESS) AS EMAIL_ADDRESS
              FROM ITPROD.SHCUMAILD
             WHERE TRIM(CUSTOMER_CODE) = @CUSTOMER_CODE
               AND TRIM(RECIPIENT_TYPE) = @RECIPIENT_TYPE
               AND TRIM(SENDER_EMAIL) = @SENDER_EMAIL
               AND TRIM(ACTIVE_STATUS) = 'Y'
             ORDER BY EMAIL_SEQ";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@CUSTOMER_CODE", (customerCode ?? "").Trim());
        param.Add("@RECIPIENT_TYPE", (recipientType ?? "").Trim());
        param.Add("@SENDER_EMAIL", (senderEmail ?? "").Trim());

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

    private List<string> GetInvoiceAttachmentPaths(string invoiceNo, out string errorMessage)
    {
        errorMessage = "";
        List<string> attachmentPaths = new List<string>();

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT SLFNAM
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
               )
             ORDER BY SLFNAM";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@SLIVNO", invoiceNo);
        param.Add("@SLIVNO_MAX", invoiceNo);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            errorMessage = db.ErrorMessage;
            return attachmentPaths;
        }

        if (dt.Rows.Count == 0)
        {
            errorMessage = "ไม่พบไฟล์ INV สำหรับแนบ Invoice " + invoiceNo;
            return attachmentPaths;
        }

        foreach (DataRow row in dt.Rows)
        {
            string filePath = Convert.ToString(row["SLFNAM"]).Trim();
            if (string.IsNullOrEmpty(filePath))
            {
                continue;
            }

            if (!File.Exists(filePath))
            {
                errorMessage = "ไม่พบไฟล์แนบ: " + filePath;
                return attachmentPaths;
            }

            attachmentPaths.Add(filePath);
        }

        if (attachmentPaths.Count == 0)
        {
            errorMessage = "ไม่พบไฟล์ INV สำหรับแนบ Invoice " + invoiceNo;
        }

        return attachmentPaths;
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

    private class SenderConfig
    {
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string AppPassword { get; set; }
    }

}
