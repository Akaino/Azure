# Admin User aktivieren
az acr update --name myregistrykairoth --admin-enabled true

# Password abfragen
az acr credential show --name myregistrykairoth --query "passwords[0].value"

# Deploy container image
az container create --resource-group "az204-2" --name "test" --image myregistrykairoth.azurecr.io/aci-helloworld:v1 --cpu 1 --memory 1 --registry-username myregistrykairoth --registry-password "" --dns-name-label "" --ports 80

# Container Status anzeigen
az container show --resource-group "az204-2" --name "test" --query instanceView.state
