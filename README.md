# ServerDemo

This demonstrates several server and protocol capabilities. It does not demonstrate best practice.

This was built using Visual Studio 2019 and .NET Core 3.0. https://dotnet.microsoft.com/download

This can be run in IIS Express, IIS, or Http.Sys. Http.Sys urls are configured in the appsettings.json file. Console and file logging are also configured in appsettings.json.

The mock fourm persists posts to an xml file in the application root by default. This can be moved by setting "Forum:StorePath" in the appsettings.json. The application will need write access to this file.