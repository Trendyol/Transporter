FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

COPY . .
WORKDIR /Transporter

RUN dotnet publish "Transporter.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Transporter.dll"]