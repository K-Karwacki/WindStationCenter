using System.Collections.Concurrent;
using Domain.Entities.IoT;

namespace Api.Services;

public class TelemetryBuffer
{
    private readonly ConcurrentDictionary<string, Telemetry> _latest = new();

    public void Update(string turbineId, Telemetry telemetry)
    {
        if (string.IsNullOrWhiteSpace(turbineId) || telemetry is null) return;
        _latest.AddOrUpdate(turbineId, telemetry, (_, __) => telemetry);
    }

    public KeyValuePair<string, Telemetry>[] DrainAll()
    {
        var items = _latest.ToArray();
        foreach (var k in items.Select(i => i.Key))
        {
            _latest.TryRemove(k, out _);
        }
        return items;
    }
}
