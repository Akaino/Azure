# Resorce Group
az.cmd group create --name "MyRG" --location "northeurope"
# Deploy from template
az.cmd group deployment create --name "MyDeployment" --resource-group "MyRG" --template-file "azuredeploy.json"
# Show storage account
az.cmd storage account show --resource-group "MyRG" --name "MyStorageAcc"
