# lambda-dns

AWS Lambda function implemented in .NET 8 that performs a DNS query.

## Build

```bash
dotnet publish src/DnsQuery/DnsQuery.csproj -c Release -o publish
```

## Deploy

Run `./publish.sh` to package and deploy the function using the AWS CLI.

## Invocation

Invoke the Lambda with an event like:

```json
{
  "hostname": "example.com",
  "type": "A"
}
```

The response includes whether the query succeeded, the duration in milliseconds and any returned addresses.
