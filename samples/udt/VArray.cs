/*
This sample demonstrates how to map, fetch, and 
 manipulate the Oracle VARRAY as a custom object. 
 This sample can use managed ODP.NET or ODP.NET Core.

Database schema setup scripts:

1. Connect to HR or another similar schema.
2. Run the following SQL scripts to create a varray of
 numbers, a table of that varray, and a stored 
 procedure inserts a new varray into that table.

drop procedure odp_varray_sample_proc;
drop table odp_varray_sample_rel_tab;
drop type odp_varray_sample_type;

create type odp_varray_sample_type as varray(10) of number; 
/

create table odp_varray_sample_rel_tab
  (col1 odp_varray_sample_type);

create procedure odp_varray_sample_proc(
  param1 IN OUT odp_varray_sample_type) as
  begin
    param1.Extend(1);
    param1(param1.Last) := 9;
    insert into odp_varray_sample_rel_tab values(param1);   
  end;
/

*/

using System;
using System.Data;
using System.Collections;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

class VArraySample
{
    static void Main(string[] args)
    {
        // Enter user id, password, and Oracle data source (i.e. net service name, EZ Connect, etc.)
        string constr = "user id=<USER ID>;password=<PASSWORD>;data source=<DATA SOURCE>";

        string sql1 = "select col1 from odp_varray_sample_rel_tab";
        string sql2 = "odp_varray_sample_proc";

        // Create a new simple varray with values 1, 2, 3, and 4.
        SimpleVarray pa = new SimpleVarray();
        pa.Array = new Int32[] { 1, 2, 3, 4 };

        // Create status array and indicate element 2 is Null.
        pa.StatusArray = new OracleUdtStatus[] { OracleUdtStatus.NotNull, 
            OracleUdtStatus.Null, OracleUdtStatus.NotNull, OracleUdtStatus.NotNull };

        OracleConnection con = null;
        OracleCommand cmd = null;
        OracleDataReader reader = null;

        try
        {
            // Establish a connection to Oracle DB.
            con = new OracleConnection(constr);
            con.Open();

            cmd = new OracleCommand(sql2, con);
            cmd.CommandType = CommandType.StoredProcedure;

            OracleParameter param = new OracleParameter();
            param.OracleDbType = OracleDbType.Array;
            param.Direction = ParameterDirection.InputOutput;

            // Note: The UdtTypeName is case-senstive.
            param.UdtTypeName = "ODP_VARRAY_SAMPLE_TYPE";
            param.Value = pa;
            cmd.Parameters.Add(param);

            // Insert SimpleVarray(1, NULL, 3, 4) into the table.
            // The stored procedure adds a fifth VARRAY element with a value
            //  of 9, then inserts the VARRAY into the table.
            cmd.ExecuteNonQuery();

            // Print out the updated Simple Varray.
            // Results should return values: 1, NULL, 3, 4, and 9.
            Console.WriteLine("Updated SimpleVarray: " + param.Value);
            Console.WriteLine();

            // Modify element 3 to Null and element 2 to Not Null.
            pa.StatusArray[1] = OracleUdtStatus.NotNull;
            pa.StatusArray[2] = OracleUdtStatus.Null;

            param.Value = pa;

            // Insert SimpleVarray(1, 2, NULL, 4) into the table.
            // The stored procedure adds a fifth VARRAY element with a value
            //  of 9, then inserts the VARRAY into the table.
            cmd.ExecuteNonQuery();

            // Print out the updated Simple Varray.
            // Results should return values: 1, 2, NULL, 4, and 9.
            Console.WriteLine("Updated SimpleVarray: " + param.Value);
            Console.WriteLine();

            // Add/Remove some elements by converting the Int32[] to an ArrayList.
            ArrayList pa1 = new ArrayList(pa.Array);

            // Create a corresponding array list to track each element's
            //  Null or NotNull status.
            ArrayList sa1 = new ArrayList(pa.StatusArray);

            // Remove the first element. (2, 3, 4)
            pa1.RemoveAt(0);
            sa1.RemoveAt(0);

            // Add element 6. (2, 3, 4, 6)
            pa1.Add(6);
            sa1.Add(OracleUdtStatus.NotNull);

            // Add element -1. (2, 3, 4, 6, -1)
            pa1.Add(-1);
            // Make the new element NULL now when inserted into DB.
            sa1.Add(OracleUdtStatus.Null);

            // Convert ArrayLists into a SimpleVarray
            pa.Array = (Int32[])pa1.ToArray(typeof(Int32));
            pa.StatusArray = (OracleUdtStatus[])sa1.ToArray(typeof(OracleUdtStatus));

            param.Value = pa;

            Console.WriteLine("Updated SimpleVarray: " + param.Value);
            Console.WriteLine();

            // Insert SimpleVarray(2, NULL, 4, 6, NULL) into the table.
            // The stored procedure adds a sixth VARRAY element with a value
            //  of 9, then inserts the VARRAY into the table.
            cmd.ExecuteNonQuery();

            cmd.CommandText = sql1;
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();

            // Fetch each row and display the VARRAY data.
            // Results should return values 2, NULL, 4, 6, NULL, and 9
            // for the last SimpleArray returned.
            int rowCount = 1;
            while (reader.Read())
            {
                // Fetch the objects as custom types.
                SimpleVarray p;
                if (reader.IsDBNull(0))
                    p = SimpleVarray.Null;
                else
                    p = (SimpleVarray)reader.GetValue(0);

                Console.WriteLine("Row {0}: {1}", rowCount++, p);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            // Clean up
            if (reader != null)
                reader.Dispose();
            if (cmd != null)
                cmd.Dispose();
            if (con != null)
                con.Dispose();
        }
    }
}

// SimpleVarray Class
// A SimpleVarray class instance represents an ODP_VARRAY_SAMPLE_TYPE object.
// A custom type must implement INullable and IOracleCustomType interfaces.
public class SimpleVarray : IOracleCustomType, INullable
{
    [OracleArrayMapping()]
    public  Int32[] Array;
    private OracleUdtStatus[] m_statusArray;

