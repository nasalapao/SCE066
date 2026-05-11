---
name: sce066-aspnet-webforms-rules
description: Use this skill for SCE066 ASP.NET Web Forms work, including .aspx UI edits, C# code-behind changes, DB2 or SQL Server related code, Thai text or encoding changes, and user requests to git push in this repository.
---

# SCE066 ASP.NET Web Forms Rules

Use this skill when working in the `SCE066` project. Keep changes scoped, preserve the existing Web Forms style, and do not invent business logic, SQL, validation rules, table mappings, or UI behavior without supporting context.

## Project Stack

- Target `.NET Framework 4.8.1`
- Use `C#`, ASP.NET Web Forms, HTML, CSS, and JavaScript
- Use Visual Studio 2022 compatible project conventions
- Database targets are `DB2 AS400` and SQL Server when present
- The system must fully support Thai text

## Working Rules

1. Use patch mode by default. Edit only the requested file, method, or UI area.
2. Do not rewrite entire files, refactor unrelated code, rename methods, or add features unless the user explicitly asks.
3. Follow existing UI, CSS, master page, helper class, and code-behind patterns.
4. If a requirement is missing and guessing would affect business logic, SQL, table fields, validation, transaction behavior, file format, or page flow, ask for the missing detail before implementing that risky part.
5. If the task has enough local context, proceed without asking.
6. Keep responses short and focused on the change, cause, and verification.

## ASP.NET Web Forms Standards

- Pages should use `.aspx`, `.aspx.cs`, and `.designer.cs` when applicable.
- Code-behind must be C#.
- Keep methods focused on one responsibility, such as `BindData()`, `ValidateInput()`, or `ShowError()`.
- Avoid putting all logic in `Page_Load`.
- Use meaningful variable and method names.
- Use familiar Web Forms control prefixes such as `txt`, `ddl`, `btn`, `lb`, and `gv`.

## Error Handling

For risky operations such as file reads, database calls, request parsing, or external dependencies:

```csharp
try
{
    lbError.Visible = false;
    lbError.Text = string.Empty;

    // operation
}
catch (Exception ex)
{
    lbError.Text = ex.Message;
    lbError.Visible = true;
}
```

Every processing page should expose errors through `lbError` when the page already follows that pattern or when adding a new processing page.

## Database Rules

- Use the project's standard database helpers:
  - `dbConnect` for DB2
  - `dbConnectSQL` for SQL Server
- Do not invent SQL, table names, field mappings, or transaction rules.
- When the existing codebase uses `string.Format(@"...")`, match that style unless the available helper supports parameterized queries.
- Be careful with DB2 AS400 schema/library names, data types, string padding, and casing.
- Do not run direct SQL `DELETE` or `UPDATE` from PowerShell, command line, or scripts against real data. Data changes must go through application logic with clear transaction handling.

## UI And CSS Rules

- Preserve the existing layout and shared style conventions.
- If no screenshot, CSS file, or UI reference is provided, use the current page and site styles as the source of truth.
- Do not create a new visual direction for a small UI change.
- For Thai UI text, verify the text remains readable after editing.

## Thai Encoding Checklist

When editing files containing Thai text:

1. Save `.aspx`, `.master`, `.cs`, `.config`, and `.md` files as UTF-8.
2. Reopen or inspect the edited file using UTF-8 to confirm Thai text is readable.
3. For Web Forms pages with Thai display issues, check page encoding such as `CodePage="65001"` and `Web.config` globalization settings such as `fileEncoding`, `requestEncoding`, and `responseEncoding`.

## New File Checklist

When creating new files:

- For Web Site projects without `.csproj`, place files in the actual website folder and mention that Visual Studio may need Refresh or `File > Open > Web Site...`.
- For Web Application projects with `.csproj`, add new `.aspx`, `.aspx.cs`, `.master`, `.master.cs`, `.config`, and dependency files to the project file when required.

## Git Push Workflow

When the user asks for `git push`, run the workflow immediately without asking again:

1. `git status`
2. `git add -A`
3. `git commit -m "[ข้อความภาษาไทยสั้นๆ อธิบายสิ่งที่ทำ]"`
4. `git push`

If push fails or an unusual file appears, stop and report the specific issue briefly.
