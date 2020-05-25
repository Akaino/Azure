# Eine Container Registry Instanz erstellen
az acr create --resource-group "<groupName>" --name "<registryName>" --sku Basic

# Login
az.cmd acr login --name "<registryName>"
