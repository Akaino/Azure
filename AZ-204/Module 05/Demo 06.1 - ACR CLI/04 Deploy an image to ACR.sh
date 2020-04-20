# Admin User aktivieren
az acr update --name "<acrName>" --admin-enabled true

# Password abfragen
az acr credential show --name "<acrName>" --query "passwords[0].value"

# Deploy container image
az container create --resource-group "<group>" --name "<acr-containerName>" 
--image "<acrLoginServer>"/aci-helloworld:v1 --cpu 1 --memory 1 
--registry-username "<acrName>" --registry-password "<acrPassword>" 
--dns-name-label "<fqdn>" --ports 80

# Container Status anzeigen
az container show --resource-group "<groupName>" --name "<acr-containerName>" --query instanceView.state
