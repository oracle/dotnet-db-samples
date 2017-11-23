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
* Name of the File               :  ManipulateAds.cs
* Creation/Modification History  :
*             24-July-2002     Created 
*
* Sample Overview:
* The purpose of this .Net sample application is to demonstrate
*         1. DML(Data Manipulation Language) operations on a Data Set for
*            LOB(Large Object)data like images, sounds etc.
*		  2. Fetching BLOB lob as an OracleLOB (ODP .Net Data Type) using an 
             OracleDataReader. 	 
*		  
* The connection to database is made using Oracle Data Provider for .Net (ODP .Net).
* DataSet is an in-memory cache of data that contains data filled by an OracleDataAdapter.
* An OracleDataAdapter serves as a bridge between the DataSet and the data source,
* OracleCommandBuilder automatically generates commands for insert,update.
* The connection to database is made using OracleConnection object. 
* 
* An OracleDataReader provides a way of reading a forward-only stream of data rows from a 
* data source. The ExecuteReader method of OracleCommand sends the CommandText to the
* Connection and builds an OracleDataReader. The connection to database is made using
* OracleConnection object. 
*
* The scenario for this sample application is to insert new advertisments and update
* existing ones in "PrintMedia" for the products available in "Products" table.
* When this application is run, a drop down list populated with product data 
* from database is displayed. The user can select a product for which he/she 
* wishes to insert/update an Advertisement. If the Advertisement Text and Image exists
* in database for the selected product then it appears in 'Ad Text' and 'Existing Ad Image'
* controls respectively (using OracleDataReader).
* To insert/update an advertisement the user can enter text for 
* advertisement and select image for advertisement by clicking on 'Browse' button.
* To commit changes the user can click on 'Save' button (using DataSet).
* To exit from the application the user can click the 'Close' button.  
**********************************************************************************/


//Standard Namespaces referenced in this sample application
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

//Namespace for ODP.Net classes
using Oracle.ManagedDataAccess.Client;

//Namespace for ODP.Net Types
using Oracle.ManagedDataAccess.Types;

namespace ManipulateAds
{
	//ManipulateAds class inherits Window's Form 
	public class ManipulateAds : System.Windows.Forms.Form
	{
	    //UI Controls
		ComboBox productCbBx;
		TextBox adTextBx;
		PictureBox existingImagePicBx;
		PictureBox newImagePicBx;
		Label headerLbl;
		Label productLbl;
		Label advLbl;
		Label existingLbl;
		Label newLbl;
		Button saveBtn;
		Button closeBtn;
		Button browseBtn;
		Container components = null;

        //Variable for storing the image name, path chosen from file-dialog
        String  strImageName ="";
		
		//To store value of current Advertisement ID
		String curAdID ="";

		//To store existing Advertisement Text 
		String strExistText="";

		//To store existing Product ID value
		int intProdID =0 ;

		//For database connection
		OracleConnection conn;

		//Constructor
		public ManipulateAds()
		{		
			//Calling this 'InitializeComponent' method, creates the UI required 
			//for this application
			InitializeComponent();
		}

