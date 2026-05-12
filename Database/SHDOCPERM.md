# SHDOCPERM -   Permission

## Purpose

Table `ITPROD.SHDOCPERM` stores page-level permissions for   by employee person code. The table name follows the existing Shipping Document table prefix used by `ITPROD.SHDOCH` and `ITPROD.SHDOCL`. The application reads and writes this table only through `App_Code/PermissionManager.cs`.

Default access is deny: if a person code does not have an active row for a page code, that page is not accessible.

## DB2 Table

```sql
CREATE TABLE ITPROD.SHDOCPERM (
    PERSON_CODE    CHAR(20)      NOT NULL,
    PAGE_CODE      VARCHAR(50)   NOT NULL,
    PERMISSION_GROUP VARCHAR(20)  NOT NULL DEFAULT '',
    ACTIVE_STATUS  CHAR(1)       NOT NULL DEFAULT 'Y',
    CREATED_DATE   TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UPDATED_DATE   TIMESTAMP,
    UPDATED_USER   CHAR(20)      NOT NULL DEFAULT '',

    CONSTRAINT PK_SHDOCPERM PRIMARY KEY (PERSON_CODE, PAGE_CODE),
    CONSTRAINT CK_SHDOCPERM_STATUS CHECK (ACTIVE_STATUS IN ('Y', 'N'))
);
```

## Data Dictionary

| Field | Type | Required | Default | Key | Description |
|---|---:|---|---|---|---|
| `PERSON_CODE` | `CHAR(20)` | Yes |  | PK | Employee code from HRIS login / JWT `ClaimTypes.NameIdentifier`. |
| `PAGE_CODE` | `VARCHAR(50)` | Yes |  | PK | Application page permission code. |
| `PERMISSION_GROUP` | `VARCHAR(20)` | Yes | empty string |  | Permission group selected in AdminPermission, for example `SSS`, `ADMIN`, or manual key. |
| `ACTIVE_STATUS` | `CHAR(1)` | Yes | `Y` |  | `Y` = enabled, `N` = disabled. |
| `CREATED_DATE` | `TIMESTAMP` | Yes | `CURRENT_TIMESTAMP` |  | Row creation timestamp. |
| `UPDATED_DATE` | `TIMESTAMP` | No | `NULL` |  | Last update timestamp. |
| `UPDATED_USER` | `CHAR(20)` | Yes | empty string |  | Person code that last changed the row. |

## Supported Page Codes

| Page Code | Page |
|---|---|
| `CUSTOMER_EMAIL` | Customer invoice email send page. |
| `ADMIN_EMAIL` | Customer email recipient management. |
| `ADMIN_EMAIL_TEMPLATE` | Customer email template management. |
| `ADMIN_EMAIL_SENDER` | Customer email sender management. |
| `PERMISSION_ADMIN` | Permission management page. |

## Seed Data

Initial permission admin:

```sql
INSERT INTO ITPROD.SHDOCPERM
    (PERSON_CODE, PAGE_CODE, PERMISSION_GROUP, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('16710231', 'PERMISSION_ADMIN', 'ADMIN', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'SYSTEM');
```

Existing table migration:

```sql
ALTER TABLE ITPROD.SHDOCPERM ADD COLUMN PERMISSION_GROUP VARCHAR(20) NOT NULL DEFAULT '';
```

## Query Examples

## Application Workflow

- `InsertEmployeePermissions(personCode, group, updatedUser)` inserts one row per supported page code.
- `UpdateEmployeeGroup(personCode, group, applyDefaultPermissions, updatedUser)` updates `PERMISSION_GROUP` and can reset `ACTIVE_STATUS` by group defaults.
- `DeleteEmployeePermissions(personCode)` deletes all permission rows for one employee.
- `SavePagePermissions(personCode, group, pagePermissions, updatedUser)` saves checkbox values from the page permission table after the page converts the grid rows into page-code/active values.
- Group defaults:
  - `SSS`: active for `CUSTOMER_EMAIL`.
  - `ADMIN`: active for every supported page.
  - Manual group: no automatic active permissions.
- The group dropdown loads `SSS`, `ADMIN`, and distinct existing `PERMISSION_GROUP` values from `ITPROD.SHDOCPERM`; `Manual` remains available for a new group key.

` .aspx`, ` _1.aspx`, and ` _RPT.aspx` are currently outside permission control and remain available to logged-in users.

Insert employee permissions for one employee:

```sql
-- Application inserts one row per supported PAGE_CODE.
INSERT INTO ITPROD.SHDOCPERM
    (PERSON_CODE, PAGE_CODE, PERMISSION_GROUP, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    (?, ?, ?, ?, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, ?);
```

Update employee group and optionally reset active flags by group default:

```sql
UPDATE ITPROD.SHDOCPERM
   SET PERMISSION_GROUP = ?,
       ACTIVE_STATUS = ?,
       UPDATED_DATE = CURRENT_TIMESTAMP,
       UPDATED_USER = ?
 WHERE PERSON_CODE = ?
   AND PAGE_CODE = ?;
```

Check active permission:

```sql
SELECT COUNT(*) AS CNT
FROM ITPROD.SHDOCPERM
WHERE PERSON_CODE = ?
  AND PAGE_CODE = ?
  AND ACTIVE_STATUS = 'Y';
```

List active page codes:

```sql
SELECT PAGE_CODE
FROM ITPROD.SHDOCPERM
WHERE PERSON_CODE = ?
  AND ACTIVE_STATUS = 'Y';
```

Save one page permission:

```sql
UPDATE ITPROD.SHDOCPERM
   SET ACTIVE_STATUS = ?,
       PERMISSION_GROUP = ?,
       UPDATED_DATE = CURRENT_TIMESTAMP,
       UPDATED_USER = ?
 WHERE PERSON_CODE = ?
   AND PAGE_CODE = ?;
```

Insert missing page permission:

```sql
INSERT INTO ITPROD.SHDOCPERM
    (PERSON_CODE, PAGE_CODE, PERMISSION_GROUP, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    (?, ?, ?, ?, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, ?);
```

Delete one page permission:

```sql
DELETE FROM ITPROD.SHDOCPERM
WHERE PERSON_CODE = ?
  AND PAGE_CODE = ?;
```

Delete all permissions for one employee:

```sql
DELETE FROM ITPROD.SHDOCPERM
WHERE PERSON_CODE = ?;
```

Update group for one employee:

```sql
UPDATE ITPROD.SHDOCPERM
   SET PERMISSION_GROUP = ?,
       UPDATED_DATE = CURRENT_TIMESTAMP,
       UPDATED_USER = ?
 WHERE PERSON_CODE = ?;
```

## Maintenance Rule

Any future database change for this permission workflow must update both:

- `Database/SHDOCPERM.sql`
- `Database/SHDOCPERM.md`
