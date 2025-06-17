using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using uniffi.convexmobile;
using UnityEngine;
using Newtonsoft.Json;

public class ConvexClient : MonoBehaviour
{
    [SerializeField] private string _convexUrl = "https://wary-elk-496.convex.cloud";
    [SerializeField] private string _clientId = "unity-app-demo";

    private MobileConvexClient client;

    void Awake()
    {
        client = new MobileConvexClient(_convexUrl, _clientId);
    }

    public async Task<string> Query(string name, Dictionary<string, string> args) =>
        await client.Query(name, args);

    public async Task<string> Mutation(string name, Dictionary<string, string> args) =>
        await client.Mutation(name, ArgsBuilder.Build(args));

    public async Task<string> Action(string name, Dictionary<string, string> args) =>
        await client.Action(name, args);

    public async Task SetAuth(string token) =>
        await client.SetAuth(token);

    public async Task<ConvexSubscriptionHandle> Subscribe(string name, Dictionary<string, string> args, ConvexQuerySubscriber subscriber) =>
        new ConvexSubscriptionHandle(await client.Subscribe(name, args, subscriber.ToInternal()));
}


public class ConvexSubscriptionHandle : IDisposable
{
    private readonly SubscriptionHandle _handle;

    internal ConvexSubscriptionHandle(SubscriptionHandle handle) => _handle = handle;
    public void Cancel() => _handle.Cancel();
    public void Dispose() => _handle.Dispose();
}

public class ConvexQuerySubscriber
{
    private readonly Action<string> _onUpdate;
    private readonly Action<string, string> _onError;
    private readonly MainThreadDispatcher _dispatcher;

    public ConvexQuerySubscriber(Action<string> onUpdate, Action<string, string> onError = null)
    {
        _onUpdate = onUpdate ?? throw new ArgumentNullException(nameof(onUpdate));
        _onError = onError ?? ((message, value) => Debug.LogError($"Subscription error: {message}, value: {value}"));
        _dispatcher = MainThreadDispatcher.Instance;
    }

    internal QuerySubscriber ToInternal() => new LambdaQuerySubscriber(
        value =>
        {
            _dispatcher.Enqueue(() => _onUpdate(value));
        },
        (message, value) => _dispatcher.Enqueue(() => _onError(message, value))
    );
}

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher _instance;
    private readonly Queue<Action> _executionQueue = new Queue<Action>();

    public static MainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("MainThreadDispatcher");
                _instance = go.AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }
}

public class LambdaQuerySubscriber : QuerySubscriber
{
    private readonly Action<string> _onUpdate;
    private readonly Action<string, string> _onError;

    public LambdaQuerySubscriber(Action<string> onUpdate, Action<string, string> onError = null)
    {
        _onUpdate = onUpdate ?? throw new ArgumentNullException(nameof(onUpdate));
        _onError = onError ?? ((message, value) => Debug.LogError($"Subscription error: {message}, value: {value}"));
    }

    public void OnError(string message, string value)
    {
        _onError(message, value);
    }

    public void OnUpdate(string value)
    {
        _onUpdate(value);
    }
}

public static class ArgsBuilder
{
    public static Dictionary<string, string> Build(Dictionary<string, string> record)
    {
        var result = new Dictionary<string, string>();

        foreach (var entry in record)
            result[entry.Key] = JsonConvert.SerializeObject(entry.Value);

        return result;
    }

    public static Dictionary<string, string> Build(Dictionary<string, object?> record)
    {
        var result = new Dictionary<string, string>();

        foreach (var entry in record)
            result[entry.Key] = JsonConvert.SerializeObject(entry.Value);

        return result;
    }

    public static Dictionary<string, string> Build(object anonymousObject)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var prop in anonymousObject.GetType().GetProperties())
            dict[prop.Name] = prop.GetValue(anonymousObject);

        return Build(dict);
    }

    public static Dictionary<string, string> Build(IEnumerable<(string Key, object? Value)> pairs)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var (key, value) in pairs)
            dict[key] = value;

        return Build(dict);
    }

    public static Dictionary<string, string> Build<T>(T value, JsonSerializerSettings? settings = null)
    {
        var json = JsonConvert.SerializeObject(value, settings ?? new JsonSerializerSettings());
        return new Dictionary<string, string>
            {
                { typeof(T).Name, json }
            };
    }

    public static Dictionary<string, string> Build(Dictionary<string, object> record, JsonSerializerSettings settings)
    {
        var result = new Dictionary<string, string>();

        foreach (var entry in record)
            result[entry.Key] = JsonConvert.SerializeObject(entry.Value, settings);

        return result;
    }

    public static Dictionary<string, string> Build(string key, string value)
    {
        return new Dictionary<string, string>
            {
                { key, JsonConvert.SerializeObject(value) }
            };
    }

    public static Dictionary<string, string> Build(string key, object value)
    {
        return new Dictionary<string, string>
            {
                { key, JsonConvert.SerializeObject(value) }
            };
    }
}