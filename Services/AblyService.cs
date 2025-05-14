using IO.Ably.Realtime;

namespace macaroni_dev.Services;

using IO.Ably;
using System;

public sealed class AblyService
{
    private static readonly Lazy<AblyService> _instance = new Lazy<AblyService>(() => new AblyService());

    private AblyRealtime _realtime;

    public static AblyService Instance => _instance.Value;

    public AblyRealtime Realtime => _realtime;

    private AblyService()
    {
        var clientOptions = new ClientOptions("AMFdjw.9APhcQ:jWOwMvN5X36v8bXgx-HRzP8yF0N0RefbPxzHT0IabHM");

        _realtime = new AblyRealtime(clientOptions);

        _realtime.Connection.On(ConnectionEvent.Connected, args =>
        {
            Console.WriteLine("Connected to Ably!");
        });
    }
}