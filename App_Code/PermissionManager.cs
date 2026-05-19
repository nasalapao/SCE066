using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Web;
using System.Web.UI;

public static class PermissionManager
{
    private const string AuthCookieName = "SCE066_TOKEN";

    public static class PageCodes
    {
        public const string CustomerEmail = "CUSTOMER_EMAIL";
        public const string AdminEmail = "ADMIN_EMAIL";
        public const string AdminEmailTemplate = "ADMIN_EMAIL_TEMPLATE";
        public const string AdminEmailSender = "ADMIN_EMAIL_SENDER";
        public const string AdminEmailLog = "ADMIN_EMAIL_LOG";
        public const string PermissionAdmin = "PERMISSION_ADMIN";
    }

    public class PermissionPage
    {
        public string PageCode { get; set; }
        public string PageName { get; set; }
        public string GroupName { get; set; }
    }

    public class EmployeePermission
    {
        public string PageCode { get; set; }
        public string PageName { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }
    }

    public class EmployeeInfo
    {
        public string PersonCode { get; set; }
        public string FullName { get; set; }
        public bool Found { get; set; }
        public string DisplayName { get; set; }
        public string PermissionGroup { get; set; }
    }

    public class PagePermissionValue
    {
        public string PageCode { get; set; }
        public bool IsActive { get; set; }
    }

    public static List<PermissionPage> GetAllPages()
    {
        List<PermissionPage> pages = new List<PermissionPage>();
        pages.Add(new PermissionPage { PageCode = PageCodes.CustomerEmail, PageName = "Customer Email", GroupName = "Main" });
        pages.Add(new PermissionPage { PageCode = PageCodes.AdminEmail, PageName = "Email Management", GroupName = "Admin" });
        pages.Add(new PermissionPage { PageCode = PageCodes.AdminEmailTemplate, PageName = "Email Template", GroupName = "Admin" });
        pages.Add(new PermissionPage { PageCode = PageCodes.AdminEmailSender, PageName = "Email Sender", GroupName = "Admin" });
        pages.Add(new PermissionPage { PageCode = PageCodes.AdminEmailLog, PageName = "Email Log", GroupName = "Admin" });
        pages.Add(new PermissionPage { PageCode = PageCodes.PermissionAdmin, PageName = "Permission Management", GroupName = "Admin" });
        return pages;
    }

    public static bool HasPermission(string personCode, string pageCode)
    {
        personCode = NormalizePersonCode(personCode);
        pageCode = NormalizePageCode(pageCode);

        if (string.IsNullOrEmpty(personCode) || string.IsNullOrEmpty(pageCode))
        {
            return false;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT COUNT(*) AS CNT
              FROM ITPROD.SHDOCPERM
             WHERE PERSON_CODE = @PERSON_CODE
               AND PAGE_CODE = @PAGE_CODE
               AND ACTIVE_STATUS = 'Y'";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PERSON_CODE", personCode);
        param.Add("@PAGE_CODE", pageCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError || dt.Rows.Count == 0)
        {
            return false;
        }

        return Convert.ToInt32(dt.Rows[0]["CNT"]) > 0;
    }

    public static List<string> GetAllowedPageCodes(string personCode)
    {
        List<string> pageCodes = new List<string>();
        personCode = NormalizePersonCode(personCode);

        if (string.IsNullOrEmpty(personCode))
        {
            return pageCodes;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT PAGE_CODE
              FROM ITPROD.SHDOCPERM
             WHERE PERSON_CODE = @PERSON_CODE
               AND ACTIVE_STATUS = 'Y'";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PERSON_CODE", personCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            return pageCodes;
        }

        foreach (DataRow row in dt.Rows)
        {
            string pageCode = NormalizePageCode(Convert.ToString(row["PAGE_CODE"]));
            if (!string.IsNullOrEmpty(pageCode) && !pageCodes.Contains(pageCode))
            {
                pageCodes.Add(pageCode);
            }
        }

        return pageCodes;
    }

