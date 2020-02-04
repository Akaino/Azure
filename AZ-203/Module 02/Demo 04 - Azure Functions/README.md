# Azure Functions
###### Preparations
- Install the Azure Functions Extension for Visual Studio Code

- (optional) Install Azure Function Core Tools to debug your function locally
  - _Did not work for me._
  - `https://docs.microsoft.com/de-de/azure/azure-functions/functions-run-local?tabs=windows`
    - (`npm install -g azure-functions-core-tools`)
    - `npm install -g azure-functions-core-tools@3`
      - To install npm for Windows visit `https://github.com/coreybutler/nvm-windows`
###### Instructions
- Click the Azure button from the sidebar
- Choose `FUNCTIONS`
  - Choose your subscription
  - Click `Create Function...`
  - Click through the helper, choose name etc.
  - Say `yes` to the workspace creation
  - Restore depencies with `dotnet restore`
- (optional) Run with `F5`
- Compile and publish the function