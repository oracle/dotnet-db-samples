ODP.NET Performance Counters Demo README
----------------------------------------
This demo shows how to programmatically collect ODP.NET performance counters so that they can be output to a file or console.

For setup, modify the connection string and connect descriptor to connect to an Oracle Database with a sample HR schema. 
Also modify the executable name to match the name of your executable in the exception handler.

In the app.config, set the performance counters to monitor. This sample monitors the number of pooled connections by setting the 
PerformanceCounters setting value to 256.

          <settings>
            <setting name="PerformanceCounters" value="256"/>
          </settings>

For ODP.NET 18c or earlier versions, use the OraProvCfg.exe utility included with the Oracle Universal Installer, MSI, or xcopy 
installs to register ODP.NET performance counters. Registration is required for the counters to be available in Windows Performance 
Monitor. 

Run the following command when using managed ODP.NET: 
* OraProvCfg /action:register /product:odpm /component:perfcounter /providerpath:"<directory path>\oracle.manageddataaccess.dll“
Or run the following command with Windows administrator privileges when using unmanaged ODP.NET:
* OraProvCfg /action:register /product:odp /component:perfcounter /providerpath:"<directory path>\oracle.dataaccess.dll“
OraProvCfg.exe is not available in ODP.NET 18c and earlier NuGet releases.

In Oracle 19c and higher, all installs, including NuGet, will have a script for registering ODP.NET counters.

In the .NET configuration file, set the PerformanceCounters parameter to indicate which counters to monitor. Refer to the ODP.NET 
documentation of the specific version for these counter values. 

You can also view ODP.NET performance counter data in Windows Performance Monitor. This requires some Windows configuration setup,
which is discussed step by step in [this video](https://www.youtube.com/watch?v=mNwaYIazxlw).
