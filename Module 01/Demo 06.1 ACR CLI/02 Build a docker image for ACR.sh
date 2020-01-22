# Docker Image laden
docker pull microsoft/aci-helloworld

# Login Server Namen der ACR Instanz abfragen
az acr list --resource-group "<groupName>" --query "[].{acrLoginServer:loginServer}" --output table

# Image mit diesem Namen Taggen
docker tag microsoft/aci-helloworld "<acrLoginServer>"/aci-helloworld:v1

# Image in die Registry pushen
docker push "<acrLoginServer>"/aci-helloworld:v1

# Create a Dockerfile with meaningful build instructions
echo FROM hello-world > Dockerfile

# Build the image
az acr build --image aci/helloworld:v1 --registry acr1301 --file Dockerfile .