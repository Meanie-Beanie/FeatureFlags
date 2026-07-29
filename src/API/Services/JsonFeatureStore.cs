
using API.Entities;
using Shared.Contracts;
using System.Text.Json;

namespace API.UnitTests.Services;

public class JsonFeatureStore
{
    private string _filepath;

    public JsonFeatureStore(string filepath)
    {
        _filepath = filepath;
    }

    public UserFeatures GetFeatures(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException($"Api key provided cannot be empty. {nameof(apiKey)}");

        var json = File.ReadAllText(_filepath);
        var entries = JsonSerializer.Deserialize<List<UserFeatures>>(json)?.ToDictionary(x => x.ApiKey, y => y.Features);

        if (entries == null || entries.Count == 0)
            throw new InvalidOperationException("");

        try
        {
            entries.TryGetValue(apiKey, out var features);

            // In case features are empty, we'll just return empty list of features.
            return new UserFeatures(apiKey, features ?? new());
        }

        catch (KeyNotFoundException ex)
        {
            throw new InvalidOperationException($"Api key provided is not found. {nameof(apiKey)}");
        }
    }
}