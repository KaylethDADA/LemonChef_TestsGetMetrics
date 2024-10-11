namespace LemonChefApi.TestsMetrics
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public ExceptionMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var controller = context.Request.RouteValues["controller"]?.ToString() ?? "UnknownController";
            var action = context.Request.RouteValues["action"]?.ToString() ?? "UnknownAction";

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var metricsBase = scope.ServiceProvider.GetRequiredService<MetricsBase>();
                    await metricsBase.TrackRequestAsync(controller, action, async () =>
                    {
                        await _next.Invoke(context);
                    });
                }
            }
            catch (Exception e)
            {
                context.Response.Clear();
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(e);
            }
        }
    }
}
