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

/*****************************************************************************************
 
* @author                        :  Jagriti
* @version                       :  1.0
* Development Environment        :  MS Visual Studio .NET 
* Name of the File               :  ManipulateProducts.cs
* Creation/Modification History  :
        23-July-2002               Created 

* Sample Overview:

* The purpose of this sample application is to demonstrate DML operations on a Dataset
* through Oracle Data Provider for .NET (ODP.NET) connection. This application provides 
* functionality to add/update products to "Favorite Stores" shopping stores.
*
* When this application is run, a list of products from database is displayed.
* The user can add a new product by navigating to the end of Data Grid and 
* creating a new row and click "Save" button to commit the changes.
* Product information can be updated by overwriting the existing information 
* in the Data Grid and clicking the "Save" button to commit changes.
*****************************************************************************************/

//Include standard namespaces used in this sample
using System;
using System.Windows.Forms;
using System.Data;
using System.ComponentModel;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;


namespace ManipulateProducts
{
  public class ManipulateProducts : System.Windows.Forms.Form
  {
    //UI components
    private Button saveBtn;
    private Label headerLbl;
    private Button closeBtn;

    //For Database connection 
    private OracleConnection conn;

    //To fill DataSet and update Datasource
    private OracleDataAdapter productsAdapter;

    //For automatically generating Commands to make changes to Database through Dataset
    private OracleCommandBuilder productsCmdBuilder;

    //In-memory cache of data
    private DataSet productsDataSet;

    // Datagrid columns 
    private DataGridTextBoxColumn Product_ID;
    private DataGridTextBoxColumn Product_Name;
    private DataGridTextBoxColumn Product_Desc;
    private DataGridTextBoxColumn Category;
    private DataGridTextBoxColumn Price;
    private DataGrid productsDataGrid;
    private DataGridTableStyle productsGridTableStyle;
		
	
    // Variables for Data Grid column validation		
    int newCurrentRow;
    int newCurrentCol;
    int oldCurrentRow;
    int oldCurrentCol;
    private System.Windows.Forms.DataGridTextBoxColumn Product_Status;
    bool okToValidate;
        
    //Constructor
    public ManipulateProducts()
    {
      //Initializes all the required Windows Components 
      InitializeComponent();

      //Intializing Data Grid column validation variables
      newCurrentRow = -1;
      newCurrentCol = -1;
      okToValidate = true;
    }
        
    /**************************************************************
    * This method is the entry point to this sample application.
    * It also displays Product data in a tabular format.
    ***************************************************************/
    public static void Main() 
    {   
      //Instantiating this class
      ManipulateProducts manipulateproducts = new ManipulateProducts();

      //Get database connection
      if (manipulateproducts.getDBConnection())
      {

        //Calling 'populateProductsDataGrid' method to populate the DataGrid from
        //database			  
        manipulateproducts.populateProductsDataGrid();

        //Run the application 
        Application.Run(manipulateproducts);
      }
			
    }

	
    /****************************************************************************
    * The purpose of this method is to populate the "productsDataGrid' with data 
    * from 'Products' database table. This method is called from Main method.
    *****************************************************************************/  
    private void populateProductsDataGrid()
    {
      try
      { 
        //Instantiate OracleDataAdapter to create DataSet
        //Fetch Product Details
        productsAdapter = new OracleDataAdapter("SELECT " +
          "Product_ID ID, " +
          "Product_Name Name, " +
          "Product_Desc Description, " +
          "Category, " +
          "Price, " +
          "Product_Status " +
          " FROM Products",conn); 
  
        //For automatically generating commands
        productsCmdBuilder = new OracleCommandBuilder(productsAdapter); 

        //Creating Dataset
        productsDataSet = new DataSet("productsDataSet");
				    
        //AddWithKey sets the Primary Key information to complete the 
        //schema information
        productsAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
			
        //Fill the DataSet 
        productsAdapter.Fill(productsDataSet, "Products");
					  
        //Binding DataSet to the DataGrid
        productsDataGrid.SetDataBinding(productsDataSet,"Products"); 

      }
				   
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }


