using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using IBM.Data.DB2.iSeries;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Net;
using System.IO;

using System.Net;
using System.Net.Mail;


public partial class SCE066_1 : System.Web.UI.Page
{
    iDB2Connection connection = new iDB2Connection("DataSource=172.16.33.49;UserID=mvxreport;Password=report;DataCompression=True;");
    iDB2Connection connection1 = new iDB2Connection("DataSource=172.16.33.49;UserID=mvxreport;Password=report;DataCompression=True;");
    private const string ClosedMessage = "ใบนี้จบงานแล้ว ไม่สามารถ Upload, Delete หรือ Edit ได้";

    private void ClearError()
    {
        lbError.Text = "";
        lbError.Visible = false;
    }

    private void ShowError(string message)
    {
        lbError.Text = message;
        lbError.Visible = true;
    }

    private void EnsureConnectionClosed()
    {
        if (connection.State != ConnectionState.Closed)
        {
            connection.Close();
        }
    }

    private bool ReadJobClosedStatus()
    {
        bool isClosed = false;

        try
        {
            EnsureConnectionClosed();
            connection.Open();

            string sql_status = "select shstsd from itprod.shdoch " +
                                " where shcono = 100 and shdivi = 'PFT' " +
                                " and shivno = '" + txtIVNO.Text.Trim() + "'";

            iDB2Command comm_status = new iDB2Command(sql_status, connection);
            object result = comm_status.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                isClosed = result.ToString().Trim().Equals("1");
            }
        }
        finally
        {
            EnsureConnectionClosed();
        }

