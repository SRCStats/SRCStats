# SRCStats

An ASP.NET MVC website for viewing stats on [Speedrun.com](https://speedrun.com) via the [API](https://speedrun.com/api/v1) and the backend [_fedata](https://speedrun.com/_fedata).

# Notice

The website is not properly launched on a public URL yet, as it's still in its pre-release form.

This code contains spoilers for the secret trophies achievable on the website. Files that are associated with those secrets will be prefixed by the following comment:
```csharp
//
//  THIS FILE CONTAINS SPOILERS FOR SECRET TROPHIES, PLEASE KEEP IT TO YOURSELF :D
//
```

Please avoid these files if you wish to not be spoiled on the trophies.

Additional warning: This code isn't very good. There's a lot of unimplemented features, unstylized HTML, and general bad code. Please view at your own risk.

# Setup

Clone the repository and ensure you have Visual Studio Community 2022 and its `ASP.NET and web development` component installed; earlier versions of VSCommunity don't support targetting .NET Core 6.

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

Finally, build the project through Visual Studio and you'll be good to go!

# License
This project is licensed under the [MIT](LICENSE) license.
