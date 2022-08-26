FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /home/srcstats/site

COPY *.csproj .
RUN dotnet restore

RUN dotnet tool install -g dotnet-ef --version 6.0.7
ENV PATH $PATH:/root/.dotnet/tools

COPY . .

ENV SRC_STATS_SQL_CONNECTION_STRING dummy

RUN dotnet ef migrations add Init

RUN dotnet publish -c Release -o /SRCStats

FROM mcr.microsoft.com/dotnet/aspnet:6.0.7
WORKDIR /home/srcstats/site
COPY --from=build /SRCStats .
ENTRYPOINT ["dotnet", "SRCStats.dll"]