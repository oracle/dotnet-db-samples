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

/**********************************************************************************
 *  @author  Jagriti
 *  @version 1.0
 *  Name of the File               :  StoredProcedure.sql
 *  Creation/Modification History  :
 *                                  10-JUL-2002     Created
 *  Overview of Package:
 *  This package specification contains definition of a ref cursor type .
 *  The variables of this type will be used by stored procedures in the application
 *  to send multiple records from database to application. 
 ***********************************************************************************/      

CREATE OR REPLACE PACKAGE ODPNet AS
  TYPE refcur IS REF CURSOR;
  PROCEDURE getProductsInfo(orderable OUT ODPNet.refcur,udevelopment OUT ODPNet.refcur);
  
END ODPNet;
/

--*******************************************************************
 --Procedure to get the product information using ref cursor arguments
 --*******************************************************************
CREATE OR REPLACE PACKAGE BODY ODPNet is 
PROCEDURE getProductsInfo(orderable OUT ODPNet.refcur,udevelopment OUT ODPNet.refcur) IS
 
 /**
 *  @author  Jagriti
 *  @version 1.0
 *  Name of the File               :  StoredProcedure.sql
 *  Creation/Modification History  :
 *                                  10-JUL-2002     Created
 *  Overview of the Procedure:

 *  It gets the information of products of two categories ('orderable' and 'under development' in two 
 *  different REF CURSOR variables

 *  Parameters:
 *  @orderable - OUT - A Ref Cursor variable passed as OUT parameter GetProductInfo 
 *  @udevelopment - OUT- A Ref Cursor variable passed as OUT parameter GetProductInfo.
 **/

 BEGIN

    --get records for products  with category as 'Orderable' in orderable ref cursor
    OPEN orderable FOR SELECT
    product_id, product_name,Product_desc, category, price FROM
    products WHERE lower(product_status)='orderable'
    ORDER BY product_id;

    --get records for products  with category as 'under development' in orderable ref cursor
   OPEN udevelopment FOR SELECT
    product_id, product_name,Product_desc, category, price FROM
    products WHERE lower(product_status)='under development'
    ORDER BY product_id;

    
END getProductsInfo;
END;
/

sho err


