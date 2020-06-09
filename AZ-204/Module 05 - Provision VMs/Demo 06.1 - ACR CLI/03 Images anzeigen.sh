# Images als Liste ausgeben
az.cmd acr repository list --name myregistry --output table

# Tags des Repositories ausgeben
az acr repository show-tags --name myregistry --repository aci-helloworld --output table