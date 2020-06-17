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
'@author  Jagriti
'@version 1.0
'Development Environment        :  MS Visual Studio .Net
'Name of the File               :  ConnectionParams.vb
'Creation/Modification History  :
'                                  24-July-2002     Created

'Overview:
'This file defines the variables for connection parameters for database.
'**************************************************************************

Module ConnectionParams

    Public datasource As String
    Public username As String
    Public password As String

    Public Sub setparams()
        'Parameters for database connection
        'Change the values to those applicable to your database

        'Replace with Connect String as TNSNames
        datasource = "oracle"
        'Username
        username = "scott"
        'Password
        password = "<PASSWORD>"
    End Sub

End Module
