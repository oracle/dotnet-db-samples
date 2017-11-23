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

/**********************************************************************************

* @author                        :  Jagriti
* @version                       :  1.0
* Development Environment        :  Microsoft Visual Studio .Net 
* Name of the File               :  ViewProducts.cs
* Creation/Modification History  :
*                   24-July-2002     Created 
*
* Sample Overview:
* The purpose of this sample application is to demonstrate how to populate a DataSet.
* The connection to database is made using Oracle Data Provider for .Net (ODP .Net).
* The data retrieval is done using DataSet which is an in-memory cache that contains
* data from database filled by a OracleDataAdapter. OracleDataAdapter serves as a bridge
* between the DataSet and the data source, retrieving data, it includes 'SelectCommand'
* that facilitates the loading of data based on the SQL query given. The connection 
* to database is made using OracleConnection object. 
*
* The scenario for this application is to display a list of products from database.
* The fetched data is displayed in a DataGrid. A 'Close' button is provided to 
* exit from the application.
**********************************************************************************/

//Standard Namespaces referenced in this sample application
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace DSPopulate
{
	//ViewProducts class inherits Window's Form 
	public class ViewProducts : Form
	{
		//UI Components
		Label headerLbl;
		Button closeBtn;
		Container components = null;

		//DataGrid and DataGrid Table Style
		DataGrid productsDataGrid;
		DataGridTableStyle productsDataGridTableStyle;

		//Columns in the DataGrid
		DataGridTextBoxColumn Product_ID;
		DataGridTextBoxColumn Product_Name;
		DataGridTextBoxColumn Product_Desc;
		DataGridTextBoxColumn Category;
		DataGridTextBoxColumn Price;
		
		//For database connection 
		OracleConnection conn;

		//To fill DataSet and update datasource
		OracleDataAdapter productsAdapter;
				
		//In-Memory cache of data
		DataSet productsDataSet;
		
		
		//Constructor
		public ViewProducts()
		{
			// Required for Windows Form Designer support
			InitializeComponent();
     	}

		/**************************************************************
		* This method is the entry point to this sample application. 
		* It also displays Products data in a tabular format.
		***************************************************************/
		public static void Main() 
		{   
			//Instantiating this class
			ViewProducts viewproducts = new ViewProducts();
            
			//Get database connection
			if (viewproducts.getDBConnection())
			{
				//Calling 'displayProducts' method to populate DataGrid from database
				viewproducts.displayProducts();

				//When this application is run, View Products Form is displayed
				Application.Run(viewproducts);
			}
		}
		
		/**********************************************************************
		* The pupose of this method is to populate 'productsDataGrid'.
		* The flow of fetching data into the DataGrid is as follows:
		* 1. The 'SelectCommand' property of OracleDataAdapter facilitates 
        *    loading of data based on the query given, to access 'Products' data.
		* 2. 'OracleDataAdapter.Fill' command loads the data from data source to the 
		*    DataSet.
		* 3. 'SetDataBinding' method of the DataGrid sets the DataSource and the 
		*    database table to which the DataGrid is bound.
		************************************************************************/
		public void displayProducts()
		{
			try
			{
		 		//Instantiate OracleDataAdapter to create DataSet
				productsAdapter = new OracleDataAdapter();
                
				//Fetch Product Details
                productsAdapter.SelectCommand = new OracleCommand("SELECT " +
					                                     "Product_ID , " +
					                                   "Product_Name , " +
					                                   "Product_Desc , " +
				                                        	"Category, " +
					                                            "Price " +
					                                              "FROM Products",conn); 
  				
				//Instantiate DataSet object
			    productsDataSet = new DataSet("productsDataSet");

				//Fill the DataSet with data from 'Products' database table
				productsAdapter.Fill(productsDataSet, "Products");

				//setting 'productsDataSet' as  the datasouce and 'Products' table
				//as the table to which the 'productsDataGrid' is Bound.
				productsDataGrid.SetDataBinding(productsDataSet,"Products"); 
			}
			catch(Exception ex)
			{
				//Display error message
				MessageBox.Show(ex.ToString());				
			}
		}

		/*******************************************************************
		* The purpose of this method is to get the database connection 
		* using the parameters given.
		* Note: Replace the datasource parameter with your datasource value
		* in ConnectionParams.cs file.
		********************************************************************/
		private Boolean getDBConnection()
		{
			try
			{
				//Connection Information	
				string connectionString = 
					
					//username
					"User Id=" + ConnectionParams.Username +

					//password
					";Password=" + ConnectionParams.Password +

					//replace with your datasource value (TNSnames)
					";Data Source=" + ConnectionParams.Datasource;

					
				//Connection to datasource, using connection parameters given above
				conn = new OracleConnection(connectionString);

				//Open database connection
				conn.Open();
				return true;
			}
			// catch exception when error in connecting to database occurs
			catch (Exception ex) 
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

		/***********************************************************************
		* This is a Windows generated code.
		* The purpose of this method is to clean up any resources being used.
		***********************************************************************/
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

		/********************************************************************************
		* This code is an automatically generated application code.
		* Note:Do not modify the contents of this method with the code editor.
		* The purpose of this method is to instantiate all the User Interface components
		* like Button, Data Grid, Labels etc., set their formatting Properties
		* and display these components in 'ViewProducts' form.
		*********************************************************************************/

		#region Windows Form Designer generated code
       	private void InitializeComponent()
		{
			this.closeBtn = new System.Windows.Forms.Button();
			this.headerLbl = new System.Windows.Forms.Label();
			this.productsDataGrid = new System.Windows.Forms.DataGrid();
			this.productsDataGridTableStyle = new System.Windows.Forms.DataGridTableStyle();
			this.Product_ID = new System.Windows.Forms.DataGridTextBoxColumn();
			this.Product_Name = new System.Windows.Forms.DataGridTextBoxColumn();
			this.Product_Desc = new System.Windows.Forms.DataGridTextBoxColumn();
			this.Category = new System.Windows.Forms.DataGridTextBoxColumn();
			this.Price = new System.Windows.Forms.DataGridTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.productsDataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// closeBtn
			// 
			this.closeBtn.BackColor = System.Drawing.SystemColors.Control;
			this.closeBtn.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.closeBtn.ForeColor = System.Drawing.Color.Black;
			this.closeBtn.Location = new System.Drawing.Point(264, 320);
			this.closeBtn.Name = "closeBtn";
			this.closeBtn.Size = new System.Drawing.Size(64, 24);
			this.closeBtn.TabIndex = 1;
			this.closeBtn.Text = "Close";
			this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
			// 
			// headerLbl
			// 
			this.headerLbl.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.headerLbl.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.headerLbl.ForeColor = System.Drawing.SystemColors.ControlText;
			this.headerLbl.Location = new System.Drawing.Point(168, 16);
			this.headerLbl.Name = "headerLbl";
			this.headerLbl.Size = new System.Drawing.Size(280, 24);
			this.headerLbl.TabIndex = 2;
			this.headerLbl.Text = "Favorite Stores";
			this.headerLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// productsDataGrid
			// 
			this.productsDataGrid.CaptionFont = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.productsDataGrid.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.productsDataGrid.CaptionText = "List of Products";
			this.productsDataGrid.DataMember = "";
			this.productsDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.productsDataGrid.Location = new System.Drawing.Point(40, 64);
			this.productsDataGrid.Name = "productsDataGrid";
			this.productsDataGrid.ReadOnly = true;
			this.productsDataGrid.Size = new System.Drawing.Size(528, 240);
			this.productsDataGrid.TabIndex = 3;
			this.productsDataGrid.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
																										 this.productsDataGridTableStyle});
			// 
			// productsDataGridTableStyle
			// 
			this.productsDataGridTableStyle.DataGrid = this.productsDataGrid;
			this.productsDataGridTableStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
																														 this.Product_ID,
																														 this.Product_Name,
																														 this.Product_Desc,
																														 this.Category,
																														 this.Price});
			this.productsDataGridTableStyle.HeaderFont = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.productsDataGridTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.productsDataGridTableStyle.MappingName = "Products";
			this.productsDataGridTableStyle.ReadOnly = true;
			// 
			// Product_ID
			// 
			this.Product_ID.Format = "";
			this.Product_ID.FormatInfo = null;
			this.Product_ID.HeaderText = "ID";
			this.Product_ID.MappingName = "Product_ID";
			this.Product_ID.Width = 50;
			// 
			// Product_Name
			// 
			this.Product_Name.Format = "";
			this.Product_Name.FormatInfo = null;
			this.Product_Name.HeaderText = "Name";
			this.Product_Name.MappingName = "Product_Name";
			this.Product_Name.Width = 120;
			// 
			// Product_Desc
			// 
			this.Product_Desc.Format = "";
			this.Product_Desc.FormatInfo = null;
			this.Product_Desc.HeaderText = "Description";
			this.Product_Desc.MappingName = "Product_Desc";
			this.Product_Desc.Width = 180;
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
			this.Price.Format = "0.00";
			this.Price.FormatInfo = null;
			this.Price.HeaderText = "Price $";
			this.Price.MappingName = "Price";
			this.Price.Width = 64;
			// 
			// ViewProducts
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(608, 365);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.productsDataGrid,
																		  this.headerLbl,
																		  this.closeBtn});
			this.ForeColor = System.Drawing.Color.Black;
			this.MaximizeBox = false;
			this.Name = "ViewProducts";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ViewProducts";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.productsDataGrid)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

	}
}

