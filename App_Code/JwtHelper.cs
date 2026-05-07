using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Jwt = System.IdentityModel.Tokens.Jwt; // ใช้ alias เพื่อแยกความแตกต่าง

public static class JwtHelper
{
    // คีย์ลับสำหรับเข้ารหัส JWT (ต้องมีขนาดอย่างน้อย 32 bytes/256 bits)
    private const string SecretKey = "YourSuperSecretKey12345678901234567890"; // 32+ characters
    private const string Issuer = "WOAProject";
    private const string Audience = "WOAProject";

    // สร้าง JWT Token
    public static string GenerateToken(string username, string levelID, string nameT, string userID, string costCenter, string workCenter, string companyID , string depID , string section)
    {
        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim("FULLNAME", nameT),
            new Claim("LevelID", levelID),
            new Claim("NameT", nameT),
            new Claim("UserID", userID),
            new Claim("CostCenter", costCenter),
            new Claim("workCenter", workCenter),
            new Claim("companyID", companyID),
            new Claim("depID", depID),
            new Claim("section", section),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new Jwt.JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.Now.AddDays(2), // อายุ 2 วัน
            signingCredentials: credentials
        );

        return new Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }

    // ตรวจสอบความถูกต้องของ Token
    public static ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new Jwt.JwtSecurityTokenHandler();
        var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        Microsoft.IdentityModel.Tokens.SecurityToken validatedToken;
        try
        {
            return tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        }
        catch
        {
            return null; // Token ไม่ถูกต้อง
        }
    }
}