# azure-basic-appservice-setup
Guide and template for setting up a basic azure appservice with some common tools

This project is guide is built as an extention to the following guide: [github.com/pckofstad/CI/CD Demo](https://github.com/pckofstad/ci-cd-demo)

The guide aims to give a quick insight in a basic Azure App Service Setup with some standard components. The focus will be on the manual Azure configuration. Clone this repo if you want a simple testable application to test your setup. 

## Overview
This guide outlines the steps for configuring an Azure App Service App with Azure SQL, Keyvault, Application Insight, and Azure Storage Account Blob Storage. The App Service App will be able to communicate with Keyvault secrets using a managed identity. Additionally, we will configure an elastic pool with 2 databases and set custom usernames and passwords for each database.

## Prerequisites
Before beginning the configuration process, make sure you have the following:
- An active Azure subscription
- Access to the Azure portal
- An docker app that is ready to be pushed to azure docker registry

## Steps
## 1. Create a resource group 
We want to group all your collected resources in
1. Search and navigate to your list of "Resource groups" and click "+ Create"
2. Select the appropriate Subscription and Region. 
3. Give the resource group an appropriate name. If you are creating a resource group for a running app, make the naming descriptiv and with a system. If you are making a resourcegroup for testing. Make it clear that it is for testing and who is testing it. 
4. Create the resource

## 2. Create an Azure SQL server
1. Search and navigate to your list of "SQL servers" and click "+ Create"
2. Select the Resource group that you just created. 
3. Give the server name a descriptive name
4. Location is recommended to be the same as where you are deploying your app to. 
5. Authentication method select "Use both SQL and Azure AD authentication"
6. Create a server admin login and a password. We will not be using this, but make sure to keep a strong password anyway. 
6. Set yourself as the admin for the databse. 
7. Create the resource
8. Ensure that the server is created before you move on. 

## 3. Create an Azure SQL elastic pool
We want to use the same hardware resourcs for several databases related to the same apps. We will therefore create an elastic pool.
1. Search and navigate to the "SQL elastic pools" anc click "+ Click"
2. Select the Resource group that you just created. 
3. Select a name for the elastic pool
4. Ensure you select the server you just created
5. For Compute+storage we only need a small server for this demo. Click "Configure elastic pool"
6. Select "Service Tier" -> DTU-Based -> Basic
7. 50 eDTUs will do for our demo. Click Apply
8. Create the resource
9. Ensure that the pool is created before you move on. 

## 4. Create an Azure SQL databases
Create your Azure SQL databases within the elastic pool.
1. Navigate into the sql-pool resource and click "Create database"
2. Most of the values are prefilled when you create it from inside the pool. Set a database name. 
3. Be aware that you migth want to change the "Collation" for the database. This is the configuration that mostly affect how data is stored and sorted. As an example, if you want results to be sorted with nordic characters at the end of the alphabet. 
5. Creaate the databse
5. Ensure that the database is created before you move on. 

## 5. Configure custom usernames and passwords for each database
We want to configure custom username and password for our database, this will provide a fail-safe if the same sql-server and sql-pool is shared between teams, apps or enviroments.
1. Navigate to the database that you just created. 
3. Click on the "Set server firewall" button and configure the firewall settings as necessary.
4. Add your client IPv4 to the list, and tick Exception -> "Allow Azure services and resources to access this server"
5. Connect to the database to execute commands against the master database. I recomend using Azure Data Studio. When logging in with you Azure Portal account you well be able to browse to you SQL server and database. 
6. You need to create a database user, a login for the user and you need to grant access to the database.
    > Against the servers master table, execute the following command:
    ```SQL
    -- create SQL login in master database you need to spesify your own password
    CREATE LOGIN BlazorAppDbUserTest
    WITH PASSWORD = 'Strong_Password_Goes_Here';
    ```
    > Against the database you just created execute the following two commands:
    ```SQL
   -- add database user for login testLogin1
    CREATE USER [BlazorAppDbUserTest]
    FROM LOGIN [BlazorAppDbUserTest]
    WITH DEFAULT_SCHEMA=dbo;
    ```
    ```SQL
    -- add user to database role(s) (i.e. db_owner)
    ALTER ROLE db_owner ADD MEMBER [BlazorAppDbUserTest];
    ```
7. You have now created a database spesific user, you can now remove access to your IP address. Navigate to the firewall setting page (Navigate to server and click on "Networking" in the left menu) of the server and remove the firewall rule you created for your IP. 
8. As a best practice and ensuring extra security, click on Connectivity" tab under the "Networking" page for the SQL server. Set the minimum TLS-version to "TLS 1.2"
8. Build your connection string. Navigate to your database in Azure. Click on "Conenction strings" in the left menu. Copy the text for "ADO.NET (SQL authentication)" into an editor, you need to change the username and password before you add this to the KeyVault. After editing it should look something like this:
    > Ensure that you fill out you name for the server, database, username and password
    ```
    Server=tcp:{servername}.database.windows.net,1433;Initial Catalog={databasename};Persist Security Info=False;User ID={username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
    ```

## 6. Create Azure Storage Account 
1. Search and navigate to the "Storage Accounts" anc click "+ Click"
2. Select the Resource group that you just created. 
3. Select a name for the storage account, names can be only lower-case letters, max 24 characters. 
4. Location is recommended to be the same as where you are deploying your app to. 
5. Standard performance will do for this guide
6. Select "Redundancy" -> "Locally-redundant storage (LRS)"
7. Create the resource. The standard settings for the Storage Account is okay for this guide. If you are deploying this in a larger production app, explore how you can restrict networking to only be accessible from your apps. 
8. When the resrouces is created, go into it. 
    > It is possible to use Managed identity to access the Storage account from our app. This will require that you have configured the app to be able to use "DefaultAzureCredential" when running in Azure, and regular connection string when running the app locally. For this demo we will just use the connection string and connect the same way from the app as we would do in our local development enviroment. 
 9. Navigate to "Access keys" in the left menu of the Storage Account instance. Copy one of the connection strings. 

## 7. Configure the App Service App
1. Search and navigate to the "App Services" anc click "+ Click"
2. Select the Resource group that you just created. 
3. Select a name for the app. 
4. For "Publish" method select "Docker Container". 
5. Select the region you have planned to deploy you app to. It is recomended to have all your resources in the same region. 
6. Under Pricing plans -> Linux Plan -> Create a new plan. Give it a suitable name. 
7. Under Pricing plan select "B1" that will do for this demo. 
8. Click "Create". The rest of the default settings is okay for this demo. 
9. When the resource is created, go into it. 
10. Stop the app for now, you need to configure more before it is ready to run. 
11. Select "Identity" under "Settings" in the left menu of the Web App you just created. Switch status for System assigned to "On". Press "Save". 
12. Selett "Configuration" under "Settings" in the left menu. Click the tap "General settings".
13. Change the settings "HTTP version" dropdown to 2.0. 
14. Change the settings "Always on" to "On". 
15. The reset of the default settings should be fine. Press "Save"

## 8. Create an Azure Key Vault
1. Search and navigate to the "Key vaults" anc click "+ Click"
2. Select the Resource group that you just created. 
3. Select a name for the key vault. 
5. Select the region you have planned to deploy you app to. It is recomended to have all your resources in the same region. 
6. Click "Create". The rest of the default settings is ok for now. 
9. When the resource is created, go into it. 
10. Click on Secrets in your left menu, then add secrets for "+ Generate/Import". And add value and key. Repeat this for the following two values with keys starting with the following: 
    ```
    SQL-CONNECTION-STRING   ->  Server=tcp:testing-pck-sql-srv.database. ...
    AzureWebJobsStorage     ->  DefaultEndpointsProtocol=https;AccountNa ...
    ```
11. Click on "access Policy" in the left menu. Click "+ Create". Check "Get" under "Secret permissions". Click "Next"
12. In the "Principal" tab, write inn the name for your newly created app. Select the app. Click "Next" on the Application tab. Then click "Create".

## 9. Configure enviroment variables for the App Service
1. Navigate to the Web App you just created. 
2. Click on "Configuration" in the left menu. Add the following two values as new "Applicaiton settings"
    > Ensure to change the name to the key vault name you just created
    ``` 
    SQL_CONNECTION_STRING   ->  @Microsoft.KeyVault(VaultName={Key vault name};SecretName=SQL-CONNECTION-STRING)
    AzureWebJobsStorage     ->  @Microsoft.KeyVault(VaultName={Key vault name};SecretName=AzureWebJobsStorage)
    ``` 
    > Remember to save after you have added the values. 

## 10. Create an Azure registry
We need a registry to keep our docker app in. We will use Azure registry
1. Search and navigate to your list of "Container Registries" and click "+ Create"
2. Select the Resource group that you just created. 
3. Give the Registry name a descriptive name, registry names can be only lower-case letters. 
4. Location is recommended to be the same as where you are deploying your app to. 
5. The diffeerence between the Selecting SKU is mainly how much space you have included before you pay extra per GB. Standard will do for our guide.
6. Create the resource
7. When the resource is created, go into it. 
8. Navigate to the Container registry instance. Click on "Access keys", enable the "Admin user. 
9. Note down "Login server", "Username" and "Password". You will need this when you want to push image to your registry. 
10. Select "Identity" under "Settings" in the left menu of the Web App you just created. Switch status for System assigned to "On". Press "Save". 
11. A new button will appear, "Azure role assignments" will appear. Click on it. Click "+ Add role assignment"
12. Under Scope select "Resource group". This means that all app services in the given resources group shoudl be able to access to your secure registry without username or password. This is what we want. 
13. Ensure the resource group you have your resources inn is selected. 
14. Under "Role" select "AcrPull"
15. Press "Save"

## 11. Push image to the registry
> This can be done in many ways. For inspiration on how to set up CI/CD check this guide: [CI/CD Demo](https://github.com/pckofstad/ci-cd-demo)

I Recomend to have a CI/CD demo setup or an image ready to be pushed to this registry before you continue. 

## 12. Configure Azure App Service to use the correct container. 
1. Navigate to the App Service you just created. Click "Deployment Center" in the left menu. 
2. Ensure the source is "Container Registry"
3. Ensure container type is "single container"
4. Registry should be set to "Azure Container Registry"
5. Ensure you have selected the same subscription as you have the registery in. It would normally be the same as the one you are in. This is normally right for this guide. 
6. Under Auhtentication select "Managed Identity"
7. Ensure "System assigned" is selected under Identity
8. Under "Registry" select the Registry you just created. 
9. Under Image name and tag, select the values for the container you pushed to the registry. If you are using the  [CI/CD Demo](https://github.com/pckofstad/ci-cd-demo), then it does not matter what you write, because it will be overwritten by the next deploy command. 
10. The rest of the values can be default value. Press "Save"
11. You are now ready to start the app. You do this in the "Overview" page in the left menu on the "Web app". You app should now be ready to test. 

## 13. Create an Application Insights instance
1. Search and navigate to your list of "Log Analytics workspaces" and click "+ Create"
2. Select the Resource group as everything else.
3. Give the workspace a suitable name. 
4. Wait for the resouces to be created before you move on. 
5. Navigate to the WebApp you have just created. 
6. Click on "Applicaiotn Insights" in the left menu. Click "Turn on Applicaiotn Insights"
7. Select "Create new resource", give the Application Instance a suitable name. Select the same resource group as everything else. Select the Log Analytics workspace you just created. 
8. Click "Apply". This will create application insight, install an extention on the WebApp and add Application Setting variables to the applicaiton. 

### You should now be able to use your app that is configured with:
- SQL database and SQL Elastic-Pool, with dedicated db users for your database. 
- Azure Storage Account
- Azure KeyVault
- Azure App Service
- Docker Registry
- Application Insight
