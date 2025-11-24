using System.Threading.RateLimiting;

namespace FlightHub.Api.Extensions;

public static class RateLimitingExtensions
{
    /// <summary>
    /// Registers a global fixed-window rate limiter:
    /// 100 requests per minute per client IP. Returns 429 when exceeded.
    /// </summary>
    public static IServiceCollection AddFlightHubRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        return services;
    }

    public static IApplicationBuilder UseFlightHubRateLimiting(this IApplicationBuilder app)
    {
        app.UseRateLimiter();
        return app;
    }
}
