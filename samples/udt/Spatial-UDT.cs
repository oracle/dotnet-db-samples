/*
This sample demonstrates how to map and fetch types 
 similar to Oracle Spatial types as custom types. This 
 sample can use managed ODP.NET or ODP.NET Core.

Database schema setup scripts:

1. Connect to HR or another similar schema.
2. Run the following SQL scripts to create Spatial
 types, a table with using these types, and insert
 a row of sample data.

drop table odp_sample_sdo_geo_obj_tab;
drop type odp_sample_sdo_geometry_type;
drop type odp_sample_sdo_ordinate_type;
drop type odp_sample_sdo_elem_info_type;
drop type odp_sample_sdo_point_type;

create type odp_sample_sdo_point_type as object
 (sdo_point_x number, sdo_point_y number, sdo_point_z number);
/

create type odp_sample_sdo_elem_info_type as varray(100) of number;
/

create type odp_sample_sdo_ordinate_type as varray(100) of number;
/

create type odp_sample_sdo_geometry_type as object 
 (sdo_gtype number, sdo_srid number, sdo_point odp_sample_sdo_point_type,
  sdo_elem_info odp_sample_sdo_elem_info_type,
  sdo_ordinate odp_sample_sdo_ordinate_type);
/

create table odp_sample_sdo_geo_obj_tab of odp_sample_sdo_geometry_type;

insert into odp_sample_sdo_geo_obj_tab values(odp_sample_sdo_geometry_type(
 123,123,odp_sample_sdo_point_type(123.45,123.45,123.45),
 odp_sample_sdo_elem_info_type(123,123),
 odp_sample_sdo_ordinate_type(123.45,123.45)));

commit;

*/

using System;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

