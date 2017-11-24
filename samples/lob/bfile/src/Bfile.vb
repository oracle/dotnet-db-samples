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

'*************************************************************************

' @author                        :  Jagriti
' @version                       :  1.0
' Development Environment        :  Microsoft Visual Studio.NET
' Name of the File               :  Bfile.vb
' Creation/Modification History  :
'                        4-Apr-2003     Created 
'
' Sample Overview: 
' The purpose of this sample is to demonstrate how BFILEs can be accessed
' through Oracle Data Provider for .NET (ODP.NET). This file contains
' methods that are used to insert and retrieve BFILE.
'***************************************************************************

' Standard Namespaces referenced in this sample
Imports System.Data
Imports System.IO
Imports System.Text

' ODP.NET specific Namespaces referenced in this sample
Imports Oracle.ManagedDataAccess.Client
Imports Oracle.ManagedDataAccess.Types

Public Class BFileFrm
    Inherits System.Windows.Forms.Form

    ' Connection Object
    Dim conn As New OracleConnection()
    Dim connectionStatus As Boolean

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

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
    Friend WithEvents UserName As System.Windows.Forms.TextBox
    Friend WithEvents Password As System.Windows.Forms.TextBox
    Friend WithEvents DataSource As System.Windows.Forms.TextBox
    Friend WithEvents ButtonConnect As System.Windows.Forms.Button
    Friend WithEvents SaveButton As System.Windows.Forms.Button
    Friend WithEvents DisplayButton As System.Windows.Forms.Button
    Friend WithEvents DirPath As System.Windows.Forms.TextBox
    Friend WithEvents PicBx As System.Windows.Forms.PictureBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents MsgTxt As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label

    Friend WithEvents PasswordLabel As System.Windows.Forms.Label

    Friend WithEvents UserNameLabel As System.Windows.Forms.Label
    Friend WithEvents Connection As System.Windows.Forms.GroupBox
    Friend WithEvents Status As System.Windows.Forms.Label
    Friend WithEvents StatusLabel As System.Windows.Forms.Label
    Friend WithEvents ButtonExit As System.Windows.Forms.Button


    Friend WithEvents ConnectStringLabel As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Disconnect As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.UserName = New System.Windows.Forms.TextBox()
        Me.DirPath = New System.Windows.Forms.TextBox()
        Me.SaveButton = New System.Windows.Forms.Button()
        Me.PicBx = New System.Windows.Forms.PictureBox()
        Me.DisplayButton = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.MsgTxt = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Password = New System.Windows.Forms.TextBox()
        Me.PasswordLabel = New System.Windows.Forms.Label()
        Me.UserNameLabel = New System.Windows.Forms.Label()
        Me.Connection = New System.Windows.Forms.GroupBox()
        Me.Status = New System.Windows.Forms.Label()
        Me.StatusLabel = New System.Windows.Forms.Label()
        Me.ButtonExit = New System.Windows.Forms.Button()
        Me.ButtonConnect = New System.Windows.Forms.Button()
        Me.ConnectStringLabel = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Disconnect = New System.Windows.Forms.Button()
        Me.DataSource = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Connection.SuspendLayout()
        Me.SuspendLayout()
        '
        'UserName
        '
        Me.UserName.Location = New System.Drawing.Point(96, 72)
        Me.UserName.Name = "UserName"
        Me.UserName.Size = New System.Drawing.Size(88, 20)
        Me.UserName.TabIndex = 0
        Me.UserName.Text = ""
        '
        'DirPath
        '
        Me.DirPath.Enabled = False
        Me.DirPath.Location = New System.Drawing.Point(32, 184)
        Me.DirPath.Name = "DirPath"
        Me.DirPath.Size = New System.Drawing.Size(248, 20)
        Me.DirPath.TabIndex = 6
        Me.DirPath.Text = ""
        '
        'SaveButton
        '
        Me.SaveButton.Enabled = False
        Me.SaveButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SaveButton.Location = New System.Drawing.Point(112, 264)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(104, 24)
        Me.SaveButton.TabIndex = 5
        Me.SaveButton.Text = "Save Image"
        '
        'PicBx
        '
        Me.PicBx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicBx.Location = New System.Drawing.Point(320, 184)
        Me.PicBx.Name = "PicBx"
        Me.PicBx.Size = New System.Drawing.Size(232, 176)
        Me.PicBx.TabIndex = 7
        Me.PicBx.TabStop = False
        '
        'DisplayButton
        '
        Me.DisplayButton.Enabled = False
        Me.DisplayButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DisplayButton.Location = New System.Drawing.Point(112, 304)
        Me.DisplayButton.Name = "DisplayButton"
        Me.DisplayButton.Size = New System.Drawing.Size(104, 24)
        Me.DisplayButton.TabIndex = 6
        Me.DisplayButton.Text = "Display Image"
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(176, 8)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(256, 24)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Accessing BFile Sample"
        '
        'MsgTxt
        '
        Me.MsgTxt.BackColor = System.Drawing.Color.White
        Me.MsgTxt.Enabled = False
        Me.MsgTxt.Location = New System.Drawing.Point(8, 376)
        Me.MsgTxt.Multiline = True
        Me.MsgTxt.Name = "MsgTxt"
        Me.MsgTxt.ReadOnly = True
        Me.MsgTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.MsgTxt.Size = New System.Drawing.Size(544, 144)
        Me.MsgTxt.TabIndex = 14
        Me.MsgTxt.Text = ""
        '
        'Label6
        '
        Me.Label6.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(8, 160)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(384, 16)
        Me.Label6.TabIndex = 15
        Me.Label6.Text = "Enter directory path on database server where image file is stored"
        '
        'Password
        '
        Me.Password.Location = New System.Drawing.Point(264, 72)
        Me.Password.Name = "Password"
        Me.Password.PasswordChar = Microsoft.VisualBasic.ChrW(42)
        Me.Password.Size = New System.Drawing.Size(80, 20)
        Me.Password.TabIndex = 1
        Me.Password.Text = ""
        '
        'PasswordLabel
        '
        Me.PasswordLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.25!)
        Me.PasswordLabel.Location = New System.Drawing.Point(192, 72)
        Me.PasswordLabel.Name = "PasswordLabel"
        Me.PasswordLabel.Size = New System.Drawing.Size(72, 24)
        Me.PasswordLabel.TabIndex = 18
        Me.PasswordLabel.Text = "Password"
        '
        'UserNameLabel
        '
        Me.UserNameLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.25!)
        Me.UserNameLabel.Location = New System.Drawing.Point(16, 72)
        Me.UserNameLabel.Name = "UserNameLabel"
        Me.UserNameLabel.Size = New System.Drawing.Size(80, 16)
        Me.UserNameLabel.TabIndex = 16
        Me.UserNameLabel.Text = "User Name"
        '
        'Connection
        '
        Me.Connection.Controls.AddRange(New System.Windows.Forms.Control() {Me.Status, Me.StatusLabel, Me.ButtonExit, Me.ButtonConnect, Me.ConnectStringLabel, Me.Label3, Me.Disconnect, Me.DataSource})
        Me.Connection.Location = New System.Drawing.Point(8, 56)
        Me.Connection.Name = "Connection"
        Me.Connection.Size = New System.Drawing.Size(544, 80)
        Me.Connection.TabIndex = 20
        Me.Connection.TabStop = False
        Me.Connection.Text = "Connection Details"
        '
        'Status
        '
        Me.Status.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Status.ForeColor = System.Drawing.Color.Red
        Me.Status.ImageAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Status.Location = New System.Drawing.Point(80, 56)
        Me.Status.Name = "Status"
        Me.Status.Size = New System.Drawing.Size(120, 16)
        Me.Status.TabIndex = 5
        Me.Status.Text = "Not Connected"
        '
        'StatusLabel
        '
        Me.StatusLabel.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StatusLabel.Location = New System.Drawing.Point(16, 56)
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(48, 16)
        Me.StatusLabel.TabIndex = 4
        Me.StatusLabel.Text = "Status: "
        '
        'ButtonExit
        '
        Me.ButtonExit.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonExit.Location = New System.Drawing.Point(496, 48)
        Me.ButtonExit.Name = "ButtonExit"
        Me.ButtonExit.Size = New System.Drawing.Size(40, 24)
        Me.ButtonExit.TabIndex = 4
        Me.ButtonExit.Text = " Exit"
        '
        'ButtonConnect
        '
        Me.ButtonConnect.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonConnect.Location = New System.Drawing.Point(328, 48)
        Me.ButtonConnect.Name = "ButtonConnect"
        Me.ButtonConnect.Size = New System.Drawing.Size(64, 24)
        Me.ButtonConnect.TabIndex = 3
        Me.ButtonConnect.Text = "Connect "
        '
        'ConnectStringLabel
        '
        Me.ConnectStringLabel.Font = New System.Drawing.Font("Arial", 10.25!)
        Me.ConnectStringLabel.Location = New System.Drawing.Point(344, 16)
        Me.ConnectStringLabel.Name = "ConnectStringLabel"
        Me.ConnectStringLabel.Size = New System.Drawing.Size(104, 24)
        Me.ConnectStringLabel.TabIndex = 0
        Me.ConnectStringLabel.Text = "Data Source"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(304, 56)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(16, 16)
        Me.Label3.TabIndex = 23
        Me.Label3.Text = "1."
        '
        'Disconnect
        '
        Me.Disconnect.Enabled = False
        Me.Disconnect.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Disconnect.Location = New System.Drawing.Point(408, 48)
        Me.Disconnect.Name = "Disconnect"
        Me.Disconnect.Size = New System.Drawing.Size(72, 24)
        Me.Disconnect.TabIndex = 24
        Me.Disconnect.Text = "Disconnect"
        '
        'DataSource
        '
        Me.DataSource.AcceptsTab = True
        Me.DataSource.Location = New System.Drawing.Point(448, 16)
        Me.DataSource.Name = "DataSource"
        Me.DataSource.Size = New System.Drawing.Size(88, 20)
        Me.DataSource.TabIndex = 2
        Me.DataSource.Text = ""
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(8, 352)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(128, 16)
        Me.Label1.TabIndex = 21
        Me.Label1.Text = "Status Bar"
        '
        'Label7
        '
        Me.Label7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(8, 184)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(16, 16)
        Me.Label7.TabIndex = 25
        Me.Label7.Text = "2."
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(80, 272)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(16, 16)
        Me.Label2.TabIndex = 26
        Me.Label2.Text = "3."
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(80, 312)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(16, 16)
        Me.Label5.TabIndex = 27
        Me.Label5.Text = "4."
        '
        'BFileFrm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(560, 533)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.Label5, Me.Label2, Me.Label7, Me.Label1, Me.Password, Me.PasswordLabel, Me.UserName, Me.UserNameLabel, Me.Connection, Me.Label6, Me.MsgTxt, Me.Label4, Me.DisplayButton, Me.PicBx, Me.SaveButton, Me.DirPath})
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "BFileFrm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Accessing BFILE Sample"
        Me.Connection.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    '*****************************************************************************************
    ' On clicking the "Connect" button this method is called.
    ' The purpose of this method is to establish connection to database.
    ' The user enters the connection parameters i.e. Username, Password, Datasource in the 
    ' text boxes provided, appropriate to his/her database. On successful database connection,
    ' the connection status is displayed as "connected" also the "Save Image",  
    ' "Display Image" buttons are enabled.
    '*****************************************************************************************
    Private Sub ButtonConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonConnect.Click

        Dim tempStr As String = UserName.Text
        Dim flag As Boolean = True

        'Check if Username width is more than 0 characters
        If (tempStr.Length = 0) Then
            flag = False
            MessageBox.Show("Username cannot be a null String", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        tempStr = Password.Text
        'Check if Password width is more than 0 characters
        If (tempStr.Length = 0) Then
            flag = False
            MessageBox.Show("Password cannot be a null String", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        tempStr = DataSource.Text
        'Check if dataSource width is more than 0 characters
        If (tempStr.Length = 0) Then
            flag = False
            MessageBox.Show("DataSource cannot be a null String", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If (flag = True) Then
            ' Trying to connect to the database 
            connectionStatus = GetDBConnection()
        End If

        'Check if the program is connected to the database and enable user 
        'controls.
        If (connectionStatus = True) Then
            Status.Text = "Connected"
            Status.ForeColor = Color.Green
            Status.Update()
            DisplayButton.Enabled = True
            SaveButton.Enabled = True
            ButtonConnect.Enabled = False
            DirPath.Enabled = True

            ' Disable connect button
            ButtonConnect.Enabled = False
            Disconnect.Enabled = True
        Else
            'Not connected to database, enable connect button
            ButtonConnect.Enabled = True
            DisplayButton.Enabled = False
            SaveButton.Enabled = False
            DirPath.Enabled = False
        End If
    End Sub


    '*******************************************************************************************
    ' The purpose of this method is to create the required database objects used in this sample.
    ' Then insert BFILE data in the database table.
    '*******************************************************************************************
    Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveButton.Click
        ' Check if directory path is entered by user
        If DirPath.Text = Nothing Then
            MsgBox("Enter value for directory path where image file exists !")
        Else
            ' Call method for database setup
            Setup()

            ' Call method to insert BFILE data
            InsertData()
        End If
    End Sub


    '********************************************************************************************
    ' The purpose of this method is to create the database objects like databse table, directory.
    ' NOTE: Ensure that prior to running this code you have connected as a database user with
    '       "CREATE ANY DIRECTORY " privilege. For more information go through Readme.html file
    '       provided with this sample.
    '*********************************************************************************************
    Private Sub Setup()

        Dim cmd As OracleCommand = New OracleCommand()
        cmd.Connection = conn

        ' Get the image path entered in the "DirPath" textbox
        Dim Path As String = DirPath.Text

        ' Drop Test_Bfile table if it already exists
        MsgTxt.AppendText("Creating Test_Bfile table..." + vbNewLine)
        MsgTxt.Update()
        Try
            cmd.CommandText = "DROP TABLE Test_Bfile"
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            'MsgTxt.AppendText(ex.Message + vbNewLine)
            'MsgTxt.Update()
        End Try

        ' Create a table Test_Bfile with a BFILE column
        Try
            cmd.CommandText = "CREATE TABLE Test_Bfile ( photo BFILE)"
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox("error: " + ex.Message)
        End Try

        ' Create database object "Directory"
        MsgTxt.AppendText("Creating BFILEDIR directory..." + vbNewLine)
        MsgTxt.Update()
        Try
            cmd.CommandText = "CREATE OR REPLACE DIRECTORY BFILEDIR AS '" + Path + "' "
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox("error: " + ex.Message)
        End Try
    End Sub

    '************************************************************************************
    ' This method inserts a sample image "poster.gif" provided with this sample.
    ' NOTE: Ensure that you have copied this file to a directory on your database server.
    '************************************************************************************
    Private Sub InsertData()

        Dim cmd As OracleCommand = New OracleCommand()
        Try
            MsgTxt.AppendText("Inserting image BFILE pointer to Test_Bfile table..." + vbNewLine)
            MsgTxt.Update()

            cmd.Connection = conn
            cmd.CommandText = "INSERT INTO Test_Bfile VALUES ( BFILENAME('BFILEDIR', 'poster.gif'))"
            Dim x As Boolean = cmd.ExecuteNonQuery()

            MsgTxt.AppendText("Image pointer has been inserted successfully to Test_Bfile table." + vbNewLine)
            MsgTxt.Update()


        Catch ex As Exception
            MsgBox("error: " + ex.Message)
        End Try
    End Sub


    '***************************************************************************************
    ' This method is executed on clicking the button with "Display Image" label.
    ' The purpose of this method is to select the BFILE locator from Test_Bfile Table 
    ' and store it in OracleBFile. Then open memory stream on the selected OracleBFile.
    ' Fetch image data and display it in picture box.
    '***************************************************************************************
    Private Sub DisplayButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DisplayButton.Click

        Dim PicCmd As OracleCommand
        Dim PicReader As OracleDataReader
        Dim pic As OracleBFile

        Try
            ' Select Bfile column
            PicCmd = New OracleCommand("SELECT photo FROM Test_Bfile", conn)
            PicReader = PicCmd.ExecuteReader()

            Dim imgExist = PicReader.Read()

            ' Check if image exist
            If (imgExist) Then
                Try
                    Dim bytePicData As Byte()

                    MsgTxt.AppendText("Displaying image, please wait ...." + vbNewLine)
                    MsgTxt.Update()

                    ' Store the BFILE locator to OracleBFile
                    pic = PicReader.GetOracleBFile(0)

                    pic.OpenFile()

                    ReDim bytePicData(pic.Length)
                    ' Check the image size
                    If (pic.Length = 0) Then
                        MsgBox("The image size is zero bytes. Unable display the image.")
                        Return
                    End If

                    Dim i As Int64 = pic.Read(bytePicData, 0, System.Convert.ToInt32(pic.Length))

                    'Get the primitive byte data into in-memory data stream
                    Dim stmPicData As MemoryStream = New MemoryStream(bytePicData)

                    ' Display image fetched using memory stream to a picture box
                    PicBx.Image = Image.FromStream(stmPicData)

                    'Fit the image to the picture box size
                    PicBx.SizeMode = PictureBoxSizeMode.StretchImage

                    MsgTxt.AppendText("Image displayed ..." + vbNewLine)
                    MsgTxt.Update()

                Catch ex1 As Exception
                    MsgBox("Error: " + ex1.Message)
                Finally
                    'Close OracleBFile
                    pic.Close()
                End Try
            End If
        Catch ex2 As Exception
            MsgBox("Error: " + ex2.Message)
        Finally
            'Close OracleDataReader
            PicReader.Close()
        End Try
    End Sub


    '*****************************************************************************
    ' The purpose of this method is to establish the database connection. 
    ' Using the database parameters given in the text boxes provided by the user.
    '*****************************************************************************
    Private Function GetDBConnection() As Boolean
        Try
            ' set provider and connection parameters for database connection
            ' get the connection parameters from the Text boxes 
            Dim connectionString As String = "Data Source=" + DataSource.Text & _
                                             ";User ID=" + UserName.Text & _
                                             ";Password=" + Password.Text

            ' Connection to datasource, using connection parameters given above
            conn.ConnectionString = connectionString

            ' open the database connection
            conn.Open()
            Return True
        Catch ex As Exception
            MsgBox("error: " + ex.Message)
        End Try
    End Function

    '*****************************************************************************
    ' The purpose of this method is to disconnect the database connection. 
    '*****************************************************************************
    Private Sub Disconnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Disconnect.Click

        Try
            If (connectionStatus = True) Then
                MsgTxt.AppendText("Disconnecting...." + vbNewLine)
                MsgTxt.Update()

                'Close database connection
                conn.Close()

                'Update status text and color 
                Status.Text = "Disconnected"
                Status.ForeColor = Color.Red
                Status.Update()
                ButtonConnect.Enabled = True
                Disconnect.Enabled = False
                DisplayButton.Enabled = False
                SaveButton.Enabled = False
                DirPath.Enabled = False
            End If
        Catch ex As Exception
            MsgBox("error: " + ex.Message)
        End Try
    End Sub

    '********************************************************************************
    ' The purpose of this method is to release resources held by this application and 
    ' close the application window.
    '********************************************************************************
    Private Sub ButtonExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonExit.Click
        'Update status text and color 
        Status.Text = "Exiting, Please wait"
        Status.Update()
        System.Threading.Thread.CurrentThread.Sleep(1500)
        conn.Close()
        'Exit the application
        Application.Exit()
    End Sub

    '*****************************************************************************
    ' This method fires when the close window button of the Bfile form is clicked.
    '*****************************************************************************
    Private Sub AccessBfile_Close(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        'Update status text and color 
        Status.Text = "Exiting, Please wait"
        Status.Update()

        MsgTxt.AppendText("Disconnecting...." + vbNewLine)
        MsgTxt.Update()
        conn.Dispose()

        System.Threading.Thread.CurrentThread.Sleep(1500)
        'Exit the application
        Application.Exit()

    End Sub
End Class
