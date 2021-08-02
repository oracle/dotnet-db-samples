/*
This sample demonstrates how to obtain Custom Type objects from 
 OracleRef objects. It also demonstrates how to update UDTs 
 through the OracleRef object and to obtain the appropriate 
 instance type for those UDTs that have an inheritance hierarchy
 from OracleRef objects. This sample can use managed ODP.NET or 
 ODP.NET Core.

Database schema setup scripts:

1. Connect to HR or another similar schema.
2. Run the following SQL scripts to create a person type, 
 a student type that inherits from person type, and a table 
 that can store both person and student objects. One row of
 each type will be added to the table.

drop table odp_ref2_sample_person_obj_tab;
drop type odp_ref2_sample_student_type;
drop type odp_ref2_sample_person_type;

create type odp_ref2_sample_person_type as object 
  (name varchar2(30), address varchar2(60), age number(3)) NOT FINAL;
/
create type odp_ref2_sample_student_type under odp_ref2_sample_person_type
  (dept_id number(2), major varchar2(20));
/

-- odp_ref2_sample_person_obj_tab can store both persons and student objects
create table odp_ref2_sample_person_obj_tab of odp_ref2_sample_person_type;
insert into odp_ref2_sample_person_obj_tab values (odp_ref2_sample_person_type('John', 'Address 1', 20));
insert into odp_ref2_sample_person_obj_tab values (odp_ref2_sample_student_type('Jim', 'Address 2', 25, NULL, 'Physics'));
 
*/

using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

