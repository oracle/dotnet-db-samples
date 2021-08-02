/*
This sample demonstrates how to map, fetch, and 
 manipulate an Oracle user-defined type (UDT) as 
 a .NET custom object. This sample can use managed 
 ODP.NET or ODP.NET Core.

Database schema setup scripts:

1. Connect to HR or another similar schema.
2. Run the following SQL scripts to create a person
 UDT, a contacts table with a person UDT column, and
 a stored procedure that updates a UDT property.

drop procedure odp_obj1_sample_upd_contacts;
drop table odp_obj1_sample_contacts;
drop type odp_obj1_sample_person_type;

create type odp_obj1_sample_person_type as object 
  (name varchar2(30), address varchar2(60), age number(3)) NOT FINAL;
/
 
create table odp_obj1_sample_contacts (
  contact        odp_obj1_sample_person_type, 
  contact_phone  varchar2(20));

create procedure odp_obj1_sample_upd_contacts(
  param1 IN OUT odp_obj1_sample_person_type,
  param2 IN     varchar2) as
  begin
    param1.age := param1.age + 1;    
    insert into odp_obj1_sample_contacts values(param1,param2);   
  end;
/

*/

using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

class ObjectUDT
{
    static void Main(string[] args)
    {
        // Enter user id, password, and Oracle data source (i.e. net service name, EZ Connect, etc.)
        string constr = "user id=<USER ID>;password=<PASSWORD>;data source=<DATA SOURCE>";

        string sql1 = "odp_obj1_sample_upd_contacts";
        string sql2 = "select c.contact from odp_obj1_sample_contacts c";    

        // Create a new Person object
        Person p1   = new Person();
        p1.Name     = "John";
        p1.Address  = "Address 1";
        p1.Age = 20;

        OracleConnection con = null;
        OracleCommand cmd = null;

        try
        {
            // Establish a connection to Oracle database
            con = new OracleConnection(constr);
            con.Open();
            cmd = new OracleCommand(sql1, con);

            try
            {
                // Insert Person object into a database and update object
                //  using a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;
                OracleParameter param1 = new OracleParameter();

                param1.OracleDbType = OracleDbType.Object;
                param1.Direction = ParameterDirection.InputOutput;

                // Note: The UdtTypeName is case-senstive
                param1.UdtTypeName = "ODP_OBJ1_SAMPLE_PERSON_TYPE";
                param1.Value = p1;

                cmd.Parameters.Add(param1);

                OracleParameter param2 = new OracleParameter();
                param2.OracleDbType = OracleDbType.Varchar2;
                param2.Direction = ParameterDirection.Input;
                param2.Value = "1-800-555-4412";

                cmd.Parameters.Add(param2);

                // Insert the UDT into the table
                cmd.ExecuteNonQuery();

                // Print out the updated Person object
                Console.WriteLine("Updated Person: " + param1.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Clean up
                if (cmd != null)
                    cmd.Parameters.Clear();
            }

            OracleDataReader reader = null;
            try
            {
                // Retrieve the updated objects from the database table
                cmd.CommandText = sql2;
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();

                // Fetch each row
                int rowCount = 1;
                while (reader.Read())
                {
                    // Fetch the objects as a custom type
                    Person p;
                    if (reader.IsDBNull(0))
                        p = Person.Null;
                    else
                        p = (Person)reader.GetValue(0);

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
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            // Clean up
            if (cmd != null)
                cmd.Dispose();
            if (con != null)
                con.Dispose();
        }
    }
}

/* Person UDT Class
   An instance of a Person class represents an ODP_OBJ1_SAMPLE_PERSON_TYPE object
   A custom type must implement INullable and IOracleCustomType interfaces
*/
public class Person : INullable, IOracleCustomType
{
    private bool          m_bIsNull;  // Whether the Person object is NULL    
    private string        m_name;     // "NAME" attribute  
    private OracleString  m_address;  // "ADDRESS" attribute  
    private int?          m_age;      // "AGE" attribute

    // Implementation of INullable.IsNull
    public virtual bool IsNull
    {
        get
        {
            return m_bIsNull;
        }
    }

    // Person.Null is used to return a NULL Person object
    public static Person Null
    {
        get
        {
            Person p = new Person();
            p.m_bIsNull = true;
            return p;
        }
    }

    // Specify the OracleObjectMappingAttribute to map "Name" to "NAME"
    [OracleObjectMappingAttribute("NAME")]
    // The mapping can also be specified using attribute index 0
    // [OracleObjectMappingAttribute(0)]
    public string Name
    {
        get
        {
            return m_name;
        }
        set
        {
            m_name = value;
        }
    }

    // Specify the OracleObjectMappingAttribute to map "Address" to "ADDRESS"
    [OracleObjectMappingAttribute("ADDRESS")]
    // The mapping can also be specified using attribute index 1
    // [OracleObjectMappingAttribute(1)]
    public OracleString Address
    {
        get
        {
            return m_address;
        }
        set
        {
            m_address = value;
        }
    }

    // Specify the OracleObjectMappingAttribute to map "Age" to "AGE"
    [OracleObjectMappingAttribute("AGE")]
    // The mapping can also be specified using attribute index 2
    // [OracleObjectMappingAttribute(2)]

    public int? Age
    {
        get
        {
            return m_age;
        }
        set
        {
            m_age = value;
        }
    }

    // Implementation of IOracleCustomType.FromCustomObject()
    public virtual void FromCustomObject(OracleConnection con, object pUdt)
    {
        // Convert from the Custom Type to Oracle Object

        // Set the "NAME" attribute.     
        // By default the "NAME" attribute will be set to NULL
        if (m_name != null)
        {
            OracleUdt.SetValue(con, pUdt, "NAME", m_name);
            // The "NAME" attribute can also be accessed by specifying index 0
            // OracleUdt.SetValue(con, pUdt, 0, m_name);
        }

        // Set the "ADDRESS" attribute.     
        // By default the "ADDRESS" attribute will be set to NULL
        if (!m_address.IsNull)
        {
            OracleUdt.SetValue(con, pUdt, "ADDRESS", m_address);
            // The "ADDRESS" attribute can also be accessed by specifying index 1
            // OracleUdt.SetValue(con, pUdt, 1, m_address);
        }

        // Set the "AGE" attribute.    

        // By default the "AGE" attribute will be set to NULL
        if (m_age != null)
        {
            OracleUdt.SetValue(con, pUdt, "AGE", m_age);
            // The "AGE attribute can also be accessed by specifying index 2
            // OracleUdt.SetValue(con, pUdt, 2, m_age);
        }    

    }

    // Implementation of IOracleCustomType.ToCustomObject()
    public virtual void ToCustomObject(OracleConnection con, object pUdt)
    {
        // Convert from the Oracle Object to a Custom Type

        // Get the "NAME" attribute
        // If the "NAME" attribute is NULL, then null will be returned
        m_name = (string)OracleUdt.GetValue(con, pUdt, "NAME");

        // The "NAME" attribute can also be accessed by specifying index 0
        // m_name = (string)OracleUdt.GetValue(con, pUdt, 0);

        // Get the "ADDRESS" attribute
        // If the "ADDRESS" attribute is NULL, then OracleString.Null will be returned
        m_address = (OracleString)OracleUdt.GetValue(con, pUdt, "ADDRESS");

        // The "NAME" attribute can also be accessed by specifying index 1
        // m_address = (OracleString)OracleUdt.GetValue(con, pUdt, 1);

        // Get the "AGE" attribute

        // If the "AGE" attribute is NULL, then null will  be returned
        m_age = (int?)OracleUdt.GetValue(con, pUdt, "AGE");
        // The "AGE" attribute can also be accessed by specifying index 2
        // m_age = (int?)OracleUdt.GetValue(con, pUdt, 2);    

    }

    public override string ToString()
    {
        // Return a string representation of the custom object
        if (m_bIsNull)
            return "Person.Null";
        else
        {
            string name     =  (m_name == null) ? "NULL" : m_name;
            string address  =  (m_address.IsNull) ? "NULL" : m_address.Value;
            string age      = (m_age == null)? "NULL" : m_age.ToString(); 

            return "Person(" + name + ", " + address + ", " + age + ")";
        }
    }
}

/* PersonFactory Class
   An instance of the PersonFactory class is used to create Person objects
*/
[OracleCustomTypeMappingAttribute("ODP_OBJ1_SAMPLE_PERSON_TYPE")]
public class PersonFactory : IOracleCustomTypeFactory
{
    // Implementation of IOracleCustomTypeFactory.CreateObject()
    public IOracleCustomType CreateObject()
    {
        // Return a new custom object
        return new Person();
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
