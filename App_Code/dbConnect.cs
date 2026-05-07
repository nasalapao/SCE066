using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;


public class dbConnect
{
    private readonly string connectionString = "DataSource=172.16.33.49;UserID=mvxreport;Password=report;DataCompression=True;";

    public bool isError { get; private set; }
    public bool isHasRow { get; private set; }
    public string ErrorMessage { get; private set; }
    public string LastQuery { get; private set; }

    //Constructor กำหนดค่าเริ่มต้น
    public dbConnect()
    {
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

        if (!ValidateQuery(query)) return dt; // ถ้า Query ไม่ถูกต้อง หยุดทำงาน

        try
        {
            using (iDB2Connection iConn = new iDB2Connection(connectionString))
            {
                iConn.Open();
                using (iDB2Command cmd = new iDB2Command(query, iConn))
                {
                    using (iDB2DataAdapter adapter = new iDB2DataAdapter(cmd))
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

    public DataTable ExecuteQuery(string query, Dictionary<string, object> parameters)
    {
        isError = false;
        isHasRow = false;
        ErrorMessage = "";
        LastQuery = query;

        DataTable dt = new DataTable();

        if (!ValidateQuery(query)) return dt; // ตรวจสอบ query เบื้องต้น

        try
        {
            using (iDB2Connection iConn = new iDB2Connection(connectionString))
            {
                iConn.Open();
                using (iDB2Command cmd = new iDB2Command(query, iConn))
                {
                    
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    using (iDB2DataAdapter adapter = new iDB2DataAdapter(cmd))
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



    public int InsertData(string query, Dictionary<string, object> parameters = null)
    {
        isError = false;
        isHasRow = false;
        ErrorMessage = "";
        LastQuery = query;

        int rowsAffected = 0;

        if (string.IsNullOrWhiteSpace(query))
        {
            isError = true;
            ErrorMessage = "Query is empty or null.";
            return 0;
        }

        try
        {
            using (iDB2Connection iConn = new iDB2Connection(connectionString))
            {
                iConn.Open();
                using (iDB2Command cmd = new iDB2Command(query, iConn))
                {
                    // 🔁 Add parameter ถ้ามี
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

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
    }//end


    public string BuildDebugQuery(string query, Dictionary<string, object> parameters)
    {
        string debugQuery = query;

        foreach (KeyValuePair<string, object> param in parameters)
        {
            string value;

            if (param.Value == null || param.Value == DBNull.Value)
            {
                value = "NULL";
            }
            else if (param.Value is DateTime)
            {
                DateTime dt = (DateTime)param.Value;
                value = "'" + dt.ToString("yyyy-MM-dd") + "'";
            }
            // ถ้าเป็น string -> ใส่ quote + escape single quote
            else if (param.Value is string)
            {
                value = "'" + param.Value.ToString().Replace("'", "''") + "'";
            }
            // ถ้าเป็นตัวเลข (int, decimal, double) -> ใส่ตรง ๆ
            else if (param.Value is int || param.Value is decimal || param.Value is double)
            {
                value = param.Value.ToString();
            }
            // Fallback สำหรับประเภทอื่น ๆ (เช่น bool, etc.)
            else
            {
                value = "'" + param.Value.ToString().Replace("'", "''") + "'";
            }

            // แทนที่ใน query
            debugQuery = ReplaceParameter(debugQuery, param.Key, value);
        }

        return debugQuery;
    }

    private string ReplaceParameter(string sql, string paramName, string paramValue)
    {
        return System.Text.RegularExpressions.Regex.Replace(
            sql,
            @"(?<!\w)" + System.Text.RegularExpressions.Regex.Escape(paramName) + @"(?!\w)", //ใช้ (?<!\w) และ (?!\w) เพื่อให้แน่ใจว่า replace เฉพาะชื่อ parameter เต็ม ๆ เท่านั้น
            paramValue
        );
    }//end

    // Method: Update/Delete และเช็คว่ามี WHERE หรือไม่
    public int DeleteUpdate(string query)
    {
        isError = false;
        isHasRow = false;
        ErrorMessage = "";
        LastQuery = query;

        int rowsAffected = 0;

        if (!ValidateQuery(query, true)) return 0; // ถ้าไม่มี WHERE หรือ Query ไม่ถูกต้อง หยุดทำงาน

        try
        {
            using (iDB2Connection iConn = new iDB2Connection(connectionString))
            {
                iConn.Open();
                using (iDB2Command cmd = new iDB2Command(query, iConn))
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
    }//end

    public int ExecuteNonQuery(string query, Dictionary<string, object> parameters)
    {
        isError = false;
        isHasRow = false;
        ErrorMessage = "";
        LastQuery = query;

        int rowsAffected = 0;

        if (!ValidateQuery(query, true)) return 0;

        try
        {
            using (iDB2Connection iConn = new iDB2Connection(connectionString))
            {
                iConn.Open();
                using (iDB2Command cmd = new iDB2Command(query, iConn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

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
    }//end

    public int InsertData(string query)
    {
        isError = false;
        isHasRow = false;
        ErrorMessage = "";
        LastQuery = query;

        int rowsAffected = 0;

        if (string.IsNullOrWhiteSpace(query))
        {
            isError = true;
            ErrorMessage = "Query is empty or null.";
            return 0;
        }

        try
        {
            using (iDB2Connection iConn = new iDB2Connection(connectionString))
            {
                iConn.Open();
                using (iDB2Command cmd = new iDB2Command(query, iConn))
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



}
