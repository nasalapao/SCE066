using System.Net;
using System.Web;

public class host
{
    private const string DevUrl = "http://localhost:60308";
    private const string ProdUrl = "http://172.16.33.37/SCE066";

    public static bool IsDev
    {
        get
        {
            try
            {
                string host = "";
                if (HttpContext.Current != null &&
                    HttpContext.Current.Request != null &&
                    HttpContext.Current.Request.Url != null)
                {
                    host = HttpContext.Current.Request.Url.Host;
                }
                return host.Contains("localhost") || host.Contains("127.0.0.1") || host.Equals("::1");
            }
            catch
            {
                return false; // ถ้ารันแบบไม่มี context เช่น background thread
            }
        }
    }

    public static string IP
    {
        get
        {
            string ip = "localhost"; // fallback default
            try
            {
                // ดึงชื่อ host ปัจจุบัน
                string hostName = Dns.GetHostName();
                // ดึง IP ทั้งหมดที่ bind อยู่กับ host นี้
                IPAddress[] addresses = Dns.GetHostAddresses(hostName);
                // เลือกเฉพาะ IPv4 ที่ไม่ใช่ loopback (127.0.0.1)
                foreach (IPAddress addr in addresses)
                {
                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                        && !IPAddress.IsLoopback(addr))
                    {
                        ip = addr.ToString();
                        break;
                    }
                }
            }
            catch
            {
                // ถ้ามี error ให้ default เป็น localhost
                ip = "localhost";
            }
            return ip;
        }
    }//end

    public static string Url
    {
        get
        {
            try
            {
                if (HttpContext.Current != null &&
                    HttpContext.Current.Request != null &&
                    HttpContext.Current.Request.Url != null)
                {
                    string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(System.UriPartial.Authority);
                    string appPath = HttpContext.Current.Request.ApplicationPath;

                    if (!string.IsNullOrEmpty(appPath) && appPath != "/")
                    {
                        baseUrl += appPath;
                    }

                    return baseUrl.TrimEnd('/');
                }
            }
            catch
            {
            }

            return IsDev ? DevUrl : ProdUrl;
        }
    }

    public static string BuildUrl(string relativeUrl)
    {
        relativeUrl = (relativeUrl ?? "").Trim();

        if (relativeUrl.StartsWith("~/"))
        {
            relativeUrl = relativeUrl.Substring(2);
        }
        else if (relativeUrl.StartsWith("/"))
        {
            relativeUrl = relativeUrl.Substring(1);
        }

        return Url.TrimEnd('/') + "/" + relativeUrl;
    }
}
