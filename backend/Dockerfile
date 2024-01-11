FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app/DragonsCrossing.Api

# Copy csproj and restore as distinct layers
RUN curl -L https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh  | sh
ARG FEED_ACCESSTOKEN
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS \
    "{\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/dragonscrossingdcx/b1a1952c-be26-46f6-8955-9dd354699472/_packaging/dcx-feed-1/nuget/v3/index.json\", \"password\":\"${FEED_ACCESSTOKEN}\"}]}"

COPY DragonsCrossing.Core/nuget.config ../DragonsCrossing.Core/nuget.config
COPY NewCombatLogic/nuget.config ../NewCombatLogic/nuget.config
COPY DragonsCrossing.Core/nuget.config ./nuget.config

COPY DragonsCrossing.Core/*.csproj ../DragonsCrossing.Core/
COPY NewCombatLogic/*.csproj ../NewCombatLogic/

ENV app__command=no_deploy
COPY DragonsCrossing.Api/*.csproj ./
RUN dotnet restore && rm ./nuget.config && rm ../DragonsCrossing.Core/nuget.config && rm ../NewCombatLogic/nuget.config

RUN dotnet new tool-manifest && dotnet tool install SwashBuckle.AspNetCore.Cli

# Copy everything else and build
COPY DragonsCrossing.Api ./
COPY DragonsCrossing.Core ../DragonsCrossing.Core
COPY NewCombatLogic ../NewCombatLogic
COPY DragonsCrossing.Domain ../DragonsCrossing.Domain
COPY DragonsCrossing.Infrastructure ../DragonsCrossing.Infrastructure

RUN dotnet publish -c Release -o out \
    && dotnet swagger tofile --output ./swagger.yml --yaml ./out/DragonsCrossing.Api.dll v2 


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/DragonsCrossing.Api/out .
COPY --from=build-env /app/DragonsCrossing.Api/swagger.* ./
#COPY --from=public-env /app/DragonsCrossing.Api.ts .

RUN mkdir wwwroot

ENTRYPOINT ["dotnet", "DragonsCrossing.Api.dll"]

