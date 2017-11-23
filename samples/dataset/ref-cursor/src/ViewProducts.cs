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

/********************************************************************************************

* @author                        :  Jagriti
* @version                       :  1.0
* Development Environment        :  Microsoft Visual Studio .Net
* Name of the File               :  ViewProducts.cs
* Creation/Modification History  :
*                         24-July-2002     Created 
*
* Sample Overview: 
* The purpose of this .NET sample application is to demonstrate how to populate a DataSet with data
* from multiple Ref Cursors which are returned as OUT parameters from a database Stored Procedure
* through Oracle Data Provider for .Net (ODP.Net) connection. The parameters of 
* 'OracleDbType.RefCursor' type are bound to the OracleCommand object. The Command type is set to
* 'Stored Procedure' for OracleCommand. The OracleDataAdapter fills the DataSet with the Ref Cursors
* returned as OUT parameter from the database stored procedure. The data from each Ref Cursor 
* is stored in a Data Table contained in the Data Set. The Data Tables are named as 'TableN' 
* where N stands for integers starting with 0. Different Data Grids can be bound to different 
* Data Tables.
*
* When this sample is run two Data Grids are displayed. One Data Grid populated with records
* for products with 'Orderable' status and other Data Grid with records for products with
* 'Under Development' status. The data is fetched from database stored procedure 
* 'getProductsInfo' from 'ODPNet' package. The 'productAdapter' executes 'productCmd'.
* Command type for 'productCmd'. The two Data Tables created are 'Products' and 'Products1'.
* The Data Grids are bound to these Data Tables. 
***********************************************************************************************/

