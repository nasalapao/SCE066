-- SHCPRLOG - Program update announcement workflow
-- Run manually on DB2 AS400 after backup/review.
-- Thai text columns use Unicode graphic data so announcements can store Thai text.

CREATE TABLE ITPROD.SHCPRLOG (
    LOG_ID             INTEGER GENERATED ALWAYS AS IDENTITY,
    PROGRAM_CODE       VARCHAR(20)      NOT NULL DEFAULT 'SCE066',
    CHANGE_DATE        DATE             NOT NULL,
    CHANGE_TITLE       VARGRAPHIC(200)  CCSID 1200 NOT NULL,
    CHANGE_DETAIL      DBCLOB(16K)      CCSID 1200 NOT NULL,
    ACTIVE_STATUS      CHAR(1)          NOT NULL DEFAULT 'Y',
    CREATED_DATE       TIMESTAMP        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UPDATED_DATE       TIMESTAMP,
    UPDATED_USER       CHAR(20)         NOT NULL DEFAULT '',

    CONSTRAINT PK_SHCPRLOG PRIMARY KEY (LOG_ID),
    CONSTRAINT CK_SHCPRLOG_STATUS CHECK (ACTIVE_STATUS IN ('Y', 'N'))
);

-- Seed data generated from git log. Keep wording user-friendly because these rows display on Home.aspx.
INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-19'), 'ปรับหน้าสิทธิ์และปุ่มส่งเมล', 'ปรับปรุงหน้าจัดการสิทธิ์และการทำงานของปุ่มส่งเมลให้ใช้งานได้ชัดเจนขึ้น', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-15'), 'เพิ่มหน้า Email Log สำหรับแอดมิน', 'เพิ่มหน้าตรวจสอบประวัติการส่งอีเมล เพื่อให้ผู้ดูแลระบบดูสถานะและรายละเอียดการส่งย้อนหลังได้', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-14'), 'จำกัดการส่งเมลแจ้ง upload invoice', 'ปรับเงื่อนไขการส่งอีเมลแจ้งอัปโหลด Invoice เพื่อลดการส่งซ้ำหรือส่งเกินความจำเป็น', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-14'), 'เพิ่มเมนู Manual และไฟล์คู่มือ', 'เพิ่มเมนูคู่มือการใช้งานและแนบไฟล์คู่มือ เพื่อให้ผู้ใช้เปิดดูขั้นตอนการใช้งานระบบได้จากหน้าเว็บ', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-12'), 'เพิ่มหน้าแจ้งเตือน Invoice อัตโนมัติ', 'เพิ่มหน้าสำหรับตรวจสอบและจัดการการแจ้งเตือน Invoice อัตโนมัติของระบบ', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-12'), 'แก้ส่งเมลแจ้งอัปโหลด Invoice', 'ปรับปรุงขั้นตอนการส่งอีเมลแจ้งอัปโหลด Invoice ให้ทำงานถูกต้องขึ้น', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-12'), 'ปรับระบบอีเมลและหน้า SCE066', 'ปรับปรุงระบบอีเมล หน้า SCE066 และรายงานที่เกี่ยวข้อง เพื่อรองรับ workflow การส่งเอกสารได้ครบขึ้น', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-11'), 'เพิ่มช่องทดสอบส่งอีเมล', 'เพิ่มช่องสำหรับทดสอบการส่งอีเมลจากหน้าตั้งค่าผู้ส่ง เพื่อให้ตรวจสอบ SMTP ได้สะดวกขึ้น', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-11'), 'ปรับกฎและหน้าเข้าสู่ระบบ', 'ปรับปรุงกฎการทำงานและหน้าเข้าสู่ระบบให้สอดคล้องกับรูปแบบการใช้งานของระบบ', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-09'), 'ปรับปรุงระบบสิทธิ์และอีเมล', 'ปรับระบบสิทธิ์ผู้ใช้งานและหน้าจัดการอีเมล เพื่อให้แอดมินควบคุมการใช้งานได้ละเอียดขึ้น', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-08'), 'เพิ่มระบบจัดการอีเมล Invoice ลูกค้า', 'เพิ่มหน้าจัดการอีเมลลูกค้า เทมเพลต และการส่ง Invoice ให้รองรับการทำงานผ่านระบบมากขึ้น', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-08'), 'ปรับเงื่อนไขประเภทผู้รับอีเมล', 'ปรับเอกสารและโครงสร้างที่เกี่ยวข้องกับประเภทผู้รับอีเมล เพื่อรองรับการตั้งค่าผู้รับได้ถูกต้อง', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-05-07'), 'เพิ่มระบบล็อกอินและจัดการอีเมลลูกค้า', 'เพิ่มระบบเข้าสู่ระบบ เมนูหลัก และหน้าจัดการอีเมลลูกค้าสำหรับ workflow เอกสารจัดส่ง', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-04-30'), 'ปรับรายงานเอกสารจัดส่งและเพิ่มคอลัมน์ INV Doc', 'ปรับรายงานเอกสารจัดส่งและเพิ่มข้อมูล INV Doc เพื่อให้รายงานมีข้อมูลครบตามการใช้งาน', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-04-24'), 'แก้ค้นหาลูกค้าและอัปเดตวันที่หน้าเว็บ', 'ปรับการค้นหาลูกค้าและการแสดงวันที่บนหน้าเว็บให้ถูกต้องขึ้น', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-04-24'), 'แก้การแจ้งเตือนเมื่อคลิกไอคอนเอกสาร', 'ปรับข้อความแจ้งเตือนเมื่อผู้ใช้คลิกไอคอนเอกสาร เพื่อให้เข้าใจสถานะได้ชัดเจนขึ้น', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-04-02'), 'ปรับ UI เอกสารและสถานะใน SCE066', 'ปรับหน้าจอเอกสารและการแสดงสถานะ เพื่อให้การตรวจสอบเอกสารใน SCE066 ใช้งานง่ายขึ้น', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-04-01'), 'ปรับคอลัมน์ upload เอกสาร SCE066', 'ปรับคอลัมน์ที่เกี่ยวข้องกับการอัปโหลดเอกสาร เพื่อให้ข้อมูลบนหน้า SCE066 ครบและชัดเจนขึ้น', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHCPRLOG
    (PROGRAM_CODE, CHANGE_DATE, CHANGE_TITLE, CHANGE_DETAIL, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
VALUES
    ('SCE066', DATE('2026-03-31'), 'เริ่มต้นระบบ SCE066', 'เพิ่มโครงสร้างเริ่มต้นของระบบ Shipping Document Control สำหรับใช้งานเอกสารจัดส่ง', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'nasalapao');

INSERT INTO ITPROD.SHDOCPERM
    (PERSON_CODE, PAGE_CODE, PERMISSION_GROUP, ACTIVE_STATUS, CREATED_DATE, UPDATED_DATE, UPDATED_USER)
SELECT '16710231', 'PROGRAM_UPDATE_ADMIN', 'ADMIN', 'Y', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'SYSTEM'
  FROM SYSIBM.SYSDUMMY1
 WHERE NOT EXISTS (
       SELECT 1
         FROM ITPROD.SHDOCPERM
        WHERE PERSON_CODE = '16710231'
          AND PAGE_CODE = 'PROGRAM_UPDATE_ADMIN'
   );
