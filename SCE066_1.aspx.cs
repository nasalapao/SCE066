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

   
    protected void Page_Load(object sender, EventArgs e)
    {
         if (!Page.IsPostBack)
          {
            list();
            ShowData();

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
        connection.Open();

        //_pass = txtPASS.Text.Trim();


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

        connection.Close();
    }

   
    protected void btnAddPDF_Click(object sender, EventArgs e)
    {
        

    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        String _date = DateTime.Now.ToString("yyyyMMdd", new CultureInfo("en-US"));




        if (e.CommandName.Equals("VIEW"))
        {

            int rowIndex = Convert.ToInt32(e.CommandArgument);

            String filelocation = ((Label)GridView1.Rows[rowIndex].FindControl("lblNAME")).Text.Trim();
            string fileext = ((Label)GridView1.Rows[rowIndex].FindControl("lblFEXT")).Text.Trim();

            WebClient User = new WebClient();

            Byte[] FileBuffer = User.DownloadData(filelocation); // conver to byte

            if (FileBuffer !=null)
            {
                /*
                // READ PDF
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", FileBuffer.Length.ToString());

                Response.BinaryWrite(FileBuffer);
                */

                
                if (fileext.Equals(".pdf"))
                {
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("content-length", FileBuffer.Length.ToString());
                }
                else
                if (fileext.Equals(".xls"))
                {
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.AppendHeader("content-disposition", "attachment; filename=Document.xls");
                }    
                else
                if (fileext.Equals(".xlsx"))
                {
                        Response.ContentType = "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AppendHeader("content-disposition", "attachment; filename=Document.xlsx");

                }
                else
                if (fileext.Equals(".docx"))
                {
                    Response.ContentType = "application/vnd.ms-word.document";
                   Response.AddHeader("Content-Disposition", "attachment; filename=Document.docx");
                }
                else
                if (fileext.Equals(".doc"))
                {
                    Response.ContentType = "application/ms-word";
                    Response.AddHeader("Content-Disposition", "attachment; filename=Document.doc");
                }

                Response.BinaryWrite(FileBuffer);
            }

         } // VIEW

    }

    protected void imgAddPDF_Click(object sender, ImageClickEventArgs e)
    {
        string fname = FileUpload1.FileName;

        string ext = Path.GetExtension(FileUpload1.PostedFile.FileName);
        string custname = "";

        connection.Open();

        string sql_datasts = "select * from itprod.shdoch " +
                             " where shcono = 100 and shdivi = 'PFT' and shivno = '" + txtIVNO.Text + "'";
        iDB2Command comm_datasts = new iDB2Command(sql_datasts, connection);
        iDB2DataReader reader_datasts = comm_datasts.ExecuteReader();

        reader_datasts.Read();

        //if (reader_datasts["shstsd"].Equals("1")) -- test 080524
        if (reader_datasts["shstsd"].Equals("0"))
        {
            Response.Write("<script LANGUAGE='JavaScript' >alert('ใบนี้ปิดแล้วไม่สามารถ Upload เพิ่มได้ !!!')</script>");
        }
        else
        {
            custname = reader_datasts["shcuno"].ToString().Trim();

            if (!fname.Trim().Equals(""))
            {

                string savePath = "\\\\172.16.33.37\\Export_Document\\";

                String _date = DateTime.Now.ToString("yyyyMMdd", new CultureInfo("en-US"));
                int _revi = 0;

                //connection.Open();
                // check revision
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
                //savePath
                string _dateu = "";

                _dateu = ddlDATU.Text.Trim();

                if (_dateu.Equals("2") && PASSW.Value.Equals("QA"))
                {
                    string sql_insert = "insert into itprod.shdocl(slcono,sldivi,slivno,sldatu,slrevi,sldate,slfnam,slfext) " +
                     "values(100,'PFT','" + txtIVNO.Text.Trim() + "','" + ddlDATU.Text + "'," + _revi + "," + _date + ",'" + savePath + "','" + ext + "')";


                    // run sql command
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
                else
                if (!PASSW.Value.Equals("QA") && !PASSW.Value.Equals("AUDIT"))
                {
                    string sql_insert = "insert into itprod.shdocl(slcono,sldivi,slivno,sldatu,slrevi,sldate,slfnam,slfext) " +
                                        "values(100,'PFT','" + txtIVNO.Text.Trim() + "','" + ddlDATU.Text + "'," + _revi + "," + _date + ",'" + savePath + "','" + ext + "')";


                    // run sql command
                    iDB2Command comm_insert = new iDB2Command(sql_insert, connection);
                    comm_insert.ExecuteNonQuery();


                    FileUpload1.SaveAs("\\\\172.16.33.37\\Export_Document\\" + fname);

                    string sql_uphead = "update itprod.shdoch set ";

                    if (ddlDATU.Text.Equals("1"))
                    {
                        sql_uphead = sql_uphead + "shdat1 = '" + _date + "'" +
                                     ",shsts1 = '' ";
                    }
                    else
                        if (ddlDATU.Text.Equals("2"))
                        {
                            sql_uphead = sql_uphead + "shdat2 = '" + _date + "'" +
                                         ",shsts2 = '' ";
                        }
                        else
                            if (ddlDATU.Text.Equals("3"))
                            {
                                sql_uphead = sql_uphead + "shdat3 = '" + _date + "'" +
                                             ",shsts3 = '' ";
                            }
                            else
                                if (ddlDATU.Text.Equals("4"))
                                {
                                    sql_uphead = sql_uphead + "shdat4 = '" + _date + "'" +
                                                 ",shsts4 = '' ";
                                }

                                else
                                    if (ddlDATU.Text.Equals("5"))
                                    {
                                        sql_uphead = sql_uphead + "shdat5 = '" + _date + "'" +
                                                     ",shsts5 = '' ";
                                    }

                                    else
                                        if (ddlDATU.Text.Equals("6"))
                                        {
                                            sql_uphead = sql_uphead + "shdat6 = '" + _date + "'" +
                                                         ",shsts6 = '' ";
                                        }
                                        else
                                            if (ddlDATU.Text.Equals("7"))
                                            {
                                                sql_uphead = sql_uphead + "shdat7 = '" + _date + "'" +
                                                             ",shsts7 = '' ";
                                            }

                    sql_uphead = sql_uphead + " where shcono = 100 and shdivi = 'PFT' " +
                               " and shivno = '" + txtIVNO.Text + "'";

                    iDB2Command comm_uphead = new iDB2Command(sql_uphead, connection);
                    comm_uphead.ExecuteNonQuery();

                    if (!ddlDATU.Text.Equals("7"))
                    {

                        //SendMailToSale(txtIVNO.Text, custname, ddlDATU.Text); -- test by id 080524
                    }

                    // เมล์ส่งไปให้ QC ตาม table itprod.SHCUMAIL สำหรับรายการ Up Ship Doc
                    if (ddlDATU.Text.Equals("1")) 
                    {

                        string sql_checkcustmail = "select * from itprod.SHCUMAIL " +
                                                   " where smcuno = '" + custname + "'";
                        iDB2Command comm_custmail = new iDB2Command(sql_checkcustmail, connection);
                        iDB2DataReader reader_custmail = comm_custmail.ExecuteReader();

                        string _eamil = "";

                        while (reader_custmail.Read()) //ข้อมูลมี ให้ ทำการส่งเมล์ไป qc ตาม table SHCUMAIL
                        {
                            _eamil = reader_custmail["smemal1"].ToString().Trim();


                            SendMailToQC(txtIVNO.Text, custname, ddlDATU.Text,_eamil);

                        }
                    }
                    // end mail ส่งไป QC รายการ up ship doc


                }
                // end -- update date in header itprod.shdoch

                // send mail to upload
               // SendMailToSale(txtIVNO.Text, custname, ddlDATU.Text);
                
                // end sen mail
                
            }
        } // end else if
        connection.Close();

         ShowData();

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
        GridView1.EditIndex = e.NewEditIndex;
        ShowData();
    }
    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        String _ivno = INVNO.Value;
        string _datu = DATU.Value;

        String _revi = ((Label)GridView1.Rows[e.RowIndex].FindControl("lblREVI")).Text.Trim();
        

        String _remk = ((TextBox)GridView1.Rows[e.RowIndex].FindControl("txtREMK")).Text.Trim();
        //String _user = ((TextBox)GridView1.Rows[e.RowIndex].FindControl("txtUSID")).Text.Trim();
        
        string sql = "update ITPROD.SHDOCL set " +
                     " SLREMK = '" +  _remk +   "'" +
                     " where SLCONO = 100 AND SLDIVI = 'PFT' " +
                      " and SLIVNO = '" + _ivno + "'" +
                      " and SLDATU = '" + _datu + "'" +
                      " AND SLREVI = " + _revi ;

        // run sql command
        iDB2Command comm = new iDB2Command(sql, connection);
        connection.Open();
        comm.ExecuteNonQuery();


        connection.Close();
        
        GridView1.EditIndex = -1;

        ShowData();
    }
    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
        ShowData();
    }

    private void SendMailToSale(String InvoiceNo,string CustCode,string doctype)
    {

        //connection.Open();

        string sql_custname = "select okcunm,length(trim(okcunm)) l_okcunm from mvxcdtprod.ocusma " +
                     " where okcono = 100 and okcuno = '" + CustCode + "'" ;

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

       // connection.Close();

        string _docname = "";

        if (doctype.Equals("1"))
            _docname = "Shipping Doc";
        else
            if (doctype.Equals("2"))
                _docname = "QA Doc";
            else
                if (doctype.Equals("3"))
                    _docname = "Form Doc";
                else if (doctype.Equals("4"))
                    _docname = "H/C Doc";
                else if (doctype.Equals("5"))
                    _docname = "FedEx/DHL Doc";
                else if (doctype.Equals("6"))
                    _docname = "P/L";
                else if (doctype.Equals("7"))
                    _docname = "Custom";

        
        MailMessage msg = new MailMessage();
        //msg.To.Add("itsaret@patayafood.com");
        //msg.To.Add("groupitbkk@patayafood.com");
        msg.To.Add("salessupportgroup@patayafood.com");
        msg.To.Add("salessupportnft@patayafood.com");
        msg.From = new MailAddress("lg-doc@it.patayafood.com");
       // msg.Subject = "Test Mail by LG-Doc ====> ";
        msg.Subject = "New Upload ====> " + "Invoice No = " + InvoiceNo + ", Customer = " + _CustName + ", Doc Type = " + _docname; 
        //msg.Subject = "Test Upload by ID ====> " + "Invoice No = " + InvoiceNo + ", Customer = " + _CustName + ", Doc Type = " + _docname; 
        //msg.Body = "Invoice No = " + InvoiceNo + ", Customer = " + _CustName + ", Doc Type = " + _docname;
        SmtpClient smtp = new SmtpClient();
        smtp.Host = "172.16.1.51";
        smtp.Port = 25;
        smtp.Credentials = new System.Net.NetworkCredential("lg-doc@it.patayafood.com", "lgpass");
        smtp.Send(msg);
        

    }

    private void SendMailToQC(String InvoiceNo, string CustCode, string doctype,string email)
    {

        //connection.Open();

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

        // connection.Close();

        string _docname = "";

        if (doctype.Equals("1"))
            _docname = "Shipping Doc";
        else
            if (doctype.Equals("2"))
                _docname = "QA Doc";
            else
                if (doctype.Equals("3"))
                    _docname = "Form Doc";
                else if (doctype.Equals("4"))
                    _docname = "H/C Doc";
                else if (doctype.Equals("5"))
                    _docname = "FedEx/DHL Doc";
                else if (doctype.Equals("6"))
                    _docname = "P/L";
                else if (doctype.Equals("7"))
                    _docname = "Custom";


        MailMessage msg = new MailMessage();
        msg.To.Add(email);
        msg.From = new MailAddress("lg-doc@it.patayafood.com");
        // msg.Subject = "Test Mail by LG-Doc ====> ";
        msg.Subject = "New Upload ====> " + "Invoice No = " + InvoiceNo + ", Customer = " + _CustName + ", Doc Type = " + _docname;
        //msg.Subject = "Test Upload by ID ====> " + "Invoice No = " + InvoiceNo + ", Customer = " + _CustName + ", Doc Type = " + _docname; 
        //msg.Body = "Invoice No = " + InvoiceNo + ", Customer = " + _CustName + ", Doc Type = " + _docname;
        SmtpClient smtp = new SmtpClient();
        smtp.Host = "172.16.1.51";
        smtp.Port = 25;
        smtp.Credentials = new System.Net.NetworkCredential("lg-doc@it.patayafood.com", "lgpass");
        smtp.Send(msg);


    }



}