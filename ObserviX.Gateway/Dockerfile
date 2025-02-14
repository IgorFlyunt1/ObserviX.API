FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["ObserviX.Gateway.csproj", "./"]
COPY ["ObserviX.Shared/ObserviX.Shared.csproj", "../ObserviX.Shared"]
RUN dotnet restore "ObserviX.Gateway.csproj"
COPY . .

RUN dotnet build "ObserviX.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ObserviX.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

ARG AZURE_APPCONFIGURATION__CONNECTIONSTRING
ENV AZURE_APPCONFIGURATION__CONNECTIONSTRING=${AZURE_APPCONFIGURATION__CONNECTIONSTRING}
RUN echo "AZURE_APPCONFIGURATION__CONNECTIONSTRING: $AZURE_APPCONFIGURATION__CONNECTIONSTRING"

COPY --from=publish /app/publish .
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "ObserviX.Gateway.dll"]
