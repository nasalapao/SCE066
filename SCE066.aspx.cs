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

using System.Diagnostics;
using System.IO;

using System.Net;
using System.Net.Mail;


public partial class SCE066 : System.Web.UI.Page
{
    
    iDB2Connection connection = new iDB2Connection("DataSource=172.16.33.49;UserID=mvxreport;Password=report;DataCompression=True;");
    iDB2Connection connection1 = new iDB2Connection("DataSource=172.16.33.49;UserID=mvxreport;Password=report;DataCompression=True;");

    string _datefrom = "";
    string _dateto = "";
    string _pass = "";

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
          //  if (datepicker1.Value.Equals("") && txtIVNO.Text.Equals("") && ddlCUST.Text.Equals(""))
          //  { DDL_Cust(); }
          //  else
          //  {
            list();
            DDL_Cust();

              if (!datepicker1.Value.Equals(""))
              { 
                 // list();
                //DDL_Cust();
                ShowData();

                
              }

              if (!txtIVNO.Text.Equals(""))
                  ShowData();


       }

    }

    private void list()
    {
        INVNO.Value = Request.QueryString.Get("INVNO");
        DATEFROM.Value = Request.QueryString.Get("DATEFROM");
        DATETO.Value = Request.QueryString.Get("DATETO");
       // DATU.Value = Request.QueryString.Get("DATU");
        PASSW.Value = Request.QueryString.Get("PASSW");
        STATUSD.Value = Request.QueryString.Get("STATUSD");

        CUNO.Value = Request.QueryString.Get("CUNO");

        txtIVNO.Text = INVNO.Value;
        txtPASS.Text = PASSW.Value;
        
        datepicker1.Value = DATEFROM.Value;
        datepicker2.Value = DATETO.Value;
        ddlSTATUS.Text = STATUSD.Value;


       // datepicker1.Value = DateTime.Now.ToString("dd/MM/yyyy");

        datepicker1.Value = DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
        datepicker2.Value = DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
             
        //ddlDATU.Text = DATU.Value;

    }

    protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
    {
        GenData();
        UpdateETD();
        ShowData();
    }


    private void GenData()
    {

        string sql_gendata = "";
        //connection.Open();

        if (!ddlCUST.Text.Equals("0"))
        {
            ShowData();
        }
        else
        {
            connection.Open();

            if (!txtIVNO.Text.Trim().Equals(""))
            {


                sql_gendata = "insert into itprod.shdoch(shcono,shdivi,shivno,shcuno,shetdd,shstsd) " +
                        "select dacono,uadivi,daconn,uacuno,dadsdt,'0' from mvxcdtprod.dconsi " +
                         " left join  " +
                         " (select distinct uacono,uadivi,uaconn,uacuno,uaortp from mvxcdtprod.odhead) odhead " +
                         " on uacono = dacono and uaconn = daconn " +
                         " where dacono = 100 and uacuno like 'ETH%' and uaortp in('TE1','TF1','TE7','TE2') " +
                         " and daconn = '" + txtIVNO.Text.Trim() + "'" +
                         " and  daconn not in(select shivno from itprod.shdoch) ";


            }
            else
            {
                _datefrom = datepicker1.Value.Substring(6, 4) + datepicker1.Value.Substring(3, 2) + datepicker1.Value.Substring(0, 2);
                _dateto = datepicker2.Value.Substring(6, 4) + datepicker2.Value.Substring(3, 2) + datepicker2.Value.Substring(0, 2);

                sql_gendata = "insert into itprod.shdoch(shcono,shdivi,shivno,shcuno,shetdd,shstsd) " +
                                    "select dacono,uadivi,daconn,uacuno,dadsdt,'0' from mvxcdtprod.dconsi " +
                                     " left join  " +
                                     " (select distinct uacono,uadivi,uaconn,uacuno,uaortp from mvxcdtprod.odhead) odhead " +
                                     " on uacono = dacono and uaconn = daconn " +
                                     " where dacono = 100 and uacuno like 'ETH%' and uaortp in('TE1','TF1','TE7','TE2') " +
                                     " and dadsdt >= " + _datefrom + " and dadsdt <= " + _dateto +
                                     " and  daconn not in(select shivno from itprod.shdoch) ";
            }
            // run sql command
            iDB2Command comm_gendata = new iDB2Command(sql_gendata, connection);
            comm_gendata.ExecuteNonQuery();

            connection.Close();

        } // end cust_code <> '-'



    }

    private void UpdateETD()
    {

        if (ddlCUST.Text.Equals("0"))
        {

            connection.Open();

            string sql_data = "";
            if (!txtIVNO.Text.Trim().Equals(""))
            {

                // data from shdoch
                sql_data = "select * from itprod.shdoch " +
                           " where shcono = 100 and shdivi = 'PFT' and shivno = '" + txtIVNO.Text.Trim() + "'";
            }
            else
            {
                _datefrom = datepicker1.Value.Substring(6, 4) + datepicker1.Value.Substring(3, 2) + datepicker1.Value.Substring(0, 2);
                _dateto = datepicker2.Value.Substring(6, 4) + datepicker2.Value.Substring(3, 2) + datepicker2.Value.Substring(0, 2);

                sql_data = "select * from itprod.shdoch " +
                           " where shcono =  100 and shdivi = 'PFT' and shetdd >= " + _datefrom +
                           " and   shetdd <= " + _dateto;
            }

            iDB2Command comm_data = new iDB2Command(sql_data, connection);
            iDB2DataReader reader_data = comm_data.ExecuteReader();

            while (reader_data.Read())
            {

                if (!txtPASS.Text.Equals("AUDIT")) // USER AUDIT ไม่สามารถแก้ไขได้
                {

                    // UPDATE ETDDATE
                    string sql_upetd = "UPDATE ITPROD.SHDOCH X " +
                                       " SET X.SHETDD = (SELECT Y.DADSDT FROM MVXCDTPROD.DCONSI Y " +
                                       "   WHERE X.SHCONO = Y.DACONO AND " +
                                       "         X.SHIVNO = Y.DACONN) " +
                                       " WHERE X.SHCONO = 100 AND X.SHDIVI = 'PFT' AND X.SHIVNO = '" + reader_data["shivno"].ToString().Trim() + "'";
                    // run sql command
                    iDB2Command comm_upetd = new iDB2Command(sql_upetd, connection);
                    comm_upetd.ExecuteNonQuery();
                } 

                // END UP ETDDATE

                // UPDATE STATUS INVOICE


                // END STATUS INVOICE

            }

            connection.Close();
        }


    }
    
    private void ShowData()
    {

        connection.Open();

        _pass = txtPASS.Text.Trim();

        string sql_rel = "";
        if (txtIVNO.Text.Trim().Equals(""))
        {
            if (!datepicker2.Value.Equals(""))
            {
                _datefrom = datepicker1.Value.Substring(6, 4) + datepicker1.Value.Substring(3, 2) + datepicker1.Value.Substring(0, 2);
                _dateto = datepicker2.Value.Substring(6, 4) + datepicker2.Value.Substring(3, 2) + datepicker2.Value.Substring(0, 2);

            }
            else{
                _datefrom = "0";
                _dateto = "99999999";
               
            }

            sql_rel = "SELECT shivno,oaorst,trim(shcuno) || '-' || okcunm as cunm,shetdd,shetad,CASE WHEN shstsd = '1' THEN 'Complete' else 'Un-Complete' end as shstsd, " +
                                 "shusid,coalesce(rtrim(char(invdoc.invdate)), '') as shdat8,shdat1,shdat2,shdat3,shdat4,shdat5,shdat6,shdat7,SHSTS1,SHSTS2,SHSTS3,SHSTS4,SHSTS5,shsts6 ,shsts7  " +
                                 " from itprod.shdoch " +
                                 " left join (select slcono,sldivi,slivno,max(sldate) as invdate from itprod.shdocl where sldatu = '8' group by slcono,sldivi,slivno) invdoc on invdoc.slcono = shcono and invdoc.sldivi = shdivi and invdoc.slivno = shivno " +
                                 " left join " +
                                 "   (select uacono,uadivi,uaconn,max(uaorno) uaorno from mvxcdtprod.odhead group by uacono,uadivi,uaconn) odhead on uacono = shcono and uadivi = shdivi and uaconn = shivno " +
                //" left join mvxcdtprod.oohead on oacono= uacono and oadivi = shdivi and oaorno = uaorno " +
                                 " left join mvxcdtprod.ocusma on okcono = uacono and okcuno = shcuno " +
                                 " left join " +
                                 "  (SELECT OQCONN,MAX(OBORST) as oaorst FROM MVXCDTPROD.MHDISH " +
                                 "   LEFT JOIN MVXCDTPROD.MHDISL ON OQCONO = URCONO AND OQDLIX = URDLIX " +
                                 "   LEFT JOIN MVXCDTPROD.OOLINE ON OBORNO = OQRIDN AND OBPONR = URRIDL  WHERE OQCONO = 100 AND OQTRDT >= 20180101 group by oqconn) data_sts on oqconn = shivno ";
  

            if (!ddlCUST.Text.Equals("0") && !ddlCUST.Text.Equals(""))
            {
                sql_rel = sql_rel +
                          " where shcuno = '" + ddlCUST.Text.Trim() + "'" +
                          " and shetdd >= " + _datefrom +
                          " and shetdd <= " + _dateto;
            }
            else
            {
                sql_rel = sql_rel + " where  shetdd >= " + _datefrom +
                                " and   shetdd <= " + _dateto;

            }

            if (ddlSTATUS.Text.Equals("0"))
                sql_rel = sql_rel + " and shstsd = '0'";
            else
                if (ddlSTATUS.Text.Equals("1"))
                    sql_rel = sql_rel + " and shstsd = '1'";
            

            sql_rel = sql_rel + " order by shetdd,shivno ";
            
        }
        else
        {

            sql_rel = "SELECT shivno,oaorst,trim(shcuno) || '-' || okcunm as cunm,shetdd,shetad,CASE WHEN shstsd = '1' THEN 'Complete' else 'Un-Complete' end as shstsd, " +
                             "shusid,coalesce(rtrim(char(invdoc.invdate)), '') as shdat8,shdat1,shdat2,shdat3,shdat4,shdat5,shdat6,shdat7,SHSTS1,SHSTS2,SHSTS3,SHSTS4,SHSTS5,shsts6,shsts7   " +
                             " from itprod.shdoch " +
                             " left join (select slcono,sldivi,slivno,max(sldate) as invdate from itprod.shdocl where sldatu = '8' group by slcono,sldivi,slivno) invdoc on invdoc.slcono = shcono and invdoc.sldivi = shdivi and invdoc.slivno = shivno " +
                             " left join " +
                             "   (select uacono,uadivi,uaconn,max(uaorno) uaorno from mvxcdtprod.odhead group by uacono,uadivi,uaconn) odhead on uacono = shcono and uadivi = shdivi and uaconn = shivno " +
                            // " left join mvxcdtprod.oohead on oacono= uacono and oadivi = shdivi and oaorno = uaorno " +
                             " left join mvxcdtprod.ocusma on okcono = uacono and okcuno = shcuno " +
                             " left join " +
                             "  (SELECT OQCONN,max(OBORST) as oaorst FROM MVXCDTPROD.MHDISH " +
                             "   LEFT JOIN MVXCDTPROD.MHDISL ON OQCONO = URCONO AND OQDLIX = URDLIX " +
                             "   LEFT JOIN MVXCDTPROD.OOLINE ON OBORNO = OQRIDN AND OBPONR = URRIDL WHERE OQCONO = 100 AND OQTRDT >= 20180101 group by oqconn) data_sts on oqconn = shivno " +
                             " where  shivno = '" + txtIVNO.Text.Trim() + "'";

            if (ddlSTATUS.Text.Equals("0"))
                sql_rel = sql_rel + " and shstsd = '0'";
            else
                if (ddlSTATUS.Text.Equals("1"))
                    sql_rel = sql_rel + " and shstsd = '1'";

            sql_rel = sql_rel + " order by shetdd,shivno ";
                            
                          

        }
       
 
        iDB2DataAdapter da = new iDB2DataAdapter(sql_rel, connection);

        DataSet ds = new DataSet();

        da.Fill(ds);
        GridView1.DataSource = ds.Tables[0];

        GridView1.DataBind();

        connection.Close();
    }

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
        ShowData();


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
        String _ivno = ((Label)GridView1.Rows[e.RowIndex].FindControl("lblIVNO")).Text.Trim();

        String _etad = ((TextBox)GridView1.Rows[e.RowIndex].FindControl("txtETAD")).Text.Trim();
        String _user = ((TextBox)GridView1.Rows[e.RowIndex].FindControl("txtUSID")).Text.Trim();
        

        if (txtPASS.Text.Equals("EXPORT"))
        {

            string sql = "update ITPROD.SHDOCH set " +
                         " SHETAD = " + _etad +
                         " ,SHUSID = '" + _user + "'" +
                         " where SHCONO = 100 AND SHDIVI = 'PFT' " +
                         " and SHIVNO = '" + _ivno + "'";

            // run sql command
            iDB2Command comm = new iDB2Command(sql, connection);
            connection.Open();
            comm.ExecuteNonQuery();
        }
        else
        {
            Response.Write("<script LANGUAGE='JavaScript' >alert('Password ผิด ไม่สามารถแก้ไขข้อมูลได้ !!!')</script>");
        }

        connection.Close();

        GridView1.EditIndex = -1;
        ShowData();
    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        

        String _date = DateTime.Now.ToString("yyyyMMdd", new CultureInfo("en-US"));
       // int _maxrevi = 0;
      //  string filelocation = "";
      //  string fileext = "";
        string _datu = "";

        if (e.CommandName.Equals("UPDAT1"))
               _datu = "1";
        if (e.CommandName.Equals("UPDAT2"))
               _datu = "2";
        if (e.CommandName.Equals("UPDAT3"))
               _datu = "3";
        if (e.CommandName.Equals("UPDAT4"))
               _datu = "4";
        if (e.CommandName.Equals("UPDAT5"))
               _datu = "5";
        if (e.CommandName.Equals("UPDAT6"))
            _datu = "6";
        if (e.CommandName.Equals("UPDAT7"))
            _datu = "7";
        if (e.CommandName.Equals("UPDAT8"))
            _datu = "8";
             
        if (e.CommandName.Equals("Appr"))
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);

            String _ivno = ((Label)GridView1.Rows[rowIndex].FindControl("lblIVNO")).Text.Trim();

            if (txtPASS.Text.Equals("LGE"))   
            {
                string sql = "update ITPROD.SHDOCH set " +
                 " SHSTSD = 1 " +
                 " where SHCONO = 100 AND SHDIVI = 'PFT' " +
                 " and SHIVNO = '" + _ivno + "'";

                connection.Open();
                // run sql command
                iDB2Command comm = new iDB2Command(sql, connection);
              
                comm.ExecuteNonQuery();
                connection.Close();

                Response.Write("<script LANGUAGE='JavaScript' >alert('เรียบร้อย !!!')</script>");

                SendMailToSale(_ivno, "Complete");

                ShowData();
                
            }
            else
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Password ผิด ไม่สามารถ Approve ได้ !!!')</script>");
            }
        } // apprive

        
        if (e.CommandName.Equals("NotApr"))
        {

            int rowIndex = Convert.ToInt32(e.CommandArgument);

            String _ivno = ((Label)GridView1.Rows[rowIndex].FindControl("lblIVNO")).Text.Trim();
            // not approve
            if (txtPASS.Text.Equals("LGE")) 
            {
                string sql = "update ITPROD.SHDOCH set " +
                 " SHSTSD = 0 " +
                 " where SHCONO = 100 AND SHDIVI = 'PFT' " +
                 " and SHIVNO = '" + _ivno + "'";

                // run sql command
                iDB2Command comm = new iDB2Command(sql, connection);
                connection.Open();
                comm.ExecuteNonQuery();
                connection.Close();

                Response.Write("<script LANGUAGE='JavaScript' >alert('เรียบร้อย !!!')</script>");

                ShowData();

            
            }
            else
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Password ผิด ไม่สามารถ Not_Approve ได้ !!!')</script>");
            }
        }

        // check password if user - lge,export use page for upload but if not use view
        if ((txtPASS.Text.Trim().Equals("LGE") || txtPASS.Text.Trim().Equals("EXPORT"))) 
        {

            // use for upload
            if ((e.CommandName.Equals("UPDAT1")) || (e.CommandName.Equals("UPDAT2")) || (e.CommandName.Equals("UPDAT3"))
                || (e.CommandName.Equals("UPDAT4")) || (e.CommandName.Equals("UPDAT5")) || (e.CommandName.Equals("UPDAT6")) ||
                (e.CommandName.Equals("UPDAT7")) || (e.CommandName.Equals("UPDAT8"))
                ) 
            {

                int rowIndex = Convert.ToInt32(e.CommandArgument);

                String _ivno = ((Label)GridView1.Rows[rowIndex].FindControl("lblIVNO")).Text.Trim();

                Response.Redirect("~/SCE066_1.aspx?INVNO=" + _ivno + "&DATU=" + _datu + "&DATEFROM=" + datepicker1.Value + "&DATETO=" + datepicker2.Value + "&PASSW=" + txtPASS.Text.Trim() + "&STATUSD="+ddlSTATUS.Text);
            }



         
        }
        
            if (txtPASS.Text.Trim().Equals("QA")) // QC USE UPLOAD UPDATE2 ONLY
            {

                // use for upload
                if (e.CommandName.Equals("UPDAT2"))
                {

                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    String _ivno = ((Label)GridView1.Rows[rowIndex].FindControl("lblIVNO")).Text.Trim();

                    Response.Redirect("~/SCE066_1.aspx?INVNO=" + _ivno + "&DATU=" + _datu + "&DATEFROM=" + datepicker1.Value + "&DATETO=" + datepicker2.Value + "&PASSW=" + txtPASS.Text.Trim() + "&STATUSD=" + ddlSTATUS.Text);
                }            
            }

            if (txtPASS.Text.Trim().Equals("SSS") || txtPASS.Text.Trim().Equals("QA") || txtPASS.Text.Trim().Equals("AUDIT") || txtPASS.Text.Trim().Equals("FA") || txtPASS.Text.Trim().Equals("SI") ) // 18/05/23 เพิ่ม SI
        {
            // use for view

            if ((e.CommandName.Equals("UPDAT1")) || (e.CommandName.Equals("UPDAT2")) || (e.CommandName.Equals("UPDAT3"))
                || (e.CommandName.Equals("UPDAT4")) || (e.CommandName.Equals("UPDAT5")) || (e.CommandName.Equals("UPDAT6")) ||
                (e.CommandName.Equals("UPDAT7")) || (e.CommandName.Equals("UPDAT8"))
                )
            {
                // view
                connection.Open();

                int rowIndex = Convert.ToInt32(e.CommandArgument);

                String _ivno = ((Label)GridView1.Rows[rowIndex].FindControl("lblIVNO")).Text.Trim();

               

                           
                    
                    string sql_upvw = "";

                    if (txtPASS.Text.Trim().Equals("SSS")) // เปลี่ยน sts view สำหรับ SSS เท่านั้น
                    {

                        // update status for view
                        if (_datu.Equals("1"))
                        {
                            sql_upvw = "update itprod.shdoch " +
                                       " set shsts1 = '1' " +
                                       "where shcono = 100 and shdivi = 'PFT' " +
                                       "  and shivno = '" + _ivno + "'";
                        }
                        else
                            if (_datu.Equals("2"))
                            {
                                sql_upvw = "update itprod.shdoch " +
                                           " set shsts2 = '1' " +
                                           "where shcono = 100 and shdivi = 'PFT' " +
                                           "  and shivno = '" + _ivno + "'";
                            }
                            else
                                if (_datu.Equals("3"))
                                {
                                    sql_upvw = "update itprod.shdoch " +
                                               " set shsts3 = '1' " +
                                               "where shcono = 100 and shdivi = 'PFT' " +
                                               "  and shivno = '" + _ivno + "'";
                                }
                                else
                                    if (_datu.Equals("4"))
                                    {
                                        sql_upvw = "update itprod.shdoch " +
                                                   " set shsts4 = '1' " +
                                                   "where shcono = 100 and shdivi = 'PFT' " +
                                                   "  and shivno = '" + _ivno + "'";
                                    }
                                    else
                                        if (_datu.Equals("5"))
                                        {
                                            sql_upvw = "update itprod.shdoch " +
                                                       " set shsts5 = '1' " +
                                                       "where shcono = 100 and shdivi = 'PFT' " +
                                                       "  and shivno = '" + _ivno + "'";
                                        }
                                        else
                                            if (_datu.Equals("6"))
                                            {
                                                sql_upvw = "update itprod.shdoch " +
                                                           " set shsts6 = '1' " +
                                                           "where shcono = 100 and shdivi = 'PFT' " +
                                                           "  and shivno = '" + _ivno + "'";
                                            }
                        //  end update status for view

                        if (!sql_upvw.Equals(""))
                        {
                            iDB2Command comm_upvw = new iDB2Command(sql_upvw, connection);
                            //connection.Open();
                            comm_upvw.ExecuteNonQuery();
                        }
                    }


                    if ((txtPASS.Text.Trim().Equals("FA")) || (txtPASS.Text.Equals("SI"))) // เปลี่ยน sts view สำหรับ FA เท่านั้น // 18/05/23 เพิ่มสิทธิให้ IS
                    {

                        // update status for view
                        if (_datu.Equals("7"))
                        {
                            sql_upvw = "update itprod.shdoch " +
                                       " set shsts7 = '1' " +
                                       "where shcono = 100 and shdivi = 'PFT' " +
                                       "  and shivno = '" + _ivno + "'";

                            iDB2Command comm_upvw = new iDB2Command(sql_upvw, connection);
                            //connection.Open();
                            comm_upvw.ExecuteNonQuery();
                        }
                    
                        //  end update status for view


                    }
                    //LINK DETAIL

                    Response.Redirect("~/SCE066_1.aspx?INVNO=" + _ivno + "&DATU=" + _datu + "&DATEFROM=" + datepicker1.Value + "&DATETO=" + datepicker2.Value + "&PASSW=" + txtPASS.Text.Trim() + "&STATUSD=" + ddlSTATUS.Text + "&CUNO=" + ddlCUST.Text);
            
               // } // end if count > 0


            }
            
            connection.Close();

        }
   
        /*
        // open Explorer
        if (e.CommandName.Equals("Explorer"))
        {

            int rowIndex = Convert.ToInt32(e.CommandArgument);

            String _ivno = ((Label)GridView1.Rows[rowIndex].FindControl("lblIVNO")).Text.Trim();
            // not approve
            if (txtPASS.Text.Equals("SSS"))
            {
                connection.Open();

                string sql_maxrevi = "select max(slrevi) maxrevi,count(*) countrec from itprod.shdocl " +
                                      " where slcono = 100 and sldivi = 'PFT' and slivno = '" + _ivno + "'" +
                                      "   and sldatu = 1 " ;
                                      

                iDB2Command comm_maxrevi = new iDB2Command(sql_maxrevi, connection);
                iDB2DataReader reader_findmax = comm_maxrevi.ExecuteReader();

                reader_findmax.Read();

                if (int.Parse(reader_findmax["countrec"].ToString()) > 0)
                {

                    _maxrevi = int.Parse(reader_findmax["maxrevi"].ToString());

                    string sql_location = "select * from itprod.shdocl " +
                                      " where slcono = 100 and sldivi = 'PFT' and slivno = '" + _ivno + "'" +
                                      "   and sldatu = 1 " +
                                      "   and slrevi =  " + _maxrevi;
                    iDB2Command comm_location = new iDB2Command(sql_location, connection);
                    iDB2DataReader reader_findloc = comm_location.ExecuteReader();

                    if (reader_findloc.HasRows)
                    {
                        reader_findloc.Read();

                        string filePath = reader_findloc["slfnam"].ToString().Trim();

                        connection.Close();
                        string argument = "/select, \"" + filePath + "\"";

                        Response.Write("START...");

                        System.Diagnostics.Process.Start("explorer.exe", argument);
                        Response.Write("STOP .." + argument);
                    }
                } // END if (int.Parse(reader_findmax["countrec"].ToString()) > 0)

            }
            else
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Password ผิด ไม่สามารถ เปิด Folder ได้ !!!')</script>");
            }
        }
        */

        
  
        //ShowData();
        

        
    }

    private void DDL_Cust()
    {
        // dropdownlist customer/disticnt
        string sql_cust =  "SELECT OKCUNM,SHCUNO FROM (" +
                           "select '-' AS OKCUNM,'0' AS SHCUNO " +
                           " from mvxcdtprod.ocusma WHERE OKCUNO = 'ETH0038' " +
                           " UNION " +
                            "select DISTINCT Okcunm,shcuno " +
                          " from itprod.shdoch " +
                          " left join mvxcdtprod.ocusma on okcuno = shcuno " +
                          ") CUST " +
                          " order by OKCUNM ";


        iDB2Command cmd = new iDB2Command(sql_cust, connection);
        iDB2DataAdapter sda = new iDB2DataAdapter(cmd);


        DataTable dt = new DataTable();
        sda.Fill(dt);
        ddlCUST.DataSource = dt;

        ddlCUST.DataTextField = "OKCUNM";
        ddlCUST.DataValueField = "SHCUNO";
        ddlCUST.DataBind();

    }



    protected void txtPASS_TextChanged(object sender, EventArgs e)
    {
       // lblPass2.Text = txtPASS.Text;
    }
    
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //string _check = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "SHSTSD")).Trim();
            // CHECK COMPLETE
            if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "SHSTSD")).Trim() != "Complete")
            {
                e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
            }

           
            if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "SHSTS1")) != "1")
            {
                e.Row.Cells[9].ForeColor = System.Drawing.Color.Red;
            }

            if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "SHSTS2")) != "1")
            {
                e.Row.Cells[11].ForeColor = System.Drawing.Color.Red;
            }

            if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "SHSTS3")) != "1")
            {
                e.Row.Cells[13].ForeColor = System.Drawing.Color.Red;
            }

            if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "SHSTS4")) != "1")
            {
                e.Row.Cells[15].ForeColor = System.Drawing.Color.Red;
            }

            if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "SHSTS5")) != "1")
            {
                e.Row.Cells[17].ForeColor = System.Drawing.Color.Red;
            }

            if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "SHSTS6")) != "1")
            {
                e.Row.Cells[19].ForeColor = System.Drawing.Color.Red;
            }
            if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "SHSTS7")) != "1")
            {
                e.Row.Cells[21].ForeColor = System.Drawing.Color.Red;
            }
            //ShowData();


           
        }

       
    }

    protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
    {

    }
    protected void imgbtnPrint_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("~/SCE066_RPT.aspx?DATEFROM=" + datepicker1.Value + "&DATETO=" + datepicker2.Value);
    }
    protected void Button1_Click(object sender, EventArgs e)
    {

        
    }

    private void SendMailToSale(String InvoiceNo,string doctype)
    {

        connection.Open();
        string CustCode = "";

        string sql_findcust = "select * from itprod.shdoch " +
                              " where shcono = 100 and shdivi = 'PFT' " +
                              "  AND  SHIVNO = '" + InvoiceNo + "'";
        iDB2Command comm_findcust = new iDB2Command(sql_findcust, connection);
        iDB2DataReader reader_findcust = comm_findcust.ExecuteReader();

        if (reader_findcust.Read())
        {
            CustCode = reader_findcust["shcuno"].ToString().Trim();
        }

        string sql_custname = "select * from mvxcdtprod.ocusma " +
                     " where okcono = 100 and okcuno = '" + CustCode + "'";

        iDB2Command comm_custname = new iDB2Command(sql_custname, connection);
        iDB2DataReader reader_custname = comm_custname.ExecuteReader();
        string _CustName = "";
        while (reader_custname.Read())
        {
            _CustName = reader_custname["okcunm"].ToString().Trim().Substring(0, 15);
        }

         connection.Close();
       

        MailMessage msg = new MailMessage();
        //msg.To.Add("itsaret@patayafood.com");
        //msg.To.Add("groupitbkk@patayafood.com");
        msg.To.Add("salessupportgroup@patayafood.com");
        msg.To.Add("salessupportnft@patayafood.com");
        msg.From = new MailAddress("lg-doc@it.patayafood.com");
        msg.Subject = "Complete Upload ====> " + "Invoice No = " + InvoiceNo + ", Customer = " + _CustName + ", Doc Type = " + doctype;
       // msg.Body = "Invoice No = " + InvoiceNo + ", Customer = " + _CustName + ", Doc Type = " + doctype;
        SmtpClient smtp = new SmtpClient();
        smtp.Host = "172.16.1.51";
        smtp.Port = 25;
        smtp.Credentials = new System.Net.NetworkCredential("lg-doc@it.patayafood.com", "lgpass");
        smtp.Send(msg);

    }




    protected void Button1_Click1(object sender, EventArgs e)
    {
 

    }
}
