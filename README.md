# Forge.Yoda
Forge.Yoda is a set of example projects about the following topics:
- How can you make a shared UI code for your WASM clients
- How can you implement a server side JWT (JSON Web Token) based authentication
- Do authentication with JWT (JSON Web Token) in your WASM clients


## Installing
After you cloned the source code, restore the NuGet packages with Visual Studio.

## Conception
The purpose of this solution to demonstrate, how it is possible to make a shared UI code base for every type of projects,
like ASP.NET Hosted Core, MAUI, WinForms and WPF applications.
The solution is the WASM/Blazor hybrid projects. In MAUI, WinForms and WPF projects, I use BlazorWebView component to
run the UI.
Everything work with Dependency Injection, so in the most cases it is possible to use the .NET built-in services,
but sometimes it is really necessary to implement a platform/technology specific service solution.

A very basic example for this situation in MAUI the HttpClient handler, which is a bit different on Android.
Check the project "Forge.Yoda.Apps.MAUI" -> MauiProgram.cs, where you can see what is going on on Android.


## Content

There are four types of projects are exists in the solution:
- Shared UI code base
- Shared code base
- Server side JWT authentication
- WASM client applications


## Content - Shared UI Code base
Projects:
- Forge.Yoda.Shared.UI.Core
- Forge.Yoda.Shared.UI

### Forge.Yoda.Shared.UI.Core
Contains the wwwroot folder which will be automatically copied into the client projects.
Put you css and js content here and link them in the index.html file. Try to keep the index.html file
content on the same as for each WASM client types, if this is not possible for any reasons,
you can create a project specific copy for the clients. Keep in mind, this scenario may increases the maintenance tasks.

### Forge.Yoda.Shared.UI
This is a Razor library project, contains the Blazor controls for WASM Hybrid clients.
This library also contains a simple example, how to use the client side authantication of JWT authentication.
The applied solution based on "Forge.Security.Jwt.Client" projects, which supports single and distributes scenarios also. This is very useful in a microservice architecture also.


## Server-side JWT Authentication
The project name which covers this topic is "Forge.Yoda.Services.Authentication". This is a very common and basic WebAPI project, using Microsoft Authentication service.
The authentication system is extended with JWT token management services. The most important points are the AuthController and the UserService.

- AuthController: handles the authentication, re-newal and validation requests. A successfully authentication results Access and Refresh tokens. Access tokens contains the most
important informations for the authentications, like Claims for example. The Refresh token is support the renewal of the access token. If the token is near at expiration,
client side can renew the access token using the refresh token. With this technique, we can avoid to send back the user always to the login screen, if the access token expires.
The client authenticated session can run smoothly and interrupt free.

- UserService: this service handles the login request from the AuthController, creates the set of Claims which I would like to add to the Access Token, finally sends the information to the generator.
The result will be a JwtTokenResult object, with an access token, a refresh token and the expire time.


## WASM client applications
The most tough parts are the different client applications. The way to make sure, the UI code base will be the same and will work on every platform.
I have created examples for the implementations for the following client types:
- ASP.NET Core Hosted client
- MAUI
- WinForms
- WPF

These projects are WASM / Blazor hybrid projects, so the UI is a web site (HTML, CSS, JS). Everything is hosted in a browser as a Web Assembly content. However in the desktop applications (WinForms, WPF, MAUI Windows) require
a WebView to show and run the WASM.


## Configuration of Forge.Yoda.Services.Authentication service
Open the Database\DatabaseContext.cs and appsettings.json files. Modify the credentials as it is necessary on your side, create your own database.
In the developer command prompt or in Package Manager Console (Tools -> NuGet Package Manager -> Package Manager Console) initialize your database.
Select "Default project" in the window as "Forge.Yoda.Services.Authentication", than type in the command prompt:

Update-Database

If your configuration and database are properly pre-configured, the database schema will be created.


## Startup
It is recommended to configure the solution startup preferences. Right click on the solution and select "Set Startup Projects..." from the context menu. Choose the "Multiple startup projects",
and always set the Action to "Start" for the project "Forge.Yoda.Services.Authentication". You can do the same for the "client" applications as you requested, one or more...

Forge.Yoda.Services.Authentication service will create the administrative account for the first time. For the default set credentials, check the source code at Codes\InititalizationAtStartup.cs file.

When start the solution, the one of the app windows will be a browser with a SwaggerUI for the Authentication service. This UI is applied for testing purpose and it is available in development mode.
The other windows are depends on your project startup settings.
