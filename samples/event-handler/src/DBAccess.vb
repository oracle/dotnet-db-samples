'* Copyright (c) 2017, Oracle And/Or its affiliates. All rights reserved. */

'******************************************************************************
' *
' * You may Not use the identified files except in compliance with The MIT
' * License (the "License.")
' *
' * You may obtain a copy of the License at
' * https://github.com/oracle/Oracle.NET/blob/master/LICENSE
' *
' * Unless required by applicable law Or agreed to in writing, software
' * distributed under the License Is distributed on an "AS IS" BASIS, WITHOUT
' * WARRANTIES Or CONDITIONS OF ANY KIND, either express Or implied.
' *
' * See the License for the specific language governing permissions And
' * limitations under the License.
' *
' *****************************************************************************/

'**************************************************************************
'@author  Abhijeet Kulkarni
'@version 1.0
'Development Environment        :  MS Visual Studio .Net
'Name of the File               :  DBAccess.vb
'Creation/Modification History  :
'                                  17-Mar-2003     Created

'Overview: This file contains DBAccess class. This class has the methods 
'that handle database interaction. It has methods that set up the connection
'to the database, create the initial schema and populate the dataGrid. Along
'with few helper method this class contains OnRowUpdating and OnRowUpdated
'subroutines that are used by dataAdapter.RowUpdating and dataAdapter.RowUpdated
'events. 
'**************************************************************************
Imports Oracle.ManagedDataAccess.Client
Imports System.Windows.Forms.TextBox
Imports System.Data
Imports System.Windows.Forms
Imports System


