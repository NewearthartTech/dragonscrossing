# Introduction

DCX frontend

# To get the @dcx modules

- follow https://dev.azure.com/dragonscrossingdcx/Dragons%20Crossing/_artifacts/feed/dcx-feed-1/connect/npm
- copy the .npmrc.template to .npmrc
- use personal access token or use vsts-npm-auth -config .npmrc

# to dev

- copy config/.env.development.template to config/.env.development
- yarn run dev
  OR
- to run with local Server
  yarn dev:localServer

- If you get a 401 error when running the app on any backend api calls then try deleting your token from Local Storage in the browser

### DEPLOYING dev contracts

- copy deployedContracts.template.json to deployedContracts.json
- update your private key. This can be found on your metamask wallet. Click the ellipses and go to account details -> export private key

- Check docker logs ,  
   docker logs lib_dcx-server_1
  You will see that the server fails with `System.Exception: Contratcs addresses not configured. Please run will /app:Command deployContracts`

- When the server fails add the entry point to this file:
  .\node_modules\@dcx\dcx-backend\lib\docker-compose.server.yml
  Add this line under the "image" line
  entrypoint: ["tail", "-f", "/dev/null"]

- to deploy the contracts run
  docker exec -it lib-dcx-server-1 dotnet DragonsCrossing.Api.dll /app:Command deployContracts

- If you get an error ^ saying insufficient funds. Go to rinkeby faucet and set eth to your metamask wallet
- Once the above command succeeds comment out the "entrypoint" line in this file .\node_modules\@dcx\dcx-backend\lib\docker-compose.server.yml

- restart the server container
  docker restart lib-dcx-server-1

## Mint New NFT

### Old Way

- If you want to mint a new NFT
  docker exec -it lib_dcx-server_1 dotnet DragonsCrossing.Api.dll /app:Command MintHero /app:Account 0xXXXXXXXXX (Metamask Account address)

### New Way Using Json Template

- Copy the updated hero list json to the server
  docker cp ./hero-list.get.json lib-dcx-server-1:/hero-list.get.json

- Mint heroes using the heroes in the hero list json
  docker exec -it lib-dcx-server-1 dotnet DragonsCrossing.Api.dll /app:Command MintHero /app:Account 0xXXXXXXXXX (Metamask Account address) /app:file /hero-list.get.json
  docker exec -it lib-dcx-server-1 dotnet DragonsCrossing.Api.dll /app:Command MintHero /app:Account 0x5ceE5a5eeEE3bE08F7EF9Cad232584F06780e756 /app:file /hero-list.get.json

- If after you have minted a hero and you navigate to localhost:3000 you get an unexpected error. Check that the DcxHeroContract address in the deployedContracts.json matches the address you are using in the getMe api call in the verification.tsx file

### To connect to Azure container registry

- You will have to authenticate to the Azure container registry to run the server locally (https://docs.microsoft.com/en-us/azure/container-registry/container-registry-authentication?tabs=azure-cli)
  - ensure Azure cli in installed
    a) osx : (https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-macos) `brew update && brew install azure-cli`
    b) https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli
  - authenticate with azure
    `az login` (Connects to Microsoft account)
  - If this fails ^ go to portal.azure.com and complete the signin then try again
    `az acr login --name newearthcb.azurecr.io` (Logs in to the Azure Container Registry)

#

# to typecheck during dev

- in a seperate shell
- yarn run typeCheck

# Dump DB State to a file

- docker exec -it lib-mongo-1 bash
- mongodump --db dcx --archive=/testing.archive.gz --gzip
- docker cp lib-mongo-1:/testing.archive.gz .

# View logs in Kubernetes

- "dir {path to kubeconfig folder}"
- "dir {path to kubeconfig yaml file}"
- "set KUBECONFIG={path to kubeconfig yaml file}"
- "kubectl -n dcx-staging get pods
- "kubectl -n dcx-staging logs --tail 10 {pod/container name}"

# To view Contract addresses hit this endpoint:

- http://localhost:8080/api/web3/gameConfig

# season 3 launch
skills were at level 1 we will bring them to same numbering as season.

- Need to save season skills to perpetual
- save season 4 skills to perpetual ->   in maintenance Q
{
        "messageType": [
            "urn:message:DragonsCrossing.Core.Sagas:RunMaintenanceMessage"
        ],
        "message": {
            "maintenanceType": 0,
            "seasonId": 4
        }
}

- update skills from template -> in maintenance Q
{
        "messageType": [
            "urn:message:DragonsCrossing.Core.Sagas:RunMaintenanceMessage"
        ],
        "message": {
            "maintenanceType": 1,
            "newSkillsVersion": 3
        }
}

## current alchemy key is XXXXXXXXXXXXXXXXX
uses dee@newearthart.tech
