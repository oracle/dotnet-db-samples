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

using System;
namespace ManipulateProducts
{

	public class ConnectionParams
	{
		//Parameters for database connection
		//Change the values to those applicable to your database
		public static string Datasource="oracle"; //Connect String as TNSNames
		public static string Username="scott";      //Username
		public static string Password="tiger";      //Password
	}
}
		