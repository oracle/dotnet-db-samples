/*
This sample demonstrates how to fetch UDTs referenced by REFs.
This sample can use managed ODP.NET or ODP.NET Core.

Database schema setup scripts:

1. Connect to HR or another similar schema.
2. Run the following SQL scripts to create a Person type
 and a table with with the Person type. Two rows will be
 inserted into the table. Finally, a table using REFs to
 the Person instances and a stored procedure that inserts 
 into that table will be created.

drop procedure odp_ref1_sample_upd_contacts;
drop table odp_ref1_sample_contacts;
drop table odp_ref1_sample_person_obj_tab;
drop type odp_ref1_sample_person_type;

create type odp_ref1_sample_person_type as object 
  (name varchar2(30), address varchar2(60), age number(3)) NOT FINAL;
/
 
create table odp_ref1_sample_person_obj_tab of odp_ref1_sample_person_type;
 
insert into odp_ref1_sample_person_obj_tab values (
  'John', 'Address 1', 20);
insert into odp_ref1_sample_person_obj_tab values (
  'Jim', 'Address 2', 25);
 
create table odp_ref1_sample_contacts (
  contact_ref  ref odp_ref1_sample_person_type, 
  contact_phone  varchar2(20));

create procedure odp_ref1_sample_upd_contacts(
  param1 IN REF odp_ref1_sample_person_type,
  param2 IN varchar2) as
  begin      
    insert into odp_ref1_sample_contacts values(param1,param2);   
  end;
/

*/

using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

class RefSample
{
    static void Main(string[] args)
    {
        // Enter user id, password, and Oracle data source (i.e. net service name, EZ Connect, etc.)
        string constr = "user id=<USER ID>;password=<PASSWORD>;data source=<DATA SOURCE>";

        string sql1 = "select ref(p) from odp_ref1_sample_person_obj_tab p " + 
                      "where p.name ='John'";
        string sql2 = "odp_ref1_sample_upd_contacts";
        string sql3 = "select deref(c.contact_ref), c.contact_phone " +
                      "from odp_ref1_sample_contacts c";

        OracleConnection con = null;
        OracleCommand cmd = null;
        OracleDataReader reader1 = null;
        OracleDataReader reader2 = null;
        OracleRef refP = null;

        try 
        { 
            // Establish a connection to Oracle DB.
            con = new OracleConnection(constr);
            con.Open();

            // Retrieve REF from the object table.
            cmd = new OracleCommand(sql1, con);
            cmd.CommandType = CommandType.Text;
            reader1 = cmd.ExecuteReader();
            reader1.Read();
            refP = reader1.GetOracleRef(0);
            Console.WriteLine("HEX value of ref object: {0}", refP.Value);
            Console.WriteLine();

            // Insert a row into the object-relational table with the
            // REF PERSON value just retrieved (i.e. John)
            cmd.CommandText = sql2;
            cmd.CommandType = CommandType.StoredProcedure;
            OracleParameter param1   = new OracleParameter();
    
            param1.OracleDbType   = OracleDbType.Ref;
            param1.Direction      = ParameterDirection.Input;

            // Note: The UdtTypeName is case-senstive.
            param1.UdtTypeName     = "ODP_REF1_SAMPLE_PERSON_TYPE";
            param1.Value = refP;
    
            cmd.Parameters.Add(param1);

            OracleParameter param2 = new OracleParameter();
            param2.OracleDbType = OracleDbType.Varchar2;
            param2.Direction = ParameterDirection.Input;   
            param2.Value = "1-800-555-4412";

            cmd.Parameters.Add(param2);
            cmd.ExecuteNonQuery();

            // Retrieve the rows containing the name, John.
            cmd.Parameters.Clear();
            cmd.CommandText = sql3;
            cmd.CommandType = CommandType.Text;
            reader2 = cmd.ExecuteReader();

            // Fetch each row
            int rowCount = 1;
            while (reader2.Read())
            {
                // Fetch the object as a custom type
                Person p;
                if (reader2.IsDBNull(0))
                    p = Person.Null;
                else
                    p = (Person)reader2.GetValue(0);
                string phone = reader2.GetString(1);
                Console.WriteLine("Row {0}: {1}, {2}", rowCount++, p, phone);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            // Clean up
            if (reader1 != null)
                reader1.Dispose();
            if (reader2 != null)
                reader2.Dispose();
            if (refP != null)
                refP.Dispose();
            if (cmd != null)
                cmd.Dispose();
            if (con != null)
                con.Dispose();
        }
    }
}

// Person Class
// An instance of a Person class represents an ODP_REF1_SAMPLE_PERSON_TYPE object.
// A custom type must implement INullable and IOracleCustomType interfaces.
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

