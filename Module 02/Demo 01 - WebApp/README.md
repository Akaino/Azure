# Simple WebApp example

###### Instructions
- Compile and run with `dotnet watch run`
- Open your browser on `https://localhost:5001`
  - Ignore security warnings

###### Create your own app
To create a WebApp in VS Code open a Terminal and run
> dotnet new webapp -o `outputDirectory`

###### Relase the ~Kraken~ App!
To release an app run
> dotnet publish -c Release

To publish the WebApp to Azure:
- Install the App Service Extension for Visual Studio Code
  - As of the time writing this extension is `preview`. _Should_ work anyway.
- Right click the `publish` directory (bin\Release\publish) and choose `Deploy to Web App`
- Click `Create new Web App`
  - Choose a name


###### Problems
There may be issues like `You do not have permission to view this directory or page.`

If such error occurs please make sure you're deploying the `publish` directory only. Not the `Release` directory!

This can be changed in `.vscode\settings.json`.
Change `"appService.deploySubpath": "bin\\Release"` to `"appService.deploySubpath": "bin\\Release\\netcoreapp3.1\\publish"`.
