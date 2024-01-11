See MSDN: https://docs.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=fluent-api

Steps to add a migration and run it:
1. Open Package Manager Console
2. Select the Infrastructure project
3. Add a new migration: Add-Migration "Description of Change here" -o Persistence\Migrations
4. Run the migration: Update-Database

Remove a Migration:
1. Don't manually remove the migration file. Instead type this
2. Remove-Migration
3. To start over and remove all migrations:
	a. Delete the database and make sure to check the box to discoonnect all active connections.
	b. Create the database DragonsCrossing
	c. Delete all migration files.
	d. Now try adding the first migration

Copy data and/or schema from local sql server to Azure Sql Database
1. open SSMS and connect to Azure SQL Database: dragonscrossingserver.database.windows.net
2. Right click database->tasks->Import Data
3. From the wizard choose data source: .net Framework Data Provider for SqlServer
4. Find ConnectionString under Data and paste the source connection string from appsettings. Ex: (localdb)\mssqllocaldb...
4a. Note: if you get an error that the Data Source name is incorrect, make sure you only have 1 back slash in the Data Source name (there are 2 slashes because of C# escaping)
5. Click Next
6. Now choose the destination Data Source which is the same as step 3. Continue following the same steps except:
7. For the connection string make sure to enter the azure sql db connection string which starts with: Server=tcp:dragonscrossingserver.database.windows.net...
7a. Make sure to enter the correct password, especially if you copied the connection string from azure.
8. Click next
9. It should show a list of tables to import into azure. Select all the ones to import and click next
10. If there are warnings about truncation (like for datetime) go ahead and click the box to ignore those errors under "On Truncation (global)".
11. Click next and start the import
12. If there are errors in the import double click it and fix it. Then try again. If some tables imported but not others (check SSMS), go back a few times to the select tables screen and deselect them.



RUNNING localSQL in linux/osx environment (run in project root folder)

1. docker-compose -f docker-compose.localsql.yml up

2. To run sql commands
docker run -it --rm mcr.microsoft.com/mssql-tools:latest
/opt/mssql-tools/bin/sqlcmd -S host.docker.internal -U sa -P hjnNjhjjj!!

3. to set appsetting
copy appsettings.Development.json.template to appsettings.Development.json 
ensure DragonsCrossingDatabase connection string is set to localhost

4. to create and seed db, ensure dotnet ef in installed (https://docs.microsoft.com/en-us/ef/core/cli/dotnet)
cd DragonsCrossing.Api
dotnet ef dbcontext info
--- ensure app context is set to ApplicationDbContext
dotnet ef database update


