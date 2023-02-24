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

## Steps

### 1. Create an Azure SQL elastic pool
The first step is to create an Azure SQL elastic pool. To do this:
1. Navigate to the Azure portal and click on the "Create a resource" button.
2. Search for "SQL elastic pool" and select the appropriate option.
3. Follow the steps to create a new elastic pool with the desired configuration settings.

### 2. Create two Azure SQL databases
Next, you need to create two Azure SQL databases within the elastic pool. To do this:
1. In the Azure portal, navigate to the elastic pool you created in step 1.
2. Click on the "Add database" button and select the appropriate options to create two databases within the pool.

### 3. Configure custom usernames and passwords for each database
Now that you have two databases within your elastic pool, you need to configure custom usernames and passwords for each database. To do this:
1. In the Azure portal, navigate to one of the databases you created in step 2.
2. Click on the "Connection strings" tab and note the server name and database name.
3. Click on the "Set server firewall" button and configure the firewall settings as necessary.
4. Click on the "Set active directory admin" button and configure the admin settings as necessary.
5. Click on the "Set database-level firewall rules" button and configure the firewall rules as necessary.
6. Navigate to the "Users" tab and add a new user with a custom username and password.
7. Repeat steps 1-6 for the second database within the elastic pool.

### 4. Create an Azure Key Vault
The next step is to create an Azure Key Vault to store the credentials and secrets for accessing the Azure SQL databases. To do this:
1. Navigate to the Azure portal and click on the "Create a resource" button.
2. Search for "Key Vault" and select the appropriate option.
3. Follow the steps to create a new key vault with the desired configuration settings.

### 5. Configure access to the Azure SQL databases
Now that you have an Azure SQL elastic pool, two databases within the pool, and a Key Vault, you need to configure access to the databases. To do this:
1. In the Azure portal, navigate to the Key Vault you created in step 4 and click on the "Access policies" tab.
2. Click on the "Add Access Policy" button and select the appropriate options to grant access to the Azure App Service App. 
3. Repeat steps 1-2 for each of the two databases within the elastic pool.

### 6. Create an Application Insights instance
The next step is to create an Application Insights instance to monitor your App Service App. To do this:
1. In the Azure portal, click on the "Create a resource" button and search for "Application Insights".
2. Follow the steps to create a new instance with the desired configuration settings.

### 7. Configure the App Service App
Now that you have created the necessary resources, you can configure your App Service App.
