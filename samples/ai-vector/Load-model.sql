DECLARE 
    ONNX_MOD_FILE VARCHAR2(100) := 'all_MiniLM_L12_v2.onnx';
    MODNAME VARCHAR2(500);
    LOCATION_URI VARCHAR2(200) := 'https://adwc4pm.objectstorage.us-ashburn-1.oci.customer-oci.com/p/eLddQappgBJ7jNi6Guz9m9LOtYe2u8LWY19GfgU8flFK4N9YgP4kTlrE9Px3pE12/n/adwc4pm/b/OML-Resources/o/';

BEGIN
    DBMS_OUTPUT.PUT_LINE('ONNX model file name in object storage is: '||ONNX_MOD_FILE); 
--------------------------------------------
-- Define a model name for the loaded model
--------------------------------------------
    SELECT UPPER(REGEXP_SUBSTR(ONNX_MOD_FILE, '[^.]+')) INTO MODNAME from dual;
    DBMS_OUTPUT.PUT_LINE('Model will be loaded and saved with name: '||MODNAME);

-----------------------------------------------------
-- Read the ONNX model file from object storage into 
-- the Autonomous Database data pump directory
-----------------------------------------------------

BEGIN DBMS_DATA_MINING.DROP_MODEL(model_name => MODNAME);
EXCEPTION WHEN OTHERS THEN NULL; END;

    DBMS_CLOUD.GET_OBJECT(                            
        credential_name => NULL,
        directory_name => 'DATA_PUMP_DIR',
        object_uri => LOCATION_URI||ONNX_MOD_FILE);

-----------------------------------------
-- Load the ONNX model to the database
-----------------------------------------                   

    DBMS_VECTOR.LOAD_ONNX_MODEL(
        directory => 'DATA_PUMP_DIR',
        file_name => ONNX_MOD_FILE,
        model_name => MODNAME);

    DBMS_OUTPUT.PUT_LINE('New model successfully loaded with name: '||MODNAME);
END;

/* Copyright (c) 2025 Oracle and/or its affiliates. All rights reserved. */

/******************************************************************************
 *
 * You may not use the identified files except in compliance with The MIT
 * License (the "License.")
 *
 * You may obtain a copy of the License at
 * https://github.com/oracle/Oracle.NET/blob/master/LICENSE.txt
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *****************************************************************************/