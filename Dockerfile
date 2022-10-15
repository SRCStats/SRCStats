FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /home/srcstats/site

COPY SRCStats.sln .
COPY src/SRCStats.csproj ./src/
RUN dotnet restore

RUN dotnet tool install -g dotnet-ef --version 6.0.8
ENV PATH $PATH:/root/.dotnet/tools

COPY . .

ENV SRC_STATS_SQL_CONNECTION_STRING dummy

RUN dotnet ef migrations add Init --project src/SRCStats.csproj

RUN dotnet publish -c Release -o /SRCStats

FROM mcr.microsoft.com/dotnet/aspnet:6.0.8
WORKDIR /home/srcstats/site
COPY --from=build /SRCStats .
ENTRYPOINT ["dotnet", "SRCStats.dll"]