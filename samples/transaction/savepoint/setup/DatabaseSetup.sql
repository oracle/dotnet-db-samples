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
 * Name of the Application        :  DatabaseSetup.sql
 * Creation/Modification History  :
 *
 *    Chandar        22-Oct-2002       Created
 *
 * Overview of Script: This script performs the clean up and creates tables with integrities 
 * required by the ODP.NET samples.
 * To view the list Database Objects which this file creates, refer to the "Database Setup" 
 * Section in the Readme.html file.
 */

SET serveroutput ON

PROMPT Connecting as SYSTEM user
CONN SYSTEM/&SystemPassword@&&TNSName

DROP USER oranet CASCADE
/

CREATE USER oranet IDENTIFIED BY oranet
/

GRANT CONNECT, RESOURCE, UNLIMITED TABLESPACE TO oranet
/

ALTER USER oranet DEFAULT TABLESPACE USERS
/

CONN oranet/oranet@&tnsname


DROP TABLE PRODUCTS CASCADE CONSTRAINTS ; 

PROMPT Creating PRODUCTS table
CREATE TABLE PRODUCTS (product_id        NUMBER(5) PRIMARY KEY,
                       product_name      VARCHAR2(200),
                       product_desc      NVARCHAR2(1000),
                       category          VARCHAR2(100),
                       price             NUMBER(15,8),
                       product_status    VARCHAR2(30),
                       weight            NUMBER(37,32),
                       modification_date DATE);

