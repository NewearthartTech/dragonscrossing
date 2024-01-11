Steps to run Discord Bot:

1) Configure you Google account
    a. Enable the Sheets API on your account: https://developers.google.com/sheets/api/quickstart/python#enable_the_api
    b. Create and download credentials file: https://developers.google.com/sheets/api/quickstart/python#authorize_credentials_for_a_desktop_application
        Replace the empty "credentials.json" file in the package provided.

2) Configure the Discord bot
    a. Create a discord bot: https://realpython.com/how-to-make-a-discord-bot-python/#how-to-make-a-discord-bot-in-the-developer-portal
        Once created, you'll see a CLIENT SECRET key for the bot. Copy it into config.json under "discord_configs" then "client_token"
    b. Add the bot to your server: https://realpython.com/how-to-make-a-discord-bot-python/#adding-a-bot-to-a-guild
        Intents needed under "Bot" settings: MESSAGE CONTENT INTENT
        OAuth2 "URL Generator":
            SCOPES to check: bot, applications.commands
            BOT PERMISSIONS to check: Manage roles, Send Messages
            Copy "GENERATED URL" at the bottom, paste it in a web browser, and add the bot to your server.
    c. Open Discord in a browser (not the app) and navigate to the guild you added the bot to
    d. Copy the Guild ID from the URL in the browser into config.json under "discord_configs" then "guild_ids" between the square brackets []
        https://discord.com/channels/<Guild ID>/<Channel ID>
    e. Create the "Whitelist" role in your guild AFTER adding the bot. Otherwise it will not be able to assign the role.
       You can use any role name as long as the name of the role matches what's in config.json under "discord_configs" then "role_name"

3) Install Python packages or host somewhere where that's automatic
    This will depend on where/how you host the bot.

4) Setup Google sheet holding codes and registration data
    a. Create a new Google Sheet and put the ID into config.json under "google_configs" then "spreadsheet_id"
        https://docs.google.com/spreadsheets/d/<THIS IS THE ID TO COPY>/edit#gid=0
    b. Create two tabs within the sheet named "Codes" and "Registrations"
    c. Create two named ranges
        "Codes" set to "Codes!A1:A1000", this gives 1000 possible active codes
        "Registrations" set to "Registrations!A1:D1000", this gives 1000 possible registered users
            These two named ranges are set in config.json under "google_configs". They tell the bot where in the sheet to read/write.

5) Run the bot using Python 3.10, included requirements.txt file has needed 3rd party modules listed
    This may require more steps depending on where/how you host the bot but generically this is:
        python main.py