        ViewState["IsJobClosed"] = isClosed;
        return isClosed;
    }

    private bool GetJobClosedStatus()
    {
        object cachedStatus = ViewState["IsJobClosed"];

        if (cachedStatus != null)
        {
            return (bool)cachedStatus;
        }

        return ReadJobClosedStatus();
    }

    private void ApplyClosedState(bool isClosed)
    {
        FileUpload1.Enabled = !isClosed;
        imgAddPDF.Enabled = !isClosed;

        FileUpload1.CssClass = isClosed ? "field-control field-disabled" : "field-control";
        imgAddPDF.CssClass = isClosed ? "icon-action upload-action action-disabled" : "icon-action upload-action";
    }

   
    protected void Page_Load(object sender, EventArgs e)
    {
         if (!Page.IsPostBack)
          {
            ClearError();

            if (!string.IsNullOrEmpty(Request.QueryString.Get("VIEWREVI")))
            {
                list();
                OpenDocument(Request.QueryString.Get("VIEWREVI"));
                return;
            }

            list();
            bool isClosed = ReadJobClosedStatus();
            ShowData();
            ApplyClosedState(isClosed);

            if (isClosed)
            {
                ShowError(ClosedMessage);
            }

         }
         else
         {
            ApplyClosedState(GetJobClosedStatus());
         }

    }


    private void list()
    {
        INVNO.Value = Request.QueryString.Get("INVNO");
        DATU.Value = Request.QueryString.Get("DATU");
        DATEFROM.Value = Request.QueryString.Get("DATEFROM");
        DATETO.Value = Request.QueryString.Get("DATETO");
        PASSW.Value = Request.QueryString.Get("PASSW");
        STATUSD.Value = Request.QueryString.Get("STATUSD");

        CUNO.Value = Request.QueryString.Get("CUNO");

        

        txtIVNO.Text = INVNO.Value;
        ddlDATU.Text = DATU.Value;
        
        //txtDATEFROM.Text = DATEFROM.Value;

    }

    private void ShowData()
    {
        try
        {
            EnsureConnectionClosed();
            connection.Open();

            string sql_rel = "SELECT * from itprod.shdocl " +
                      " where  slcono = 100 and sldivi = 'PFT' " +
                      " and slivno = '" + txtIVNO.Text.Trim() + "'" +
                      " and sldatu = '" + ddlDATU.Text + "'" +
                      " order by slrevi desc ";

            iDB2DataAdapter da = new iDB2DataAdapter(sql_rel, connection);
            DataSet ds = new DataSet();

            da.Fill(ds);
            GridView1.DataSource = ds.Tables[0];
            GridView1.DataBind();
        }
        catch (Exception ex)
        {
            ShowError("โหลดข้อมูลเอกสารไม่สำเร็จ : " + ex.Message);
        }
        finally
        {
            EnsureConnectionClosed();
        }
    }

   
    protected void btnAddPDF_Click(object sender, EventArgs e)
    {
        

    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ClearError();

        if (e.CommandName.Equals("DELDOC"))
        {
            try
            {
                if (ReadJobClosedStatus())
                {
                    ShowError(ClosedMessage);
                    ApplyClosedState(true);
                    return;
                }

                int rowIndex = Convert.ToInt32(e.CommandArgument);
                string revi = ((Label)GridView1.Rows[rowIndex].FindControl("lblREVI")).Text.Trim();
                string filePath = ((Label)GridView1.Rows[rowIndex].FindControl("lblNAME")).Text.Trim();

                if (PASSW.Value.Equals("AUDIT"))
                {
                    ShowError("สิทธิ์นี้ไม่สามารถลบเอกสารได้");
                    return;
                }

                EnsureConnectionClosed();
                connection.Open();

                string sql_delete = "delete from itprod.shdocl " +
                                    " where slcono = 100 and sldivi = 'PFT' " +
                                    " and slivno = '" + txtIVNO.Text.Trim() + "'" +
                                    " and sldatu = '" + ddlDATU.Text + "'" +
                                    " and slrevi = " + revi;

                iDB2Command comm_delete = new iDB2Command(sql_delete, connection);
                comm_delete.ExecuteNonQuery();

                ResetHeaderDocumentStatus();
                EnsureConnectionClosed();

                try
                {
                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch
                {
                }

                ShowData();
            }
            catch (Exception ex)
            {
                ShowError("ลบเอกสารไม่สำเร็จ : " + ex.Message);
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }
    }

    private string GetHeaderDateField()
    {
        return "shdat" + ddlDATU.Text.Trim();
    }

    private string GetHeaderStatusField()
    {
        return "shsts" + ddlDATU.Text.Trim();
    }

    private bool IsDuplicateDocument(string filePath)
    {
        string sql_dup = "select count(*) as cnt from itprod.shdocl " +
                         " where slcono = 100 and sldivi = 'PFT' " +
                         " and slivno = '" + txtIVNO.Text.Trim() + "'" +
                         " and sldatu = '" + ddlDATU.Text + "'" +
                         " and slfnam = '" + filePath + "'";

        iDB2Command comm_dup = new iDB2Command(sql_dup, connection);
        iDB2DataReader reader_dup = comm_dup.ExecuteReader();

        if (reader_dup.Read())
        {
            return int.Parse(reader_dup["cnt"].ToString()) > 0;
        }

        return false;
    }

    private void ResetHeaderDocumentStatus()
    {
        string dateField = GetHeaderDateField();
        string statusField = GetHeaderStatusField();

        string sql_last = "select max(sldate) as lastdate from itprod.shdocl " +
                          " where slcono = 100 and sldivi = 'PFT' " +
                          " and slivno = '" + txtIVNO.Text.Trim() + "'" +
                          " and sldatu = '" + ddlDATU.Text + "'";

        iDB2Command comm_last = new iDB2Command(sql_last, connection);
        iDB2DataReader reader_last = comm_last.ExecuteReader();

        string lastDate = "";
        if (reader_last.Read() && reader_last["lastdate"] != DBNull.Value)
        {
            lastDate = reader_last["lastdate"].ToString().Trim();
        }

        string dateValue = lastDate.Equals("") && ddlDATU.Text.Trim().Equals("1")
            ? "NULL"
            : (lastDate.Equals("") ? "0" : lastDate);

        string sql_uphead = "update itprod.shdoch set " +
                            dateField + " = " + dateValue + ", " +
                            statusField + " = '' " +
                            " where shcono = 100 and shdivi = 'PFT' " +
                            " and shivno = '" + txtIVNO.Text.Trim() + "'";

        iDB2Command comm_uphead = new iDB2Command(sql_uphead, connection);
        comm_uphead.ExecuteNonQuery();
    }

    private void OpenDocument(string revi)
    {
        try
        {
            EnsureConnectionClosed();
            connection.Open();

            string sql_rel = "SELECT slfnam,slfext from itprod.shdocl " +
                      " where  slcono = 100 and sldivi = 'PFT' " +
                      " and slivno = '" + txtIVNO.Text.Trim() + "'" +
                      " and sldatu = '" + ddlDATU.Text + "'" +
                      " and slrevi = " + revi;

            iDB2Command cmd = new iDB2Command(sql_rel, connection);
            iDB2DataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                String filelocation = reader["slfnam"].ToString().Trim();
                string fileext = reader["slfext"].ToString().Trim().ToLowerInvariant();

                WebClient User = new WebClient();
                Byte[] FileBuffer = User.DownloadData(filelocation);

                if (FileBuffer != null)
                {
                    Response.Clear();

                    if (fileext.Equals(".pdf"))
                    {
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("content-length", FileBuffer.Length.ToString());
                        Response.AddHeader("Content-Disposition", "inline; filename=Document.pdf");
                    }
                    else if (fileext.Equals(".xls"))
                    {
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.AppendHeader("content-disposition", "attachment; filename=Document.xls");
                    }
                    else if (fileext.Equals(".xlsx"))
                    {
                        Response.ContentType = "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AppendHeader("content-disposition", "attachment; filename=Document.xlsx");
                    }
                    else if (fileext.Equals(".docx"))
                    {
                        Response.ContentType = "application/vnd.ms-word.document";
                        Response.AddHeader("Content-Disposition", "attachment; filename=Document.docx");
                    }
                    else if (fileext.Equals(".doc"))
                    {
                        Response.ContentType = "application/ms-word";
                        Response.AddHeader("Content-Disposition", "attachment; filename=Document.doc");
                    }

                    Response.BinaryWrite(FileBuffer);
                    Response.End();
                }
            }
            else
            {
                ShowError("ไม่พบไฟล์เอกสารที่ต้องการเปิด");
            }
        }
        catch (System.Threading.ThreadAbortException)
        {
            throw;
        }
        catch (Exception ex)
        {
            ShowError("เปิดไฟล์ไม่สำเร็จ : " + ex.Message);
        }
        finally
        {
            EnsureConnectionClosed();
        }
    }

    protected string GetViewUrl(object revi)
    {
        return string.Format("~/SCE066_1.aspx?INVNO={0}&DATU={1}&DATEFROM={2}&DATETO={3}&PASSW={4}&STATUSD={5}&CUNO={6}&VIEWREVI={7}",
            HttpUtility.UrlEncode(INVNO.Value),
            HttpUtility.UrlEncode(DATU.Value),
            HttpUtility.UrlEncode(DATEFROM.Value),
            HttpUtility.UrlEncode(DATETO.Value),
            HttpUtility.UrlEncode(PASSW.Value),
            HttpUtility.UrlEncode(STATUSD.Value),
            HttpUtility.UrlEncode(CUNO.Value),
            HttpUtility.UrlEncode(Convert.ToString(revi)));
    }

    protected void imgAddPDF_Click(object sender, ImageClickEventArgs e)
    {
        ClearError();

        try
        {
            if (ReadJobClosedStatus())
            {
                ShowError(ClosedMessage);
                ApplyClosedState(true);
                return;
            }

            string fname = FileUpload1.FileName;

            if (!FileUpload1.HasFile || fname.Trim().Equals(""))
            {
                ShowError("กรุณาแนบไฟล์ก่อนกด Upload");
                return;
            }

            string ext = Path.GetExtension(FileUpload1.PostedFile.FileName);
            string custname = "";

            EnsureConnectionClosed();
            connection.Open();

            string sql_datasts = "select * from itprod.shdoch " +
                                 " where shcono = 100 and shdivi = 'PFT' and shivno = '" + txtIVNO.Text + "'";
            iDB2Command comm_datasts = new iDB2Command(sql_datasts, connection);
            iDB2DataReader reader_datasts = comm_datasts.ExecuteReader();

            if (!reader_datasts.Read())
            {
                ShowError("ไม่พบข้อมูล Invoice ที่ต้องการ Upload");
                return;
            }

            if (reader_datasts["shstsd"].Equals("1"))
            {
                ShowError("ใบนี้ปิดแล้วไม่สามารถ Upload เพิ่มได้");
                return;
            }

            custname = reader_datasts["shcuno"].ToString().Trim();

            string savePath = "\\\\172.16.33.37\\Export_Document\\";

            String _date = DateTime.Now.ToString("yyyyMMdd", new CultureInfo("en-US"));
            int _revi = 0;

            string sql_revi = "select * from itprod.shdocl " +
                              " where slcono = 100 and sldivi = 'PFT' and slivno = '" + txtIVNO.Text + "'" +
                              " and   sldatu = '" + ddlDATU.Text + "'";

            iDB2Command comm_revi = new iDB2Command(sql_revi, connection);
            iDB2DataReader reader_revi = comm_revi.ExecuteReader();

            if (!reader_revi.HasRows)
            {
                _revi = 1;
            }
            else
            {
                string sql_revirun = "select max(slrevi) maxrevi from itprod.shdocl " +
                                     " where slcono = 100 and sldivi = 'PFT' and slivno = '" + txtIVNO.Text + "'" +
                                     " and   sldatu = '" + ddlDATU.Text + "'";

                iDB2Command comm_revirun = new iDB2Command(sql_revirun, connection);
                iDB2DataReader reader_revirun = comm_revirun.ExecuteReader();

                while (reader_revirun.Read())
                {
                    _revi = int.Parse(reader_revirun["maxrevi"].ToString()) + 1;
                }
            }

            savePath += fname;
            if (IsDuplicateDocument(savePath))
            {
                ShowError("ไฟล์นี้ถูก Upload ไว้แล้ว กรุณาตรวจสอบก่อนทำรายการซ้ำ");
                return;
            }
            string _dateu = ddlDATU.Text.Trim();

            if (_dateu.Equals("2") && PASSW.Value.Equals("QA"))
            {
                string sql_insert = "insert into itprod.shdocl(slcono,sldivi,slivno,sldatu,slrevi,sldate,slfnam,slfext) " +
                 "values(100,'PFT','" + txtIVNO.Text.Trim() + "','" + ddlDATU.Text + "'," + _revi + "," + _date + ",'" + savePath + "','" + ext + "')";

                iDB2Command comm_insert = new iDB2Command(sql_insert, connection);
                comm_insert.ExecuteNonQuery();

                FileUpload1.SaveAs("\\\\172.16.33.37\\Export_Document\\" + fname);

                string sql_uphead = "update itprod.shdoch set ";

                if (ddlDATU.Text.Equals("2"))
                {
                    sql_uphead = sql_uphead + "shdat2 = '" + _date + "'" +
                                ",shsts2 = '' ";
                }

                sql_uphead = sql_uphead + " where shcono = 100 and shdivi = 'PFT' " +
                           " and shivno = '" + txtIVNO.Text + "'";

                iDB2Command comm_uphead = new iDB2Command(sql_uphead, connection);
                comm_uphead.ExecuteNonQuery();

                SendMailToSale(txtIVNO.Text, custname, ddlDATU.Text);
            }
            else if (!PASSW.Value.Equals("QA") && !PASSW.Value.Equals("AUDIT"))
            {
                string sql_insert = "insert into itprod.shdocl(slcono,sldivi,slivno,sldatu,slrevi,sldate,slfnam,slfext) " +
                                    "values(100,'PFT','" + txtIVNO.Text.Trim() + "','" + ddlDATU.Text + "'," + _revi + "," + _date + ",'" + savePath + "','" + ext + "')";

                iDB2Command comm_insert = new iDB2Command(sql_insert, connection);
                comm_insert.ExecuteNonQuery();

                FileUpload1.SaveAs("\\\\172.16.33.37\\Export_Document\\" + fname);

                string sql_uphead = "update itprod.shdoch set ";

                if (ddlDATU.Text.Equals("1"))
                {
                    sql_uphead = sql_uphead + "shdat1 = '" + _date + "'" +
                                 ",shsts1 = '' ";
                }
                else if (ddlDATU.Text.Equals("2"))
                {
                    sql_uphead = sql_uphead + "shdat2 = '" + _date + "'" +
                                 ",shsts2 = '' ";
                }
                else if (ddlDATU.Text.Equals("3"))
                {
                    sql_uphead = sql_uphead + "shdat3 = '" + _date + "'" +
                                 ",shsts3 = '' ";
                }
                else if (ddlDATU.Text.Equals("4"))
                {
                    sql_uphead = sql_uphead + "shdat4 = '" + _date + "'" +
                                 ",shsts4 = '' ";
                }
                else if (ddlDATU.Text.Equals("5"))
                {
                    sql_uphead = sql_uphead + "shdat5 = '" + _date + "'" +
                                 ",shsts5 = '' ";
                }
                else if (ddlDATU.Text.Equals("6"))
                {
                    sql_uphead = sql_uphead + "shdat6 = '" + _date + "'" +
                                 ",shsts6 = '' ";
                }
                else if (ddlDATU.Text.Equals("7"))
                {
                    sql_uphead = sql_uphead + "shdat7 = '" + _date + "'" +
                                 ",shsts7 = '' ";
                }
                else if (ddlDATU.Text.Equals("8"))
                {
                    sql_uphead = sql_uphead + "shdat8 = '" + _date + "'" +
                                 ",shsts8 = '' ";
                }

                sql_uphead = sql_uphead + " where shcono = 100 and shdivi = 'PFT' " +
                           " and shivno = '" + txtIVNO.Text + "'";

                iDB2Command comm_uphead = new iDB2Command(sql_uphead, connection);
                comm_uphead.ExecuteNonQuery();

                if (ddlDATU.Text.Equals("1"))
                {
                    string sql_checkcustmail = "select * from itprod.SHCUMAIL " +
                                               " where smcuno = '" + custname + "'";
                    iDB2Command comm_custmail = new iDB2Command(sql_checkcustmail, connection);
                    iDB2DataReader reader_custmail = comm_custmail.ExecuteReader();

                    string _eamil = "";

                    while (reader_custmail.Read())
                    {
                        _eamil = reader_custmail["smemal1"].ToString().Trim();
                        SendMailToQC(txtIVNO.Text, custname, ddlDATU.Text, _eamil);
                    }
                }
            }

            EnsureConnectionClosed();
            ShowData();
        }
        catch (Exception ex)
        {
            ShowError("Upload ไม่สำเร็จ : " + ex.Message);
        }
        finally
        {
            EnsureConnectionClosed();
        }

    }
    protected void imgHome_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("~/SCE066.aspx?INVNO=" + txtIVNO.Text + "&DATEFROM=" + DATEFROM.Value  + "&DATETO=" + DATETO.Value + "&PASSW="+ PASSW.Value + "&STATUSD="+ STATUSD.Value + "&CUNO=" + CUNO.Value);
  }
    protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GridView1.EditIndex = -1;

        ShowData();
    }
    protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (ReadJobClosedStatus())
        {
            ClearError();
            ShowError(ClosedMessage);
            ApplyClosedState(true);
            return;
        }

        GridView1.EditIndex = e.NewEditIndex;
        ShowData();
    }
    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        ClearError();

        try
        {
            if (ReadJobClosedStatus())
            {
                ShowError(ClosedMessage);
                ApplyClosedState(true);
                return;
            }

            String _ivno = INVNO.Value;
            string _datu = DATU.Value;
            String _revi = ((Label)GridView1.Rows[e.RowIndex].FindControl("lblREVI")).Text.Trim();
            String _remk = ((TextBox)GridView1.Rows[e.RowIndex].FindControl("txtREMK")).Text.Trim();

            string sql = "update ITPROD.SHDOCL set " +
                         " SLREMK = '" + _remk + "'" +
                         " where SLCONO = 100 AND SLDIVI = 'PFT' " +
                          " and SLIVNO = '" + _ivno + "'" +
                          " and SLDATU = '" + _datu + "'" +
                          " AND SLREVI = " + _revi;

            iDB2Command comm = new iDB2Command(sql, connection);
            EnsureConnectionClosed();
            connection.Open();
            comm.ExecuteNonQuery();

            GridView1.EditIndex = -1;
            EnsureConnectionClosed();
            ShowData();
        }
        catch (Exception ex)
        {
            ShowError("บันทึก Remark ไม่สำเร็จ : " + ex.Message);
        }
        finally
        {
            EnsureConnectionClosed();
        }
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            bool isClosed = GetJobClosedStatus();

            foreach (TableCell cell in e.Row.Cells)
            {
                foreach (Control ctrl in cell.Controls)
                {
                    LinkButton btn = ctrl as LinkButton;
                    if (btn != null && btn.CommandName == "DELDOC")
                    {
                        btn.CssClass = "delete-doc-btn";

                        if (isClosed)
                        {
                            btn.Enabled = false;
                            btn.OnClientClick = "";
                            btn.CssClass += " action-disabled";
                        }
                        else
                        {
                            btn.OnClientClick = "return confirm('ต้องการลบเอกสารนี้ใช่หรือไม่?');";
                        }
                    }

                    if (btn != null && btn.CommandName == "Edit")
                    {
                        btn.CssClass = "edit-doc-btn";

                        if (isClosed)
                        {
                            btn.Enabled = false;
                            btn.CssClass += " action-disabled";
                        }
                    }
                }
            }
        }
    }
    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
        ShowData();
    }

    private void SendMailToSale(String InvoiceNo,string CustCode,string doctype)
    {
        try
        {
            string sql_custname = "select okcunm,length(trim(okcunm)) l_okcunm from mvxcdtprod.ocusma " +
                         " where okcono = 100 and okcuno = '" + CustCode + "'";

            iDB2Command comm_custname = new iDB2Command(sql_custname, connection);
            iDB2DataReader reader_custname = comm_custname.ExecuteReader();
            string _CustName = "";
            int _lname = 0;

            while (reader_custname.Read())
            {
                _lname = int.Parse(reader_custname["l_okcunm"].ToString().Trim());

                if (_lname > 15)
                    _CustName = reader_custname["okcunm"].ToString().Trim().Substring(0, 15);
                else
                    _CustName = reader_custname["okcunm"].ToString().Trim();
            }

            string _docname = "";

            if (doctype.Equals("1"))
                _docname = "Shipping Doc";
            else if (doctype.Equals("2"))
                _docname = "QA Doc";
            else if (doctype.Equals("3"))
                _docname = "Form Doc";
            else if (doctype.Equals("4"))
                _docname = "H/C Doc";
            else if (doctype.Equals("5"))
                _docname = "FedEx/DHL Doc";
            else if (doctype.Equals("6"))
                _docname = "P/L";
            else if (doctype.Equals("7"))
                _docname = "Custom";
            else if (doctype.Equals("8"))
                _docname = "Invoice";

            MailMessage msg = new MailMessage();
            if (host.IsDev)
            {
                msg.To.Add("siripong.j@patayafood.com");
            }
            else
            {
                msg.To.Add("salessupportgroup@patayafood.com");
                msg.To.Add("salessupportnft@patayafood.com");
            }
            msg.From = new MailAddress("lg-doc@it.patayafood.com");
            msg.Subject = "New Upload ====> " + "Invoice No = " + InvoiceNo + ", Customer = " + _CustName + ", Doc Type = " + _docname;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "172.16.1.51";
            smtp.Port = 25;
            smtp.Credentials = new System.Net.NetworkCredential("lg-doc@it.patayafood.com", "lgpass");
            smtp.Send(msg);
        }
        catch (Exception ex)
        {
            ShowError("ส่งอีเมลแจ้ง Upload ไม่สำเร็จ : " + ex.Message);
        }
    }

    private void SendMailToQC(String InvoiceNo, string CustCode, string doctype,string email)
    {
        try
        {
            string sql_custname = "select okcunm,length(trim(okcunm)) l_okcunm from mvxcdtprod.ocusma " +
                         " where okcono = 100 and okcuno = '" + CustCode + "'";

            iDB2Command comm_custname = new iDB2Command(sql_custname, connection);
            iDB2DataReader reader_custname = comm_custname.ExecuteReader();
            string _CustName = "";
            int _lname = 0;

            while (reader_custname.Read())
            {
                _lname = int.Parse(reader_custname["l_okcunm"].ToString().Trim());

                if (_lname > 15)
                    _CustName = reader_custname["okcunm"].ToString().Trim().Substring(0, 15);
                else
                    _CustName = reader_custname["okcunm"].ToString().Trim();
            }

            string _docname = "";

            if (doctype.Equals("1"))
                _docname = "Shipping Doc";
            else if (doctype.Equals("2"))
                _docname = "QA Doc";
            else if (doctype.Equals("3"))
                _docname = "Form Doc";
            else if (doctype.Equals("4"))
                _docname = "H/C Doc";
            else if (doctype.Equals("5"))
                _docname = "FedEx/DHL Doc";
            else if (doctype.Equals("6"))
                _docname = "P/L";
            else if (doctype.Equals("7"))
                _docname = "Custom";
            else if (doctype.Equals("8"))
                _docname = "Invoice";

            MailMessage msg = new MailMessage();
            if (host.IsDev)
            {
                msg.To.Add("siripong.j@patayafood.com");
            }
            else
            {
                msg.To.Add(email);
            }
            msg.From = new MailAddress("lg-doc@it.patayafood.com");
            msg.Subject = "New Upload ====> " + "Invoice No = " + InvoiceNo + ", Customer = " + _CustName + ", Doc Type = " + _docname;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "172.16.1.51";
            smtp.Port = 25;
            smtp.Credentials = new System.Net.NetworkCredential("lg-doc@it.patayafood.com", "lgpass");
            smtp.Send(msg);
        }
        catch (Exception ex)
        {
            ShowError("ส่งอีเมลแจ้ง QC ไม่สำเร็จ : " + ex.Message);
        }
    }



}
