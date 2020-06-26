# Example-ServiceDependencies-CoreImageGallery
This application demonstrates:
  - How to use dependency injection in Azure Function v3 projects to have a startup.cs file like in ASP.NET Core
  - How to use the Connected Services tab to configure dependencies for ASP.NET Core projects (serviceDependencies.json file included)
  - How to use the Publish tab to configure dependencies on Azure services for ASP.NET Core projects


Image Gallery
5 Azure Services Sample

Azure Services
- App Service (Web App)
- Functions
- Application Insights
- SQL
- Storage

Technologies
- ASP.NET Core 3.1
- Azure Functions v3
- Visual Studio 2019
- Entity Framework
- Azure

Getting Started
1) Clone the repo
2) Run EF migrations by executing 'dotnet ef database update'
3) Start the storage emulator
4) F5 the CoreImageGallery project
5) Right-click > Debug > Start new instance the CoreImageGallery.AzureFunctions project