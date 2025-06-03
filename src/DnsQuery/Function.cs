using System.Diagnostics;
using Amazon.Lambda.Core;
using DnsClient;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaDns;

public class DnsQueryRequest
{
    public string Hostname { get; set; } = string.Empty;
    public string Type { get; set; } = "A";
}

public class DnsQueryResponse
{
    public bool Success { get; set; }
    public double Duration { get; set; }
    public string? Error { get; set; }
    public DnsDetails? Details { get; set; }
}

public class DnsDetails
{
    public List<string> Addresses { get; set; } = new();
}

public class Function
{
    private readonly LookupClient _client = new();

    public async Task<DnsQueryResponse> FunctionHandler(DnsQueryRequest request, ILambdaContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var queryType = Enum.TryParse<QueryType>(request.Type, true, out var qt) ? qt : QueryType.A;
            var queryResult = await _client.QueryAsync(request.Hostname, queryType);
            var addresses = queryResult.Answers
                .Where(a => a is ARecord or AaaaRecord)
                .Select(a => a switch
                {
                    ARecord aRec => aRec.Address.ToString(),
                    AaaaRecord a6Rec => a6Rec.Address.ToString(),
                    _ => string.Empty
                })
                .Where(a => !string.IsNullOrEmpty(a))
                .ToList();
            sw.Stop();
            return new DnsQueryResponse
            {
                Success = true,
                Duration = sw.Elapsed.TotalMilliseconds,
                Details = new DnsDetails { Addresses = addresses }
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new DnsQueryResponse
            {
                Success = false,
                Duration = sw.Elapsed.TotalMilliseconds,
                Error = ex.Message
            };
        }
    }
}
