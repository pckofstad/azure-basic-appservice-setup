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
