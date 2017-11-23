/**
 * @author  Jagriti
 * @version 1.0
 *
 * Name of the Application        :  DMLOperOnDS
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


REM *****************************************************************
PROMPT Creating table Products
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
PROMPT Inserting Data in Products Table
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
 