Public Class DBAccess
    Protected conn As OracleConnection
    Protected dg As DataGrid
    Public isConnected As Boolean
    Public sbox As TextBox
    Public dataAdapter As OracleDataAdapter
    Public dset As DataSet
    Public ocb As OracleCommandBuilder
    Public updatingCount As Int32
    Public updatedCount As Int32

    '************************************************************************
    'The constructor. Reference of TextBox is passed from Form1.vb. This text
    'box is used to diplay status messages.
    '************************************************************************
    Public Sub New(ByRef box As TextBox)
        MyBase.new()
        'isConnected is set to False (Not connected state)
        isConnected = False
        sbox = box
    End Sub

    '************************************************************************
    'This method returns connection object.
    '************************************************************************
    Public Function getConnection() As OracleConnection
        'Return connection reference
        Return conn
    End Function

    '************************************************************************
    'This method trys to connect to the database using given connection 
    'parameters.
    '************************************************************************
    Public Function connect(ByVal username As String, ByVal password As String, ByVal datasource As String) As String
        'Create the connect string
        Dim Str As String = New String("User Id=" + username + ";Password=" + password + ";Data Source=" + datasource)
        Dim errStr As String = "Not Connected"
        Try
            'Retrieve connection object
            sbox.AppendText("Connecting to the database.." + Environment.NewLine)
            sbox.Update()
            'Create new OracleConnection instance 
            conn = New OracleConnection(Str)
            'Open the connection
            conn.Open()
            'Set the isConnected to true(Connected State)
            isConnected = True
            'Display message
            errStr = "Connected to the database as " + username
            sbox.AppendText(errStr + Environment.NewLine)
            sbox.Update()
        Catch ex As Exception
            'If there is an error isConnected is set to false
            isConnected = False
            'Display message
            sbox.AppendText("Error ocurred while connecting to the Database" + Environment.NewLine)
            sbox.AppendText("Error Message" + ex.ToString() + Environment.NewLine)
        End Try
        Return errStr
    End Function

    '************************************************************************
    'This method creates countrytab table required by this application and 
    'inserts some data in the table.
    '************************************************************************
    Public Function CreateTable()
        Try
            sbox.AppendText("Dropping the table countrytab" + Environment.NewLine)
            sbox.Update()
            'Create an OracleCommand object using the connection object
            Dim cmd As OracleCommand = New OracleCommand("DROP TABLE countrytab", conn)
            cmd.CommandType = CommandType.Text
            Try
                'Drop the table if it exists
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                sbox.AppendText("Table does not exist" + Environment.NewLine)
                sbox.Update()
            End Try
            sbox.AppendText("Creating required table " + Environment.NewLine)
            sbox.Update()
            'Create table countrytab
            cmd = New OracleCommand("CREATE TABLE countrytab(countryname VARCHAR2(30) PRIMARY KEY," & _
                 "population  NUMBER(12),language VARCHAR2(20) NOT NULL ,currency VARCHAR2(20) NOT NULL)", conn)
            cmd.CommandType = CommandType.Text
            cmd.ExecuteNonQuery()
            sbox.AppendText("Table Created" + Environment.NewLine)
            sbox.Update()
            sbox.AppendText("Inserting data in created table" + Environment.NewLine)
            sbox.Update()
            'Insert rows in the table
            cmd = New OracleCommand("INSERT INTO countrytab VALUES('United States',280000000 ,'English American','US Dollar $')", conn)
            cmd.CommandType = CommandType.Text
            cmd.ExecuteNonQuery()
            cmd.Dispose()
            cmd = New OracleCommand("INSERT INTO countrytab VALUES('United Kingdom',59000000 ,'English British','Pound Sterling')", conn)
            cmd.CommandType = CommandType.Text
            cmd.ExecuteNonQuery()
            cmd.Dispose()
            cmd = New OracleCommand("INSERT INTO countrytab VALUES('Germany',82000000 ,'German','Euro')", conn)
            cmd.CommandType = CommandType.Text
            cmd.ExecuteNonQuery()
            cmd.Dispose()
            cmd = New OracleCommand("INSERT INTO countrytab VALUES('France',59000000 ,'French','Euro')", conn)
            cmd.CommandType = CommandType.Text
            cmd.ExecuteNonQuery()
            cmd.Dispose()
            sbox.AppendText("Four rows Inserted" + Environment.NewLine)
            sbox.Update()
        Catch ex As Exception
            'If an exception is caught set isConnected to false
            isConnected = False
            sbox.AppendText("An error oucurred while creating table" + Environment.NewLine)
            sbox.AppendText(ex.toString() + Environment.NewLine)
            sbox.Update()
        End Try
    End Function

    '************************************************************************
    'This method closes and disposes the connection.
    '************************************************************************
    Public Function closeConnection()
        Try
            'Check if connection is open or not
            If Not conn.State.Closed Then
                conn.Close()
                conn.Dispose()
            End If
        Catch ex As Exception
        Finally
            sbox.AppendText("Connection closed" + Environment.NewLine)
            sbox.Update()
        End Try
    End Function

    '************************************************************************
    'This method populates the datagrid and sets up EventHandlers
    '************************************************************************
    Public Function PopulateDataGrid(ByRef dgrid As DataGrid)
        Dim pk As DataColumn()
        Try
            dg = dgrid
            sbox.AppendText("Populating data grid.." + Environment.NewLine)
            sbox.Update()
            'Select query for the dataAdapter
            dataAdapter = New OracleDataAdapter("SELECT countryname,population,language,currency FROM countrytab", conn)
            ocb = New OracleCommandBuilder(dataAdapter)
            'Create new dataset instance
            dset = New DataSet()
            'Fill the dataset
            dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
            dataAdapter.Fill(dset, "COUNTRYTAB")
            'Initially the EventHandlers are enabled so add RowUpdating handler
            AddHandler dataAdapter.RowUpdating, AddressOf OnRowUpdating
            'Set flag to 1
            updatingCount = 1
            'Initially the EventHandlers are enabled so add RowUpdated handler
            AddHandler dataAdapter.RowUpdated, AddressOf OnRowUpdated
            'Set flag to 1
            updatedCount = 1
            dset.Tables(0).Columns(0).ReadOnly = True
            dset.Tables(0).Columns(1).ReadOnly = True
            dset.Tables(0).Columns(2).MaxLength = 20
            dset.Tables(0).Columns(3).MaxLength = 20
            dg.SetDataBinding(dset, "COUNTRYTAB")
        Catch ex As Exception
            sbox.AppendText("Error while populating datagrid" + Environment.NewLine)
            sbox.AppendText(ex.ToString() + Environment.NewLine)
            sbox.Update()
        End Try
    End Function

    '************************************************************************
    'This method updates the database using dataAdapter
    '************************************************************************
    Public Function UpdateRecords()
        Try
            'Check if there are any null cells in the DataGrid
            Dim flag As Boolean = NullCellsPresent(dg)
            'Update the database with the help of dataAdapter update method
            If flag = False Then
                If (updatingCount = 0 And updatedCount = 0) Then
                    sbox.AppendText("No event handlers have been set." + Environment.NewLine)
                    sbox.Update()
                End If
                dataAdapter.Update(dset, "COUNTRYTAB")
            End If
        Catch ex As OracleException
            sbox.AppendText("Error while Updating the database" + Environment.NewLine)
            sbox.AppendText(ex.ToString() + Environment.NewLine)
            sbox.Update()
        End Try
    End Function

    '************************************************************************
    'RowUpdating EventHandler 
    '************************************************************************
    Private Sub OnRowUpdating(ByVal sender As Object, ByVal e As OracleRowUpdatingEventArgs)
        sbox.ForeColor = Color.DarkBlue
        sbox.AppendText("** OnRowUpdating() Called **" + Environment.NewLine)
        sbox.Update()
    End Sub

    '************************************************************************
    'RowUpdated EventHandler
    '************************************************************************
    Private Sub OnRowUpdated(ByVal sender As Object, ByVal e As OracleRowUpdatedEventArgs)
        sbox.ForeColor = Color.Red
        sbox.AppendText("****** OnRowUpdated() Called **" + Environment.NewLine)
        sbox.Update()
    End Sub

    '************************************************************************
    'This method adds a RowUpdating event handler if it is not added already.
    '************************************************************************
    Public Function AddUpdatingHandler()
        If updatingCount = 0 Then
            'Add the eventHandler
            AddHandler dataAdapter.RowUpdating, AddressOf OnRowUpdating
            updatingCount = 1
        End If
    End Function

    '************************************************************************
    'This method removes a RowUpdating event handler if it is exists already.
    '************************************************************************
    Public Function RemoveUpdatingHandler()
        If updatingCount = 1 Then
            'Remove the eventHandler
            RemoveHandler dataAdapter.RowUpdating, AddressOf OnRowUpdating
            updatingCount = 0
        End If
    End Function

    '************************************************************************
    'This method adds a RowUpdated event handler if it is not added already.
    '************************************************************************
    Public Function AddUpdatedHandler()
        If updatedCount = 0 Then
            'Add the eventHandler
            AddHandler dataAdapter.RowUpdated, AddressOf OnRowUpdated
            updatedCount = 1
        End If
    End Function

    '************************************************************************
    'This method removes a RowUpdated event handler if it is exists already.
    '************************************************************************
    Public Function RemoveUpdatedHandler()
        If updatedCount = 1 Then
            'Remove the eventHandler
            RemoveHandler dataAdapter.RowUpdated, AddressOf OnRowUpdated
            updatedCount = 0
        End If
    End Function


    '************************************************************************
    'This method returns true if any of the editable columns are empty
    '************************************************************************
    Public Function NullCellsPresent(ByRef dg As DataGrid) As Boolean
        Dim flag As Boolean = False
        Dim ds As DataSet = dg.DataSource
        Dim dt As DataTable = ds.Tables(0)
        Dim dr As DataRow
        Dim str As String
        Dim str1 As String
        For Each dr In dt.Rows
            str = dr.Item(2)
            str1 = dr.Item(3)
            If str.Length = 0 Or str1.Length = 0 Then
                flag = True
            End If
        Next
        If flag = True Then
            MessageBox.Show("Language or Currency values cannot be empty strings", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
        NullCellsPresent = flag
    End Function
End Class
