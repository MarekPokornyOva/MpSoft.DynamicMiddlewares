# MpSoft.DynamicMiddlewares

[![Package Version](https://img.shields.io/nuget/v/MpSoft.DynamicMiddlewares.svg)](https://www.nuget.org/packages/MpSoft.DynamicMiddlewares)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MpSoft.DynamicMiddlewares.svg)](https://www.nuget.org/packages/MpSoft.DynamicMiddlewares)
[![License](https://img.shields.io/github/license/MarekPokornyOva/MpSoft.DynamicMiddlewares.svg)](https://github.com/MarekPokornyOva/MpSoft.DynamicMiddlewares/blob/master/LICENSE)

### Description
DynamicMiddlewares allows to register and unregister middlewares dynamically after application start.

### Reason
Some projects needs to have a middleware returning a temporary status. After the status dismiss, it's useless to have the middleware in HTTP request processing pipe.

### Usage
1) Include Nuget package - https://www.nuget.org/packages/MpSoft.DynamicMiddlewares/ in application's project.
2) Register services - call services.AddDynamicMiddlewares(); within during startup.
3) Configure middlewares - see \Samples\01\InvalidConfiguration\Startup.cs.

### Sample usage
Default Sample configuration contains value treated by application as invalid. Therefore the application returns "Invalid configuration" message for every call. To fix that and let application to work properly by bringing controllers to use, enter a value to "Success" property in appsettings.json.

### Release notes
[See](./ReleaseNotes.md)
