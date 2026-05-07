using System;
using System.Data;
using System.Data.SqlClient;

public class dbConnectSQL
{
    private readonly string connectionString = "Server=172.16.33.105;database=Cyberhrm;UID=Tiger;PASSWORD=Tigersoft1998;";



    public bool isError { get; private set; }
    public bool isHasRow { get; private set; }
    public string ErrorMessage { get; private set; }
    public string LastQuery { get; private set; }

    // Constructor: กำหนดค่าเริ่มต้น
    public dbConnectSQL()
    {
        connectionString = "Server=172.16.33.105;database=Cyberhrm;UID=Tiger;PASSWORD=Tigersoft1998;";
        isError = false;
        isHasRow = false;
        ErrorMessage = "";
        LastQuery = "";
    }

    public dbConnectSQL(string customConnectionString)
    {
        connectionString = customConnectionString;
        isError = false;
        isHasRow = false;
        ErrorMessage = "";
        LastQuery = "";
    }

    public dbConnectSQL(int serverId)
    {
        switch (serverId)
        {
            case 105:
                connectionString = "Server=172.16.33.105;Database=Cyberhrm;UID=Tiger;PWD=Tigersoft1998;";
                break;
            case 7:
                connectionString = "Server=172.16.33.7;Database=HRIS;UID=sa;PWD=itadmin;";
                break;
            default:
                throw new ArgumentException("ไม่รู้จัก serverId ที่ระบุ");
        }

        isError = false;
        isHasRow = false;
        ErrorMessage = "";
        LastQuery = "";
    }


    // Method: ตรวจสอบว่า Query ถูกต้องหรือไม่
    private bool ValidateQuery(string query, bool isUpdateOrDelete = false)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            isError = true;
            ErrorMessage = "Query is empty or null.";
            return false;
        }

        string queryUpper = query.Trim().ToUpper();

        // ห้ามใช้ DROP, TRUNCATE
        if (queryUpper.StartsWith("DROP") || queryUpper.StartsWith("TRUNCATE"))
        {
            isError = true;
            ErrorMessage = "Dangerous query detected!";
            return false;
        }

        // ถ้าเป็น UPDATE หรือ DELETE ต้องมี WHERE
        if (isUpdateOrDelete && !queryUpper.Contains("WHERE"))
        {
            isError = true;
            ErrorMessage = "Unsafe query detected! UPDATE or DELETE queries must contain WHERE clause.";
            return false;
        }

        return true;
    }

    // Method: ดึงข้อมูล (SELECT) และคืนค่าเป็น DataTable
    public DataTable ExecuteQuery(string query)
    {
        isError = false;
        isHasRow = false;
        ErrorMessage = "";
        LastQuery = query;

        DataTable dt = new DataTable();

        if (!ValidateQuery(query)) return dt;

        try
        {
            using (SqlConnection sConn = new SqlConnection(connectionString))
            {
                sConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, sConn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                        isHasRow = dt.Rows.Count > 0;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            isError = true;
            ErrorMessage = ex.Message;
        }

        return dt;
    }

    // Method: Update/Delete และเช็คว่ามี WHERE หรือไม่
    public int DeleteUpdate(string query)
    {
        isError = false;
        isHasRow = false;
        ErrorMessage = "";
        LastQuery = query;

        int rowsAffected = 0;

        if (!ValidateQuery(query, true)) return 0;

        try
        {
            using (SqlConnection sConn = new SqlConnection(connectionString))
            {
                sConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, sConn))
                {
                    rowsAffected = cmd.ExecuteNonQuery();
                    isHasRow = rowsAffected > 0;
                }
            }
        }
        catch (Exception ex)
        {
            isError = true;
            ErrorMessage = ex.Message;
        }

        return rowsAffected;
    }

    public DataTable GetLoginPerson(string personCode, string password)
    {
        return GetLoginPerson(personCode, password, false);
    }

    public DataTable GetLoginPerson(string personCode, string password, bool ignorePassword)
    {
        isError = false;
        isHasRow = false;
        ErrorMessage = "";

        DataTable dt = new DataTable();

        const string sql = @"
        SELECT TOP 1
               PersonID,
               PersonCode,
               ISNULL(FnameT, '') AS FnameT,
               ISNULL(LnameT, '') AS LnameT,
               ISNULL(Cmb2ID, '') AS Cmb2ID,
               ISNULL(Cmb2NameT, '') AS Cmb2NameT
        FROM [HRIS].[dbo].[PersonDetail]
        WHERE RTRIM(LTRIM(PersonCode)) = @PersonCode
          AND (@IgnorePassword = 1 OR ISNULL(Pws, '') = @Password)
          AND NULLIF(LTRIM(RTRIM(CONVERT(NVARCHAR(50), EndDate))), '') IS NULL";

        LastQuery = sql;

        if (string.IsNullOrWhiteSpace(personCode) || (!ignorePassword && string.IsNullOrWhiteSpace(password)))
        {
            return dt;
        }

        try
        {
            using (SqlConnection sConn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, sConn))
            {
                cmd.Parameters.Add("@PersonCode", SqlDbType.VarChar, 50).Value = personCode.Trim();
                cmd.Parameters.Add("@Password", SqlDbType.VarChar, 100).Value = password ?? "";
                cmd.Parameters.Add("@IgnorePassword", SqlDbType.Bit).Value = ignorePassword;

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                    isHasRow = dt.Rows.Count > 0;
                }
            }
        }
        catch (Exception ex)
        {
            isError = true;
            ErrorMessage = ex.Message;
        }

        return dt;
    }

    public string CallNamebyDipid(string dipid)
    {
        // รีเซ็ตสถานะมาตรฐาน
        isError = false;
        isHasRow = false;
        ErrorMessage = "";
        LastQuery = "";

        if (string.IsNullOrWhiteSpace(dipid))
            return "";

        const string sql = @"
        SELECT TOP 1 ISNULL(Cmb2NameT, '') AS DeptName
        FROM [HRIS].[dbo].[PersonDetail]
        WHERE RTRIM(LTRIM(Cmb2ID)) = @dipid
        ORDER BY Cmb2NameT";

        LastQuery = sql; // เก็บไว้ตรวจสอบ (หลีกเลี่ยงการ log ค่าพารามิเตอร์จริง)

        try
        {
            using (SqlConnection sConn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, sConn))
            {
                cmd.Parameters.Add("@dipid", SqlDbType.VarChar, 50).Value = dipid.Trim();

                sConn.Open();
                object result = cmd.ExecuteScalar();

                string deptName = (result == null) ? "" : Convert.ToString(result).Trim();
                isHasRow = !string.IsNullOrEmpty(deptName);
                return deptName;
            }
        }
        catch (Exception ex)
        {
            isError = true;
            ErrorMessage = ex.Message;
            return "";
        }
    }


}