//Standard Namespaces referenced in this sample application
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace DSwithRefCur
{
	public class ViewProducts : System.Windows.Forms.Form
	{
		//UI Components 
		private Container components = null;
		private Button closeBtn;
		private Label headerLbl;

		//Data Grid component for Orderable Product Status
        private DataGrid orderableDataGrid;

		//Data Grid Table style and its corresponding columns for Orderable Product Status
		private DataGridTableStyle orderableDataGridTableStyle;
		private DataGridTextBoxColumn ProductID;
		private DataGridTextBoxColumn ProductName;
		private DataGridTextBoxColumn ProductDesc;
		private DataGridTextBoxColumn Category;
		private DataGridTextBoxColumn Price;

		//DataGrid component for Under Development Product Status
		private DataGrid udevelopmentDataGrid;

		//Data Grid Table style and its corresponding columns for Under Developement Product Status
		private DataGridTableStyle udevelopmentDataGridTableStyle;
		private DataGridTextBoxColumn ProductID1;
		private DataGridTextBoxColumn ProductName1;
		private DataGridTextBoxColumn ProductDesc1;
		private DataGridTextBoxColumn Category1;
		private DataGridTextBoxColumn Price1;
				
		//For database connection 
		OracleConnection conn;
		

		/*******************************************************************
		* This method is the entry point for this sample application.
		* It also calls 'populateProducts' method that fills the Data Grids
		*******************************************************************/
		static void Main() 
		{
			//Instantiating this class
	    	ViewProducts viewproducts = new ViewProducts();

			//Get database connection
			if (viewproducts.getDBConnection())
			{
               
				//Calls populateProducts method that fetches data from
     			//database and fills the Data Grids
				viewproducts.populateProducts();

				//Start the application and display the View Products form
				Application.Run(viewproducts);
			}
		}

		//Constructor
		public ViewProducts()
		{
			// Required for Windows Form Designer support
			InitializeComponent();
		}


		/********************************************************************************
		* The purpose of this method is to populate the Data Grids with data from
		* Ref Cursors returned as OUT parameters from 'ODPNet.getProductsInfo'
		* database Stored Procedure.
		* Following is the flow for this method:
        * 1. For 'productCmd' OracleCommand object set the command as
		*    a call to database Stored Procedure 'ODPNet.getProductsInfo'.
		* 2. Set command type as 'Stored Procedure' for 'productCmd'.
		* 3. Bind the Ref Cursor parameters as 'OracleDbType.RefCursor' to the 
		*    OracleCommand object.
		* 4. 'Fill' method for 'productAdapter' OracleDataAdapter fills 'productsDataSet'
		*     with data returned upon executing OracleCommand object .
		* 5. 'ProductDataSet' contains two Data Tables 'Products' and 'Products1'.
		*    'Products' Data Table contains data from 'orderable' Ref Cursor 
		*    parameter. 'Products1' Data Table contains data from 'udevelopment'
		*    Ref Cursor parameter.
		* 6. 'orderableDataGrid' is bound to 'Products' Data Table and
		*    'udevelopmentDataGrid' is bound to 'Products1' Data Table.
		********************************************************************************/
		private void populateProducts()
		{
			//To fill DataSet from datasource
			OracleDataAdapter productAdapter;

			//Represents Stored Procedure to execute against a data source
			OracleCommand productCmd;

			//In-memory cache of data
			DataSet productsDataSet;
			
			try{

                    //Step 1.//
             	    //Instantiate the command object
					productCmd = new OracleCommand();

                    //Call 'getProductsInfo' stored procedure of 'ODPNet' 
				    // database package
				    productCmd.CommandText = "ODPNet.getProductsInfo";


                    //Step 2.//
				    //Set the command Type to Stored Procedure
					productCmd.CommandType = CommandType.StoredProcedure;

				    //Set the connection instance
				    productCmd.Connection = conn;
					

                    //Step 3.//
				    //Bind the Ref Cursor parameters to the OracleCommand object
				    //for 'orderable' product status
				    productCmd.Parameters.Add("orderable",OracleDbType.RefCursor,
					               DBNull.Value,ParameterDirection.Output);

                    //for 'under development' product status
				    productCmd.Parameters.Add("udevelopment",OracleDbType.RefCursor, 
					          DBNull.Value,ParameterDirection.Output);


 				    //Step 4.//
                    //Set the command for the Data Adapter
					productAdapter = new OracleDataAdapter(productCmd);
				
                    //Instantiate Data Set object
					productsDataSet = new DataSet("productsDataSet");


				    //Step 5.//
				    //Fill Data Set with data from Data Adapter
					productAdapter.Fill(productsDataSet,"Products");

 
				    //Step 6.//    
				    //Bind 'orderableDataGrid' with 'Products' Data Table
					orderableDataGrid.SetDataBinding(productsDataSet,"Products");
				    
				    //Bind 'udevelopmentDataGrid' with 'Products1' Data Table
					udevelopmentDataGrid.SetDataBinding(productsDataSet,"Products1");
		
		}
			catch(Exception ex){
				MessageBox.Show(ex.ToString());
        	}
	  }

		/*******************************************************************
		* The purpose of this method is to get the database connection 
		* using the parameters given.
		* Note: Replace the datasource parameter with your datasource value.
		********************************************************************/
		private Boolean getDBConnection()
		{
			try
			{
				//Connection Information	
				string connectionString = 
					
					//Username
					"User Id=" + ConnectionParams.Username +

					//Password
					";Password=" + ConnectionParams.Password +

					//Replace with your datasource value (TNSNames)
					";Data Source=" + ConnectionParams.Datasource ;
					
			
				//Connection to datasource, using connection parameters given above
				conn = new OracleConnection(connectionString);

				//Open database connection
				conn.Open();

				return true;
			}
			catch (Exception ex) // catch exception when error in connecting to database occurs
			{
				//Display error message
				MessageBox.Show(ex.ToString());
				return false;
			}
		}

		/**********************************************************************
		* This method is called on the click event of the 'Close' button.
		* The purpose of this method is to close the form 'ViewProducts' and
		* then exit out of the application.
		**********************************************************************/
		private void closeBtn_Click(object sender, System.EventArgs e)
		{
			conn.Close();
			this.Close();
			Application.Exit();
		}

		/*********************************************************************
		* The purpose of this method is to clean up any resources being used.
		*********************************************************************/
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/***********************************************************************
		* This is a required method for Designer support, its purpose is
		* to intialize the UI controls and their related properties.
		* NOTE: Do not modify the contents of this method with the code editor.
		************************************************************************/	
		private void InitializeComponent()
		{
			this.orderableDataGrid = new System.Windows.Forms.DataGrid();
			this.orderableDataGridTableStyle = new System.Windows.Forms.DataGridTableStyle();
			this.ProductID = new System.Windows.Forms.DataGridTextBoxColumn();
			this.ProductName = new System.Windows.Forms.DataGridTextBoxColumn();
			this.ProductDesc = new System.Windows.Forms.DataGridTextBoxColumn();
			this.Category = new System.Windows.Forms.DataGridTextBoxColumn();
			this.Price = new System.Windows.Forms.DataGridTextBoxColumn();
			this.udevelopmentDataGrid = new System.Windows.Forms.DataGrid();
			this.udevelopmentDataGridTableStyle = new System.Windows.Forms.DataGridTableStyle();
			this.ProductID1 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.ProductName1 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.ProductDesc1 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.Category1 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.Price1 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.headerLbl = new System.Windows.Forms.Label();
			this.closeBtn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.orderableDataGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.udevelopmentDataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// orderableDataGrid
			// 
			this.orderableDataGrid.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.orderableDataGrid.CaptionText = "List of Orderable Products";
			this.orderableDataGrid.DataMember = "";
			this.orderableDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.orderableDataGrid.Location = new System.Drawing.Point(72, 112);
			this.orderableDataGrid.Name = "orderableDataGrid";
			this.orderableDataGrid.Size = new System.Drawing.Size(536, 152);
			this.orderableDataGrid.TabIndex = 0;
			this.orderableDataGrid.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
																										  this.orderableDataGridTableStyle});
			// 
			// orderableDataGridTableStyle
			// 
			this.orderableDataGridTableStyle.DataGrid = this.orderableDataGrid;
			this.orderableDataGridTableStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
																														  this.ProductID,
																														  this.ProductName,
																														  this.ProductDesc,
																														  this.Category,
																														  this.Price});
			this.orderableDataGridTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.orderableDataGridTableStyle.MappingName = "Products";
			this.orderableDataGridTableStyle.ReadOnly = true;
			// 
			// ProductID
			// 
			this.ProductID.Format = "";
			this.ProductID.FormatInfo = null;
			this.ProductID.HeaderText = "ID";
			this.ProductID.MappingName = "Product_ID";
			this.ProductID.Width = 50;
			// 
			// ProductName
			// 
			this.ProductName.Format = "";
			this.ProductName.FormatInfo = null;
			this.ProductName.HeaderText = "Name";
			this.ProductName.MappingName = "Product_Name";
			this.ProductName.Width = 120;
			// 
			// ProductDesc
			// 
			this.ProductDesc.Format = "";
			this.ProductDesc.FormatInfo = null;
			this.ProductDesc.HeaderText = "Description";
			this.ProductDesc.MappingName = "Product_Desc";
			this.ProductDesc.Width = 180;
			// 
			// Category
			// 
			this.Category.Format = "";
			this.Category.FormatInfo = null;
			this.Category.HeaderText = "Category";
			this.Category.MappingName = "Category";
			this.Category.Width = 75;
			// 
			// Price
			// 
			this.Price.Format = "";
			this.Price.FormatInfo = null;
			this.Price.HeaderText = "Price $";
			this.Price.MappingName = "Price";
			this.Price.Width = 56;
			// 
			// udevelopmentDataGrid
			// 
			this.udevelopmentDataGrid.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.udevelopmentDataGrid.CaptionText = "List of Products Under Development";
			this.udevelopmentDataGrid.DataMember = "";
			this.udevelopmentDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.udevelopmentDataGrid.Location = new System.Drawing.Point(72, 312);
			this.udevelopmentDataGrid.Name = "udevelopmentDataGrid";
			this.udevelopmentDataGrid.Size = new System.Drawing.Size(536, 152);
			this.udevelopmentDataGrid.TabIndex = 1;
			this.udevelopmentDataGrid.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
																											 this.udevelopmentDataGridTableStyle});
			// 
			// udevelopmentDataGridTableStyle
			// 
			this.udevelopmentDataGridTableStyle.DataGrid = this.udevelopmentDataGrid;
			this.udevelopmentDataGridTableStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
																															 this.ProductID1,
																															 this.ProductName1,
																															 this.ProductDesc1,
																															 this.Category1,
																															 this.Price1});
			this.udevelopmentDataGridTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.udevelopmentDataGridTableStyle.MappingName = "Products1";
			this.udevelopmentDataGridTableStyle.ReadOnly = true;
			// 
			// ProductID1
			// 
			this.ProductID1.Format = "";
			this.ProductID1.FormatInfo = null;
			this.ProductID1.HeaderText = "ID";
			this.ProductID1.MappingName = "Product_ID";
			this.ProductID1.Width = 51;
			// 
			// ProductName1
			// 
			this.ProductName1.Format = "";
			this.ProductName1.FormatInfo = null;
			this.ProductName1.HeaderText = "Name";
			this.ProductName1.MappingName = "Product_Name";
			this.ProductName1.Width = 125;
			// 
			// ProductDesc1
			// 
			this.ProductDesc1.Format = "";
			this.ProductDesc1.FormatInfo = null;
			this.ProductDesc1.HeaderText = "Description";
			this.ProductDesc1.MappingName = "Product_Desc";
			this.ProductDesc1.Width = 180;
			// 
			// Category1
			// 
			this.Category1.Format = "";
			this.Category1.FormatInfo = null;
			this.Category1.HeaderText = "Category";
			this.Category1.MappingName = "Category";
			this.Category1.Width = 75;
			// 
			// Price1
			// 
			this.Price1.Format = "";
			this.Price1.FormatInfo = null;
			this.Price1.HeaderText = "Price $";
			this.Price1.MappingName = "Price";
			this.Price1.Width = 64;
			// 
			// headerLbl
			// 
			this.headerLbl.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.headerLbl.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.headerLbl.ForeColor = System.Drawing.Color.Black;
			this.headerLbl.Location = new System.Drawing.Point(192, 24);
			this.headerLbl.Name = "headerLbl";
			this.headerLbl.Size = new System.Drawing.Size(280, 24);
			this.headerLbl.TabIndex = 3;
			this.headerLbl.Text = "Favorite Stores";
			this.headerLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// closeBtn
			// 
			this.closeBtn.BackColor = System.Drawing.SystemColors.Control;
			this.closeBtn.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.closeBtn.Location = new System.Drawing.Point(304, 496);
			this.closeBtn.Name = "closeBtn";
			this.closeBtn.Size = new System.Drawing.Size(64, 24);
			this.closeBtn.TabIndex = 6;
			this.closeBtn.Text = "Close";
			this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
			// 
			// ViewProducts
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(672, 541);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.closeBtn,
																		  this.headerLbl,
																		  this.udevelopmentDataGrid,
																		  this.orderableDataGrid});
			this.MaximizeBox = false;
			this.Name = "ViewProducts";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ViewProducts";
			((System.ComponentModel.ISupportInitialize)(this.orderableDataGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.udevelopmentDataGrid)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		
	}
}
