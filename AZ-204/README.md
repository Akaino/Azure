# Azure - Examples

###### Introduction
As I'm not a huge fan of Microsofts MOC Labs I created this repository to have a bunch of examples to the different Modules in the AZ-204 course.
It's intended for the students to have something to get started during and after the course.

All examples are created for dotnet core `3.1`.

###### Preparation
Some examples make use of the `authfile.json` in the `Credentials` directory.
To easily create such file make use of use
> az ad sp create-for-rbac --sdk-auth > authfile.json

Also, many (if not all) examples make use of a basic `azureConfig.json` in their root directory.

###### Usage
Create `authfile.json` or edit the `azureConfig.json` where it's present.
Compile. Hope for no erros.
Run.

###### Disclaimer
Some Deployments are rather cost intensive. 
Use with caution!

Also, don't forget to remove unused resources.

###### Helpful Resources
<a href="https://docs.microsoft.com/de-de/azure/storage/common/storage-cors-support">Cross Origin Resource Sharing (CORS) in Azure Services</a>
<a href="https://docs.microsoft.com/de-de/azure/app-service/overview-local-cache">App Service Local Cache</a>