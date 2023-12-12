# Microsoft Entra ID/Azure Active Directory ODP.NET Sample Code 
These ODP.NET sample code files show how to connect to an Oracle database using a token obtained from Microsoft Entra ID, also known as Azure Active Directory (AD), or with Azure AD single sign-on. 

In the main directory, there is one sample for each ODP.NET provider type (core, managed, and unmanaged). The samples shows Oracle access token management and Azure AD authentication. They require the following to test with an Azure AD token:
* Data Source
* Azure AD app registration client identifier
* Azure AD tenant identifier
* Azure AD app registration secret value
* Azure AD database registration scope

In the SSO sub-directory, this sample shows how to use ODP.NET Azure AD SSO with service principal authentication. It can use either ODP.NET Core or managed ODP.NET and requires the following to test with:
* Data Source
* Azure AD app registration client identifier
* Azure AD tenant identifier
* Azure AD app registration secret value
* Azure AD protected resource identifier
