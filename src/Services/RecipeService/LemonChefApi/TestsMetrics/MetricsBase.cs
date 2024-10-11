using Prometheus;
using System.Diagnostics;

namespace LemonChefApi.TestsMetrics
{
    public class MetricsBase
    {
        private static readonly Counter RequestCounter;
        private static readonly Histogram ResponseTimeHistogram;
        private static readonly Gauge MemoryUsageGauge;

        static MetricsBase()
        {
            RequestCounter = Metrics.CreateCounter("app_requests_total", "Total number of requests to the application");
            ResponseTimeHistogram = Metrics.CreateHistogram("app_response_time_seconds", "Response time in seconds for requests", new HistogramConfiguration
            {
                LabelNames = new[] { "controller", "action" }
            });
            MemoryUsageGauge = Metrics.CreateGauge("app_memory_usage_bytes", "Current memory usage in bytes");
        }

        public void TrackRequest(string controller, string action, Action actionCode)
        {
            RequestCounter.Inc();
            var stopwatch = Stopwatch.StartNew();
            UpdateMemoryUsage();

            actionCode();

            stopwatch.Stop();
            ResponseTimeHistogram.WithLabels(controller, action).Observe(stopwatch.Elapsed.TotalSeconds);
        }

        public async Task TrackRequestAsync(string controller, string action, Func<Task> actionCode)
        {
            RequestCounter.Inc();
            var stopwatch = Stopwatch.StartNew();
            UpdateMemoryUsage();

            await actionCode();

            stopwatch.Stop();
            ResponseTimeHistogram.WithLabels(controller, action).Observe(stopwatch.Elapsed.TotalSeconds);
        }

        private void UpdateMemoryUsage()
        {
            var process = Process.GetCurrentProcess();
            MemoryUsageGauge.Set(process.WorkingSet64);
        }
    }
}
