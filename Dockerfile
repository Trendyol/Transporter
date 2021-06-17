FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

RUN apt-get update
RUN apt-get install -qqy sqlite3 libsqlite3-dev

WORKDIR /workdir

COPY . .

RUN dotnet publish "Transporter.Service.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /publish
COPY --from=build-env /publish /publish
ENTRYPOINT ["dotnet", "Transporter.Service.dll"]
