using System;
using System.Security.Claims;
using System.Web;
using System.Web.UI;

public partial class Site : MasterPage
{
    private const string AuthCookieName = "SCE066_TOKEN";

    protected void Page_Load(object sender, EventArgs e)
    {
        ClaimsPrincipal principal = GetAuthenticatedPrincipal();

        if (principal == null)
        {
            RedirectToLogin();
            return;
        }

        if (!Page.IsPostBack)
        {
            BindUserInfo(principal);
        }
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        HttpCookie cookie = new HttpCookie(AuthCookieName, "");
        cookie.HttpOnly = true;
        cookie.Path = ResolveUrl("~/");
        cookie.Expires = DateTime.Now.AddDays(-1);
        Response.Cookies.Add(cookie);

        Response.Redirect("~/Login.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private ClaimsPrincipal GetAuthenticatedPrincipal()
    {
        HttpCookie cookie = Request.Cookies[AuthCookieName];

        if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
        {
            return null;
        }

        return JwtHelper.ValidateToken(cookie.Value);
    }

    private void BindUserInfo(ClaimsPrincipal principal)
    {
        litFullName.Text = GetClaimValue(principal, "NameT");
        litPersonCode.Text = GetClaimValue(principal, ClaimTypes.NameIdentifier);
        litDepartment.Text = GetClaimValue(principal, "section");
    }

    private string GetClaimValue(ClaimsPrincipal principal, string type)
    {
        Claim claim = principal.FindFirst(type);
        return claim == null ? "" : Server.HtmlEncode(claim.Value);
    }

    private void RedirectToLogin()
    {
        Response.Redirect("~/Login.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
    }
}
