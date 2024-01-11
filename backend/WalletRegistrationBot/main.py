from bot import run_wallet_registration_bot

if __name__ == '__main__':
    import json

    with open("config.json", "r") as config_file:
        config = json.load(config_file)

    run_wallet_registration_bot(config)
