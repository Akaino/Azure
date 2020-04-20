# login to Azure if you haven't already
# az login

# variables
location=""
vmName=""
resourceGroup=""

# Create the vm
az vm create \
    --location $location \
    --resource-group $resourceGroup \
    --name $vmName #\
    #--image win2016datacenter \ # optional
    #--admin-username azureuser \ # optional
    #--admin-password myPassword # optional