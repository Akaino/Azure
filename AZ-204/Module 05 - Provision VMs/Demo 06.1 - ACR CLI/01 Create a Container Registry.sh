# Eine Container Registry Instanz erstellen
az acr create --resource-group "<groupName>" --name "<registryName>" --sku Basic

# Login
az acr login --name "<registryName>"