    public static List<EmployeePermission> GetEmployeePermissions(string personCode)
    {
        List<EmployeePermission> permissions = new List<EmployeePermission>();
        Dictionary<string, bool> activeByPage = GetActivePermissionMap(personCode);
        List<PermissionPage> pages = GetAllPages();

        foreach (PermissionPage page in pages)
        {
            bool isActive = activeByPage.ContainsKey(page.PageCode) && activeByPage[page.PageCode];
            permissions.Add(new EmployeePermission
            {
                PageCode = page.PageCode,
                PageName = page.PageName,
                GroupName = page.GroupName,
                IsActive = isActive
            });
        }

        return permissions;
    }

    public static EmployeeInfo GetEmployeeInfo(string personCode)
    {
        EmployeeInfo info = new EmployeeInfo();
        info.PersonCode = NormalizePersonCode(personCode);
        info.FullName = "";
        info.Found = false;
        info.DisplayName = "";
        info.PermissionGroup = GetPermissionGroup(info.PersonCode);

        if (string.IsNullOrEmpty(info.PersonCode))
        {
            return info;
        }

        dbConnectSQL db = new dbConnectSQL(7);
        DataTable dt = db.GetLoginPerson(info.PersonCode, "", true);
        if (db.isError || dt.Rows.Count == 0)
        {
            return info;
        }

        string firstName = Convert.ToString(dt.Rows[0]["FnameT"]).Trim();
        string lastName = Convert.ToString(dt.Rows[0]["LnameT"]).Trim();

        info.FullName = (firstName + " " + lastName).Trim();
        info.Found = true;
        info.DisplayName = string.IsNullOrEmpty(info.FullName) ? info.PersonCode : info.FullName + " (" + info.PersonCode + ")";
        return info;
    }

    public static List<EmployeeInfo> GetPermissionEmployees()
    {
        List<EmployeeInfo> employees = new List<EmployeeInfo>();

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT PERSON_CODE,
                   MAX(PERMISSION_GROUP) AS PERMISSION_GROUP,
                   MAX(UPDATED_DATE) AS LAST_UPDATED_DATE
              FROM ITPROD.SHDOCPERM
             GROUP BY PERSON_CODE
             ORDER BY MAX(PERMISSION_GROUP), PERSON_CODE";

        DataTable dt = db.ExecuteQuery(sql, new Dictionary<string, object>());
        if (db.isError)
        {
            return employees;
        }

        foreach (DataRow row in dt.Rows)
        {
            string personCode = NormalizePersonCode(Convert.ToString(row["PERSON_CODE"]));
            if (string.IsNullOrEmpty(personCode))
            {
                continue;
            }

            EmployeeInfo employee = GetEmployeeInfo(personCode);
            employee.PermissionGroup = NormalizePermissionGroup(Convert.ToString(row["PERMISSION_GROUP"]));
            if (string.IsNullOrEmpty(employee.DisplayName))
            {
                employee.DisplayName = employee.Found && !string.IsNullOrEmpty(employee.FullName)
                    ? employee.FullName + " (" + personCode + ")"
                    : personCode;
            }

            employees.Add(employee);
        }

        return employees;
    }

    public static List<string> GetPermissionGroups()
    {
        List<string> groups = new List<string>();
        AddGroup(groups, "SSS");
        AddGroup(groups, "ADMIN");

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT DISTINCT PERMISSION_GROUP
              FROM ITPROD.SHDOCPERM
             WHERE COALESCE(PERMISSION_GROUP, '') <> ''
             ORDER BY PERMISSION_GROUP";

        DataTable dt = db.ExecuteQuery(sql, new Dictionary<string, object>());
        if (!db.isError)
        {
            foreach (DataRow row in dt.Rows)
            {
                AddGroup(groups, Convert.ToString(row["PERMISSION_GROUP"]));
            }
        }

        return groups;
    }

