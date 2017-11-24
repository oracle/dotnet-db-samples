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

'**********************************************************************************

' @author                        :  Jagriti
' @version                       :  1.0
' Development Environment        :  Microsoft Visual Studio .Net 
' Name of the File               :  ViewProductsFrm.vb
' Creation/Modification History  :
'                        24-July-2002     Created 
'
' Sample Overview:
' The purpose of this .NET sample application is to demonstrate how to populate a Dataset.
' The connection to database is made using Oracle Data Provider for .NET. 
' The data retrieval is done using Dataset which is an in-memory cache 
' that contains data from database filled by a OracleDataAdapter.
' OracleDataAdapter serves as a bridge between the DataSet and the data source, 
' retrieving data, it includes 'SelectCommand' that facilitates the loading of data
' based on the SQL query given. The connection to database is made using 
' OracleConnection object. 
'
' This sample application is a Visual Basic .Net application that displays a list
' of products from database. The fetched data is displayed in a DataGrid. 
' A 'Close' button is provided to exit from the application.
'**********************************************************************************

'Standard Namespaces referenced in this sample
Imports System.IO
Imports Oracle.ManagedDataAccess.Client
Imports Oracle.ManagedDataAccess.Types

Class ViewProductsFrm
    Inherits Form

    'For database connection
    Dim conn As New OracleConnection()

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents HeaderLbl As System.Windows.Forms.Label
    Friend WithEvents CloseBtn As System.Windows.Forms.Button
    Friend WithEvents productsDataGridTableStyle As System.Windows.Forms.DataGridTableStyle
    Friend WithEvents Product_ID As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents Product_Name As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents Product_Desc As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents Category As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents Price As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents productsDataGrid As System.Windows.Forms.DataGrid
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.productsDataGrid = New System.Windows.Forms.DataGrid()
        Me.productsDataGridTableStyle = New System.Windows.Forms.DataGridTableStyle()
        Me.Product_ID = New System.Windows.Forms.DataGridTextBoxColumn()
        Me.Product_Name = New System.Windows.Forms.DataGridTextBoxColumn()
        Me.Product_Desc = New System.Windows.Forms.DataGridTextBoxColumn()
        Me.Category = New System.Windows.Forms.DataGridTextBoxColumn()
        Me.Price = New System.Windows.Forms.DataGridTextBoxColumn()
        Me.HeaderLbl = New System.Windows.Forms.Label()
        Me.CloseBtn = New System.Windows.Forms.Button()
        CType(Me.productsDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'productsDataGrid
        '
        Me.productsDataGrid.CaptionFont = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.productsDataGrid.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.productsDataGrid.CaptionText = "List of Products"
        Me.productsDataGrid.DataMember = ""
        Me.productsDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText
        Me.productsDataGrid.Location = New System.Drawing.Point(48, 88)
        Me.productsDataGrid.Name = "productsDataGrid"
        Me.productsDataGrid.ReadOnly = True
        Me.productsDataGrid.Size = New System.Drawing.Size(528, 232)
        Me.productsDataGrid.TabIndex = 6
        Me.productsDataGrid.TableStyles.AddRange(New System.Windows.Forms.DataGridTableStyle() {Me.productsDataGridTableStyle})
        '
        'productsDataGridTableStyle
        '
        Me.productsDataGridTableStyle.DataGrid = Me.productsDataGrid
        Me.productsDataGridTableStyle.GridColumnStyles.AddRange(New System.Windows.Forms.DataGridColumnStyle() {Me.Product_ID, Me.Product_Name, Me.Product_Desc, Me.Category, Me.Price})
        Me.productsDataGridTableStyle.HeaderFont = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.productsDataGridTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText
        Me.productsDataGridTableStyle.MappingName = "Products"
        '
        'Product_ID
        '
        Me.Product_ID.Format = ""
        Me.Product_ID.FormatInfo = Nothing
        Me.Product_ID.HeaderText = "ID"
        Me.Product_ID.MappingName = "Product_ID"
        Me.Product_ID.Width = 50
        '
        'Product_Name
        '
        Me.Product_Name.Format = ""
        Me.Product_Name.FormatInfo = Nothing
        Me.Product_Name.HeaderText = "Name"
        Me.Product_Name.MappingName = "Product_Name"
        Me.Product_Name.Width = 120
        '
        'Product_Desc
        '
        Me.Product_Desc.Format = ""
        Me.Product_Desc.FormatInfo = Nothing
        Me.Product_Desc.HeaderText = "Description"
        Me.Product_Desc.MappingName = "Product_Desc"
        Me.Product_Desc.Width = 180
        '
        'Category
        '
        Me.Category.Format = ""
        Me.Category.FormatInfo = Nothing
        Me.Category.HeaderText = "Category"
        Me.Category.MappingName = "Category"
        Me.Category.Width = 75
        '
        'Price
        '
        Me.Price.Format = "0.00"
        Me.Price.FormatInfo = Nothing
        Me.Price.HeaderText = "Price $"
        Me.Price.MappingName = "Price"
        Me.Price.Width = 64
        '
        'HeaderLbl
        '
        Me.HeaderLbl.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.HeaderLbl.Font = New System.Drawing.Font("Verdana", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HeaderLbl.ForeColor = System.Drawing.SystemColors.ControlText
        Me.HeaderLbl.Location = New System.Drawing.Point(168, 24)
        Me.HeaderLbl.Name = "HeaderLbl"
        Me.HeaderLbl.Size = New System.Drawing.Size(280, 24)
        Me.HeaderLbl.TabIndex = 5
        Me.HeaderLbl.Text = "Favorite Stores"
        Me.HeaderLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CloseBtn
        '
        Me.CloseBtn.BackColor = System.Drawing.SystemColors.Control
        Me.CloseBtn.Font = New System.Drawing.Font("Verdana", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CloseBtn.ForeColor = System.Drawing.Color.Black
        Me.CloseBtn.Location = New System.Drawing.Point(272, 344)
        Me.CloseBtn.Name = "CloseBtn"
        Me.CloseBtn.Size = New System.Drawing.Size(64, 24)
        Me.CloseBtn.TabIndex = 4
        Me.CloseBtn.Text = "Close"
        '
        'ViewProductsFrm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(616, 389)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.productsDataGrid, Me.HeaderLbl, Me.CloseBtn})
        Me.MaximizeBox = False
        Me.Name = "ViewProductsFrm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "View Products"
        CType(Me.productsDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

    ' *****************************************************************************
    ' This method is called on the load event of "ViewProductsFrm" form.
    ' The purpose of this method is to populate "productsDataGrid".
    ' The flow of fetching data into the DataGrid is as follows:
    ' 1. Get the database connection using the parameters given in 
    '    "ConnectionParams.vb" class.
    ' 2. The "SelectCommand" property of OracleDataAdapter facilitates loading of
    '    data based on the SQL query given to access "Products" data.
    ' 3. "OracleDataAdapter.Fill" method loads the data from data source to the DataSet.
    ' 4. "SetDataBinding" method of the DataGrid sets the DataSource and the 
    '    database table to which the DataGrid is bound.
    '********************************************************************************
    Private Sub ViewProductsFrm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'redirect to error handler
        On Error GoTo ErrorText

        'Step 1.    
        'set connection parameters in connectionParams.vb module
        ConnectionParams.setparams()

        'set provider and connection parameters for database connection
        'get the connection parameters from connectionParams.vb file
        Dim connectionString As String = "Data Source=" + ConnectionParams.datasource & _
                                         ";User ID=" + ConnectionParams.username & _
                                         ";Password=" + ConnectionParams.password


        'Connection to datasource, using connection parameters given above
        conn = New OracleConnection(connectionString)

        'Open database connection
        conn.Open()

        'Step 2.
        'Instantiate OracleDataAdapter to create DataSet
        Dim productsAdapter As OracleDataAdapter = New OracleDataAdapter()

        'Fetch Product Details
        productsAdapter.SelectCommand = New OracleCommand("SELECT Product_id, product_name, " & _
                                          " product_desc, category, price FROM products", conn)

        'In-Memory cache of data
        Dim productsDataSet As DataSet = New DataSet("productsDataSet")

        'Step 3.
        'Fill the DataSet 
        productsAdapter.Fill(productsDataSet, "Products")

        'Step 4
        'setting 'productsDataSet' as datasouce and 'Products' table
        'as the table to which the 'productsDatagrid' is Bound.
        productsDataGrid.SetDataBinding(productsDataSet, "Products")

        If (Not Err.Number = 0) Then
ErrorText:  'error handler
            MsgBox("Source :" + Err.Source + " Description :" + Err.Description, , "Error")
            End
        End If


    End Sub

    '**********************************************************************
    ' This method is called on the click event of the 'Close' button.
    ' The purpose of this method is to close the database connection and
    ' then exit out of the application.
    '*********************************************************************
    Private Sub CloseBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CloseBtn.Click

        On Error GoTo ErrorText 'redirect to error handler

        conn.Close()
        Application.Exit()

        If (Not Err.Number = 0) Then
ErrorText:  'error handler
            MsgBox("Source :" + Err.Source + " Description :" + Err.Description, , "Error")
            End
        End If
    End Sub
End Class
