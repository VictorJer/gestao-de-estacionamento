FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY backend/ backend/
RUN dotnet publish backend/tools/Estacionamento.Provisioning/Estacionamento.Provisioning.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Estacionamento.Provisioning.dll"]
