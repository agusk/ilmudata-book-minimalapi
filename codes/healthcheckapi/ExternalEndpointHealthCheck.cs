using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class ExternalEndpointHealthCheck : IHealthCheck
{
    private readonly string _externalUrl;

    public ExternalEndpointHealthCheck(string externalUrl)
    {
        _externalUrl = externalUrl;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        using (var httpClient = new HttpClient())
        {
            try
            {
                var response = await httpClient.GetAsync(_externalUrl, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy($"The check for {_externalUrl} is OK!");
                }

                return HealthCheckResult.Unhealthy($"The check for {_externalUrl} failed.");
            }
            catch
            {
                return HealthCheckResult.Unhealthy($"The check for {_externalUrl} failed with an exception.");
            }
        }
    }
}