class RefInheritanceSample
{
    static void Main(string[] args)
    {
        // Enter user id, password, and Oracle data source (i.e. net service name, EZ Connect, etc.)
        string constr = "user id=<USER ID>;password=<PASSWORD>;data source=<DATA SOURCE>";

        string sqlInsertRef = "insert into odp_ref2_sample_person_obj_tab values (:1)";
        string sqlSelectRef = "select ref(p) from odp_ref2_sample_person_obj_tab p";
        string sqlSelectValue = "select value(p) from odp_ref2_sample_person_obj_tab p";
        string udtTypeNameP = "ODP_REF2_SAMPLE_PERSON_TYPE";
        string objTabNameP = "ODP_REF2_SAMPLE_PERSON_OBJ_TAB";

        // Create a new Person object.
        Person p = new Person();
        p.Name = "John";
        p.Address = "Address 1";
        p.Age = 20;

        // Create a new Student object.
        Student s = new Student();
        s.Name = "Jim";
        s.Address = "Address 2";
        s.Age = 25;
        s.Major = "Physics";

        OracleConnection con = null;
        OracleCommand cmd = new OracleCommand();
        OracleDataReader reader = null;
        OracleTransaction txn = null;
        OracleRef refP = null;  // person REF
        OracleRef refS = null;  // student REF

        try
        {
            // Establish a connection to Oracle DB.
            con = new OracleConnection(constr);
            con.Open();
            cmd.Connection = con;

            try
            {
                // Inserting a person and a student instance into the odp_ref2_sample_person_obj_tab
                txn = con.BeginTransaction();

                cmd.CommandText = sqlInsertRef;
                OracleParameter param = new OracleParameter("inParam", OracleDbType.Object, ParameterDirection.Input);
                param.UdtTypeName = udtTypeNameP;
                cmd.Parameters.Add(param);

                // Insert person
                param.Value = p;
                cmd.ExecuteNonQuery();

                // Insert Student
                param.Value = s;
                cmd.ExecuteNonQuery();

                txn.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in inserting into {0}: {1}", objTabNameP, ex.Message);
            }
            finally
            {
                if (txn != null)
                {
                    txn.Dispose();
                    txn = null;
                }
                if (cmd != null)
                    cmd.Parameters.Clear();
            }

            try
            {
                // Retrieving REF from odp_ref2_sample_person_obj_tab
                cmd.CommandText = sqlSelectRef;

                reader = cmd.ExecuteReader();
                int row = 1;

                while (reader.Read())
                {
                    if (row == 1)
                        refP = reader.GetOracleRef(0);
                    else
                        refS = reader.GetOracleRef(0);
                    row++;
                }

                // Fetch rows from database table.
                Person p1 = (Person)refP.GetCustomObject();
                Student s1 = (Student)refS.GetCustomObject();
                Console.WriteLine("Person: " + p1);
                Console.WriteLine("Student: " + s1);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in selecting from {0}: {1}", objTabNameP, ex.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
            }

            try
            {
                // Update person object.
                txn = con.BeginTransaction();

                Person p2 = (Person)refP.GetCustomObject();

                // Update person's age using OracleRef.
                p2.Age = p2.Age + 1;
                refP.Update(p2);

                // p2 is updated to the database.
                txn.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in updating person in {0}: {1}", objTabNameP, ex.Message);
            }
            finally
            {
                if (txn != null)
                {
                    txn.Dispose();
                    txn = null;
                }
            }

            // Delete student object.
            try
            {
                txn = con.BeginTransaction();
                refS.Delete();
                txn.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in deleting student from {0}: {1}", objTabNameP, ex.Message);
            }
            finally
            {
                if (txn != null)
                {
                    txn.Dispose();
                    txn = null;
                }
            }

            try
            {
                // Retrieve rows from the database table.
                cmd.CommandText = sqlSelectValue;
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();

                // Fetch each row.
                int rowCount = 1;
                while (reader.Read())
                {
                    // Fetch the objects as a custom type.
                    Person p3;
                    if (reader.IsDBNull(0))
                        p3 = Person.Null;
                    else
                        p3 = (Person)reader.GetValue(0);

                    Console.WriteLine("Row {0}: {1}", rowCount++, p3);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in selecting from {0}: {1}", objTabNameP, ex.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Get exception: " + ex.Message);
        }
        finally
        {
            // Clean up
            if (refS != null)
                refS.Dispose();
            if (refP != null)
                refP.Dispose();
            if (reader != null)
                reader.Dispose();
            if (cmd != null)
                cmd.Dispose();
            if (txn != null)
                txn.Dispose();
            if (con != null)
            {
                con.Close();
                con.Dispose();
            }
        }
    }

    // Person Class
    // A Person class instance represents an ODP_REF2_SAMPLE_PERSON_TYPE object.
    // A custom type must implement INullable and IOracleCustomType interfaces.
    public class Person : INullable, IOracleCustomType
    {
        private bool m_bIsNull;    // Whether the Person object is NULL    
        private string m_name;     // "NAME" attribute  
        private OracleString m_address;  // "ADDRESS" attribute  
        private int? m_age;              // "AGE" attribute

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
            // Convert from the Oracle Object to a Custom Type.

            // Get the "NAME" attribute.
            // If the "NAME" attribute is NULL, then null will be returned.
            // The "NAME" attribute can also be accessed by specifying index 0.
            m_name = (string)OracleUdt.GetValue(con, pUdt, "NAME");

            // Get the "ADDRESS" attribute.
            // If the "ADDRESS" attribute is NULL, then OracleString.Null will be returned.
            // The "ADDRESS" attribute can also be accessed by specifying index 1.
            m_address = (OracleString)OracleUdt.GetValue(con, pUdt, "ADDRESS");

            // Get the "AGE" attribute.
            // If the "AGE" attribute is NULL, then null will  be returned.
            // The "AGE" attribute can also be accessed by specifying index 2.
            m_age = (int?)OracleUdt.GetValue(con, pUdt, "AGE");
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
                string age = (m_age == null) ? "NULL" : m_age.ToString();
                return "Person(" + name + ", " + address + ", " + age + ")";
            }
        }
    }

    // PersonFactory Class
    // An instance of the PersonFactory class is used to create Person objects.
    [OracleCustomTypeMappingAttribute("ODP_REF2_SAMPLE_PERSON_TYPE")]
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
    // An instance of a Student class represents an  ODP_REF2_SAMPLE_STUDENT_TYPE object
    // Note: We do not map the "DEPT_ID" attribute (attribute index 3). So, it will always
    // be NULL. A custom type must implement INullable and IOracleCustomType interfaces.
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
        // The mapping can also be specified using attribute index 5.
        // [OracleObjectMappingAttribute(5)]
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
            // By default the "MAJOR" attribute will be set to NULL.
            // The "MAJOR" attribute can also be accessed by specifying index 5.
            if (m_major != null)
                OracleUdt.SetValue(con, pUdt, "MAJOR", m_major);
        }

        // Implementation of IOracleCustomType.ToCustomObject()
        public override void ToCustomObject(OracleConnection con, object pUdt)
        {
            // Convert from the Oracle Object to a Custom Type.
            // Invoke the base class conversion method.
            base.ToCustomObject(con, pUdt);

            // Get the "MAJOR" attribute.
            // If the "MAJOR" attribute is NULL, then "null" will be returned.
            // The "MAJOR" attribute can also be accessed by specifying index 5.
            m_major = (string)OracleUdt.GetValue(con, pUdt, "MAJOR");
        }

        public override string ToString()
        {
            // Return a string representation of the custom object.
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
                return "Student(" + name + ", " + address + ", " + age + ", " +
                  major + ")";
            }
        }
    }

    // StudentFactory Class
    // An instance of the StudentFactory class is used to create Student objects.
    [OracleCustomTypeMappingAttribute("ODP_REF2_SAMPLE_STUDENT_TYPE")]
    public class StudentFactory : IOracleCustomTypeFactory
    {
        // Implementation of IOracleCustomTypeFactory.CreateObject()
        public IOracleCustomType CreateObject()
        {
            // Return a new custom object.
            return new Student();
        }
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
