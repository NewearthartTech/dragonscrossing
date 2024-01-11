#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app/

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

RUN curl -L https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh  | sh
ARG FEED_ACCESSTOKEN
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS \
    "{\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/dragonscrossingdcx/b1a1952c-be26-46f6-8955-9dd354699472/_packaging/dcx-feed-1/nuget/v3/index.json\", \"password\":\"${FEED_ACCESSTOKEN}\"}]}"

COPY DragonsCrossing.Core/nuget.config ./DragonsCrossing.Core/nuget.config
COPY NewCombatLogic/nuget.config ./NewCombatLogic/nuget.config
COPY DragonsCrossing.Core/nuget.config ./createTemplates/nuget.config

COPY DragonsCrossing.Core/*.csproj ./DragonsCrossing.Core/
COPY NewCombatLogic/*.csproj ./NewCombatLogic/

COPY ["createTemplates/createTemplates.csproj", "createTemplates/"]
RUN dotnet restore "createTemplates/createTemplates.csproj" && rm ./createTemplates/nuget.config && rm ./DragonsCrossing.Core/nuget.config && rm ./NewCombatLogic/nuget.config

COPY . .
WORKDIR "/src/createTemplates"
RUN dotnet build "createTemplates.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "createTemplates.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "createTemplates.dll"]
