# Build des Containers
docker build ./aci-helloworld -t aci-tutorial-app

# Nach dem Buildvorgang schauen wir uns die Images an
docker images

# Container lokal laufen lassen
docker run –d -p 8080:80 aci-tutorial-app

# Laufende Container anzeigen
docker container ls -a

# Namen des Loginservers abfragen
az acr show --name <acrName> --query loginServer

# ACR Password abfragen
az acr credential show --name <acrName> --query "passwords[0].value"

# Container bereitstellen
az container create --resource-group AZ204 --name helloworld --image registrykr.azurecr.io/helloworld:v1 --cpu 2 --memory 4 --registry-login-server registry.azurecr.io --registry-username registrykr --registry-password 0000 --dns-name-label mydnss --ports 80

# Fortschritt prüfen
az container show --resource-group myResourceGroup --name aci-tutorial-app --query provisioningState

# Anwendungs-URL abfragen
az container show --resource-group myResourceGroup --name aci-tutorial-app --query ipAddress.fqdn

# Container Logs einsehen
az container logs --resource-group myResourceGroup --name aci-tutorial-app
