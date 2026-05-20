using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AdminProgramUpdate : Page
{
    private const string ProgramCode = "SCE066";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PermissionManager.RedirectIfNoPermission(this, PermissionManager.PageCodes.ProgramUpdateAdmin))
        {
            return;
        }

        if (!Page.IsPostBack)
        {
            ClearMessage();
            ClearForm();
            BindProgramUpdateList();
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        ClearMessage();
        ClearForm();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        ClearMessage();
        SaveProgramUpdate();
    }

    protected void btnReload_Click(object sender, EventArgs e)
    {
        ClearMessage();
        ClearForm();
        BindProgramUpdateList();
    }

    protected void gvProgramUpdate_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ClearMessage();

        if (e.CommandName != "EditLog" && e.CommandName != "DeleteLog")
        {
            return;
        }

        int rowIndex;
        if (!int.TryParse(Convert.ToString(e.CommandArgument), out rowIndex) ||
            rowIndex < 0 ||
            rowIndex >= gvProgramUpdate.DataKeys.Count)
        {
            ShowError("Selected program update row is invalid");
            return;
        }

        if (e.CommandName == "EditLog")
        {
            LoadRowToForm(gvProgramUpdate.DataKeys[rowIndex]);
        }
        else
        {
            DeleteProgramUpdate(gvProgramUpdate.DataKeys[rowIndex]);
        }
    }

    private void BindProgramUpdateList()
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT LOG_ID,
                   PROGRAM_CODE,
                   CHANGE_DATE,
                   CHANGE_TITLE,
                   CHANGE_DETAIL,
                   ACTIVE_STATUS,
                   CREATED_DATE,
                   UPDATED_DATE,
                   UPDATED_USER
              FROM ITPROD.SHCPRLOG
             WHERE PROGRAM_CODE = @PROGRAM_CODE
             ORDER BY CHANGE_DATE DESC, LOG_ID DESC
             FETCH FIRST 200 ROWS ONLY";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PROGRAM_CODE", ProgramCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        gvProgramUpdate.DataSource = dt;
        gvProgramUpdate.DataBind();
    }

    private void SaveProgramUpdate()
    {
        DateTime changeDate;
        string title;
        string detail;

        if (!ValidateInput(out changeDate, out title, out detail))
        {
            return;
        }

        if (hdMode.Value == "EDIT")
        {
            UpdateProgramUpdate(changeDate, title, detail);
        }
        else
        {
            InsertProgramUpdate(changeDate, title, detail);
        }
    }

    private void InsertProgramUpdate(DateTime changeDate, string title, string detail)
    {
        dbConnect db = new dbConnect();
        string sql = string.Format(@"
            INSERT INTO ITPROD.SHCPRLOG
                (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
            VALUES
                (@PROGRAM_CODE, DATE('{0}'), @CHANGE_TITLE, @CHANGE_DETAIL, @ACTIVE_STATUS, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @UPDATED_USER)",
            FormatDbDate(changeDate));

        db.InsertData(sql, BuildInsertParams(changeDate, title, detail));

        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        ShowSuccess("Add Program Update success");
        ClearForm();
        BindProgramUpdateList();
    }

    private void UpdateProgramUpdate(DateTime changeDate, string title, string detail)
    {
        int logId;
        if (!int.TryParse(hdLogId.Value, out logId))
        {
            ShowError("Please select update row before save");
            return;
        }

        dbConnect db = new dbConnect();
        string sql = string.Format(@"
            UPDATE ITPROD.SHCPRLOG
               SET CHANGE_DATE = DATE('{0}'),
                   CHANGE_TITLE = @CHANGE_TITLE,
                   CHANGE_DETAIL = @CHANGE_DETAIL,
                   ACTIVE_STATUS = @ACTIVE_STATUS,
                   UPDATED_DATE = CURRENT_TIMESTAMP,
                   UPDATED_USER = @UPDATED_USER
             WHERE LOG_ID = @LOG_ID
               AND PROGRAM_CODE = @PROGRAM_CODE",
            FormatDbDate(changeDate));

        int rows = db.ExecuteNonQuery(sql, BuildUpdateParams(changeDate, title, detail, logId));
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (rows == 0)
        {
            ShowError("Program update not found for update");
            return;
        }

        ShowSuccess("Update Program Update success");
        ClearForm();
        BindProgramUpdateList();
    }

    private void DeleteProgramUpdate(DataKey key)
    {
        int logId;
        if (!int.TryParse(Convert.ToString(key.Values["LOG_ID"]), out logId))
        {
            ShowError("Selected program update row is invalid");
            return;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            DELETE FROM ITPROD.SHCPRLOG
             WHERE LOG_ID = @LOG_ID
               AND PROGRAM_CODE = @PROGRAM_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@LOG_ID", logId);
        param.Add("@PROGRAM_CODE", ProgramCode);

        int rows = db.ExecuteNonQuery(sql, param);
        if (db.isError)
        {
            ShowError(db.ErrorMessage);
            return;
        }

        if (rows == 0)
        {
            ShowError("Program update not found for delete");
            return;
        }

        ShowSuccess("Delete Program Update success");
        ClearForm();
        BindProgramUpdateList();
    }

    private bool ValidateInput(out DateTime changeDate, out string title, out string detail)
    {
        changeDate = DateTime.MinValue;
        title = (txtChangeTitle.Text ?? "").Trim();
        detail = (txtChangeDetail.Text ?? "").Trim();

        if (!TryParseChangeDate((txtChangeDate.Text ?? "").Trim(), out changeDate))
        {
            ShowError("กรุณาเลือกวันที่อัปเดต");
            return false;
        }

        if (string.IsNullOrEmpty(title))
        {
            ShowError("กรุณากรอกหัวข้อ");
            return false;
        }

        if (string.IsNullOrEmpty(detail))
        {
            ShowError("กรุณากรอกรายละเอียด");
            return false;
        }

        return true;
    }

    private Dictionary<string, object> BuildInsertParams(DateTime changeDate, string title, string detail)
    {
        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PROGRAM_CODE", ProgramCode);
        param.Add("@CHANGE_TITLE", title);
        param.Add("@CHANGE_DETAIL", detail);
        param.Add("@ACTIVE_STATUS", chkActive.Checked ? "Y" : "N");
        param.Add("@UPDATED_USER", PermissionManager.GetCurrentPersonCode(this));
        return param;
    }

    private Dictionary<string, object> BuildUpdateParams(DateTime changeDate, string title, string detail, int logId)
    {
        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@CHANGE_TITLE", title);
        param.Add("@CHANGE_DETAIL", detail);
        param.Add("@ACTIVE_STATUS", chkActive.Checked ? "Y" : "N");
        param.Add("@UPDATED_USER", PermissionManager.GetCurrentPersonCode(this));
        param.Add("@LOG_ID", logId);
        param.Add("@PROGRAM_CODE", ProgramCode);
        return param;
    }

    private void LoadRowToForm(DataKey key)
    {
        hdMode.Value = "EDIT";
        hdLogId.Value = Convert.ToString(key.Values["LOG_ID"]);

        DateTime changeDate;
        if (DateTime.TryParse(Convert.ToString(key.Values["CHANGE_DATE"]), out changeDate))
        {
            txtChangeDate.Text = FormatInputDate(changeDate);
        }
        else
        {
            txtChangeDate.Text = "";
        }

        txtChangeTitle.Text = Convert.ToString(key.Values["CHANGE_TITLE"]);
        txtChangeDetail.Text = Convert.ToString(key.Values["CHANGE_DETAIL"]);
        chkActive.Checked = Convert.ToString(key.Values["ACTIVE_STATUS"]).Trim() == "Y";
    }

    private void ClearForm()
    {
        hdMode.Value = "NEW";
        hdLogId.Value = "";
        txtChangeDate.Text = FormatInputDate(DateTime.Today);
        txtChangeTitle.Text = "";
        txtChangeDetail.Text = "";
        chkActive.Checked = true;
    }

    private bool TryParseChangeDate(string value, out DateTime changeDate)
    {
        changeDate = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "M/d/yyyy" };
        foreach (string format in formats)
        {
            DateTime parsed;
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                changeDate = NormalizeBuddhistYear(parsed);
                return true;
            }
        }

        DateTime fallback;
        if (DateTime.TryParse(value, CultureInfo.GetCultureInfo("th-TH"), DateTimeStyles.None, out fallback) ||
            DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out fallback))
        {
            changeDate = NormalizeBuddhistYear(fallback);
            return true;
        }

        return false;
    }

    private DateTime NormalizeBuddhistYear(DateTime value)
    {
        if (value.Year > 2400)
        {
            return new DateTime(value.Year - 543, value.Month, value.Day);
        }

        return value;
    }

    private string FormatInputDate(DateTime value)
    {
        return value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    }

    private string FormatDbDate(DateTime value)
    {
        return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
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
