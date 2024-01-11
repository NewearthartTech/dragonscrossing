# Wen - 9:35 PM Jun/1
db.GameStates.updateOne({_id:492}, {$set:{"Hero.dailyQuestsUsed":0}});


#Dee 9/23 - updating experience points on prod

mongodump --uri mongodb+srv://doadmin:7B58JN09ES4lWp21@private-db-dcx-mainnet-a75ac727.mongo.ondigitalocean.com/dcx_perpetual?authSource=admin --gzip --archive=/dcx_perpetua_9_23_mainnet_b4_experiencePointsChange.gz
mongodump --uri mongodb+srv://doadmin:7B58JN09ES4lWp21@private-db-dcx-mainnet-a75ac727.mongo.ondigitalocean.com/dcx_season_4?authSource=admin --gzip --archive=/dcx_season_4_9_23_mainnet_b4_experiencePointsChange.gz

kubectl cp dcx-staging/mongodb-node-0:dcx_perpetua_9_23_mainnet_b4_experiencePointsChange.gz ./dcx_perpetua_9_23_mainnet_b4_experiencePointsChange.gz
kubectl cp dcx-staging/mongodb-node-0:dcx_season_4_9_23_mainnet_b4_experiencePointsChange.gz ./dcx_season_4_9_23_mainnet_b4_experiencePointsChange.gz


-- use dcx_perpetual;
switched to db dcx_perpetual
db-dcx-mainnet:PRIMARY> db.perpetualHeros.find({maxExperiencePoints:8}).count();
1088
db-dcx-mainnet:PRIMARY> db.perpetualHeros.find({maxExperiencePoints:7}).count();
2044


db.perpetualHeros.updateMany({maxExperiencePoints:8},{$set:{maxExperiencePoints:7}});
{ "acknowledged" : true, "matchedCount" : 1088, "modifiedCount" : 1088 }

db-dcx-mainnet:PRIMARY> use dcx_season_4;
switched to db dcx_season_4

db-dcx-mainnet:PRIMARY> db.GameStates.find({"Hero.maxExperiencePoints":8}).count();
208
db-dcx-mainnet:PRIMARY> db.GameStates.find({"Hero.maxExperiencePoints":7}).count();
23

db.GameStates.updateMany({"Hero.maxExperiencePoints":8},{$set:{"Hero.maxExperiencePoints":7}});
{ "acknowledged" : true, "matchedCount" : 209, "modifiedCount" : 209 }


