/* Copyright (c) 2017, Oracle and/or its affiliates. All rights reserved. */

/******************************************************************************
 *
 * You may not use the identified files except in compliance with The MIT
 * License (the "License.")
 *
 * You may obtain a copy of the License at
 * https://github.com/oracle/Oracle.NET/blob/master/LICENSE
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *****************************************************************************/

/**
 * @author  Jagriti
 * @version 1.0
 *
 * Name of the Application        :  DSDRWithLOB
 * Creation/Modification History  :
 *
 *    Jagriti        27-June-2002       Created
 *
 * Overview of Script:  This SQL script creates the table 
 *  required by this application and inserts data in this table.
 *    
 *  Parameters required to run this script :
 *  1. Database user name 
 *  2. Password
 *  3. Connect string
 *     
 */

SET ECHO OFF

REM *****************************************************************
PROMPT Connecting to the database with given credentials
REM *****************************************************************

CONNECT &username/&password@&connectstring;

REM *****************************************************************
PROMPT Dropping the tables if it exists
REM *****************************************************************

DROP TABLE PRODUCTS CASCADE CONSTRAINTS ; 
DROP TABLE PRINTMEDIA CASCADE CONSTRAINTS ; 
DROP SEQUENCE ADID_SEQ;

REM *****************************************************************
PROMPT Creating PRODUCTS table
REM *****************************************************************

CREATE TABLE PRODUCTS ( 
  PRODUCT_ID      NUMBER (5)    NOT NULL, 
  PRODUCT_NAME    VARCHAR2 (200), 
  PRODUCT_DESC    VARCHAR2 (500), 
  CATEGORY        VARCHAR2 (100), 
  PRICE           NUMBER (10,2), 
  PRODUCT_STATUS  VARCHAR2 (30), 
  PRIMARY KEY ( PRODUCT_ID ) ) ; 

REM *****************************************************************
PROMPT Creating PRINTMEDIA table
REM *****************************************************************
CREATE TABLE PRINTMEDIA ( 
  AD_ID             NUMBER (5)    NOT NULL, 
  PRODUCT_ID        NUMBER (5), 
  AD_TEXT           VARCHAR2 (200), 
  AD_IMAGE          BLOB, 
  DATE_OF_CREATION  DATE, 
  PRIMARY KEY ( AD_ID ) ) ; 

ALTER TABLE PRINTMEDIA ADD 
 FOREIGN KEY (PRODUCT_ID) 
  REFERENCES PRODUCTS (PRODUCT_ID) ;


REM *****************************************************************
PROMPT Creating ADID_SEQ Sequence
REM *****************************************************************

CREATE SEQUENCE ADID_SEQ START WITH 1200;

REM *****************************************************************
PROMPT Creating POPULATE_ADID trigger
REM *****************************************************************

CREATE OR REPLACE TRIGGER POPULATE_ADID
 BEFORE INSERT ON PRINTMEDIA FOR EACH ROW
DECLARE
TEMPVAL NUMBER;
BEGIN
  SELECT ADID_SEQ.NEXTVAL INTO TEMPVAL FROM DUAL;
  :NEW.AD_ID := TEMPVAL;
END;
/

REM *****************************************************************
PROMPT Inserting data into PRODUCTS table
REM *****************************************************************

INSERT INTO PRODUCTS ( PRODUCT_ID, PRODUCT_NAME, PRODUCT_DESC, CATEGORY, PRICE, PRODUCT_STATUS ) VALUES ( 
1000, 'Sports cap', 'White color, pure cotton', 'clothes', 3, 'orderable'); 
INSERT INTO PRODUCTS ( PRODUCT_ID, PRODUCT_NAME, PRODUCT_DESC, CATEGORY, PRICE, PRODUCT_STATUS ) VALUES ( 
1002, 'Kids T-Shirt', 'Soft T-shirt with MickeyMouse logo', 'clothes', 7, 'orderable'); 
INSERT INTO PRODUCTS ( PRODUCT_ID, PRODUCT_NAME, PRODUCT_DESC, CATEGORY, PRICE, PRODUCT_STATUS ) VALUES ( 
1003, 'Black Jacket', 'Trendy range, with 4 pockets', 'clothes', 8, 'orderable'); 
INSERT INTO PRODUCTS ( PRODUCT_ID, PRODUCT_NAME, PRODUCT_DESC, CATEGORY, PRICE, PRODUCT_STATUS ) VALUES ( 
1001, 'Pen set', 'Colored pen set', 'stationary', 4, 'under development'); 
commit;
 
