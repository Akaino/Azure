# Images als Liste ausgeben
az.cmd acr repository list --name "<acrName>" --output table

# Tags des Repositories ausgeben
az.cmd acr repository show-tags --name "<acrName>" --repository "<repositoryName>" --output table