#!/bin/bash

# Login to Azure
az login

# Replace the following URL with a public GitHub repo URL
gitrepo=https://github.com/Azure-Samples/php-docs-hello-world
#gitrepo=https://github.com/phpbb/phpbb

webappname=mywebapp$RANDOM

# Create a resource group.
az group create --location "<location>" --name "<resourceGroup>"
# Create an App Service plan in `FREE` tier.
az appservice plan create --name $webappname --resource-group "<resourceGroup>" --sku FREE
# Create a web app.
az webapp create --name $webappname --resource-group "<resourceGroup>" --plan $webappname
# Deploy code from a public GitHub repository.
az webapp deployment source config --name $webappname --resource-group "<resourceGroup>" --repo-url $gitrepo --branch master --manual-integration
# --manual-integration Disables auto sync between source control and web
# https://docs.microsoft.com/en-us/cli/azure/webapp/deployment/source?view=azure-cli-latest#az-webapp-deployment-source-config

# Copy the result of the following command into a browser to see the web app.
echo http://$webappname.azurewebsites.net


# For PHP
# Web App Configuration
# General -> PHP 7
# Path Mappings -> / = ../wwwRoot/phpBB
# Console -> cd phpBB -> php ../composer.phar install