    public static bool SavePermission(string targetPersonCode, string pageCode, bool active, string updatedUser, out string errorMessage)
    {
        return SavePermission(targetPersonCode, pageCode, active, "", updatedUser, out errorMessage);
    }

    public static bool SavePermission(string targetPersonCode, string pageCode, bool active, string permissionGroup, string updatedUser, out string errorMessage)
    {
        errorMessage = "";
        targetPersonCode = NormalizePersonCode(targetPersonCode);
        pageCode = NormalizePageCode(pageCode);
        permissionGroup = NormalizePermissionGroup(permissionGroup);
        updatedUser = NormalizePersonCode(updatedUser);

        if (string.IsNullOrEmpty(targetPersonCode))
        {
            errorMessage = "Person code is required.";
            return false;
        }

        if (!IsKnownPageCode(pageCode))
        {
            errorMessage = "Invalid page code.";
            return false;
        }

        if (PermissionExists(targetPersonCode, pageCode))
        {
            return UpdatePermission(targetPersonCode, pageCode, active, permissionGroup, updatedUser, out errorMessage);
        }

        return InsertPermission(targetPersonCode, pageCode, active, permissionGroup, updatedUser, out errorMessage);
    }

    public static bool InsertEmployeePermissions(string personCode, string permissionGroup, string updatedUser, out string errorMessage)
    {
        errorMessage = "";
        personCode = NormalizePersonCode(personCode);
        permissionGroup = NormalizePermissionGroup(permissionGroup);
        updatedUser = NormalizePersonCode(updatedUser);

        if (string.IsNullOrEmpty(personCode))
        {
            errorMessage = "Person code is required.";
            return false;
        }

        if (string.IsNullOrEmpty(permissionGroup))
        {
            errorMessage = "Group is required.";
            return false;
        }

        List<PermissionPage> pages = GetAllPages();
        foreach (PermissionPage page in pages)
        {
            bool isActive = GetDefaultActiveByGroup(permissionGroup, page.PageCode);
            if (!SavePermission(personCode, page.PageCode, isActive, permissionGroup, updatedUser, out errorMessage))
            {
                return false;
            }
        }

        return true;
    }

