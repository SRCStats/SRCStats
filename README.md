# SRCStats

An ASP.NET MVC website for viewing stats on [Speedrun.com](https://speedrun.com) via the [API](https://speedrun.com/api/v1), the backend [_fedata](https://speedrun.com/_fedata), and the experimental [API v2](https://speedrun.com/api/v2) endpoints.

# Notice

The website is not properly launched on a public URL yet, as it's still in its pre-release form.

This code contains spoilers for the secret trophies achievable on the website. Files that are associated with those secrets will be prefixed by the following comment:
```csharp
//
//  THIS FILE CONTAINS SPOILERS FOR SECRET TROPHIES, PLEASE KEEP IT TO YOURSELF :D
//
```

Please avoid these files if you wish to not be spoiled on the trophies.

# Setup

Clone the repository and ensure you have Visual Studio Community 2022 and its `ASP.NET and web development` component installed; earlier versions of VSCommunity don't support targetting .NET Core 6. Additionally, ensure you have [Node.js](https://nodejs.org/en/) installed. If you plan to use a Node.js install on WSL, you'll have to select the "WSL" configuration for building and publishing.

Configure a new SQL instance, or use an existing one. (Developing locally is recommended to be with Microsoft SQL Server). Additionally, configure a new MongoDB instance, or use an existing one. ([Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/) w/ MongoDB API is used in production, but any will do.)
Run the following commands to set the environment variables:

*The {Value} fields are placeholders.*

```bat
setx SRC_STATS_SQL_CONNECTION_STRING {Connection_String}
setx SRC_STATS_MONGODB_CONNECTION_STRING {Connection_String}
```

In Package Manager Console, type the following commands:

```posh
Update-Package
Add-Migration Init
Update-Database
```

Finally, build the project through Visual Studio on the Debug configuration and you'll be good to go!

## Testing

To use the testing suite in SRCStats.Tests, or to add new tests, you'll need to configure the Tests project. Navigate to the `tests/` directory and enter the following commands:

```bat
dotnet build
powershell bin/Debug/net6.0/playwright.ps1 install
```

Finally, you'll be able to run the tests with `dotnet test`. To only run a specific test or group of tests, use NUnit's [--filter & FullyQualifiedName](https://learn.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests?pivots=nunit) arguments in the `dotnet test` command.

# Contributing
If you'd like to contribute to the project, thank you! We'd really appreciate your help. A good start would be to work on any issues marked as `good first issue` on the Issues tab. Simply fork the repository, create a branch for your fix, and let us know about it so we can link it to the issue! Please don't work on any issues that already have a branch or pull request linked. If you'd like to work on an issue that isn't marked with `good first issue`, let us know first so we can discuss the specifics!

If you have any issues building the site, with the site itself, or if you have a feature request, an idea to optimize API calls, or anything else, feel free to open an issue for it!

# License
This project is licensed under the [MIT](LICENSE) license.
