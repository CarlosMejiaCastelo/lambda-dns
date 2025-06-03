#!/bin/bash
set -e

dotnet publish src/DnsQuery/DnsQuery.csproj -c Release -o publish
cd publish
zip -r ../DnsQuery.zip .
cd ..
aws lambda update-function-code --function-name DnsQuery --zip-file fileb://DnsQuery.zip --publish --region us-east-1 --profile adminuser
