# To test mint Iframe locally

- in the root folder of web

'$(PWD)/testMintFrame' should be the absolute path for testMinFrame folder

docker run -it --rm -v C:\Development\DragonsCrossingWeb\testMintFrame:/usr/share/nginx/html -p 8082:80 nginx:alpine

- for dee

docker run -it --rm -v /Users/dee/dragonCross/DragonsCrossingWeb/testMintFrame:/usr/share/nginx/html -p 8082:80 nginx:alpine