        /***************************************************************
		* This method is the entry point to this sample application. 
		* It also populates Products dropdown listbox with intial values
		* from database. 
		****************************************************************/
    [STAThread]
		static void Main() 
		{
			//Instantiating this class
			ManipulateAds manipulateads = new ManipulateAds();

			//Get database connection
			if (manipulateads.getDBConnection())
			{
				//Calling 'populateList' method to populate 'productCbBx' list 
				//from Database
				manipulateads.populateList();

				//When this application is run, "ManipulateAds' form is run
				Application.Run(manipulateads);
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
			// Catch exception when error in connecting to database occurs
			catch (Exception ex) 
			{
				//Display error message
				MessageBox.Show(ex.ToString());
				return false;
			}
		}

			
		/*******************************************************************
		* This method is called on the click event of the 'Browse' button,
		* The purpose of this method is to display a File-Dialog, from 
		* which the user can choose the desired image for the advertisement. 
		* The chosen image gets displayed in the 'newImagePicBx'
		* Picture Box.
		*******************************************************************/
		private void browseBtn_Click(object sender, System.EventArgs e)
		{
			try
			{
				//Instantiate File Dialog box
				FileDialog fileDlg = new OpenFileDialog();

				//Set the initial directory
				fileDlg.InitialDirectory = "c:\\" ;

				//Filter image(.jpg, .bmp, .gif) files only
				fileDlg.Filter = "Image File (*.jpg;*.bmp;*.gif)|*.jpg;*.bmp;*.gif";

				//Restores the current directory before closing
				fileDlg.RestoreDirectory = true ;

				//When file is selected from the File Dialog
				if(fileDlg.ShowDialog()== DialogResult.OK)
				{
					//Store the name of selected file into a variable
					strImageName =fileDlg.FileName;	
				
					//Create a bitmap for selected image
					Bitmap newImage= new Bitmap(strImageName);

					//Fit the image to the size of picture box
					newImagePicBx.SizeMode = PictureBoxSizeMode.StretchImage;

					//Show the bitmap in picture box
					newImagePicBx.Image=(Image)newImage;
				}
		
				//No image chosen
				fileDlg=null;
			}
			catch(System.ArgumentException ex)
			{  //Display error message, if image is invalid
				strImageName = "";
				System.Windows.Forms.MessageBox.Show(ex.ToString());				
			}
			catch(Exception ex)
			{  //Display error message
				System.Windows.Forms.MessageBox.Show(ex.ToString());				
			}
		}

		/*******************************************************************************
		* This method is called on the click event of the 'Save' button,
		* It calls "updateData" method for data insertion and updation of advertisements.
		* The advertisement data is stored in the "PrintMedia" table.
		********************************************************************************/
		private void saveBtn_Click(object sender, System.EventArgs e)
		{
			updateData();
		}

		/*****************************************************************************
		* This method is called from the click event of Save button and 
		* SelectedIndexChanged event of Products DropDown list.
		* 
	    * The purpose of this method is to demonstrate DML operations on a Data Set for
        * LOB(Large Object)data. The functionalitity of this method is to insert 
        * a new advertisement or update an existing advertisement for the product chosen from 
		* the 'productCbBx' list. The advertisement data is stored in the 
		* database table 'PrintMedia'. 
		* 
		* The flow of this method is as follows:
		* 1. Instantiate an OracleDataAdapter object with the query for 'PrintMedia'
		*    table.
		* 2. Configure the schema to match with Data Source. Set Primary Key information.
		* 3. OracleCommandBuilder automatically generates the command for loading data
		*    for the given query.
		* 4. The Dataset is filled with data that is loaded through OracleDataAdapter.
		* 5. Create a DataRow in a DataTable contained in the DataSet for a new 
		*    advertisement or find the current advertisement DataRow for existing 
		*    advertisement.
		* 6. Convert new advertisement image into a byte array.
		* 7. Assign the corresponding values to the columns in the Data Row.
		* 8. Add the Data Row to the Data Set for a new advertisement or end the edit
		*    operation for existing advertisement.
		* 9. Update the database with the Data Set values. Hence adding/updating
		*    'PrintMedia' table data.
		*************************************************************************/
		private void updateData()
		{
      	try
			{
			    //Check if Ad Image or Ad Text is changed.
                if (strImageName != "" || strExistText != adTextBx.Text) 
				{
					//Change the default cursor to 'WaitCursor'(an HourGlass)
					this.Cursor = Cursors.WaitCursor;
                					
					//If curAdId is null then insert record
					if (curAdID == "") 
					{

						//To fill DataSet and update datasource
						OracleDataAdapter printmediaAdapter;

						//In-memory cache of data
						DataSet printmediaDataSet;

						
						//Data Row contained in Data Table
						DataRow printmediaRow;
		
						//For automatically generating commands to make changes to database through DataSet
						OracleCommandBuilder printmediaCmdBldr;

						//Step 1.//
						//Query for 'PrintMedia' table given with OracleDbAdapter
						printmediaAdapter = new OracleDataAdapter("SELECT ad_text, ad_image, " +
							" product_id, date_of_creation  FROM PrintMedia",conn);
						
						//Instantiate a DataSet object
						printmediaDataSet= new DataSet("PrintMedia");


	                    //Step 2.//	
						//AddWithKey sets the Primary Key information to complete the 
						//schema information
						printmediaAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

						//Configures the schema to match with Data Source
						printmediaAdapter.FillSchema(printmediaDataSet, SchemaType.Source, "PrintMedia");

						
						//Step 3.//
						//In this case 'OracleCommandBuilder' automatically generates
						//'SelectCommand'
						printmediaCmdBldr = new OracleCommandBuilder(printmediaAdapter);
					

                        //Step 4.//
						//Adapter fills the DataSet with 'PrintMedia' data
						printmediaAdapter.Fill(printmediaDataSet,"PrintMedia");


					    //Step 5.//
						//Create a new row in the DataTable contained in the DataSet
						 printmediaRow = printmediaDataSet.Tables["PrintMedia"].NewRow();

 
						//part of Step 7.//
						//Assigning the value of advertisement text
						printmediaRow["Ad_text"] = adTextBx.Text;
			
						//If image is added
						if (strImageName != "")
						{
							//Step 6.//
							//providing read access to the file chosen using the 'Browse' button
							FileStream fs = new FileStream(@strImageName, FileMode.Open,FileAccess.Read);

							//Create a byte array of file stream length
							byte[] adImageData = new byte[fs.Length];

							//Read block of bytes from stream into the byte array
							fs.Read(adImageData,0,System.Convert.ToInt32(fs.Length));

							//Close the File Stream 
							fs.Close();

							// part of Step 7.//
							//Assigning the byte array containing image data
							printmediaRow["Ad_image"] = adImageData;
						}
						
						//Step 7.//
						//Assigning product id value with the 'ValueMember' of product drop down list
						printmediaRow["product_id"] = intProdID;
						
						//Assigning date of creation for advertisement to current date
						printmediaRow["date_of_creation"] = System.DateTime.Today;


						//Step 8.//
						//Adding the 'printmediaRow' to the DataSet
						printmediaDataSet.Tables["PrintMedia"].Rows.Add(printmediaRow);

						//Step 9.//
						//Update the database table 'PrintMedia' with new printmedia rows
						printmediaAdapter.Update(printmediaDataSet,"PrintMedia");

						//On successful Insertion of Ad Image display the image in "exisitingImagePicBx"
						//and clear the "newImagePicBx"
						if (strImageName != "") 
						{
							//Create a bitmap for selected image
							Bitmap newImage= new Bitmap(strImageName);

							//Fit the image to the size of picture box
							existingImagePicBx.SizeMode = PictureBoxSizeMode.StretchImage;

							//Show the bitmap in picture box
							existingImagePicBx.Image=(Image)newImage;

							//Clear contents
							newImagePicBx.Image = null;
						}

						//Reset Values
						strImageName= "";
						strExistText=adTextBx.Text;

						//Set the wait cursor to default cursor
						this.Cursor = Cursors.Default;
                    }

					//If advertisement exists, then update
 					else
					{
						//Change the default cursor to 'WaitCursor'(an HourGlass)
						this.Cursor = Cursors.WaitCursor;
                    
						//To fill Dataset and update datasource
						OracleDataAdapter printmediaAdapter;

						//In-memory cache of data
						DataSet printmediaDataSet;

						//Data Row contained in Data Table
						DataRow printmediaRow;
		
						//For automatically generating commands to make changes to database through DataSet
						OracleCommandBuilder printmediaCmdBldr;


						//Step 1.//
						//Query for 'PrintMedia' table given with OracleDataAdapter
						printmediaAdapter = new OracleDataAdapter("SELECT ad_id, ad_text, ad_image, "+
							  "  date_of_creation FROM PrintMedia WHERE ad_id ="+ curAdID,conn);

						//Instantiate a DataSet object
						printmediaDataSet= new DataSet("PrintMedia");


						//Step 2.//
						//AddWithKey sets the Primary Key information to complete the 
						//schema information
						printmediaAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

						//Configures the schema to match with Data Source
						printmediaAdapter.FillSchema(printmediaDataSet, SchemaType.Source, "PrintMedia");


						//Step 3.//
						//In this case 'OracleCommandBuilder' automatically generates
						//'SelectCommand'
						printmediaCmdBldr = new OracleCommandBuilder(printmediaAdapter);

					
						//Step 4.//
						//OracleDataAdapter fills the DataSet with 'PrintMedia' table data
						printmediaAdapter.Fill(printmediaDataSet,"PrintMedia");
						

						//Step 5.//
						//Get the current advertisement row for updation
						printmediaRow = printmediaDataSet.Tables[0].Rows.Find(curAdID);

                        //Start the edit operation on the current row
						printmediaRow.BeginEdit();

					
						//part of Step 7.//
						//Assigning the value of advertisement text
						printmediaRow["Ad_text"] = adTextBx.Text;
			
						if (strImageName != "")
						{
							//Step 6.//
							//providing read access to the file chosen using the 'Browse' button
							FileStream fs = new FileStream(@strImageName, FileMode.Open,FileAccess.Read);

							//Create a byte array of file stream length
							byte[] adImageData = new byte[fs.Length];

							//Read block of bytes from stream into the byte array
							fs.Read(adImageData,0,System.Convert.ToInt32(fs.Length));

							//Close the File Stream 
							fs.Close();

							//part of Step 7.//
							//Assigning the byte array containing image data
							printmediaRow["Ad_image"] = adImageData;
						}
						
						//Step 7.//
						//Assigning date of creation for advertisement to current date
						printmediaRow["date_of_creation"] = System.DateTime.Today;


						//Step 8.//
						//End the editing current row operation
						printmediaRow.EndEdit();


						//Step 9.//
						//Update the database table 'PrintMedia' with new printmedia rows
						printmediaAdapter.Update(printmediaDataSet,"PrintMedia");


						//On successful Updation of Ad Image display the image in "exisitingImagePicBx"
						//and clear the "newImagePicBx"
					   if (strImageName != "") 
						{
							//Create a bitmap for selected image
							Bitmap newImage= new Bitmap(strImageName);

							//Fit the image to the size of picture box
							existingImagePicBx.SizeMode = PictureBoxSizeMode.StretchImage;

							//Show the bitmap in picture box
							existingImagePicBx.Image=(Image)newImage;
                            
						    //Clear contents
							newImagePicBx.Image = null;
						}

						//Reset variables
						strImageName= "";
						strExistText=adTextBx.Text;

						//Set the wait cursor to default cursor
						this.Cursor = Cursors.Default;

			 	}
			
				    //Display message on successful data updatation
					MessageBox.Show("Data saved successfully");
				}
				else
				{
					MessageBox.Show("Select image or change text for the advertisement!");
				}
			}catch(Exception ex)
			{   //Display error message
				 System.Windows.Forms.MessageBox.Show(ex.ToString());				
			 }
		}
			
		
		/***********************************************************************
		* This method is called when an Item is selected from 'productCbBx'
		* drop down list. The purpose of this method to demonstrate how to
		* fetch BLOB lob as an OracleLOB (ODP .Net Data Type) using an OracleDataReader.
		* The flow of the method is as follows:
		* 1. Clear the contents of Ad Text Box, Existing Ad Image and 
		*    New Ad Image.
		* 2. Populate OracleDataReader with data from 'PrintMedia' table, through 
		*    ExecuteReader method of OracleCommand object. The data is fetched based
		*    on the Product selected from 'productCbBx' list.
		* 3. Assign value for Ad Text from the OracleDataReader.
		* 4. Ad Image(BLOB) is read into a Byte array, then used to construct
		*    MemoryStream and passed to PictureBox.
		* Hence displaying the existing advertisement information for the 
		* Product selected from drop down list box. 	 
		***********************************************************************/
		private void ProductCbBx_SelectedIndexChanged(object sender, System.EventArgs e)
		{
	        //For fetching read only rows from datasource  
		    OracleDataReader viewPicReader;
			
			//For executing SQL statements against datasource
			OracleCommand viewPicCmd;
		
            //To store MessageBox result
			DialogResult x;

			//If Ad Image or Ad Text is changed then promt user to save.
			if (strImageName != "" || strExistText != adTextBx.Text)
			{
				//MessageBox prompting user whether he/she wishes to save changes made
				x= MessageBox.Show("Do you want to save changes ?", "Save Dialog",MessageBoxButtons.YesNo);

			    //If the user wishes to save changes
				if (x == DialogResult.Yes)
				{
					//call the method for insertion or updation of advertisement
					updateData();

					//Reset variable
					intProdID =  int.Parse(productCbBx.GetItemText(productCbBx.SelectedValue));         
				}
				//If the user doesn't wish to save changes
				else
				{
                    //Reset variables
					strImageName ="";
					intProdID =  int.Parse(productCbBx.GetItemText(productCbBx.SelectedValue));         
				}
				}
			try
				
			{   			  
				//Step 1.//
				//Clear contents
				adTextBx.Text ="";	
				existingImagePicBx.Image = null;
				newImagePicBx.Image = null;
				strImageName = "";
				curAdID ="";
				strExistText="";
				    				     
					
				//Fetch Product Details using OracleCommand for the selected product from the list
				viewPicCmd = new OracleCommand("SELECT " +
					"Ad_ID , " +
					"Ad_Text, " +
					"Ad_Image " +
					"FROM PrintMedia " +
					"WHERE product_id =" + productCbBx.GetItemText(productCbBx.SelectedValue) ,conn); 
  				
				//Set OracleConnection for this instance of OracleCommand
				viewPicCmd.Connection =conn;

				//Set Command type as text
				viewPicCmd.CommandType = CommandType.Text;

				//Sends the CommandText to the Connection and builds an OleDbDataReader
				viewPicReader = viewPicCmd.ExecuteReader();
				    
				//Read data    
				Boolean recordExist = viewPicReader.Read();
				
			 // MessageBox.Show(viewPicReader.GetInt32(0).ToString());
				//If data exists 
				if (recordExist) 
				{
					         
					//Store current Advertisement value
					curAdID = viewPicReader.GetInt32(0).ToString();
				
	                //If Ad Text exists
	   			   if (viewPicReader.GetValue(1).ToString() != "")
					{
						//Assign the 'Ad Text' TextBox to Advertisement text fetched from database
						adTextBx.Text =  viewPicReader.GetString(1);

						//set variable
						strExistText =  viewPicReader.GetString(1);
					}
				   
					
					//If Ad Image exists 
					if (viewPicReader.GetValue(2).ToString() != "")
					{

						//Fetch the BLOB data through OracleDataReader using OracleBlob type
						OracleBlob blob = viewPicReader.GetOracleBlob(2);
	

						//Create a byte array of the size of the Blob obtained
						Byte[] byteBLOBData =  new Byte[blob.Length];

						//Read blob data into byte array
						int i = blob.Read(byteBLOBData,0,System.Convert.ToInt32(blob.Length));
			            
						//Get the primitive byte data into in-memory data stream
						MemoryStream stmBLOBData = new MemoryStream(byteBLOBData);

						//Assign the 'Existing Ad Image' to the memory stream
						existingImagePicBx.Image= Image.FromStream(stmBLOBData);

						//Fit the image to the picture box size
						existingImagePicBx.SizeMode = PictureBoxSizeMode.StretchImage;
					}
					//close the OracleDataReader
					viewPicReader.Close();
				}
 
				//Reset  variable
				intProdID =  int.Parse(productCbBx.GetItemText(productCbBx.SelectedValue));         
				
			}
				//Catch exception when accessing arrary element out of bound
			catch (System.IndexOutOfRangeException rangeException)
			{
				//Do nothing
				rangeException.ToString();
			}
			catch (Exception ex)
			{
				//Display error message
				System.Windows.Forms.MessageBox.Show( ex.ToString());
			}
     	}

		/***********************************************************************
		* The purpose of this method is to populate  Products Drop Down list
		* with data from "Products" table. "Product Name' data is displayed in the
		* List, whereas the actual value stored is 'Product ID' 
		***********************************************************************/
		void populateList()
		{
			
			//To fill DataSet and update datasource
			OracleDataAdapter productsAdapter;

			//In-memory cache of data
			DataSet productsDataSet;

			//No selection
			adTextBx.SelectionStart = 0;

			try
			{
				//Instantiate OracleDataAdapter to create DataSet
				productsAdapter = new OracleDataAdapter();

				//Fetch Product Details
				productsAdapter.SelectCommand = new OracleCommand
					("SELECT Product_ID, Product_Name FROM Products ",conn); 
  				
				//Instantiate a DataSet object
				productsDataSet = new DataSet("ProductsDataSet");
			
				//Fill the DataSet 
				productsAdapter.Fill(productsDataSet, "Products");

				//Product Name is shown in the list displayed
				productCbBx.DisplayMember = productsDataSet.Tables[0].Columns["Product_Name"].ToString () ;
                  
				//Product Id is the actual value contained in the list
				productCbBx.ValueMember = productsDataSet.Tables[0].Columns["Product_ID"].ToString();

				//Assign DataSet as a data source for the List Box
				productCbBx.DataSource = productsDataSet.Tables["Products"].DefaultView;
			}
			catch(Exception ex)
			{   //Display error message
				System.Windows.Forms.MessageBox.Show(ex.ToString());				
			}
		}

		/**********************************************************************
		* This method is called on the click event of the 'Close' button.
		* The purpose of this method is to close the database connection,
		* the form 'ManipulateAds' and then exit out of the application.
		**********************************************************************/
		private void closeBtn_Click(object sender, System.EventArgs e)
		{
			conn.Close();
			this.Close();
			Application.Exit();
		}

		/***********************************************************************
		* This code is an automatically generated application code.
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

		/************************************************************************
		* This code is an automatically generated application code.
		* Note:Do not modify the contents of this method with the code editor.
		* The purpose of this method is to instantiate all the User Interface
		* components like Button, Picture Box, Labels etc., set their formatting
		* Properties and display these components in 'ManipulateAds' form.
		************************************************************************/
		private void InitializeComponent()
		{
      this.headerLbl = new System.Windows.Forms.Label();
      this.productLbl = new System.Windows.Forms.Label();
      this.productCbBx = new System.Windows.Forms.ComboBox();
      this.adTextBx = new System.Windows.Forms.TextBox();
      this.advLbl = new System.Windows.Forms.Label();
      this.existingImagePicBx = new System.Windows.Forms.PictureBox();
      this.existingLbl = new System.Windows.Forms.Label();
      this.saveBtn = new System.Windows.Forms.Button();
      this.closeBtn = new System.Windows.Forms.Button();
      this.browseBtn = new System.Windows.Forms.Button();
      this.newImagePicBx = new System.Windows.Forms.PictureBox();
      this.newLbl = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.existingImagePicBx)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.newImagePicBx)).BeginInit();
      this.SuspendLayout();
      // 
      // headerLbl
      // 
      this.headerLbl.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.headerLbl.ForeColor = System.Drawing.SystemColors.ControlText;
      this.headerLbl.Location = new System.Drawing.Point(168, 8);
      this.headerLbl.Name = "headerLbl";
      this.headerLbl.Size = new System.Drawing.Size(272, 32);
      this.headerLbl.TabIndex = 0;
      this.headerLbl.Text = "Favorite Stores";
      this.headerLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // productLbl
      // 
      this.productLbl.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.productLbl.ForeColor = System.Drawing.SystemColors.ControlText;
      this.productLbl.Location = new System.Drawing.Point(8, 80);
      this.productLbl.Name = "productLbl";
      this.productLbl.Size = new System.Drawing.Size(112, 24);
      this.productLbl.TabIndex = 2;
      this.productLbl.Text = "Select Product ";
      this.productLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // productCbBx
      // 
      this.productCbBx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.productCbBx.Location = new System.Drawing.Point(128, 80);
      this.productCbBx.Name = "productCbBx";
      this.productCbBx.Size = new System.Drawing.Size(152, 21);
      this.productCbBx.TabIndex = 10;
      this.productCbBx.SelectedIndexChanged += new System.EventHandler(this.ProductCbBx_SelectedIndexChanged);
      // 
      // adTextBx
      // 
      this.adTextBx.Location = new System.Drawing.Point(128, 128);
      this.adTextBx.MaxLength = 200;
      this.adTextBx.Multiline = true;
      this.adTextBx.Name = "adTextBx";
      this.adTextBx.Size = new System.Drawing.Size(352, 48);
      this.adTextBx.TabIndex = 1;
      // 
      // advLbl
      // 
      this.advLbl.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.advLbl.ForeColor = System.Drawing.SystemColors.ControlText;
      this.advLbl.Location = new System.Drawing.Point(24, 136);
      this.advLbl.Name = "advLbl";
      this.advLbl.Size = new System.Drawing.Size(88, 23);
      this.advLbl.TabIndex = 4;
      this.advLbl.Text = "Ad Text";
      this.advLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // existingImagePicBx
      // 
      this.existingImagePicBx.BackColor = System.Drawing.Color.White;
      this.existingImagePicBx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.existingImagePicBx.Location = new System.Drawing.Point(128, 248);
      this.existingImagePicBx.Name = "existingImagePicBx";
      this.existingImagePicBx.Size = new System.Drawing.Size(152, 104);
      this.existingImagePicBx.TabIndex = 5;
      this.existingImagePicBx.TabStop = false;
      // 
      // existingLbl
      // 
      this.existingLbl.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.existingLbl.ForeColor = System.Drawing.SystemColors.ControlText;
      this.existingLbl.Location = new System.Drawing.Point(136, 216);
      this.existingLbl.Name = "existingLbl";
      this.existingLbl.Size = new System.Drawing.Size(136, 24);
      this.existingLbl.TabIndex = 6;
      this.existingLbl.Text = "Existing Ad Image";
      this.existingLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // saveBtn
      // 
      this.saveBtn.BackColor = System.Drawing.SystemColors.Control;
      this.saveBtn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.saveBtn.ForeColor = System.Drawing.SystemColors.ControlText;
      this.saveBtn.Location = new System.Drawing.Point(216, 384);
      this.saveBtn.Name = "saveBtn";
      this.saveBtn.Size = new System.Drawing.Size(56, 24);
      this.saveBtn.TabIndex = 7;
      this.saveBtn.Text = "Save";
      this.saveBtn.UseVisualStyleBackColor = false;
      this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
      // 
      // closeBtn
      // 
      this.closeBtn.BackColor = System.Drawing.SystemColors.Control;
      this.closeBtn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.closeBtn.ForeColor = System.Drawing.SystemColors.ControlText;
      this.closeBtn.Location = new System.Drawing.Point(312, 384);
      this.closeBtn.Name = "closeBtn";
      this.closeBtn.Size = new System.Drawing.Size(56, 24);
      this.closeBtn.TabIndex = 8;
      this.closeBtn.Text = "Close";
      this.closeBtn.UseVisualStyleBackColor = false;
      this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
      // 
      // browseBtn
      // 
      this.browseBtn.BackColor = System.Drawing.SystemColors.Control;
      this.browseBtn.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.browseBtn.ForeColor = System.Drawing.SystemColors.ControlText;
      this.browseBtn.Location = new System.Drawing.Point(496, 248);
      this.browseBtn.Name = "browseBtn";
      this.browseBtn.Size = new System.Drawing.Size(64, 24);
      this.browseBtn.TabIndex = 9;
      this.browseBtn.Text = "Browse";
      this.browseBtn.UseVisualStyleBackColor = false;
      this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
      // 
      // newImagePicBx
      // 
      this.newImagePicBx.BackColor = System.Drawing.Color.White;
      this.newImagePicBx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.newImagePicBx.Location = new System.Drawing.Point(328, 248);
      this.newImagePicBx.Name = "newImagePicBx";
      this.newImagePicBx.Size = new System.Drawing.Size(152, 104);
      this.newImagePicBx.TabIndex = 11;
      this.newImagePicBx.TabStop = false;
      // 
      // newLbl
      // 
      this.newLbl.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.newLbl.ForeColor = System.Drawing.SystemColors.ControlText;
      this.newLbl.Location = new System.Drawing.Point(344, 216);
      this.newLbl.Name = "newLbl";
      this.newLbl.Size = new System.Drawing.Size(120, 24);
      this.newLbl.TabIndex = 12;
      this.newLbl.Text = "New Ad Image";
      this.newLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // ManipulateAds
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(592, 429);
      this.Controls.Add(this.newLbl);
      this.Controls.Add(this.newImagePicBx);
      this.Controls.Add(this.productCbBx);
      this.Controls.Add(this.browseBtn);
      this.Controls.Add(this.closeBtn);
      this.Controls.Add(this.saveBtn);
      this.Controls.Add(this.existingLbl);
      this.Controls.Add(this.existingImagePicBx);
      this.Controls.Add(this.advLbl);
      this.Controls.Add(this.adTextBx);
      this.Controls.Add(this.productLbl);
      this.Controls.Add(this.headerLbl);
      this.Name = "ManipulateAds";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Manipulate Advertisements";
      this.Load += new System.EventHandler(this.ManipulateAds_Load);
      ((System.ComponentModel.ISupportInitialize)(this.existingImagePicBx)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.newImagePicBx)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

		}

    private void ManipulateAds_Load(object sender, EventArgs e)
    {

    }
	
	
	}
}
