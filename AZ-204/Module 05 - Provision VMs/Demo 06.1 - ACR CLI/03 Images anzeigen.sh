# Images als Liste ausgeben
az.cmd acr repository list --name myregistrykairoth --output table

# Tags des Repositories ausgeben
az acr repository show-tags --name myregistrykairoth --repository aci-helloworld --output table