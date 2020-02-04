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
az container create --resource-group myResourceGroup --name aci-tutorial-app --image <acrLoginServer>/aci-tutorial-app:v1 --cpu 1 --memory 1 --registry-login-server <acrLoginServer> --registry-username <acrName> --registry-password <acrPassword> --dns-name-label <aciDnsLabel> --ports 80

# Fortschritt prüfen
az container show --resource-group myResourceGroup --name aci-tutorial-app --query provisioningState

# Anwendungs-URL abfragen
az container show --resource-group myResourceGroup --name aci-tutorial-app --query ipAddress.fqdn

# Container Logs einsehen
az container logs --resource-group myResourceGroup --name aci-tutorial-app
