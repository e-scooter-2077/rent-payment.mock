# Azure Function Template
## Quickstart
* After using the template, rename the solution with the name of your Function or Function App.
* Create an Azure Function project inside the solution and implement it.
* Add the code below to the function's `.csproj` file, substituting `path-to-repository-root` with the relative path to the root of the repository (where the `.stylecop.json` file is located).
If done correctly, you should see the `.stylecop.json` file imported inside the project in Visual Studio
```xml
<PropertyGroup>
  <RootFolder>path-to-repository-root</RootFolder>
</PropertyGroup>
```
* Setup on [Azure Portal](https://portal.azure.com/#home) the required resources:
  * A resource group
  * A storage account
  * A function app with .NET runtime and other configuration (function version required is 3)
* Change the runtime version from `dotnet` to `dotnet-isolated` with this command, run it in the Azure CLI or **Azure Cloud Shell**:
```
az functionapp config appsettings set --name <your-azure-app-name> --resource-group <your-resource-group> --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated
```
* Setup the `AZURE_CREDENTIALS` secret, either as a repository secret or as an organization secret. 
* Setup the CI `env` parameters `RELEASE_PREFIX` (any string will do) and `AZURE_FUNCTIONAPP_NAME` (the name of the function app on Azure)

# Launch CI
To trigger the CI for a release, run the following commands inside the repository's directory:

```
git tag -a v#.#.# -m "v#.#.#"
git push origin v#.#.#
```

Alternatively, you can use the [git-release](https://github.com/francescodente/git-release) tool.