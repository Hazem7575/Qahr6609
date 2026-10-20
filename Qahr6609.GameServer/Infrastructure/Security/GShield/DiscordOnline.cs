using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public static class DiscordOnline
{
    private static int _onlineCount = 0;
    private static readonly HttpClient http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    private static int _refreshing = 0;
    private static long _lastRefreshTicks = 0;

    /// <summary>Minimum time between two widget requests, regardless of how many players ask.</summary>
    public const int MinRefreshSeconds = 60;

    public static int OnlineCount { get { return Volatile.Read(ref _onlineCount); } }

    /// <summary>
    /// Starts a background refresh when the cached value is older than MinRefreshSeconds.
    /// Never blocks the caller; at most one request is in flight.
    /// </summary>
    public static void RequestRefresh()
    {
        long now = DateTime.UtcNow.Ticks;
        if (now - Interlocked.Read(ref _lastRefreshTicks) < TimeSpan.FromSeconds(MinRefreshSeconds).Ticks)
            return;
        if (Interlocked.CompareExchange(ref _refreshing, 1, 0) != 0)
            return;
        Interlocked.Exchange(ref _lastRefreshTicks, now);
        Task.Run(async () =>
        {
            try { await RefreshAsync().ConfigureAwait(false); }
            finally { Interlocked.Exchange(ref _refreshing, 0); }
        });
    }

    /// <summary>
    /// Fetch the Discord online count. On failure the last known value is kept.
    /// </summary>
    public static async Task RefreshAsync()
    {
        try
        {
            string url = "https://discord.com/api/guilds/1396921487998324757/widget.json";
            string json = await http.GetStringAsync(url).ConfigureAwait(false);

            var obj = JObject.Parse(json);
            var token = obj["presence_count"];
            int value = 0;

            if (token != null)
            {
                if (token.Type == JTokenType.Integer)
                    value = token.Value<int>();
                else if (token.Type == JTokenType.String)
                    int.TryParse(token.Value<string>(), out value);
            }
            Volatile.Write(ref _onlineCount, value);
        }
        catch
        {
            // keep the cached value
        }
    }
}
