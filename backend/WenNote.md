# Docker MongoDB commands

docker exec -it dragonscrossingapi-mongo-1 mongo
show dbs
show collections
use dcx_season_1
use dcx_perpetual
db.GameStates.find({_id:3}).pretty();
db.GameStates.find().limit(1);
db.GameStates.updateOne({_id:3},{$set:{"Hero.experiencePoints":20}});
db.GameStates.updateOne({_id:1},{$set:{"inactiveBossTiles":[14,19,24]}}); <!-- To unset the final boss tile -->
db.GameStates.drop()
db.dropDatabase();

# Nuke DB

docker ps
docker stop <containerName>
docker rm <containerName>
  
# K8 commands

set KUBECONFIG=??
kubectl -n dcx-staging get pods
kubectl -n dcx-staging logs dcx-server-d889f54f7-jz957
kubectl -n dcx-staging logs --tail 10 dcx-server-d889f54f7-jz957
  
kubectl -n dcx-staging get deployments <!-- staging is the main environment-->
kubectl -n dcx-staging scale deployment/dcx-server --replicas=3   <!--This sacles the server to 3 -->
kubectl -n dcx-staging delete pod -dcx-server-XXX <!-- This removes a pod --> 
 kubectl -n dcx-beta get pods <!-- beta is the beta environment-->

dcx-mainnet is the prod collection

kubectl -n dcx-staging exec -it mongodb-node-0 -- mongo

kubectl -n dcx-staging exec -it  mongoclient -- bash
mongo "mongodb+srv://doadmin:7B58JN09ES4lWp21@private-db-dcx-mainnet-a75ac727.mongo.ondigitalocean.com"


kubectl -n dcx-mainnet exec -it  mongoclient -- bash
mongo "mongodb+srv://doadmin:7B58JN09ES4lWp21@private-db-dcx-mainnet-a75ac727.mongo.ondigitalocean.com"


kubectl -n dcx-beta port-forward rabbitmq-0 15673:15672 <!-- This is to port over the rabbit mq from the beta to local -->

kubectl -n dcx-mainnet port-forward rabbitmq-0 15673:15672

kubectl -n dcx-mainnet port-forward service/kibana 5603:5601
#Command Args

"commandLineArgs": "/app:Command minthero /app:Account 0xC4c7252B86f61c810b208bee43CB87e601C04429 /app:file /Users/cses0/source/heroTemplate/heroList.json",
"commandLineArgs": "/app:command deployContracts",

#Mint URLS
https://beta.staging.dragonscrossing.com/api/mints/whitelist
https://game.dragonscrossing.com/api/mints/loadwhitelist/finallist.json



item_questRefresh.json

#clear lonares

rs0:PRIMARY> db.perpetualHeros.update({isLoanedHero:{$exists:1}},{$unset:{isLoanedHero:1}});
WriteResult({ "nMatched" : 1, "nUpserted" : 0, "nModified" : 1 })
rs0:PRIMARY> db.perpetualHeros.find({isLoanedHero:{$exists:1}},{isLoanedHero:1}).pretty();


# clear b4 run toi prod
delete all incomplete season Signup orders

db.nftOwners.getIndexes();
db.nftOwners.dropIndex("contractAddress_1_tokenId_1")