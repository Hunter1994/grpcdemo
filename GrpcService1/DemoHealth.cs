using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GrpcService1
{
    public class DemoHealth : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return new HealthCheckResult();
        }
    }
}
