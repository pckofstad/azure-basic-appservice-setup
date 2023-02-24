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