    // Person.Null is used to return a NULL Person object.
    public static Person Null
    {
        get
        {
            Person p = new Person();
            p.m_bIsNull = true;
            return p;
        }
    }

    // Specify the OracleObjectMappingAttribute to map "Name" to "NAME".
    [OracleObjectMappingAttribute("NAME")]
    // The mapping can also be specified using attribute index 0.
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

    // Specify the OracleObjectMappingAttribute to map "Address" to "ADDRESS".
    [OracleObjectMappingAttribute("ADDRESS")]
    // The mapping can also be specified using attribute index 1.
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

    // Specify the OracleObjectMappingAttribute to map "Age" to "AGE".
    [OracleObjectMappingAttribute("AGE")]
    // The mapping can also be specified using attribute index 2.
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
        // Convert from the Custom Type to Oracle Object.

        // Set the "NAME" attribute.     
        // By default the "NAME" attribute will be set to NULL.
        if (m_name != null)
        {
            OracleUdt.SetValue(con, pUdt, "NAME", m_name);
            // The "NAME" attribute can also be accessed by specifying index 0.
            // OracleUdt.SetValue(con, pUdt, 0, m_name);
        }

        // Set the "ADDRESS" attribute.     
        // By default the "ADDRESS" attribute will be set to NULL.
        if (!m_address.IsNull)
        {
            OracleUdt.SetValue(con, pUdt, "ADDRESS", m_address);
            // The "ADDRESS" attribute can also be accessed by specifying index 1.
            // OracleUdt.SetValue(con, pUdt, 1, m_address);
        }

        // Set the "AGE" attribute.    
        // By default the "AGE" attribute will be set to NULL.
        if (m_age != null)
        {
            OracleUdt.SetValue(con, pUdt, "AGE", m_age);
            // The "AGE attribute can also be accessed by specifying index 2.
            // OracleUdt.SetValue(con, pUdt, 2, m_age);
        }    
    }

    // Implementation of IOracleCustomType.ToCustomObject()
    public virtual void ToCustomObject(OracleConnection con, object pUdt)
    {
        // Convert from the Oracle Object to a Custom Type.

        // Get the "NAME" attribute.
        // If the "NAME" attribute is NULL, then null will be returned.
        m_name = (string)OracleUdt.GetValue(con, pUdt, "NAME");
        // The "NAME" attribute can also be accessed by specifying index 0.
        // m_name = (string)OracleUdt.GetValue(con, pUdt, 0);

        // Get the "ADDRESS" attribute.
        // If the "ADDRESS" attribute is NULL, then OracleString.Null will be returned.
        m_address = (OracleString)OracleUdt.GetValue(con, pUdt, "ADDRESS");
        // The "ADDRESS" attribute can also be accessed by specifying index 1.
        // m_address = (OracleString)OracleUdt.GetValue(con, pUdt, 1);

        // Get the "AGE" attribute.
        // If the "AGE" attribute is NULL, then null will  be returned.
        m_age = (int?)OracleUdt.GetValue(con, pUdt, "AGE");
        // The "AGE" attribute can also be accessed by specifying index 2.
        // m_age = (int?)OracleUdt.GetValue(con, pUdt, 2);   
    }

    public override string ToString()
    {
        // Return a string representation of the custom object.
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

// PersonFactory Class
// An instance of the PersonFactory class is used to create Person objects.
[OracleCustomTypeMappingAttribute("ODP_REF1_SAMPLE_PERSON_TYPE")]
public class PersonFactory : IOracleCustomTypeFactory
{
    // Implementation of IOracleCustomTypeFactory.CreateObject()
    public IOracleCustomType CreateObject()
    {
        // Return a new custom object.
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
