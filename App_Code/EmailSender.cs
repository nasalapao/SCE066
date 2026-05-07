using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Data;              // เผื่อมีการคืนค่า DataTable
                                // ไม่ต้อง using IBM.Data.DB2.iSeries อีก

public class EmailSender
{
    /* ---------- 1. SMTP CONFIG ---------- */
    private readonly string smtpHost = "172.16.1.51";
    private readonly int smtpPort = 25;
    private readonly string fromEmail = "woa-system@it.patayafood.com";
    private readonly string fromPwd = "";          // ถ้าไม่ใช้ AUTH ปล่อยว่างได้

    /* ---------- 2. FOOTER ---------- */
    private readonly string footer =
        "<hr style=\"border:0;border-top:1px dashed #bbb\" />" +
        "<small style=\"color:#666\">This e-mail was sent automatically by WOA-System</small>";

    /* ================================================================
       PUBLIC: ส่งอีเมล + LOG
       ================================================================ */

    public bool Send(string recipientCsv, string subject, string bodyHtml, string docNo = null)
    {
        string status;
        string errMsg = null;

        try
        {
            using (var smtp = new SmtpClient(smtpHost, smtpPort))
            {
                // ตั้งค่า SMTP credentials ถ้ามี
                if (!string.IsNullOrEmpty(fromPwd))
                    smtp.Credentials = new System.Net.NetworkCredential(fromEmail, fromPwd);

                // สร้าง MailMessage
                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(fromEmail);
                    msg.Subject = subject ?? "";
                    msg.Body = (bodyHtml ?? "") + footer;
                    msg.IsBodyHtml = true;

                    // ✅ ถ้าเป็น dev → ส่งให้เฉพาะ test
                    if (host.IsDev)
                    {
                        msg.To.Add("siripong.j@patayafood.com");
                    }
                    else
                    {
                        // Production: ส่งให้ผู้รับจริง
                        if (!string.IsNullOrWhiteSpace(recipientCsv))
                        {
                            foreach (var email in recipientCsv.Split(';'))
                            {
                                if (!string.IsNullOrWhiteSpace(email))
                                {
                                    msg.To.Add(email.Trim());
                                }
                            }
                        }

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

            status = "SUCCESS";
        }
        catch (Exception ex)
        {
            status = "FAILED";
            errMsg = ex.Message;

            // Log error สำหรับ debugging
            System.Diagnostics.Debug.WriteLine(string.Format("Email send failed: {0}", ex.Message));
        }

        /* ---------- LOG ---------- */
        WriteLog(recipientCsv, subject, bodyHtml, status, errMsg, docNo);

        return status == "SUCCESS";
    }






    /* ================================================================
       PRIVATE: เขียน LOG ด้วย dbConnect
       ================================================================ */
    private void WriteLog(string recipients, string subject, string body,
                          string status, string errorMsg, string docNo)
    {
        const string sql = @"
            INSERT INTO ITPROD.WOA_EMAILLOG
                   (RECIPIENTS, SUBJECT, BODY, STATUS, ERROR_MSG, DOC_NO)
            VALUES  (@RECIPIENTS, @SUBJECT, @BODY, @STATUS, @ERROR_MSG, @DOC_NO)";

        var param = new Dictionary<string, object>();
        param.Add("@RECIPIENTS", recipients ?? "");
        param.Add("@SUBJECT", subject ?? "");
        param.Add("@BODY", body ?? "");   // VARGRAPHIC (CCSID 13488) รองรับ Unicode
        param.Add("@STATUS", status);
        param.Add("@ERROR_MSG", errorMsg ?? "");
        param.Add("@DOC_NO", docNo ?? "");

        var db = new dbConnect();
        int rows = db.InsertData(sql, param);

        /* หากเขียน LOG ไม่ได้ จะไม่ throw loop; บันทึก Debug แทน */
        if (db.isError)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Cannot write WOA_EMAILLOG: {0}", db.ErrorMessage));
        }
    }

    /* ================================================================
       OPTIONAL: ดึง LOG
       ================================================================ */
    public DataTable GetLogs(string docNo = null)
    {
        string sql = @"
            SELECT LOG_ID, SEND_DATE, RECIPIENTS, SUBJECT, STATUS, ERROR_MSG, DOC_NO
              FROM ITPROD.WOA_EMAILLOG";

        var prms = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(docNo))
        {
            sql += " WHERE DOC_NO = @DOC_NO";
            prms["@DOC_NO"] = docNo;
        }

        sql += " ORDER BY SEND_DATE DESC";

        var db = new dbConnect();
        return db.ExecuteQuery(sql, prms);
    }


    public string GetEmailByPersonCodeHRIS(string personCode)
    {
        if (string.IsNullOrWhiteSpace(personCode))
            return "";

        string email = "";

        try
        {
            // SQL ป้องกัน SQL Injection ด้วย parameter (หากต้องการความปลอดภัยเพิ่ม)
            string query = string.Format(
                "SELECT [Email] FROM [HRIS].[dbo].[PersonDetail] WHERE [PersonCode] = '{0}'",
                personCode.Replace("'", "''") // escape single quote
            );

            dbConnectSQL db = new dbConnectSQL("Server=172.16.33.7;database=HRIS;UID=sa;PASSWORD=itadmin;");
            var dt = db.ExecuteQuery(query);

            if (!db.isError && dt.Rows.Count > 0)
            {
                email = dt.Rows[0]["Email"].ToString().Trim();
            }
        }
        catch (Exception ex)
        {
            // สามารถ log error เพิ่มเติมได้ที่นี่
            email = "";
        }

        return email;
    }
    public string GetNameFromHRIS(string personCode)
    {
        if (string.IsNullOrWhiteSpace(personCode))
            return "";

        string fullName = "";
        try
        {
            
            string escapedPersonCode = personCode.Replace("'", "''");
            string query = string.Format(@"
            SELECT [FnameE], [LnameE]
            FROM [HRIS].[dbo].[PersonDetail]
            WHERE [PersonCode] = '{0}'", escapedPersonCode);

            dbConnectSQL db = new dbConnectSQL("Server=172.16.33.7;database=HRIS;UID=sa;PASSWORD=itadmin;");
            var dt = db.ExecuteQuery(query);

            if (!db.isError && dt.Rows.Count > 0)
            {
               
                string firstName = dt.Rows[0]["FnameE"] == DBNull.Value ? "" : dt.Rows[0]["FnameE"].ToString().Trim();
                string lastName = dt.Rows[0]["LnameE"] == DBNull.Value ? "" : dt.Rows[0]["LnameE"].ToString().Trim();


                if (!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName))
                {
                    fullName = (firstName + " " + lastName).Trim();
                }
                else
                {
                    fullName = string.Empty;
                }

            }
        }
        catch (Exception )
        {
            fullName = "";
        }
        return fullName;
    }
}
