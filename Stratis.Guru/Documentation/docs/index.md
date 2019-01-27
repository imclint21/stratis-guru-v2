# City Chain Explorer Documentation

This documentation describes how to use the City Chain Explorer API.

## Create Stratis Key Pair
This endpoint generate a new City Chain Address and returns a public and private key pair.    
```
GET https://stratis.guru/api/create-address
```
```json
{
    "publicKey": "SamTnzohx7PfZGhi5oak8BGhRvCBQQWoto",
    "privateKey": "VNZne4ucXuc6NgR7W6xjXZfz7VimP7PfkXQFGhisuurUwA8HdZEZ"
}
```
***Warning:** Use this endpoint only for testing purposes*


## Get Price (in USD)
Get the $STRAT Price, this API query [CoinMarketCap](https://coinmarketcap.com/fr/currencies/stratis/) API.
```
GET https://stratis.guru/api/price
```
```json
{
    "usdPrice": 1.5942872362,
    "last24Change": 0.0438
}
```

* * *

# Block Explorer Endpoints
This API use a [Nako Node](https://github.com/CoinVault/Nako) to query the Stratis Blockchain.
## Query Address
```
GET https://stratis.guru/api/address/SR2ZXnhRnMqJNoeDiCFUnaug7TKHJocwDd
```
```json
{
    "coinTag": "STRAT",
    "address": "SR2ZXnhRnMqJNoeDiCFUnaug7TKHJocwDd",
    "balance": 0,
    "totalReceived": 278.03566,
    "totalSent": 278.03566,
    "unconfirmedBalance": 0,
    "transactions": [
        {
            "index": 0,
            "type": null,
            "transactionHash": "adebb15afa28622c01ac31d6af50ffd8c13c9bd93de4c0c3cb5a66e3c13d5334",
            "spendingTransactionHash": "bfe9a2a964ac06a1579e9fd5a2bde2de1c837083aa3c50ea2f7d0099f440d3e7",
            "pubScriptHex": "76a91428f4fa037ae89fdcb77c3b0084a37bd15d3d619488ac",
            "coinBase": "",
            "value": 100,
            "blockIndex": 1037620,
            "confirmations": 26247,
            "time": 0
        },
        {
            "index": 1,
            "type": null,
            "transactionHash": "a6d372cb275f56317e2a45d7c458e32f8da6b8da7bc04d0fcfefd49f5c15fab2",
            "spendingTransactionHash": "bfe9a2a964ac06a1579e9fd5a2bde2de1c837083aa3c50ea2f7d0099f440d3e7",
            "pubScriptHex": "76a91428f4fa037ae89fdcb77c3b0084a37bd15d3d619488ac",
            "coinBase": "",
            "value": 100,
            "blockIndex": 1037740,
            "confirmations": 26127,
            "time": 0
        },
        {
            "index": 1,
            "type": null,
            "transactionHash": "5b366eb32656f82dad68bc2197ce3d36ff4be64ea55f9e3e616fa37ea5213b34",
            "spendingTransactionHash": "bfe9a2a964ac06a1579e9fd5a2bde2de1c837083aa3c50ea2f7d0099f440d3e7",
            "pubScriptHex": "76a91428f4fa037ae89fdcb77c3b0084a37bd15d3d619488ac",
            "coinBase": "",
            "value": 32.14566,
            "blockIndex": 1037889,
            "confirmations": 25978,
            "time": 0
        },
        {
            "index": 0,
            "type": null,
            "transactionHash": "666f3f4d01a7b39a02d9880ca003f45afa8740d3b850da42310f55cbc22c9524",
            "spendingTransactionHash": "067c056cedcfb9c673654ced6a572470f80761a1a99876294a4ae534b3855fce",
            "pubScriptHex": "76a91428f4fa037ae89fdcb77c3b0084a37bd15d3d619488ac",
            "coinBase": "",
            "value": 1.5,
            "blockIndex": 1042172,
            "confirmations": 21695,
            "time": 0
        },
        {
            "index": 0,
            "type": null,
            "transactionHash": "43bd175d32057a228e9e9fdb2928d0e776a551ad1c73a493a3804f0137c0a7fe",
            "spendingTransactionHash": "067c056cedcfb9c673654ced6a572470f80761a1a99876294a4ae534b3855fce",
            "pubScriptHex": "76a91428f4fa037ae89fdcb77c3b0084a37bd15d3d619488ac",
            "coinBase": "",
            "value": 34.8,
            "blockIndex": 1042710,
            "confirmations": 21157,
            "time": 0
        },
        {
            "index": 0,
            "type": null,
            "transactionHash": "ca59e055d187c6806b697b4ba0f7f3423ccffa4eb54c38afcae34f1eaaae8506",
            "spendingTransactionHash": "067c056cedcfb9c673654ced6a572470f80761a1a99876294a4ae534b3855fce",
            "pubScriptHex": "76a91428f4fa037ae89fdcb77c3b0084a37bd15d3d619488ac",
            "coinBase": "",
            "value": 9.59,
            "blockIndex": 1045265,
            "confirmations": 18602,
            "time": 0
        }
    ],
    "unconfirmedTransactions": []
}
```

## Transaction
```
GET https://stratis.guru/api/transaction/adebb15afa28622c01ac31d6af50ffd8c13c9bd93de4c0c3cb5a66e3c13d5334
```
```json
{
    "coinTag": "STRAT",
    "blockHash": "75cd023f52b3098ed0f758d23ac5fccf1a66e591e72aecb4c23596d79c316bf0",
    "blockIndex": 1037620,
    "timestamp": "2018-10-18T14:06:24",
    "transactionId": "adebb15afa28622c01ac31d6af50ffd8c13c9bd93de4c0c3cb5a66e3c13d5334",
    "confirmations": 26246,
    "inputs": [
        {
            "inputIndex": 0,
            "inputAddress": "",
            "coinBase": null,
            "inputTransactionId": "65290a6d8fb2b895981142dda9ef67c6d330be9d1b9bbcaa726eb1f3e1d02e34"
        }
    ],
    "outputs": [
        {
            "address": "SR2ZXnhRnMqJNoeDiCFUnaug7TKHJocwDd",
            "balance": 100,
            "index": 0,
            "outputType": "pubkeyhash"
        },
        {
            "address": "SXJbYx3mPrnPoorAtfZu76VP3iXdNwfLmH",
            "balance": 399.9999,
            "index": 1,
            "outputType": "pubkeyhash"
        }
    ]
}
```

## Block
```
GET https://stratis.guru/api/block/1063868
```
```json
{
    "coinTag": "STRAT",
    "blockHash": "d96025a693036996f3f9aedfb7f6be96cc23a9e4f0e7507c9599f7038c0fe063",
    "blockIndex": 1063868,
    "blockSize": 446,
    "blockTime": 1541622208,
    "nextBlockHash": null,
    "previousBlockHash": "6558784e62bc908bd97e467662651d027d05f64021f231ea444c4ee6c7cbd49f",
    "synced": true,
    "transactionCount": 2,
    "transactions": [
        "7b3d43b5b5b1d4920884b43d38c2ec7746041a1cd3999b05b997296058cfc8d1",
        "5c70319c270347a0cd74bda087cee875e2e3bfafda9f0399948933b0c8f88659"
    ]
}
```
