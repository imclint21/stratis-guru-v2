# Stratus.Guru Block Explorer | Multi-Chain Block Explorer

![](https://travis-ci.org/clintnetwork/stratis-guru-v2.svg?branch=master)

Stratis.Guru reborn from his [Version 1](https://github.com/clintnetwork/Stratis.guru) with some new features, completely rebuild in .Net Core, and supported by the Stratis community.
You will find a $STRAT Price Ticker, an address generator, a block explorer, a community lottery and an API.

![](https://i.imgur.com/rOKYCvr.png)

![](https://pix.watch/8SpQJe/xxERbX.png)

![](https://pix.watch/tVEuE0/ezGVMC.png)

![](https://pix.watch/BpSo4r/Jwv-h6.jpeg)

![](https://pix.watch/fTHGnh/vUUJsT.png)

## How the Stratis.Guru Lottery Works ?
For now it's a very simple process, you make a deposit with the amount that you want, and you put your nickname/withdraw address, when the lotery countdown end, I manually choose a winner by using random.org (this system will change in the future)
All $STRAT coins are stored in cold wallet, by using an xpub.

## Block Indexer API
Stratis.Guru relies on the Nako block indexer API. To run your own local Nako instance, you must have Docker installed.

First navigate to the Docker/Nako/ folder, where the docker.compose.yml file should be located, then you can for debugging and development run:

```sh
docker-compose up
```

This will initiate the nako indexer (including the API), the stratis fullnode daemon and mongodb for storage.

To verify that it worked, you can open the stats page to see sync status:

[http://localhost:9040/api/stats](http://localhost:9040/api/stats)

It might take a minute or two for the fullnode daemon to connect to other nodes. The API won't respond correctly, until blockchain download has started.

## Configuration
To run the Stratis.Guru, you need a appsettings.json. This is currently not included in the source code, so you must manually add it to the root of the project.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "mongodb://localhost:27017"
  },
  "NakoApi": {
    "Endpoint": "http://localhost:9040/api/"
  },
  "FixerApi": {
    "ApiKey": "",
    "Endpoint": ""
  },
  "Ticker": {
    "ApiUrl": "https://api.coinmarketcap.com/v2/ticker/1343/"
  },
  "Sentry": {
    "Dsn": "https://ed8ea72e1f6341ae901d96691d9e58a0@sentry.io/1359208",
    "IncludeRequestPayload": true,
    "IncludeActivityData": true,
    "Logging": {
      "MinimumBreadcrumbLevel": "Information"
    }
  },
  "Setup": {
    "Title": "Stratis.guru",
    "Chain": "Stratis",
    "Footer": ""
  },
  "Features": {
    "Home": true,
    "Ticker":  false,
    "Lottery": false,
    "Explorer": true,
    "Vanity": false,
    "Generator": false,
    "API": true,
    "About": true,
    "Footer": false
  }
}
```

## About the Author
Proudly Crafted with 💖 by Clint.Network — Help me to maintain by sending $STRAT at [SXDaQGs56aC9ZjFzTdbhudNXTbyxU5aNXJ](https://chainz.cryptoid.info/strat/address.dws?SXDaQGs56aC9ZjFzTdbhudNXTbyxU5aNXJ.htm).