    public static bool UpdateEmployeeGroup(string personCode, string permissionGroup, bool applyDefaultPermissions, string updatedUser, out string errorMessage)
    {
        errorMessage = "";
        personCode = NormalizePersonCode(personCode);
        permissionGroup = NormalizePermissionGroup(permissionGroup);
        updatedUser = NormalizePersonCode(updatedUser);

        if (string.IsNullOrEmpty(personCode))
        {
            errorMessage = "Person code is required.";
            return false;
        }

        if (string.IsNullOrEmpty(permissionGroup))
        {
            errorMessage = "Group is required.";
            return false;
        }

        if (!EmployeePermissionExists(personCode))
        {
            errorMessage = "Permission data not found for this person code.";
            return false;
        }

        List<PermissionPage> pages = GetAllPages();
        foreach (PermissionPage page in pages)
        {
            bool active = applyDefaultPermissions
                ? GetDefaultActiveByGroup(permissionGroup, page.PageCode)
                : HasPermission(personCode, page.PageCode);

            if (PermissionExists(personCode, page.PageCode))
            {
                if (!UpdatePermission(personCode, page.PageCode, active, permissionGroup, updatedUser, out errorMessage))
                {
                    return false;
                }
            }
            else
            {
                if (!InsertPermission(personCode, page.PageCode, active, permissionGroup, updatedUser, out errorMessage))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool SavePagePermissions(string personCode, string permissionGroup, List<PagePermissionValue> pagePermissions, string updatedUser, out string errorMessage)
    {
        errorMessage = "";
        personCode = NormalizePersonCode(personCode);
        permissionGroup = NormalizePermissionGroup(permissionGroup);
        updatedUser = NormalizePersonCode(updatedUser);

        if (string.IsNullOrEmpty(personCode))
        {
            errorMessage = "Person code is required.";
            return false;
        }

        if (string.IsNullOrEmpty(permissionGroup))
        {
            permissionGroup = GetPermissionGroup(personCode);
        }

        if (pagePermissions == null || pagePermissions.Count == 0)
        {
            errorMessage = "Permission data is empty.";
            return false;
        }

        foreach (PagePermissionValue permission in pagePermissions)
        {
            if (permission == null)
            {
                continue;
            }

            if (!SavePermission(personCode, permission.PageCode, permission.IsActive, permissionGroup, updatedUser, out errorMessage))
            {
                return false;
            }
        }

        return true;
    }

    public static bool RedirectIfNoPermission(Page page, string pageCode)
    {
        if (page == null)
        {
            return false;
        }

        string personCode = GetCurrentPersonCode(page);
        if (string.IsNullOrEmpty(personCode))
        {
            RedirectToLogin(page);
            return false;
        }

        if (HasPermission(personCode, pageCode))
        {
            return true;
        }

        page.Response.Redirect("~/Home.aspx?PermissionDenied=" + page.Server.UrlEncode(pageCode), false);
        CompleteRequest();
        return false;
    }

    public static string GetCurrentPersonCode(Page page)
    {
        if (page == null || page.Request == null)
        {
            return "";
        }

        HttpCookie cookie = page.Request.Cookies[AuthCookieName];
        if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
        {
            return "";
        }

        ClaimsPrincipal principal = JwtHelper.ValidateToken(cookie.Value);
        if (principal == null)
        {
            return "";
        }

        Claim claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return claim == null ? "" : NormalizePersonCode(claim.Value);
    }

    private static Dictionary<string, bool> GetActivePermissionMap(string personCode)
    {
        Dictionary<string, bool> activeByPage = new Dictionary<string, bool>();
        personCode = NormalizePersonCode(personCode);

        if (string.IsNullOrEmpty(personCode))
        {
            return activeByPage;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT PAGE_CODE, ACTIVE_STATUS
              FROM ITPROD.SHDOCPERM
             WHERE PERSON_CODE = @PERSON_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PERSON_CODE", personCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError)
        {
            return activeByPage;
        }

        foreach (DataRow row in dt.Rows)
        {
            string pageCode = NormalizePageCode(Convert.ToString(row["PAGE_CODE"]));
            if (!string.IsNullOrEmpty(pageCode))
            {
                activeByPage[pageCode] = Convert.ToString(row["ACTIVE_STATUS"]).Trim() == "Y";
            }
        }

        return activeByPage;
    }

    private static bool PermissionExists(string personCode, string pageCode)
    {
        dbConnect db = new dbConnect();
        string sql = @"
            SELECT COUNT(*) AS CNT
              FROM ITPROD.SHDOCPERM
             WHERE PERSON_CODE = @PERSON_CODE
               AND PAGE_CODE = @PAGE_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PERSON_CODE", personCode);
        param.Add("@PAGE_CODE", pageCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError || dt.Rows.Count == 0)
        {
            return false;
        }

        return Convert.ToInt32(dt.Rows[0]["CNT"]) > 0;
    }

    private static bool EmployeePermissionExists(string personCode)
    {
        personCode = NormalizePersonCode(personCode);
        if (string.IsNullOrEmpty(personCode))
        {
            return false;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT COUNT(*) AS CNT
              FROM ITPROD.SHDOCPERM
             WHERE PERSON_CODE = @PERSON_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PERSON_CODE", personCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError || dt.Rows.Count == 0)
        {
            return false;
        }

        return Convert.ToInt32(dt.Rows[0]["CNT"]) > 0;
    }

    public static bool InsertPermission(string personCode, string pageCode, bool active, string updatedUser, out string errorMessage)
    {
        return InsertPermission(personCode, pageCode, active, "", updatedUser, out errorMessage);
    }

    public static bool InsertPermission(string personCode, string pageCode, bool active, string permissionGroup, string updatedUser, out string errorMessage)
    {
        errorMessage = "";
        personCode = NormalizePersonCode(personCode);
        pageCode = NormalizePageCode(pageCode);
        permissionGroup = NormalizePermissionGroup(permissionGroup);
        updatedUser = NormalizePersonCode(updatedUser);

        if (!ValidatePermissionKey(personCode, pageCode, out errorMessage))
        {
            return false;
        }

        if (PermissionExists(personCode, pageCode))
        {
            errorMessage = "Permission already exists.";
            return false;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            INSERT INTO ITPROD.SHDOCPERM
                (PERSON_CODE, PAGE_CODE, PERMISSION_GROUP, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
            VALUES
                (@PERSON_CODE, @PAGE_CODE, @PERMISSION_GROUP, @ACTIVE_STATUS, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @UPDATED_USER)";

        Dictionary<string, object> param = BuildInsertParams(personCode, pageCode, active, permissionGroup, updatedUser);
        db.InsertData(sql, param);

        errorMessage = db.isError ? db.ErrorMessage : "";
        return !db.isError;
    }

    public static bool UpdatePermission(string personCode, string pageCode, bool active, string updatedUser, out string errorMessage)
    {
        return UpdatePermission(personCode, pageCode, active, "", updatedUser, out errorMessage);
    }

    public static bool UpdatePermission(string personCode, string pageCode, bool active, string permissionGroup, string updatedUser, out string errorMessage)
    {
        errorMessage = "";
        personCode = NormalizePersonCode(personCode);
        pageCode = NormalizePageCode(pageCode);
        permissionGroup = NormalizePermissionGroup(permissionGroup);
        updatedUser = NormalizePersonCode(updatedUser);

        if (!ValidatePermissionKey(personCode, pageCode, out errorMessage))
        {
            return false;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            UPDATE ITPROD.SHDOCPERM
               SET ACTIVE_STATUS = @ACTIVE_STATUS,
                   PERMISSION_GROUP = @PERMISSION_GROUP,
                   UPDATED_DATE = CURRENT_TIMESTAMP,
                   UPDATED_USER = @UPDATED_USER
             WHERE PERSON_CODE = @PERSON_CODE
               AND PAGE_CODE = @PAGE_CODE";

        Dictionary<string, object> param = BuildUpdateParams(personCode, pageCode, active, permissionGroup, updatedUser);
        db.ExecuteNonQuery(sql, param);

        errorMessage = db.isError ? db.ErrorMessage : "";
        return !db.isError;
    }

    public static bool DeletePermission(string personCode, string pageCode, out string errorMessage)
    {
        errorMessage = "";
        personCode = NormalizePersonCode(personCode);
        pageCode = NormalizePageCode(pageCode);

        if (!ValidatePermissionKey(personCode, pageCode, out errorMessage))
        {
            return false;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            DELETE FROM ITPROD.SHDOCPERM
             WHERE PERSON_CODE = @PERSON_CODE
               AND PAGE_CODE = @PAGE_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PERSON_CODE", personCode);
        param.Add("@PAGE_CODE", pageCode);

        db.ExecuteNonQuery(sql, param);
        errorMessage = db.isError ? db.ErrorMessage : "";
        return !db.isError;
    }

    public static bool DeleteEmployeePermissions(string personCode, out string errorMessage)
    {
        errorMessage = "";
        personCode = NormalizePersonCode(personCode);

        if (string.IsNullOrEmpty(personCode))
        {
            errorMessage = "Person code is required.";
            return false;
        }

        dbConnect db = new dbConnect();
        string sql = @"
            DELETE FROM ITPROD.SHDOCPERM
             WHERE PERSON_CODE = @PERSON_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PERSON_CODE", personCode);

        db.ExecuteNonQuery(sql, param);
        errorMessage = db.isError ? db.ErrorMessage : "";
        return !db.isError;
    }

    public static string GetPermissionGroup(string personCode)
    {
        personCode = NormalizePersonCode(personCode);
        if (string.IsNullOrEmpty(personCode))
        {
            return "";
        }

        dbConnect db = new dbConnect();
        string sql = @"
            SELECT MAX(PERMISSION_GROUP) AS PERMISSION_GROUP
              FROM ITPROD.SHDOCPERM
             WHERE PERSON_CODE = @PERSON_CODE";

        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PERSON_CODE", personCode);

        DataTable dt = db.ExecuteQuery(sql, param);
        if (db.isError || dt.Rows.Count == 0)
        {
            return "";
        }

        return NormalizePermissionGroup(Convert.ToString(dt.Rows[0]["PERMISSION_GROUP"]));
    }

    private static Dictionary<string, object> BuildInsertParams(string personCode, string pageCode, bool active, string permissionGroup, string updatedUser)
    {
        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@PERSON_CODE", personCode);
        param.Add("@PAGE_CODE", pageCode);
        param.Add("@PERMISSION_GROUP", permissionGroup);
        param.Add("@ACTIVE_STATUS", active ? "Y" : "N");
        param.Add("@UPDATED_USER", updatedUser);
        return param;
    }

    private static Dictionary<string, object> BuildUpdateParams(string personCode, string pageCode, bool active, string permissionGroup, string updatedUser)
    {
        Dictionary<string, object> param = new Dictionary<string, object>();
        param.Add("@ACTIVE_STATUS", active ? "Y" : "N");
        param.Add("@PERMISSION_GROUP", permissionGroup);
        param.Add("@UPDATED_USER", updatedUser);
        param.Add("@PERSON_CODE", personCode);
        param.Add("@PAGE_CODE", pageCode);
        return param;
    }

    private static bool IsKnownPageCode(string pageCode)
    {
        List<PermissionPage> pages = GetAllPages();
        foreach (PermissionPage page in pages)
        {
            if (page.PageCode == pageCode)
            {
                return true;
            }
        }

        return false;
    }

    private static bool ValidatePermissionKey(string personCode, string pageCode, out string errorMessage)
    {
        errorMessage = "";

        if (string.IsNullOrEmpty(personCode))
        {
            errorMessage = "Person code is required.";
            return false;
        }

        if (!IsKnownPageCode(pageCode))
        {
            errorMessage = "Invalid page code.";
            return false;
        }

        return true;
    }

    private static bool GetDefaultActiveByGroup(string permissionGroup, string pageCode)
    {
        permissionGroup = NormalizePermissionGroup(permissionGroup);
        pageCode = NormalizePageCode(pageCode);

        if (permissionGroup == "ADMIN")
        {
            return true;
        }

        if (permissionGroup == "SSS")
        {
            return pageCode == PageCodes.CustomerEmail;
        }

        return false;
    }

    private static void AddGroup(List<string> groups, string value)
    {
        value = NormalizePermissionGroup(value);
        if (!string.IsNullOrEmpty(value) && !groups.Contains(value))
        {
            groups.Add(value);
        }
    }

    private static string NormalizePersonCode(string value)
    {
        return (value ?? "").Trim();
    }

    private static string NormalizePageCode(string value)
    {
        return (value ?? "").Trim().ToUpper();
    }

    private static string NormalizePermissionGroup(string value)
    {
        return (value ?? "").Trim().ToUpper();
    }

    private static void RedirectToLogin(Page page)
    {
        string returnUrl = page.Request.RawUrl;
        string loginUrl = "~/Login.aspx";
        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            loginUrl += "?ReturnUrl=" + page.Server.UrlEncode(returnUrl);
        }

        page.Response.Redirect(loginUrl, false);
        CompleteRequest();
    }

    private static void CompleteRequest()
    {
        if (HttpContext.Current != null && HttpContext.Current.ApplicationInstance != null)
        {
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}
