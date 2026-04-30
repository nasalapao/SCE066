using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using IBM.Data.DB2.iSeries;
using System.Data.SqlClient;
using System.Data;

using Microsoft.Reporting.WebForms;

public partial class SCE066_RPT : System.Web.UI.Page
{

    iDB2Connection connection = new iDB2Connection("DataSource=172.16.33.49;UserID=mvxreport;Password=report;DataCompression=True;");   
   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DDL_User();
            DDL_Cust();
            list();

            //ShowReport();

        }

    }

    private void list()
    {
        datepicker1.Value = Request.QueryString.Get("DATEFROM");
        datepicker2.Value = Request.QueryString.Get("DATETO");

        DATEFROM.Value = datepicker1.Value;
        DATETO.Value = datepicker2.Value;

       // MONO.Value = Request.QueryString.Get("MONO");

      //  txtMONO1.Text = MONO.Value;

    }

    private void DDL_User()
    {
        // dropdownlist customer/disticnt
        string sql_user = "select shusid from " +
                          "( " +
                          "  select '%' as shusid from mvxcdtprod.ocusma " +
                          " where okcono = 100 and okcuno = 'ETH0001' " +

                          "  union " +

                          "  select distinct shusid as shusid " +
                          "  from itprod.shdoch) " +
                          "  data_user order by shusid ";


        iDB2Command cmd_user = new iDB2Command(sql_user, connection);
        iDB2DataAdapter sda= new iDB2DataAdapter(cmd_user);


        DataTable dt = new DataTable();
        sda.Fill(dt);
        ddlUSER.DataSource = dt;

        ddlUSER.DataTextField = "SHUSID";
        ddlUSER.DataValueField = "SHUSID";
        ddlUSER.DataBind();

    }

    private void DDL_Cust()
    {
        // dropdownlist customer/disticnt
        string sql_cust = "SELECT OKCUNM,SHCUNO FROM (" +
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

    private void ShowReport()
    {

        string _datefrom = datepicker1.Value.Substring(6, 4) + datepicker1.Value.Substring(3, 2) + datepicker1.Value.Substring(0, 2);
        string _dateto = datepicker2.Value.Substring(6, 4) + datepicker2.Value.Substring(3, 2) + datepicker2.Value.Substring(0, 2);

        DATEFROM.Value = datepicker1.Value;
        DATETO.Value = datepicker2.Value;

        string _cust = ddlCUST.Text.Trim();

        //reset
        rptViewer.Reset();

        // data source
        DataTable dt = GetData(_datefrom,_dateto,ddlUSER.Text,ddlCUST.Text.Trim());
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);

        rptViewer.LocalReport.DataSources.Add(rds);

        // path
        rptViewer.LocalReport.ReportPath = "SCE066_Report.rdlc";

        // parameter
        ReportParameter[] para = new ReportParameter[4];
        para[0] = new ReportParameter("FromDate",_datefrom);
        para[1] = new ReportParameter("ToDate", _dateto);
        para[2] = new ReportParameter("USERID", ddlUSER.Text);
        para[3] = new ReportParameter("Customer",ddlCUST.Text.Trim());

        // Pass Parameters for Local Report
        rptViewer.LocalReport.SetParameters(para);

        // refresh
        rptViewer.LocalReport.Refresh();     
    }

    
    private DataTable GetData(string _datefrom,string _dateto, string userid,string _customer)
    {
       
        DataTable dt = new DataTable();

        string sql = "select shusid,shivno,oaorst,shcuno,okcunm,drtx40,shetdd,shetad,dacdld,diff_dat1,diff_dat8,diff_dat2,diff_dat3,diff_dat4,diff_dat5,diff_dat6,diff_dat7,shdat8,shstatus," +
                     " trim(slremk1) || ' ' ||  case when trim(slremk2) is null then '' else trim(slremk2) end  || ' ' || case when trim(slremk3) is null then '' else trim(slremk3) end " +
                     " || ' ' || case when trim(slremk4) is null then '' else trim(slremk4) end || ' ' || case when trim(slremk5) is null then '' else trim(slremk5) end || '' || case when trim(slremk6) is null then '' else trim(slremk6) end as slremk " +
                     " , csytab_tepy.cttx15 as payterm,UATEDL As DeliTerm " +
                     "from " +
                     "   ( " +
                     "   select shcono,shdivi,shusid,shivno,shcuno,shetdd,shetad,darout,dacdld,shdat1,shdat2,shdat3,shdat4,shdat5, " +
                     "   case when coalesce(shdat8,0) = 0 then coalesce(rtrim(char(invdoc.invdate)), '') else rtrim(char(shdat8)) end as shdat8, " +
                     "  DAYS(SUBSTR( CHAR(dacdld ),1,4) ||'-' || SUBSTR(CHAR (dacdld),5,2) || '-' || SUBSTR(CHAR (dacdld ),7,2))  as deadlinedate, " +
                     "   DAYS(SUBSTR( CHAR(shdat1),1,4) ||'-' || SUBSTR(CHAR (shdat1),5,2) || '-' || SUBSTR(CHAR (shdat1),7,2))  as date1, " +
                     "   DAYS(SUBSTR( CHAR(shetdd),1,4) ||'-' || SUBSTR(CHAR (shetdd),5,2) || '-' || SUBSTR(CHAR (shetdd),7,2)) - " +
                     "   DAYS(SUBSTR( CHAR(shdat1),1,4) ||'-' || SUBSTR(CHAR (shdat1),5,2) || '-' || SUBSTR(CHAR (shdat1),7,2)) AS DIFF_DAT1, " +
                     "   case when coalesce(shdat8,0) = 0 and invdoc.invdate is null then null else " +
                     "   DAYS(SUBSTR( CHAR(shetdd),1,4) ||'-' || SUBSTR(CHAR (shetdd),5,2) || '-' || SUBSTR(CHAR (shetdd),7,2)) - " +
                     "   DAYS(SUBSTR( CHAR(case when coalesce(shdat8,0) = 0 then invdoc.invdate else shdat8 end),1,4) ||'-' || SUBSTR(CHAR (case when coalesce(shdat8,0) = 0 then invdoc.invdate else shdat8 end),5,2) || '-' || SUBSTR(CHAR (case when coalesce(shdat8,0) = 0 then invdoc.invdate else shdat8 end),7,2)) end AS DIFF_DAT8, " +
                     "   DAYS(SUBSTR( CHAR(shetdd),1,4) ||'-' || SUBSTR(CHAR (shetdd),5,2) || '-' || SUBSTR(CHAR (shetdd),7,2)) - " +
                     "   DAYS(SUBSTR( CHAR(shdat2),1,4) ||'-' || SUBSTR(CHAR (shdat2),5,2) || '-' || SUBSTR(CHAR (shdat2),7,2)) AS DIFF_DAT2, " +
                     "   DAYS(SUBSTR( CHAR(shetdd),1,4) ||'-' || SUBSTR(CHAR (shetdd),5,2) || '-' || SUBSTR(CHAR (shetdd),7,2)) - " +
                     "   DAYS(SUBSTR( CHAR(shdat3),1,4) ||'-' || SUBSTR(CHAR (shdat3),5,2) || '-' || SUBSTR(CHAR (shdat3),7,2)) AS DIFF_DAT3, " +
                     "   DAYS(SUBSTR( CHAR(shetdd),1,4) ||'-' || SUBSTR(CHAR (shetdd),5,2) || '-' || SUBSTR(CHAR (shetdd),7,2)) - " +
                     "   DAYS(SUBSTR( CHAR(shdat4),1,4) ||'-' || SUBSTR(CHAR (shdat4),5,2) || '-' || SUBSTR(CHAR (shdat4),7,2)) AS DIFF_DAT4, " +
                     "   DAYS(SUBSTR( CHAR(shetdd),1,4) ||'-' || SUBSTR(CHAR (shetdd),5,2) || '-' || SUBSTR(CHAR (shetdd),7,2)) - " +
                     "   DAYS(SUBSTR( CHAR(shdat5),1,4) ||'-' || SUBSTR(CHAR (shdat5),5,2) || '-' || SUBSTR(CHAR (shdat5),7,2)) AS DIFF_DAT5, " +
                     "   DAYS(SUBSTR( CHAR(shetdd),1,4) ||'-' || SUBSTR(CHAR (shetdd),5,2) || '-' || SUBSTR(CHAR (shetdd),7,2)) - " +
                     "   DAYS(SUBSTR( CHAR(shdat6),1,4) ||'-' || SUBSTR(CHAR (shdat6),5,2) || '-' || SUBSTR(CHAR (shdat6),7,2)) AS DIFF_DAT6, " +
                     "   DAYS(SUBSTR( CHAR(shetdd),1,4) ||'-' || SUBSTR(CHAR (shetdd),5,2) || '-' || SUBSTR(CHAR (shetdd),7,2)) - " +
                     "   DAYS(SUBSTR( CHAR(shdat7),1,4) ||'-' || SUBSTR(CHAR (shdat7),5,2) || '-' || SUBSTR(CHAR (shdat7),7,2)) AS DIFF_DAT7 " +
                     "   ,case when shstsd = '0' then 'Un-Complete' else 'Complete' end shstatus " +
                     "   from itprod.shdoch " +
                     "   left join (select slcono,sldivi,slivno,max(sldate) as invdate from itprod.shdocl where sldatu = '8' group by slcono,sldivi,slivno) invdoc on invdoc.slcono = shcono and invdoc.sldivi = shdivi and invdoc.slivno = shivno " +
                     "   left join mvxcdtprod.dconsi on dacono = shcono and daconn = shivno " +
                     "  where shetdd >= " + _datefrom + " and shetdd <= " + _dateto;

        if (!_customer.Equals("0") && !_customer.Equals(""))
        {
            sql = sql + "   and shcuno = '" + _customer + "'";
        }



        sql = sql +  " ) " +
                     "   data " +
                     " left join mvxcdtprod.ocusma on okcono = shcono and okcuno = shcuno " +
            //   " left join " +
            //  " (select uacono,uadivi,uaconn,uarout,max(uaorno) uaorno from mvxcdtprod.odhead group by uacono,uadivi,uaconn,uarout) odhead on uacono = shcono and uadivi = shdivi and uaconn = shivno " +
                     " left join " +
                     "   (select distinct drcono,drrutp,drrout,drtx40 from mvxcdtprod.droute where drcono = 100 and drrutp in('1','3')) droute on drcono = shcono and drrout = darout " +
                     " left join " +
                     "   (SELECT distinct OQCONN,max(OBORST) as oaorst FROM MVXCDTPROD.MHDISH " +
                     "    LEFT JOIN MVXCDTPROD.MHDISL ON OQCONO = URCONO AND OQDLIX = URDLIX " +
                     "    LEFT JOIN MVXCDTPROD.OOLINE ON OBORNO = OQRIDN AND OBPONR = URRIDL WHERE OQCONO = 100 AND OQTRDT >= 20180101 group by oqconn) data_sts on oqconn = shivno " +
                     " left join " +
                     "   ( " +
                     "   select shivno as remivno ,slremk1,slremk2,slremk3,slremk4,slremk5,slremk6 " +
                     "   from itprod.shdoch " +
                     "   left join " +
                     "   (select slivno,max(slrevi) maxrevi1 from itprod.shdocl where sldatu = '1' group by slivno) shdocl_max1 on shivno = shdocl_max1.slivno " +
                     "   left join " +
                     "   (select slivno,slrevi,case when (slremk is null or slremk = '') then ' ' else 'Ship Doc. ' || slremk end as slremk1 from itprod.shdocl where sldatu = '1') shdocl_1 on shivno = shdocl_1.slivno and shdocl_1.slrevi = shdocl_max1.maxrevi1 " +
                     "   left join " +
                     "   (select slivno,max(slrevi) maxrevi2 from itprod.shdocl where sldatu = '2' group by slivno) shdocl_max2 on shivno = shdocl_max2.slivno " +
                     "   left join " +
                     "   (select slivno,slrevi,case when (slremk is null or slremk = '')  then ' ' else 'QA Doc. ' || slremk end as slremk2 from itprod.shdocl where sldatu = '2') shdocl_2 on shivno = shdocl_2.slivno and shdocl_2.slrevi = shdocl_max2.maxrevi2 " +
                     "   left join " +
                     "   (select slivno,max(slrevi) maxrevi3 from itprod.shdocl where sldatu = '3' group by slivno) shdocl_max3 on shivno = shdocl_max3.slivno " +
                     "   left join " +
                     "   (select slivno,slrevi,case when (slremk is null or slremk = '')  then ' ' else 'Form Doc. ' || slremk end as slremk3 from itprod.shdocl where sldatu = '3') shdocl_3 on shivno = shdocl_3.slivno and shdocl_3.slrevi = shdocl_max3.maxrevi3 " +
                     "   left join " +
                     "   (select slivno,max(slrevi) maxrevi4 from itprod.shdocl where sldatu = '4' group by slivno) shdocl_max4 on shivno = shdocl_max4.slivno " +
                     "   left join " +
                     "   (select slivno,slrevi,case when (slremk is null or slremk = '')  then ' ' else 'HC Doc.' || slremk end as slremk4 from itprod.shdocl where sldatu = '4') shdocl_4 on shivno = shdocl_4.slivno and shdocl_4.slrevi = shdocl_max4.maxrevi4 " +
                     "   left join " +
                     "   (select slivno,max(slrevi) maxrevi5 from itprod.shdocl where sldatu = '5' group by slivno) shdocl_max5 on shivno = shdocl_max5.slivno " +
                     "   left join " +
                     "   (select slivno,slrevi,case when (slremk is null or slremk = '') then ' ' else 'DHL Doc.' || slremk end as slremk5 from itprod.shdocl where sldatu = '5') shdocl_5 on shivno = shdocl_5.slivno and shdocl_5.slrevi = shdocl_max5.maxrevi5 " +

                     "  left join  " +
                     "   (select slivno,max(slrevi) maxrevi6 from itprod.shdocl where sldatu = '6' group by slivno) shdocl_max6 on shivno = shdocl_max6.slivno " +
                     "  left join " +
                     "   (select slivno,slrevi,case when (slremk is null or slremk = '') then ' ' else 'P/L.' || slremk end as slremk6 from itprod.shdocl where sldatu = '6') shdocl_6 on shivno = shdocl_6.slivno and shdocl_6.slrevi = shdocl_max6.maxrevi6 " +
                     "   ) rem on shivno = remivno " +
                     "  left join (select distinct uacono,uadivi,uaconn,uatepy,uatedl from mvxcdtprod.odhead) odhead on uacono = shcono and uadivi = shdivi and uaconn = shivno " +
                     " left join mvxcdtprod.csytab csytab_tepy on csytab_tepy.ctcono = shcono and csytab_tepy.ctstco = 'TEPY' and csytab_tepy.ctlncd = 'GB' and csytab_tepy.ctstky = uatepy " +
                     " left join mvxcdtprod.csytab csytab_tedl on csytab_tedl.ctcono = shcono and csytab_tedl.ctstco = 'TEDL' and csytab_tedl.ctlncd = 'GB' AND csytab_tedl.ctstky = uatedl ";
                   
                     //"   where shetdd >= " + _datefrom + " and shetdd <= " + _dateto;
        if (!userid.Equals("%"))
            sql = sql + " where shusid = '" + userid + "'";
        else
            sql = sql + " where (shusid is null or shusid like '%') ";

        if (ddlTYPE.Text.Trim().Equals("Delay"))
        {
            sql = sql + "   and (diff_dat1 < 0 or diff_dat8 < 0 or diff_dat2 < 0 or diff_dat3 < 0 or diff_dat4 < 0 or diff_dat5 < 0 or diff_dat6 < 0 or diff_dat7 < 0) ";
        }
        sql = sql + " order by shusid , shetdd ";
        
        iDB2DataAdapter adp = new iDB2DataAdapter(sql, connection);
        adp.Fill(dt);


        return dt;
         

    }

    protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
    {
        ShowReport();
    }
    protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
    {

        Response.Redirect("~/SCE066.aspx?DATEFROM=" + DATEFROM.Value + "&DATETO=" + DATETO.Value);
  
       // Response.Redirect("~/SCE066.aspx?DATEFROM=" + datepicker1.Value + "&DATETO=" + datepicker2.Value );
    }
}
