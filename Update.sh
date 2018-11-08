#!/bin/bash
git pull
dotnet publish -c Release
systemctl restart stratis-guru.service