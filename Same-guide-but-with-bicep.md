

Following the guiode

# Setting up the same resouces with Bicep scripts





## 1. First, create a Bicep template file for the Azure SQL elastic pool. This file should include the following code:

```Bicep
param serverName string
param poolName string
param poolCapacity int
param poolMaxSize string

resource sqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: serverName
  location: resourceGroup().location
  properties: {
    administratorLogin: 'adminlogin'
    administratorLoginPassword: 'adminpassword'
  }
}

resource elasticPool 'Microsoft.Sql/servers/elasticPools@2021-02-01-preview' = {
  name: '${sqlServer.name}/${poolName}'
  properties: {
    dtu: poolCapacity
    maxSizeBytes: poolMaxSize
  }
}
```

## 2. Deploy the Bicep template file using the Azure CLI with the following command:

```PowerShell
az deployment group create --resource-group <resource-group-name> --template-file <path-to-bicep-template-file> --parameters serverName=<sql-server-name> poolName=<elastic-pool-name> poolCapacity=<elastic-pool-capacity> poolMaxSize=<max-pool-size>
```

## 3. Next, create a Bicep template file for the Azure Storage Account Blob Storage. This file should include the following code:

```Bicep
param accountName string
param containerName string

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: accountName
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    enableHttpsTrafficOnly: true
  }
}

resource storageContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-06-01' = {
  name: '${storageAccount.name}/${containerName}'
  properties: {
    publicAccess: 'None'
  }
}
```

## 4. Deploy the Bicep template file using the Azure CLI with the following command:

```Powershell
az deployment group create --resource-group <resource-group-name> --template-file <path-to-bicep-template-file> --parameters accountName=<storage-account-name> containerName=<storage-container-name>
```

## 5. Next, create a Bicep template file for the Azure App Service App with Key Vault integration and managed identity enabled. This file should include the following code:

```Bicep
param appName string
param appPlanName string
param keyVaultName string
param keyVaultSecretName string

resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: appPlanName
  location: resourceGroup().location
  sku: {
    name: 'S1'
    tier: 'Standard'
  }
}

resource webApp 'Microsoft.Web/sites@2021-02-01' = {
  name: appName
  location: resourceGroup().location
  dependsOn: [
    appServicePlan
  ]
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    identity: {
      type: 'SystemAssigned'
    }
    siteConfig: {
      appSettings: [
        {
          name: 'VaultUri'
          value: '@Microsoft.KeyVault(SecretUri=https://$(keyVaultName).vault.azure.net/secrets/$(keyVaultSecretName))'
        }
      ]
    }
  }
}

resource keyVaultAccessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2019-09-01-preview' = {
  name: 'addWebAppMSI'
  properties: {
    objectId: reference(resourceId('Microsoft.Web/sites', webApp.name), '2018-02-01', 'Full').identity.principalId
    permissions: {
      secrets: [
        'get'
      ]
    }
    tenantId: subscription().tenantId
  }
}

output webAppIdentityPrincipalId string = webApp.identity.principalId

 ```

 ## 5. Deploy the Bicep template file using the Azure CLI with the following command:
 
 ```PowerShell
az deployment group create --resource-group <resource-group-name> --template-file <path-to-bicep-template-file> --parameters appName=<app-name> appPlanName=<app-service-plan-name> keyVaultName=<key-vault-name> keyVaultSecretName=<key-vault-secret-name>

 ```