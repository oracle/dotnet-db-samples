/*  
This sample demonstrates how to map, fetch, and manipulate 
 a nested table of UDTs that has an inheritance hierarchy 
 (i.e. parent and child types). This sample can use managed 
 ODP.NET or ODP.NET Core.

Database schema setup scripts:

1. Connect to HR or another similar schema.
2. Run the following SQL scripts to create a person type, 
 a student type that inherits from person type, a nested 
 table of person types, and a table with a nested table column.

drop table odp_nt_sample_person_rel_tab;
drop type odp_nt_sample_person_coll_type;
drop type odp_nt_sample_student_type;
drop type odp_nt_sample_person_type;

create type odp_nt_sample_person_type as object 
  (name varchar2(30), address varchar2(60), age number(3)) NOT FINAL;
/
create type odp_nt_sample_student_type under odp_nt_sample_person_type
 (dept_id number(2), major varchar2(20));
/
create type odp_nt_sample_person_coll_type as 
 table of odp_nt_sample_person_type;
/
create table odp_nt_sample_person_rel_tab 
 (col1 odp_nt_sample_person_coll_type) nested table col1 store as nt_s;

*/

using System;
using System.Data;
using System.Collections;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

class NestedTableSample
{
    static void Main(string[] args)
    {
        // Enter user id, password, and Oracle data source (i.e. net service name, EZ Connect, etc.)
        string constr = "user id=<USER ID>;password=<PASSWORD>;data source=<DATA SOURCE>";

        string sql1 = "insert into odp_nt_sample_person_rel_tab values(:param)";
        string sql2 = "select col1 from odp_nt_sample_person_rel_tab";

        // Create a new Person object
        Person p1 = new Person();
        p1.Name = "John";
        p1.Address = "Address 1";
        p1.Age = 20;

        // Create a new Student object
        Student s1 = new Student();
        s1.Name = "Jim";
        s1.Address = "Address 2";
        s1.Age = 25;
        s1.Major = "Physics";

        // Create a second Student object
        Student s2 = new Student();
        s2.Name = "Alex";
        s2.Address = "Address 3";
        s2.Age = 21;
        s2.Major = "Math";

        // Create a new Person array
        Person[] pa = new Person[] { p1, s1 };

        OracleConnection con = null;
        OracleCommand cmd = null;
        OracleDataReader reader = null;

        try
        {
            // Establish a connection to Oracle DB
            con = new OracleConnection(constr);
            con.Open();

            cmd = new OracleCommand(sql1, con);
            cmd.CommandText = sql1;

            OracleParameter param = new OracleParameter();
            param.OracleDbType = OracleDbType.Array;
            param.Direction = ParameterDirection.Input;

            // Note: The UdtTypeName is case-senstive
            param.UdtTypeName = "ODP_NT_SAMPLE_PERSON_COLL_TYPE";
            param.Value = pa;
            cmd.Parameters.Add(param);

            // Insert a nested table of (person, student) into the table column
            cmd.ExecuteNonQuery();

            // Modify some elements in Person array
            pa[1].Address = "Modified Address";
            pa[1].Age = pa[1].Age + 1;

            // Add/Remove some elements by converting the Person[] to an ArrayList
            ArrayList list = new ArrayList(pa);

            // Remove the first element
            list.RemoveAt(0);

            // Add the second student
            list.Add(s2);
            pa = (Person[])list.ToArray(typeof(Person));

            param.Value = pa;

            // The array now has two students.
            // Insert a nested table of (student, student) into the table column.
            cmd.ExecuteNonQuery();

            cmd.CommandText = sql2;
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();

            // Fetch each row
            int rowCount = 1;
            while (reader.Read())
            {
                // Fetch the array and print out each element.
                // Observe that four new elements were inserted with this app.
                Person[] p = (Person[])reader.GetValue(0);
                for (int i = 0; i < p.Length; i++)
                    Console.WriteLine("Row {0}, Person[{1}]: {2} ", rowCount, i, p.GetValue(i));
                rowCount++;
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

// Person Class
// An instance of a Person class represents an ODP_NT_SAMPLE_PERSON_TYPE object.
// A custom type must implement INullable and IOracleCustomType interfaces. 
public class Person : INullable, IOracleCustomType
{
    private bool m_bIsNull;         // Whether the Person object is NULL    
    private string m_name;          // "NAME" attribute  
    private OracleString m_address; // "ADDRESS" attribute  
    private int?    m_age;          // "AGE" attribute

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
        // Convert from the Custom Type to Oracle Object

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
        // Convert from the Oracle Object to a Custom Type

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
        // Return a string representation of the custom object
        if (m_bIsNull)
            return "Person.Null";
        else
        {
            string name = (m_name == null) ? "NULL" : m_name;
            string address = (m_address.IsNull) ? "NULL" : m_address.Value;
            string age      = (m_age == null)? "NULL" : m_age.ToString(); 

            return "Person(" + name + ", " + address + ", " + age + ")";
        }
    }
}

// PersonFactory Class
// An instance of the PersonFactory class is used to create Person objects.
[OracleCustomTypeMappingAttribute("ODP_NT_SAMPLE_PERSON_TYPE")]
public class PersonFactory : IOracleCustomTypeFactory
{
    // Implementation of IOracleCustomTypeFactory.CreateObject()
    public IOracleCustomType CreateObject()
    {
        // Return a new custom object
        return new Person();
    }
}

// Student Class
// A Student class instance represents an ODP_NT_SAMPLE_STUDENT_TYPE object.
//  Note: We do not map the "DEPT_ID" attribute (attribute index 3) so it
//  will always be NULL. A custom type must implement INullable and
//  IOracleCustomType interfaces.
public class Student : Person, INullable, IOracleCustomType
{
    private bool m_bIsNull;           // Whether the Student object is NULL
    private string m_major;           // "MAJOR" attribute

    // Implementation of INullable.IsNull
    public override bool IsNull
    {
        get
        {
            return m_bIsNull;
        }
    }

    // Student.Null is used to return a NULL Student object.
    public new static Student Null
    {
        get
        {
            Student s = new Student();
            s.m_bIsNull = true;
            return s;
        }
    }

    // Specify the OracleObjectMappingAttribute to map "Major" to "MAJOR".
    [OracleObjectMappingAttribute("MAJOR")]
    // The mapping can also be specified using attribute index 4.
    // [OracleObjectMappingAttribute(4)]
    public string Major
    {
        get
        {
            return m_major;
        }
        set
        {
            m_major = value;
        }
    }

    // Implementation of IOracleCustomType.FromCustomObject()
    public override void FromCustomObject(OracleConnection con, object pUdt)
    {
        // Convert from the Custom Type to Oracle Object.
        // Invoke the base class conversion method.
        base.FromCustomObject(con, pUdt);

        // Set the "MAJOR" attribute. 
        // By default, the "MAJOR" attribute will be set to NULL.
        if (m_major != null)
            OracleUdt.SetValue(con, pUdt, "MAJOR", m_major);

        // The "MAJOR" attribute can also be accessed by specifying index 4.
        // OracleUdt.SetValue(con, pUdt, 4, m_major);
    }

    // Implementation of IOracleCustomType.ToCustomObject()
    public override void ToCustomObject(OracleConnection con, object pUdt)
    {
        // Convert from the Oracle Object to a Custom Type.
        // Invoke the base class conversion method.
        base.ToCustomObject(con, pUdt);

        // Get the "MAJOR" attribute.
        // If the "MAJOR" attribute is NULL, then "null" will be returned.
        m_major = (string)OracleUdt.GetValue(con, pUdt, "MAJOR");
        // The "MAJOR" attribute can also be accessed by specifying index 4.
        // m_major = (string)OracleUdt.GetValue(con, pUdt, 4);
    }

    public override string ToString()
    {
        // Return a string representation of the custom object
        if (m_bIsNull)
        {
            return "Student.Null";
        }
        else
        {
            string name = (Name == null) ? "NULL" : Name;
            string address = (Address.IsNull) ? "NULL" : Address.Value;
            string age = (Age == null) ? "NULL" : Age.ToString();
            string major = (m_major == null) ? "NULL" : m_major;

            return "Student(" + name + ", " + address + ", " + age + ", " + major + ")";
        }
    }
}

// StudentFactory Class
//  An instance of the StudentFactory class is used to create Student objects.
[OracleCustomTypeMappingAttribute("ODP_NT_SAMPLE_STUDENT_TYPE")]
public class StudentFactory : IOracleCustomTypeFactory
{
    // Implementation of IOracleCustomTypeFactory.CreateObject()
    public IOracleCustomType CreateObject()
    {
        // Return a new custom object
        return new Student();
    }
}

// PersonArrayFactory Class
// An instance of the PersonArrayFactory class is used to create Person array.
[OracleCustomTypeMappingAttribute("ODP_NT_SAMPLE_PERSON_COLL_TYPE")]
public class PersonArrayFactory : IOracleArrayTypeFactory
{
    // IOracleArrayTypeFactory Inteface
    public Array CreateArray(int numElems)
    {
        return new Person[numElems];
    }

    public Array CreateStatusArray(int numElems)
    {
        // An OracleUdtStatus[] is not required to store null status information.
        return null;
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
