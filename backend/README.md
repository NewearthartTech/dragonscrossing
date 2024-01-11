# DragonsCrossing
Crypto Game

# to start local sql
docker-compose -f docker-compose.localsql.yml up -d

# to deploy contrats
Run with /app:command deployContracts

#to mint heros
/app:Command minthero /app:file /Users/dee/downloads/hero-list.get.json /app:Account 0xXXXXXX 

# add Item to Hero Inventory
/app:Command addheroinventory /app:HeroId 1 /app:file /Users/dee/downloads/testItem.get.json



# Getting Started
1. copy DragonsCrossing.Api/appsettings.Development.template.json DragonsCrossing.Api/appsettings.Development.json and modify to suit
2. start mongo and rabbit mq using `docker-compose -f docker-compose.dev.yml up -d`
3.  If you have the error "Unable to find package ColourBox.cbContract" then go to this link https://docs.microsoft.com/en-us/azure/devops/artifacts/nuget/dotnet-setup?view=azure-devops 
	Follow the instructions and install the required packages
	- Download and install the .NET Core SDK
	- Download and install Azure Artifacts Credential Provider (Follow this link with respect to your OS https://github.com/microsoft/artifacts-credprovider#azure-artifacts-credential-provider)

# build and deploy docker image
- in the DragonsCrossingApi folder
docker build --platform linux/amd64 -f Dockerfile.whitelistBot -t newearthcb.azurecr.io/dcx-whitelistbot:1.1 . 

- to test locally
docker run -it --rm  newearthcb.azurecr.io/dcx-whitelistbot:1.1

## push and deploy
- we are not including build and  for this in CI/CD for now. so
docker push newearthcb.azurecr.io/dcx-whitelistbot:1.1

- ensure we are connected to the right cluster
kubectl get namespaces |grep dcx-prerelease
kubectl apply -f deploy-WhiteListBot.yaml

## for game balance branch
- merge latest api branch to game-balance branch
- update balance-frontend-version.txt with front-end  version
- update toBalance.k8.template.yml with new contracts

## Season Control
- show all open season
http://localhost:5264/api/season/openSeasons

- show all open and closed seasons
http://localhost:5264/api/season/openSeasons/all

- show season by id
http://localhost:5264/api/season/openSeasons/2

- load season
http://localhost:5264/api/season/loadSeason/season1.json

# k8 deploy of Mongo4.4
ensure to call rs.initiate(); to intialize the replica set

"One or more errors occurred. (insufficient funds for gas * price + value: address 0x63ea8bBC62912ee0ac5aA0820143D21Ed2124674 have
23840802900000000 want
130315281600000000: eth_sendRawTransaction)"

#space out item NFT
use dcx_perpetual
db.GeneratedSequences.find().pretty();
{ "_id" : "NftizedItem", "lastId" : 10 }


# Mainnet Mongo cluster
VPC

username = doadmin
password = 7B58JN09ES4lWp21 hide
host = mongodb+srv://private-db-dcx-mainnet-a75ac727.mongo.ondigitalocean.com

username = doadmin
password = <replace-with-your-password>
host = mongodb+srv://private-db-dcx-mainnet-a75ac727.mongo.ondigitalocean.com
database = admin


# dee current notes
docker exec -it dragonscrossingapi_mongo_1 mongodump --gzip  --db dcx_season_1  --archive=/b4chance.gz
docker cp dragonscrossingapi_mongo_1:/b4chance.gz ~/tmpStuff/b4chance.gz

docker exec -it dragonscrossingapi_mongo_1 bash
mongo
use dcx_season_1
db.dropDatabase();
exit
mongorestore --gzip  --archive=/b4chance.gz
exit



-- images to change
/Users/dee/dragonCross/DragonsCrossingWeb/public/img/miscellaneous/wonderingWizard.jpg
/Users/dee/dragonCross/DragonsCrossingWeb/public/img/miscellaneous/rustingArandomWeapon.jpg

--- fixDie
- for wonderingwiz
combatOrNonCombat:90
GenerateRandomChanceEncounter:4

//to loose arm wrestle
ChanceEncounterGoodRoll:90


-Rusting A Random Weapon
GenerateRandomChanceEncounter:4



-Riddler
GenerateRandomChanceEncounter:5


-lovecraftianMonster
GenerateRandomChanceEncounter:6

# To clear dcx orders
use dcx_perpetual
db.DcxOrders.find({fulfillmentTxnHash:null}).count();
db.DcxOrders.updateMany({fulfillmentTxnHash:null},{$unset:{fulfillmentTxnHash:1}});

db.DcxOrders.find({fulfillmentTxnHash:{$exists:0}}).count();

-clean existing
db.DcxOrders.find({fulfillmentTxnHash:"DUMMY HASH"}).count();

db.DcxOrders.updateMany({fulfillmentTxnHash:"DUMMY HASH"},{$unset:{fulfillmentTxnHash:1}});


# get full leader board
https://

https://core.app/tools/testnet-faucet/?subnet=dfk

BUILD BREAK



# reg fix
use dcx_perpetual;

db.DcxOrders.find({chainId:{$exists:1}},{fulfillmentTxnHash:1,chainId:1}).pretty();
db.DcxOrders.find({fulfillmentTxnHash:"0x"}).pretty();


"chainId" : NumberLong(42161),

https://game.dragonscrossing.com/api/Season/signupCompleted/53935/0xe8358da0e30c7fbbb2fad530cb583b14d9acce1a6b65e7b4d7f29848ea11c1b1


# tx URLs

https://game.dragonscrossing.com/api/Camping/ItemSecureCompleted/TXHASH