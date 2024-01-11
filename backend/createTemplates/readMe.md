# sample goggle sheet for items
https://docs.google.com/spreadsheets/d/1sYkNOcAtNCXnnhvXsfHtJY4rryEOi-nWWs2zDrOT7yA/edit?usp=sharing

# to build locally

- create a PAT from Azure with all access

docker build -t newearthart/dxcreatetemplate:6.1 --platform linux/amd64 -f createTemplates/Dockerfile  --build-arg FEED_ACCESSTOKEN={PAT}  .

docker push newearthart/dxcreatetemplate:6.1

# run locally

docker run --rm -it -v /Users/dee/dragonCross/DragonsCrossingApi:/data newearthart/dxcreatetemplate:1.0 /data/DragonsCrossing.Core/templates /data/createTemplates/sample/dcxTemplate-Items-Sheet1.csv

