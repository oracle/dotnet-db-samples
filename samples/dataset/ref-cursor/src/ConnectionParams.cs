/* Copyright (c) 2017, Oracle and/or its affiliates. All rights reserved. */

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

/**************************************************************************
@author  Jagriti
@version 1.0
Development Environment        :  MS Visual Studio .Net
Name of the File               :  ConnectionParams.cs
Creation/Modification History  :
                                  24-July-2002     Created

Overview
This file defines the variables for connection parameters for database.
**************************************************************************/

using System;
namespace DSwithRefCur
{
	public class ConnectionParams
	{
		//Parameters for database connection
		//Change the values to those applicable to your database

		//Connect String as TNSNames
		public static string Datasource="oracle"; 
		public static string Username="oranet";      //Username
		public static string Password="oranet";      //Password
	}
}
		