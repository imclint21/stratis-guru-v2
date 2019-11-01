# Block Explorer Documentation

This documentation describes how to use the Block Explorer API.

## Create Stratis Key Pair
This endpoint generate a new Address and returns a public and private key pair.    
```
GET https://explorer.city-chain.org/api/create-address
```
```json
{
    "publicKey": "SamTnzohx7PfZGhi5oak8BGhRvCBQQWoto",
    "privateKey": "VNZne4ucXuc6NgR7W6xjXZfz7VimP7PfkXQFGhisuurUwA8HdZEZ"
}
```
***Warning:** Use this endpoint only for testing purposes*


## Get Price (in USD)
Get the $CITY Price, this API query p2pb2b API.
```
GET https://explorer.city-chain.org/api/price
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
GET https://explorer.city-chain.org/api/address/CGZorSSjEEsKEAVen9x4CSHHFcUhVD7gH4
```
```json
{
	"coinTag": "CITY",
	"address": "CGZorSSjEEsKEAVen9x4CSHHFcUhVD7gH4",
	"balance": 15000000000,
	"totalReceived": 15000000000,
	"totalSent": 0,
	"unconfirmedBalance": 0,
	"transactions": [
		{
			"index": 1,
			"type": "TX_PUBKEYHASH",
			"transactionHash": "8c65bc9eeddaa515cce808ede05c1286d9c2f23a8c4102a505874b2d830dc4a2",
			"spendingTransactionHash": null,
			"spendingBlockIndex": null,
			"pubScriptHex": "OP_DUP OP_HASH160 011e8ed8779c01dcb5bb326fd66796e3d9c080e7 OP_EQUALVERIFY OP_CHECKSIG",
			"coinBase": false,
			"coinStake": false,
			"value": 15000000000,
			"blockIndex": 464862,
			"confirmations": 758,
			"time": 1572604368
		}
	],
	"unconfirmedTransactions": []
}
```

## Transaction
```
GET https://explorer.city-chain.org/api/transaction/8c65bc9eeddaa515cce808ede05c1286d9c2f23a8c4102a505874b2d830dc4a2
```
```json
{
	"coinTag": "CITY",
	"blockHash": "c89c0b0d1e7561ca084676a89dd8240c0161f175d315ea64144aea9d1990065f",
	"blockIndex": 464862,
	"timestamp": "2019-10-31T20:18:24",
	"transactionId": "8c65bc9eeddaa515cce808ede05c1286d9c2f23a8c4102a505874b2d830dc4a2",
	"confirmations": 753,
	"isCoinbase": false,
	"isCoinstake": false,
	"lockTime": "Height : 0",
	"rbf": false,
	"version": 1,
	"inputs": [
		{
			"inputIndex": 0,
			"inputAddress": "",
			"coinBase": null,
			"inputTransactionId": "618032da3d12d1d2d9aeca2f6b2247e513808d45c3ac779e9111c9cf251aea74",
			"scriptSig": "483045022100aae90705932ddd8e2bb5d3d59c9a7f2e4e3214a7b725858c0c7fb3a016b9f5740220563872b162e0519b45bd1e988145c4f69bce235e6a0a16d6b34b8466f1c5bb2d0121024d2678aa685cabc76b3faf26520382c60a5f89a7b04f3c94262d4937bf1c0cdc",
			"scriptSigAsm": "3045022100aae90705932ddd8e2bb5d3d59c9a7f2e4e3214a7b725858c0c7fb3a016b9f5740220563872b162e0519b45bd1e988145c4f69bce235e6a0a16d6b34b8466f1c5bb2d01 024d2678aa685cabc76b3faf26520382c60a5f89a7b04f3c94262d4937bf1c0cdc",
			"witScript": "",
			"sequenceLock": "4294967295"
		}
	],
	"outputs": [
		{
			"address": "CMsdK6pkemJdbe3y8vxSJdefmjAiKhU8eZ",
			"balance": 2308449940000,
			"index": 0,
			"outputType": "TX_PUBKEYHASH",
			"scriptPubKeyAsm": "OP_DUP OP_HASH160 3b55e027f379efe03e8ed9e5b53f5886b60a1e2e OP_EQUALVERIFY OP_CHECKSIG",
			"scriptPubKey": "76a9143b55e027f379efe03e8ed9e5b53f5886b60a1e2e88ac",
			"spentInTransaction": null
		},
		{
			"address": "CGZorSSjEEsKEAVen9x4CSHHFcUhVD7gH4",
			"balance": 15000000000,
			"index": 1,
			"outputType": "TX_PUBKEYHASH",
			"scriptPubKeyAsm": "OP_DUP OP_HASH160 011e8ed8779c01dcb5bb326fd66796e3d9c080e7 OP_EQUALVERIFY OP_CHECKSIG",
			"scriptPubKey": "76a914011e8ed8779c01dcb5bb326fd66796e3d9c080e788ac",
			"spentInTransaction": null
		}
	]
}
```

## Block
```
GET https://explorer.city-chain.org/api/block/464862
```
```json
{
	"coinTag": "CITY",
	"blockHash": "c89c0b0d1e7561ca084676a89dd8240c0161f175d315ea64144aea9d1990065f",
	"blockIndex": 464862,
	"blockSize": 632,
	"blockTime": 1572553104,
	"nextBlockHash": null,
	"previousBlockHash": "380a605f4d5fc3dcb42a4388a4af3186107fcd5670176ed38024f162bb365317",
	"synced": true,
	"transactionCount": 3,
	"confirmations": 1,
	"bits": "1a08acef",
	"merkleroot": "dd66a1a9eb5fa54ea3b76ff600723144f817972cd47ae4c07a8d064f4e01a342",
	"nonce": 0,
	"version": 536870912,
	"difficulty": 0,
	"chainWork": null,
	"posBlockSignature": "473045022100e53326a544e1de7b689c4b26156690fc4b2f89b946213ab0537faab828fc7f4b02201c4b650df87712b62e03ec168d17630f4cd0b56889ca7f181ef577ca5080bc02",
	"posModifierv2": "433a0a1841615c8f1b982c286154b05861f58936a1c5f72e982d3fbfcdeadcf3",
	"posFlags": "proof-of-stake",
	"posHashProof": "000133bd414e60172acf1ef8addc04681182eb26c9b10d383990151afbe3b48c",
	"posBlockTrust": "000000000000000000000000000000000000000000000000001d822000000000",
	"posChainTrust": "00000000000000000000000000000000000000000000004e251ab659e33ecdfa",
	"transactions": [
		"fb447c78a2a4509bff460b468d79444a69f4a962ffd38daae2aab9a583dc876e",
		"0ea0a7dc97b00e9f29d4bb7659871eac038eef64db6de230ed708b4b60356ddd",
		"8c65bc9eeddaa515cce808ede05c1286d9c2f23a8c4102a505874b2d830dc4a2"
	]
}
```