    // OracleUdtStatus enumeration values specify the object attribute
    // or collection element status.
    public OracleUdtStatus[] StatusArray
    {
        get
        {
            return this.m_statusArray;
        }
        set
        {
            this.m_statusArray = value;
        }
    }

    private bool m_bIsNull;

    // Implementation of INullable.IsNull
    public bool IsNull
    {
        get
        {
            return m_bIsNull;
        }
    }

    // SimpleVarray.Null is used to return a NULL SimpleVarray object.
    public static SimpleVarray Null
    {
        get
        {
            SimpleVarray obj = new SimpleVarray();
            obj.m_bIsNull = true;
            return obj;
        }
    }

    // Implementation of IOracleCustomType.ToCustomObject()
    public void ToCustomObject(OracleConnection con, object pUdt)
    {
        object objectStatusArray = null;
        Array = (Int32[])OracleUdt.GetValue(con, pUdt, 0, out objectStatusArray);
        m_statusArray = (OracleUdtStatus[])objectStatusArray;
    }

    // Implementation of IOracleCustomType.FromCustomObject()
    public void FromCustomObject(OracleConnection con, object pUdt)
    {
        OracleUdt.SetValue(con, pUdt, 0, Array, m_statusArray);
    }

    public override string ToString()
    {
        // Return a string representation of the custom object.
        if (m_bIsNull)
            return "SimpleVarray.Null";
        else
        {
            string rtnstr = String.Empty;
            if (m_statusArray[0] == OracleUdtStatus.Null)
                rtnstr = "NULL";
            else
                rtnstr = Array.GetValue(0).ToString();
            for (int i = 1; i < m_statusArray.Length; i++)
            {
                if (m_statusArray[i] == OracleUdtStatus.Null)
                    rtnstr += "," + "NULL";
                else
                    rtnstr += "," + Array.GetValue(i).ToString();
            }
            return "SimpleVarray(" + rtnstr + ")";
        }
    }
}

// SimpleVarrayFactory Class
// An instance of the SimpleVarrayFactory class is used to create 
// SimpleVarray objects.
[OracleCustomTypeMapping("ODP_VARRAY_SAMPLE_TYPE")]
public class SimpleVarrayFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
{
    // IOracleCustomTypeFactory
    public IOracleCustomType CreateObject()
    {
        return new SimpleVarray();
    }

    // IOracleArrayTypeFactory Inteface
    public Array CreateArray(int numElems)
    {
        return new Int32[numElems];
    }

    public Array CreateStatusArray(int numElems)
    {
        // CreateStatusArray may return null if null status information 
        // is not required.
        return new OracleUdtStatus[numElems];
    }
}

/* Copyright (c) 2021, Oracle and/or its affiliates. All rights reserved. */

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
