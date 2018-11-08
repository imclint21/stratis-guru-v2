#!/bin/bash
git pull
systemctl stop stratis-guru.service
dotnet publish -c Release
systemctl start stratis-guru.service