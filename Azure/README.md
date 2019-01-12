# Deploy Blockchain Indexer to Azure

You can easily deploy the Nako blockchain indexer to Microsoft Azure with a one-click deployment. Select the blockchain you want to deploy below to get started.

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fclintnetwork%2Fstratis-guru-v2%2Ffeature%2Fmultichain%2FAzure%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a><a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2Fclintnetwork%2Fstratis-guru-v2%2Ffeature%2Fmultichain%2FAzure%2Fazuredeploy.json" target="_blank">
    <img src="http://armviz.io/visualizebutton.png"/>
</a>

Change the "url" property to change the blockchain to deploy.

You need the Nako indexer for Stratis.Guru to get data.

## VM Sizes

Testing has shown that "Standard_F1" is not powerful enough to run the indexer, and the API endpoint never becomes available. Ensure you are running a good enough VM size for the full setup.

Suggested sizes:

- Standard_F2 (2 core, 4 GB, 32 GB disk)
- Standard_F4 (4 core, 8 GB, 64 GB disk)

## Verification

To test and verify that the deployment was successfully, locate the public IP address or DNS name of your virtual machine, and open the statistics part of the API:

http://URL/api/stats

This should return something like:

```json
{
  "CoinTag": "STRAT",
  "Progress": "11856/79000 - 67144",
  "TransactionsInPool": 0,
  "SyncBlockIndex": 11856,
  "ClientInfo": {
    "Version": "3000001",
    "ProtocolVersion": "70012",
    "WalletVersion": null,
    "Balance": 0.0,
    "Blocks": 79000,
    "TimeOffset": 0.0,
    "Connections": 9,
    "Proxy": "",
    "Testnet": false,
    "KeyPoolEldest": 0,
    "KeyPoolSize": 0,
    "PayTxFee": 0.0,
    "RelayTxFee": 0.0,
    "Errors": ""
  }
}
```

## Configuration

When you deploy the ARM template to Microsoft Azure, it will create a Linux VM with Ubuntu and install Docker CE.

When everything is installed, it will proceed to download and run the docker-compose.yml that was specified in the URL parameter.

This will launch the following docker instances:

- coin-nako
- coin-client
- coin-mongo

The nako instance is running the indexer and the rest API.

The client is the daemon for the selected blockchain.

The mongo instance, is the storage for all the block and transaction details.

## Network Security Group (Firewall)

Out of the box, ports 80, 433, 9000 and 22 is open to the Internet. Make sure you add restrictions to the 22 port, ensuring only your own IP is allowed to connect remotely to the shell.

Deploying this template will not open your daemon to the public.

Please open the TCP port for the blockchain's P2P communication, to make your deployed node, a public node. This will help the network.

### Standard Ports

```
Stratis: 16178

City Chain: 4333

Bitcoin: 8333
```

## Remote Shell Connection

You can connect to your docker host (Linux VM) by using SSH and supply the username and password you enter during deployment.

## Connect Stratis.Guru to your Nako indexer

You should consider locking down the NSG (firewall) on your deployed blockchain indexer, ensuring only your deployed Stratis.Guru instance is allowed to query the API.

Change the setup.json and enter your Nako.ApiUrl there, to connect with your deployed indexer instance.