    /****************************************************************** 
    * On the click event of 'Save' button, any insertion/updation made
    * to the 'ProductsDataGrid' is saved to the database.
    * Since OracleDataAdapter is used here, the commands for 
    * insertion/updatation are automatically created.
    *******************************************************************/
    private void SaveBtn_Click(object sender, System.EventArgs e)
    {
      try
      {
		
        // Calling a method to check if the value entered for any column is null
        Boolean flag = NullCellsPresent(productsDataGrid);

        if (flag == false)
        {

          // Following is the code to modify data in 'productsDataSet'.
          // InsertCommand, UpdateCommand are automatically generated 
          // using OracleDataAdapter.Update command based on the event taken place.
          productsAdapter.Update(productsDataSet,"Products");
 
          //Display confirmation message
          MessageBox.Show("Changes saved successfully !");}
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(),"Exception Occured");			
      }
			
    }
    /***************************************************************************
    * The purpose of this method is to get the database connection using the
    * parameters given in ConnectionParams class
    * Note: Replace the datasource parameter with your datasource value.
    ***************************************************************************/
    private Boolean getDBConnection()
    {
      try
      {
        //Connection Information	
        string ConnectionString = 
          //username
          "User Id=" + ConnectionParams.Username +

          //password
          ";Password=" +ConnectionParams.Password +

          //replace with your datasource value (TNSNames)
          ";Data Source=" + ConnectionParams.Datasource;

					
        //Connection to datasource, using connection parameters given above
        conn = new OracleConnection(ConnectionString);

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

		

    /***********************************************************************
    * This method is called when a data cell value gets changed 
    * The purpose of this method to validate price to be a positve integer. 
    ************************************************************************/
    public bool IsValidValue(int row, int col, string newText)
    {
      bool returnValue = true;
      try
      {
        //Price column has index = 4
        if(col == 4)

          //Check if price entered is invalid
          returnValue = (double.Parse(newText) > 0);
      }
      catch(Exception ex)
      {
        //Error is thrown if invalid characters are entered
        ex.ToString();
        returnValue = false;
      }
      return returnValue;
    }

    /**********************************************************************
    * This method is called on the load event of 'ManipulateProducts' form
    * The purpose of this method is to initialize the variables used for 
    * Data Grid validation.
    ***********************************************************************/
    private void ManipulateProducts_Load(object sender, System.EventArgs e)
    {
      try
      {
        //set to initial current cell
        oldCurrentRow = 0;
        oldCurrentCol = 0;;
        productsDataGrid.CurrentCell = new DataGridCell(oldCurrentRow, oldCurrentCol);		
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(),"Exception Occured");				
      }
    }

    /***************************************************************************
    * This method is called when the user navigates between different cells in
    * 'productsDataGrid'. Validation for price column is done here.
    ****************************************************************************/
    private void Handle_CurrentCellChanged(object sender, System.EventArgs e)
    {
      try
      {
        newCurrentRow = productsDataGrid.CurrentCell.RowNumber;
        newCurrentCol = productsDataGrid.CurrentCell.ColumnNumber;
        string newText = productsDataGrid[oldCurrentRow, oldCurrentCol].ToString();
        // If invalid value is entered
        if( okToValidate && !IsValidValue(oldCurrentRow, oldCurrentCol, newText))
        {
          MessageBox.Show("Enter valid values for price!");
          okToValidate = false;
          productsDataGrid.CurrentCell = new DataGridCell(oldCurrentRow, oldCurrentCol);
          okToValidate = true;

        }
        oldCurrentRow = newCurrentRow;
        oldCurrentCol = newCurrentCol;
      }
      catch (System.IndexOutOfRangeException rangeException)
      {
        rangeException.ToString();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(),"Exception Occured");				
      }
			
    }


