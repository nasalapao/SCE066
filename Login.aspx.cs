using System;
using System.Data;
using System.Web;
using System.Web.UI;

public partial class Login : System.Web.UI.Page
{
    private const string AuthCookieName = "SCE066_TOKEN";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (HasValidLogin())
        {
            Response.Redirect("~/Home.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (!Page.IsPostBack)
        {
            ClearError();
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        ClearError();

        string personCode = txtPersonCode.Text.Trim();
        string password = txtPassword.Text;
        bool isDev = host.IsDev;

        if (string.IsNullOrWhiteSpace(personCode) || (!isDev && string.IsNullOrWhiteSpace(password)))
        {
            ShowError(isDev ? "กรุณากรอกรหัสพนักงาน" : "กรุณากรอกรหัสพนักงานและรหัสผ่าน");
            return;
        }

        try
        {
            dbConnectSQL db = new dbConnectSQL(7);
            DataTable dt = db.GetLoginPerson(personCode, password, isDev);

            if (db.isError)
            {
                ShowError(db.ErrorMessage);
                return;
            }

            if (dt.Rows.Count == 0)
            {
                ShowError(isDev ? "ไม่พบรหัสพนักงาน หรือพนักงานสิ้นสุดการทำงานแล้ว" : "รหัสพนักงานหรือรหัสผ่านไม่ถูกต้อง หรือพนักงานสิ้นสุดการทำงานแล้ว");
                return;
            }

            DataRow row = dt.Rows[0];
            string fullName = (Convert.ToString(row["FnameT"]).Trim() + " " + Convert.ToString(row["LnameT"]).Trim()).Trim();

            string token = JwtHelper.GenerateToken(
                Convert.ToString(row["PersonCode"]).Trim(),
                "",
                fullName,
                Convert.ToString(row["PersonID"]).Trim(),
                "",
                "",
                "",
                Convert.ToString(row["Cmb2ID"]).Trim(),
                Convert.ToString(row["Cmb2NameT"]).Trim()
            );

            HttpCookie cookie = new HttpCookie(AuthCookieName, token);
            cookie.HttpOnly = true;
            cookie.Path = ResolveUrl("~/");
            cookie.Expires = DateTime.Now.AddDays(2);
            Response.Cookies.Add(cookie);

            Response.Redirect("~/Home.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

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

    private bool HasValidLogin()
    {
        HttpCookie cookie = Request.Cookies[AuthCookieName];

        if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
        {
            return false;
        }

        return JwtHelper.ValidateToken(cookie.Value) != null;
    }
}
