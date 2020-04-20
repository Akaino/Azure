# Create resource group
az group create --name "<groupName>" --location "<location>"

# Create aks cluster
az aks create --resource-group "<groupName>" --name "<clusterName>" --node-count 1 --enable-addons monitoring --generate-ssh-keys

# Install ask cli (kubectl)
#az.cmd aks install-cli

# Get aks credentials
az aks get-credentials --resource-group "<groupName>" --name "<clusterName>"

# Use kubectl to list nodes
kubectl get nodes