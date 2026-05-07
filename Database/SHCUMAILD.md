# SHCUMAILD - Customer Invoice Email

## Purpose

ตาราง `ITPROD.SHCUMAILD` ใช้เก็บ email สำหรับส่ง invoice ให้ลูกค้า โดยเก็บข้อมูลแบบ normalized: 1 email ต่อ 1 row และแยกประเภทผู้รับเป็น `TO` / `CC`

กรณี email ที่ต้อง CC ทุกฉบับ ให้เก็บครั้งเดียวโดยใช้ `CUSTOMER_CODE = 'ALL'` และ `RECIPIENT_TYPE = 'CC'`

## DB2 Table

```sql
CREATE TABLE ITPROD.SHCUMAILD (
    CUSTOMER_CODE    CHAR(10)      NOT NULL,
    RECIPIENT_TYPE   CHAR(2)       NOT NULL,
    EMAIL_SEQ        INTEGER       NOT NULL,
    EMAIL_ADDRESS    VARCHAR(254)  NOT NULL,
    ACTIVE_STATUS    CHAR(1)       NOT NULL DEFAULT 'Y',
    CREATED_DATE     TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UPDATED_DATE     TIMESTAMP,

    CONSTRAINT PK_SHCUMAILD PRIMARY KEY (CUSTOMER_CODE, RECIPIENT_TYPE, EMAIL_SEQ),
    CONSTRAINT UQ_SHCUMAILD_01 UNIQUE (CUSTOMER_CODE, RECIPIENT_TYPE, EMAIL_ADDRESS),
    CONSTRAINT CK_SHCUMAILD_TYPE CHECK (RECIPIENT_TYPE IN ('TO', 'CC')),
    CONSTRAINT CK_SHCUMAILD_STATUS CHECK (ACTIVE_STATUS IN ('Y', 'N'))
);
```

## Data Dictionary

| Field | Type | Required | Default | Key | Description |
|---|---:|---|---|---|---|
| `CUSTOMER_CODE` | `CHAR(10)` | Yes |  | PK, UQ | รหัสลูกค้า/ETH code เช่น `ETH0121`; ใช้ `ALL` สำหรับ CC กลางทุกฉบับ |
| `RECIPIENT_TYPE` | `CHAR(2)` | Yes |  | PK, UQ | ประเภทผู้รับ: `TO` หรือ `CC` |
| `EMAIL_SEQ` | `INTEGER` | Yes |  | PK | ลำดับ email ตาม Excel |
| `EMAIL_ADDRESS` | `VARCHAR(254)` | Yes |  | UQ | Email address |
| `ACTIVE_STATUS` | `CHAR(1)` | Yes | `Y` |  | สถานะใช้งาน: `Y` ใช้งาน, `N` ปิดใช้งาน |
| `CREATED_DATE` | `TIMESTAMP` | Yes | `CURRENT_TIMESTAMP` |  | วันที่สร้างข้อมูล |
| `UPDATED_DATE` | `TIMESTAMP` | No | `NULL` |  | วันที่แก้ไขล่าสุด |

## Import Mapping From Excel

Source file: `Email list for invoice only.xlsx`

| Excel Column | Target Field | Rule |
|---|---|---|
| `Unit` | `CUSTOMER_CODE` | ใช้เฉพาะ ETH code ด้านหน้า เช่น `ETH0121` |
| Column 4 | `RECIPIENT_TYPE`, `EMAIL_ADDRESS` | แตก email แล้ว insert เป็น `RECIPIENT_TYPE = 'TO'` |
| Column 5 | `RECIPIENT_TYPE`, `EMAIL_ADDRESS` | ถ้าเป็น CC กลางทุกฉบับ ให้ insert ครั้งเดียวเป็น `CUSTOMER_CODE = 'ALL'`, `RECIPIENT_TYPE = 'CC'` |

Import rules:

- แตก email ด้วย newline และ semicolon
- trim ช่องว่างก่อน insert
- ข้ามค่าว่าง
- ไม่ insert email ซ้ำใน customer/type เดียวกัน
- กำหนด `EMAIL_SEQ` ตามลำดับ email ที่พบใน Excel

## Query Example

```sql
SELECT RECIPIENT_TYPE, EMAIL_SEQ, EMAIL_ADDRESS
FROM ITPROD.SHCUMAILD
WHERE CUSTOMER_CODE = ?
  AND ACTIVE_STATUS = 'Y'
ORDER BY RECIPIENT_TYPE DESC, EMAIL_SEQ;
```

Query สำหรับส่ง invoice พร้อม CC กลาง:

```sql
SELECT RECIPIENT_TYPE, EMAIL_SEQ, EMAIL_ADDRESS
FROM ITPROD.SHCUMAILD
WHERE ACTIVE_STATUS = 'Y'
  AND (
      CUSTOMER_CODE = ?
      OR (CUSTOMER_CODE = 'ALL' AND RECIPIENT_TYPE = 'CC')
  )
ORDER BY RECIPIENT_TYPE DESC, CUSTOMER_CODE, EMAIL_SEQ;
```

## Notes

- ตารางนี้ไม่แก้หรือแทนที่ `ITPROD.SHCUMAIL` เดิม
- ใช้สำหรับ email invoice ตามไฟล์ Excel
- หน้า Admin สามารถใช้ `ACTIVE_STATUS` เพื่อปิด email โดยไม่ต้องลบข้อมูล
