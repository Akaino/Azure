# Create resource group
az group create --name "<groupName>" --location "<location>"

# Create aks cluster
az aks create --resource-group "aksexample" --name "akscluster" --node-count 1 --enable-addons monitoring --generate-ssh-keys

# Install ask cli (kubectl)
#az.cmd aks install-cli

# Get aks credentials
az aks get-credentials --resource-group "aksexample" --name "mycluster"

# Use kubectl to list nodes
kubectl get nodes