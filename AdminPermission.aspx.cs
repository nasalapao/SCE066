using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AdminPermission : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PermissionManager.RedirectIfNoPermission(this, PermissionManager.PageCodes.PermissionAdmin))
        {
            return;
        }

        if (!Page.IsPostBack)
        {
            ClearMessage();
            BindGroupList();
            BindEmployeeList();
            ResetForm();
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        ClearMessage();
        ResetForm();
        txtPersonCode.Focus();
    }

    protected void btnInsertEmployee_Click(object sender, EventArgs e)
    {
        ClearMessage();
        InsertEmployee();
    }

    protected void btnLoadEmployee_Click(object sender, EventArgs e)
    {
        ClearMessage();
        LoadPersonCode();
    }

    protected void btnUpdateEmployee_Click(object sender, EventArgs e)
    {
        ClearMessage();
        UpdateEmployee();
    }

    protected void btnDeleteEmployee_Click(object sender, EventArgs e)
    {
        ClearMessage();
        DeleteEmployee();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearMessage();
        ResetForm();
    }

    protected void btnSavePermission_Click(object sender, EventArgs e)
    {
        ClearMessage();
        SavePagePermissions();
    }

    protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "SelectEmployee" && e.CommandName != "EditEmployee")
        {
            return;
        }

        int rowIndex;
        if (!int.TryParse(Convert.ToString(e.CommandArgument), out rowIndex) || rowIndex < 0 || rowIndex >= gvEmployees.DataKeys.Count)
        {
            ShowError("Invalid employee row.");
            return;
        }

        string personCode = Convert.ToString(gvEmployees.DataKeys[rowIndex].Value).Trim();
        string permissionGroup = PermissionManager.GetPermissionGroup(personCode);

        ClearMessage();
        LoadEmployee(personCode, permissionGroup, e.CommandName == "EditEmployee");
    }

    private void InsertEmployee()
    {
        string personCode = txtPersonCode.Text.Trim();
        string permissionGroup = ResolvePermissionGroup();
        if (!ValidateEmployeeForm(personCode, permissionGroup))
        {
            return;
        }

        string errorMessage;
        bool success = PermissionManager.InsertEmployeePermissions(personCode, permissionGroup, PermissionManager.GetCurrentPersonCode(this), out errorMessage);
        if (!success)
        {
            ShowError(errorMessage);
            return;
        }

        BindEmployeeList();
        LoadEmployee(personCode, permissionGroup, true);
        ShowSuccess("Employee permission inserted.");
    }

    private void UpdateEmployee()
    {
        string personCode = txtPersonCode.Text.Trim();
        string permissionGroup = ResolvePermissionGroup();
        if (!ValidateEmployeeForm(personCode, permissionGroup))
        {
            return;
        }

        string errorMessage;
        bool success = PermissionManager.UpdateEmployeeGroup(personCode, permissionGroup, true, PermissionManager.GetCurrentPersonCode(this), out errorMessage);
        if (!success)
        {
            ShowError(errorMessage);
            return;
        }

        BindEmployeeList();
        LoadEmployee(personCode, permissionGroup, true);
        ShowSuccess("Employee group updated.");
    }

    private void DeleteEmployee()
    {
        string personCode = txtPersonCode.Text.Trim();
        if (string.IsNullOrEmpty(personCode))
        {
            ShowError("Person Code is required.");
            return;
        }

        string errorMessage;
        bool success = PermissionManager.DeleteEmployeePermissions(personCode, out errorMessage);
        if (!success)
        {
            ShowError(errorMessage);
            return;
        }

        BindEmployeeList();
        ResetForm();
        ShowSuccess("Employee permission deleted.");
    }

    private void SavePagePermissions()
    {
        string personCode = hdSelectedPersonCode.Value.Trim();
        if (string.IsNullOrEmpty(personCode))
        {
            ShowError("Please select employee before saving permission.");
            return;
        }

        string permissionGroup = hdPermissionGroup.Value.Trim();
        List<PermissionManager.PagePermissionValue> pagePermissions = ReadPagePermissions();

        string errorMessage;
        bool success = PermissionManager.SavePagePermissions(personCode, permissionGroup, pagePermissions, PermissionManager.GetCurrentPersonCode(this), out errorMessage);
        if (!success)
        {
            ShowError(errorMessage);
            return;
        }

        BindEmployeeList();
        LoadEmployee(personCode, permissionGroup, false);
        ShowSuccess("Page permission saved.");
    }

    private void LoadPersonCode()
    {
        string personCode = txtPersonCode.Text.Trim();
        if (string.IsNullOrEmpty(personCode))
        {
            ShowError("Person Code is required.");
            txtPersonCode.Focus();
            return;
        }

        string permissionGroup = PermissionManager.GetPermissionGroup(personCode);
        if (!string.IsNullOrEmpty(permissionGroup))
        {
            LoadEmployee(personCode, permissionGroup, true);
            return;
        }

        permissionGroup = ResolvePermissionGroup();
        hdMode.Value = "NEW";
        hdSelectedPersonCode.Value = personCode;
        hdPermissionGroup.Value = permissionGroup;

        txtPersonCode.Text = personCode;
        txtPersonCode.Enabled = true;
        ddlPermissionGroup.Enabled = true;
        txtManualPermissionGroup.Enabled = true;

        btnInsertEmployee.Visible = true;
        btnUpdateEmployee.Visible = false;
        btnDeleteEmployee.Visible = false;

        BindEmployeeInfo(personCode, permissionGroup);
        BindPermissionTable(personCode);
        pnlPermissions.Visible = true;
    }

    private List<PermissionManager.PagePermissionValue> ReadPagePermissions()
    {
        List<PermissionManager.PagePermissionValue> pagePermissions = new List<PermissionManager.PagePermissionValue>();

        foreach (GridViewRow row in gvPermissions.Rows)
        {
            if (row.RowIndex < 0 || row.RowIndex >= gvPermissions.DataKeys.Count)
            {
                continue;
            }

            CheckBox chkActive = row.FindControl("chkActive") as CheckBox;
            if (chkActive == null)
            {
                continue;
            }

            pagePermissions.Add(new PermissionManager.PagePermissionValue
            {
                PageCode = Convert.ToString(gvPermissions.DataKeys[row.RowIndex].Value),
                IsActive = chkActive.Checked
            });
        }

        return pagePermissions;
    }

    private void LoadEmployee(string personCode, string permissionGroup, bool editMode)
    {
        hdMode.Value = editMode ? "EDIT" : "VIEW";
        hdSelectedPersonCode.Value = personCode;
        hdPermissionGroup.Value = permissionGroup;

        txtPersonCode.Text = personCode;
        txtPersonCode.Enabled = editMode;
        SetGroupControls(permissionGroup);
        ddlPermissionGroup.Enabled = editMode;
        txtManualPermissionGroup.Enabled = editMode;

        btnInsertEmployee.Visible = false;
        btnUpdateEmployee.Visible = editMode;
        btnDeleteEmployee.Visible = editMode;

        BindEmployeeInfo(personCode, permissionGroup);
        BindPermissionTable(personCode);
        pnlPermissions.Visible = true;
    }

    private void ResetForm()
    {
        hdMode.Value = "NEW";
        hdSelectedPersonCode.Value = "";
        hdPermissionGroup.Value = "";

        txtPersonCode.Text = "";
        txtPersonCode.Enabled = true;
        SelectGroupValue("SSS");
        ddlPermissionGroup.Enabled = true;
        txtManualPermissionGroup.Text = "";
        txtManualPermissionGroup.Enabled = true;

        btnInsertEmployee.Visible = true;
        btnUpdateEmployee.Visible = false;
        btnDeleteEmployee.Visible = false;

        ShowEmptyEmployee();
        BindPermissionTable("");
        pnlPermissions.Visible = false;
    }

    private void BindEmployeeList()
    {
        gvEmployees.DataSource = PermissionManager.GetPermissionEmployees();
        gvEmployees.DataBind();
    }

    private void BindGroupList()
    {
        ddlPermissionGroup.Items.Clear();
        foreach (string group in PermissionManager.GetPermissionGroups())
        {
            ddlPermissionGroup.Items.Add(new ListItem(group, group));
        }

        ddlPermissionGroup.Items.Add(new ListItem("Manual", "MANUAL"));
    }

    private void BindEmployeeInfo(string personCode, string permissionGroup)
    {
        PermissionManager.EmployeeInfo employee = PermissionManager.GetEmployeeInfo(personCode);
        string displayName = employee.Found && !string.IsNullOrEmpty(employee.FullName) ? employee.FullName : "Not found in HRIS";

        litEmployeeName.Text = "<span class=\"employee-name\">" + Server.HtmlEncode(displayName) + "</span>";
        litEmployeeCode.Text = "<span class=\"employee-code\">" + Server.HtmlEncode(employee.PersonCode) + "</span>";
        litPermissionGroup.Text = "<span class=\"employee-group\">" + Server.HtmlEncode(string.IsNullOrEmpty(permissionGroup) ? "NO GROUP" : permissionGroup) + "</span>";
    }

    private void ShowEmptyEmployee()
    {
        litEmployeeName.Text = "<span class=\"employee-name\">New employee</span>";
        litEmployeeCode.Text = "<span class=\"employee-code\">-</span>";
        litPermissionGroup.Text = "<span class=\"employee-group\">NO GROUP</span>";
    }

    private void BindPermissionTable(string personCode)
    {
        List<PermissionManager.EmployeePermission> permissions = PermissionManager.GetEmployeePermissions(personCode);
        List<PermissionManager.EmployeePermission> orderedPermissions = new List<PermissionManager.EmployeePermission>();

        foreach (PermissionManager.EmployeePermission permission in permissions)
        {
            if (permission.IsActive)
            {
                orderedPermissions.Add(permission);
            }
        }

        foreach (PermissionManager.EmployeePermission permission in permissions)
        {
            if (!permission.IsActive)
            {
                orderedPermissions.Add(permission);
            }
        }

        gvPermissions.DataSource = orderedPermissions;
        gvPermissions.DataBind();
    }

    private bool ValidateEmployeeForm(string personCode, string permissionGroup)
    {
        if (string.IsNullOrEmpty(personCode))
        {
            ShowError("Person Code is required.");
            return false;
        }

        if (string.IsNullOrEmpty(permissionGroup))
        {
            ShowError("Group is required.");
            return false;
        }

        return true;
    }

    private string ResolvePermissionGroup()
    {
        string selectedGroup = ddlPermissionGroup.SelectedValue.Trim().ToUpper();
        if (selectedGroup == "MANUAL")
        {
            return txtManualPermissionGroup.Text.Trim().ToUpper();
        }

        return selectedGroup;
    }

    private void SetGroupControls(string permissionGroup)
    {
        permissionGroup = (permissionGroup ?? "").Trim().ToUpper();
        txtManualPermissionGroup.Text = "";

        if (ddlPermissionGroup.Items.FindByValue(permissionGroup) != null)
        {
            SelectGroupValue(permissionGroup);
            return;
        }

        SelectGroupValue("MANUAL");
        txtManualPermissionGroup.Text = permissionGroup;
    }

    private void SelectGroupValue(string value)
    {
        ListItem item = ddlPermissionGroup.Items.FindByValue(value);
        if (item == null)
        {
            return;
        }

        ddlPermissionGroup.ClearSelection();
        item.Selected = true;
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
