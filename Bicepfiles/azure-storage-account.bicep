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
