using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;

public partial class Home : Page
{
    private const string ProgramCode = "SCE066";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            BindProgramUpdateLog();
        }
    }

    private void BindProgramUpdateLog()
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT CHANGE_DATE,
                   CHANGE_TITLE,
                   CHANGE_DETAIL
              FROM ITPROD.SHCPRLOG
             WHERE PROGRAM_CODE = @PROGRAM_CODE
               AND ACTIVE_STATUS = 'Y'
             ORDER BY CHANGE_DATE DESC, LOG_ID DESC
             FETCH FIRST 10 ROWS ONLY";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PROGRAM_CODE", ProgramCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            rptProgramUpdate.Visible = false;
            lbUpdateMessage.Text = "ไม่สามารถโหลดรายการอัปเดตโปรแกรมได้ กรุณาติดต่อผู้ดูแลระบบ";
            lbUpdateMessage.CssClass = "update-message is-error";
            lbUpdateMessage.Visible = true;
            return;
        }

        if (dt.Rows.Count == 0)
        {
            rptProgramUpdate.Visible = false;
            lbUpdateMessage.Text = "ยังไม่มีรายการอัปเดตโปรแกรม";
            lbUpdateMessage.CssClass = "update-message";
            lbUpdateMessage.Visible = true;
            return;
        }

        lbUpdateMessage.Visible = false;
        rptProgramUpdate.Visible = true;
        rptProgramUpdate.DataSource = dt;
        rptProgramUpdate.DataBind();
    }

    protected string FormatChangeDate(object value)
    {
        if (value == null || value == DBNull.Value)
        {
            return "";
        }

        DateTime dateValue;
        if (!DateTime.TryParse(Convert.ToString(value), out dateValue))
        {
            return Server.HtmlEncode(Convert.ToString(value));
        }

        return dateValue.ToString("dd/MM/yyyy");
    }
}
