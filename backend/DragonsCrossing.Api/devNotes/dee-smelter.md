mongodump --db dcx_season_1 --gzip --archive=/savedbb4smelter_1.gz

mongodump --db dcx_season_1 --gzip --archive=/savedbb4smelter_2.gz

mongodump --db dcx_season_1 --gzip --archive=/savedbb4_wonderus.gz


mongodump --db dcx_season_1 --gzip --archive=/savedbb4_camp.gz

mongodump --db dcx_season_1 --gzip --archive=/savedbb4_wondrousThicket_creatence.gz

mongodump --db dcx_season_1 --gzip --archive=/savedbb4_oldcorocus.gz

use dcx_season_1
db.dropDatabase();
exit
mongorestore --gzip --archive=/savedbb4_wondrousThicket_creatence.gz

use dcx_season_1
db.GameStates.find({_id:1},{"Hero.inventory._id":1,"Hero.extraDailyQuestGiven":1}).pretty();

db.GameStates.updateOne({_id:1},{$push:{"Hero.inventory":{
				"_t" : "ItemDto",
				"type" : "ItemDto",
				"slug" : "quest-refresh",
				"name" : "Refresh Quests",
				"itemDropSound" : "ring",
				"slot" : 10,
				"dieDamage" : [ ],
				"heroStatComplianceDictionary" : [ ],
				"allowedHeroClassList" : [
					0,
					1,
					2
				],
				"imageSlug" : "curious-band-of-amber",
				"levelRequirement" : 1,
				"_id" : "f8bea254-93b7-4972-92c9-d2bb8a21125b",
				"nftTokenId" : 0,
				"itemIndex" : null,
				"rarity" : 0,
				"bonusDamage" : 0,
				"affectedAttributes" : [
					[
						10,
						0
					]
				]
			}
}});


db.GameStates.updateOne({_id:1},{$push:{"Hero.inventory": {
				"_t" : "ItemDto",
				"type" : "ItemDto",
				"slug" : "stubborn-iron-ring",
				"name" : "Stubborn Iron Ring",
				"itemDropSound" : "ring",
				"slot" : 4,
				"dieDamage" : [ ],
				"heroStatComplianceDictionary" : [ ],
				"allowedHeroClassList" : [
					0,
					1,
					2
				],
				"imageSlug" : "stubborn-iron-ring",
				"levelRequirement" : 1,
				"_id" : "2d432660-e07e-4d37-89c9-f50c70dbf92c",
				"nftTokenId" : 0,
				"itemIndex" : null,
				"rarity" : 3,
				"bonusDamage" : 0,
				"affectedAttributes" : [
					[
						10,
						364
					],
					[
						11,
						186
					],
					[
						12,
						333
					],
					[
						5,
						1
					],
					[
						0,
						1
					],
					[
						1,
						1
					],
					[
						2,
						0
					],
					[
						8,
						2
					]
				]
			}
}});