    /******************************************************************
     This method returns true if any of the editable columns are empty 
     or if the value for the price entered < 0.
     ******************************************************************/
    public  Boolean NullCellsPresent(DataGrid dg) 
    {
      DataSet ds = (DataSet) dg.DataSource;
      DataTable dt = ds.Tables[0];
      String str1, str2, str3, str4, str5;
		
      foreach (DataRow dr in dt.Rows) 
      {
        str1 = dr.ItemArray[1].ToString();
        str2 = dr.ItemArray[2].ToString();
        str3 = dr.ItemArray[3].ToString();
        str4 = dr.ItemArray[4].ToString();
        str5 = dr.ItemArray[5].ToString();
        if (str1.Length == 0  || str2.Length == 0 || str3.Length == 0|| 
          str5.Length == 0)
        {
          MessageBox.Show("Please enter not null values for all fields!", 
            "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return (true);

        }
        else if ( !(double.Parse(str4) > 0)) 
        {
          MessageBox.Show("Please enter a value > 0 for price!", 
            "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return (true);
        }
	
      }
				
      return (false);
    }
 
    /****************************************************************************
    * On the click event of 'Close' button, firstly the database connection is
    *  closed then the window is closed and lastly the application is closed.
    ****************************************************************************/
    private void CloseBtn_Click(object sender, System.EventArgs e)
    {
      try
      {
        conn.Close();
        this.Close();
        Application.Exit();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(),"Exception Occured");				
      }
    }

		#region Windows Form Designer generated code
    /***********************************************************************
    * This is a required method for Designer support, its purpose is
    * to intialize the UI controls and their related properties.
    * NOTE: Do not modify the contents of this method with the code editor.
    ************************************************************************/		
    private void InitializeComponent()
    {
      this.productsDataGrid = new System.Windows.Forms.DataGrid();
      this.productsGridTableStyle = new System.Windows.Forms.DataGridTableStyle();
      this.Product_ID = new System.Windows.Forms.DataGridTextBoxColumn();
      this.Product_Name = new System.Windows.Forms.DataGridTextBoxColumn();
      this.Product_Desc = new System.Windows.Forms.DataGridTextBoxColumn();
      this.Category = new System.Windows.Forms.DataGridTextBoxColumn();
      this.Price = new System.Windows.Forms.DataGridTextBoxColumn();
      this.Product_Status = new System.Windows.Forms.DataGridTextBoxColumn();
      this.saveBtn = new System.Windows.Forms.Button();
      this.closeBtn = new System.Windows.Forms.Button();
      this.headerLbl = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.productsDataGrid)).BeginInit();
      this.SuspendLayout();
      // 
      // productsDataGrid
      // 
      this.productsDataGrid.AlternatingBackColor = System.Drawing.SystemColors.Window;
      this.productsDataGrid.BackgroundColor = System.Drawing.SystemColors.AppWorkspace;
      this.productsDataGrid.CaptionBackColor = System.Drawing.SystemColors.ActiveCaption;
      this.productsDataGrid.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.productsDataGrid.CaptionText = "List of Products";
      this.productsDataGrid.DataMember = "";
      this.productsDataGrid.HeaderBackColor = System.Drawing.SystemColors.Control;
      this.productsDataGrid.HeaderFont = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.productsDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
      this.productsDataGrid.Location = new System.Drawing.Point(40, 64);
      this.productsDataGrid.Name = "productsDataGrid";
      this.productsDataGrid.Size = new System.Drawing.Size(720, 232);
      this.productsDataGrid.TabIndex = 0;
      this.productsDataGrid.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
                                                                                                 this.productsGridTableStyle});
      this.productsDataGrid.CurrentCellChanged += new System.EventHandler(this.Handle_CurrentCellChanged);
      // 
      // productsGridTableStyle
      // 
      this.productsGridTableStyle.AlternatingBackColor = System.Drawing.SystemColors.Window;
      this.productsGridTableStyle.BackColor = System.Drawing.SystemColors.Window;
      this.productsGridTableStyle.DataGrid = this.productsDataGrid;
      this.productsGridTableStyle.ForeColor = System.Drawing.SystemColors.WindowText;
      this.productsGridTableStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
                                                                                                             this.Product_ID,
                                                                                                             this.Product_Name,
                                                                                                             this.Product_Desc,
                                                                                                             this.Category,
                                                                                                             this.Price,
                                                                                                             this.Product_Status});
      this.productsGridTableStyle.GridLineColor = System.Drawing.SystemColors.Control;
      this.productsGridTableStyle.HeaderBackColor = System.Drawing.SystemColors.Control;
      this.productsGridTableStyle.HeaderFont = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.productsGridTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
      this.productsGridTableStyle.MappingName = "Products";
      // 
      // Product_ID
      // 
      this.Product_ID.Format = "";
      this.Product_ID.FormatInfo = null;
      this.Product_ID.HeaderText = "ID";
      this.Product_ID.MappingName = "ID";
      this.Product_ID.Width = 40;
      // 
      // Product_Name
      // 
      this.Product_Name.Format = "";
      this.Product_Name.FormatInfo = null;
      this.Product_Name.HeaderText = "Name";
      this.Product_Name.MappingName = "Name";
      this.Product_Name.Width = 140;
      // 
      // Product_Desc
      // 
      this.Product_Desc.Format = "";
      this.Product_Desc.FormatInfo = null;
      this.Product_Desc.HeaderText = "Description";
      this.Product_Desc.MappingName = "Description";
      this.Product_Desc.Width = 262;
      // 
      // Category
      // 
      this.Category.Format = "";
      this.Category.FormatInfo = null;
      this.Category.HeaderText = "Category";
      this.Category.MappingName = "Category";
      this.Category.Width = 68;
      // 
      // Price
      // 
      this.Price.Format = "0.00";
      this.Price.FormatInfo = null;
      this.Price.HeaderText = "Price $";
      this.Price.MappingName = "Price";
      this.Price.Width = 64;
      // 
      // Product_Status
      // 
      this.Product_Status.Format = "";
      this.Product_Status.FormatInfo = null;
      this.Product_Status.HeaderText = "Status";
      this.Product_Status.MappingName = "Product_Status";
      this.Product_Status.Width = 105;
      // 
      // saveBtn
      // 
      this.saveBtn.BackColor = System.Drawing.SystemColors.Control;
      this.saveBtn.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.saveBtn.ForeColor = System.Drawing.Color.Black;
      this.saveBtn.Location = new System.Drawing.Point(304, 336);
      this.saveBtn.Name = "saveBtn";
      this.saveBtn.Size = new System.Drawing.Size(64, 24);
      this.saveBtn.TabIndex = 1;
      this.saveBtn.Text = "Save";
      this.saveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
      // 
      // closeBtn
      // 
      this.closeBtn.BackColor = System.Drawing.SystemColors.Control;
      this.closeBtn.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.closeBtn.ForeColor = System.Drawing.Color.Black;
      this.closeBtn.Location = new System.Drawing.Point(408, 336);
      this.closeBtn.Name = "closeBtn";
      this.closeBtn.Size = new System.Drawing.Size(72, 24);
      this.closeBtn.TabIndex = 2;
      this.closeBtn.Text = "Close";
      this.closeBtn.Click += new System.EventHandler(this.CloseBtn_Click);
      // 
      // headerLbl
      // 
      this.headerLbl.BackColor = System.Drawing.SystemColors.Control;
      this.headerLbl.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.headerLbl.ForeColor = System.Drawing.SystemColors.ControlText;
      this.headerLbl.Location = new System.Drawing.Point(272, 16);
      this.headerLbl.Name = "headerLbl";
      this.headerLbl.Size = new System.Drawing.Size(192, 23);
      this.headerLbl.TabIndex = 3;
      this.headerLbl.Text = "Favorite Stores";
      this.headerLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // ManipulateProducts
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(792, 389);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.headerLbl,
                                                                  this.closeBtn,
                                                                  this.saveBtn,
                                                                  this.productsDataGrid});
      this.MaximizeBox = false;
      this.Name = "ManipulateProducts";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Manipulate Products ";
      ((System.ComponentModel.ISupportInitialize)(this.productsDataGrid)).EndInit();
      this.ResumeLayout(false);

    }
        #endregion

  }

}