class SpatialTypeSample
{
    static void Main(string[] args)
    {
        // Enter user id, password, and Oracle data source (i.e. net service name, EZ Connect, etc.)
        string constr = "user id=<USER ID>;password=<PASSWORD>;data source=<DATA SOURCE>";

        string sql1 = "select value(p) from odp_sample_sdo_geo_obj_tab p";

        OracleConnection con = null;
        OracleCommand cmd = null;
        OracleDataReader reader = null;

        try
        {
            // Establish a connection to Oracle database
            con = new OracleConnection(constr);
            con.Open();

            cmd = new OracleCommand(sql1, con);
            reader = cmd.ExecuteReader();

            // Fetch each row
            int rowCount = 1;
            while (reader.Read())
            {
                // Fetch the objects as a custom type
                SdoGeometry p;
                if (reader.IsDBNull(0))
                    p = SdoGeometry.Null;
                else
                    p = (SdoGeometry)reader.GetValue(0);

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

// Oracle Spatial Classes

// The SdoPoint class has the attributes X, Y, and Z, all of type double.
public class SdoPoint : IOracleCustomType, INullable
{
    [OracleObjectMapping(0)]
    public double X;
    [OracleObjectMapping(1)]
    public double Y;
    [OracleObjectMapping(2)]
    public double Z;

    private bool m_bIsNull;

    public bool IsNull
    {
        get
        {
            return m_bIsNull;
        }
    }

    public static SdoPoint Null
    {
        get
        {
            SdoPoint obj = new SdoPoint();
            obj.m_bIsNull = true;
            return obj;
        }
    }

    public override string ToString()
    {
        if (m_bIsNull)
            return "SdoPoint.Null";
        else
            return "SdoPoint(" + X + "," + Y + "," + Z + ")";
    }

    public void ToCustomObject(OracleConnection con, object pUdt)
    {
        // If the UDT may contain NULL attribute data, enable the following code
        //if (!OracleUdt.IsDBNull(con, pUdt, 0))
        X = (double)OracleUdt.GetValue(con, pUdt, 0);

        // If the UDT may contain NULL attribute data, enable the following code
        //if (!OracleUdt.IsDBNull(con, pUdt, 1))
        Y = (double)OracleUdt.GetValue(con, pUdt, 1);

        // If the UDT may contain NULL attribute data, enable the following code
        //if (!OracleUdt.IsDBNull(con, pUdt, 2))
        Z = (double)OracleUdt.GetValue(con, pUdt, 2);
    }

    public void FromCustomObject(OracleConnection con, object pUdt)
    {
        OracleUdt.SetValue(con, pUdt, 0, X);
        OracleUdt.SetValue(con, pUdt, 1, Y);
        OracleUdt.SetValue(con, pUdt, 2, Z);
    }
}

// An instance of the SdoPointFactory class is used to create SdoPoint objects.
[OracleCustomTypeMapping("ODP_SAMPLE_SDO_POINT_TYPE")]
public class SdoPointFactory : IOracleCustomTypeFactory
{
    // IOracleCustomTypeFactory Inteface
    public IOracleCustomType CreateObject()
    {
        SdoPoint sdoPoint = new SdoPoint();
        return sdoPoint;
    }
}

// The SdoGeometry class is a spatial object's geometric description stored in a single row,
//  in a single column of object type ODP_SAMPLE_SDO_GEOMETRY_TYPE in an user-defined table.
// The object is similar to the standard Oracle Spatial SDO_GEOMETRY object type.
public class SdoGeometry : INullable, IOracleCustomType
{
    [OracleObjectMapping(0)]
    public int _gtype;
    [OracleObjectMapping(1)]
    public int _srid;
    [OracleObjectMapping(2)]
    public SdoPoint _point;
    [OracleObjectMapping(3)]
    public int[] _elementInfo;
    [OracleObjectMapping(4)]
    public double[] _ordinates;

    private bool m_bIsNull;

    public bool IsNull
    {
        get
        {
            return m_bIsNull;
        }
    }

    public static SdoGeometry Null
    {
        get
        {
            SdoGeometry obj = new SdoGeometry();
            obj.m_bIsNull = true;
            return obj;
        }
    }

    public void ToCustomObject(OracleConnection con, object pUdt)
    {
        // If the UDT may contain NULL attribute data, enable the following code
        //if (!OracleUdt.IsDBNull(con, pUdt, 0))
        _gtype = (int)OracleUdt.GetValue(con, pUdt, 0);

        // If the UDT may contain NULL attribute data, enable the following code
        //if (!OracleUdt.IsDBNull(con, pUdt, 0))
        _srid = (int)OracleUdt.GetValue(con, pUdt, 1);
        _point = (SdoPoint)OracleUdt.GetValue(con, pUdt, 2);
        _elementInfo = (int[])OracleUdt.GetValue(con, pUdt, 3);
        _ordinates = (double[])OracleUdt.GetValue(con, pUdt, 4);
    }

    public void FromCustomObject(OracleConnection con, object pUdt)
    {
        OracleUdt.SetValue(con, pUdt, 0, _gtype);
        OracleUdt.SetValue(con, pUdt, 1, _srid);
        OracleUdt.SetValue(con, pUdt, 2, _point);
        OracleUdt.SetValue(con, pUdt, 3, _elementInfo);
        OracleUdt.SetValue(con, pUdt, 4, _ordinates);
    }

    public int[] ElementInfo
    {
        get
        {
            return _elementInfo;
        }
    }

    public double[] Ordinates
    {
        get
        {
          return _ordinates;
        }
    }

    public override string ToString()
    {
        string eleminfostr = String.Empty, ordinatesstr = String.Empty;
        if (m_bIsNull)
            return "SdoGeometry.Null";
        else
        {
            eleminfostr = _elementInfo[0].ToString();
            for (int i = 1; i < _elementInfo.Length; i++)
                eleminfostr += "," + _elementInfo[i];
            eleminfostr = "ElementInfo(" + eleminfostr + ")";

            ordinatesstr = _ordinates[0].ToString();
            for (int i = 1; i < _ordinates.Length; i++)
                ordinatesstr += "," + _ordinates[i];
            ordinatesstr = "Ordinates(" + ordinatesstr + ")";
        }
        return String.Format("SdoGeometry({0},{1},{2},{3},{4})",
          _gtype, _srid, _point, eleminfostr, ordinatesstr);
    }
}

// An instance of the SdoGeometryFactory class is used to create SdoGeometry objects.
[OracleCustomTypeMapping("ODP_SAMPLE_SDO_GEOMETRY_TYPE")]
public class SdoGeometryFactory : IOracleCustomTypeFactory
{
    // IOracleCustomTypeFactory Inteface
    public IOracleCustomType CreateObject()
    {
        return new SdoGeometry();
    }
}

// An instance of the SdoElemInfoArrayFactory class is used to create an
//  SDO element information array.
[OracleCustomTypeMapping("ODP_SAMPLE_SDO_ELEM_INFO_TYPE")]
public class SdoElemInfoArrayFactory : IOracleArrayTypeFactory
{
    // IOracleArrayTypeFactory.CreateArray Inteface
    public Array CreateArray(int numElems)
    {
        return new int[numElems];
    }

    // IOracleArrayTypeFactory.CreateStatusArray
    public Array CreateStatusArray(int numElems)
    {
        // An OracleUdtStatus[] is not required to store null status information
        // if there is no NULL attribute data in the element array
        return null;
    }
}

// An instance of the SdoOrdinateArrayFactory class is used to create an SDO
//  ordinate array, which indicates the spatial object coordinate boundaries.
[OracleCustomTypeMapping("ODP_SAMPLE_SDO_ORDINATE_TYPE")]
public class SdoOrdinateArrayFactory : IOracleArrayTypeFactory
{
    // IOracleArrayTypeFactory.CreateArray Inteface
    public Array CreateArray(int numElems)
    {
        return new double[numElems];
    }

    // IOracleArrayTypeFactory.CreateStatusArray
    public Array CreateStatusArray(int numElems)
    {
        // An OracleUdtStatus[] is not required to store null status information
        // if there is no NULL attribute data in the element array
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
