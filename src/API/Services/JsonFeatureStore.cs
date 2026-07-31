using API.Entities;
using API.Interfaces;
using Shared.Contracts;
using System.Text.Json;

namespace API.Services;

public sealed class JsonFeatureStore : IFeatureStore
{
    private string _filepath;

    public JsonFeatureStore(string filepath)
    {
        if (string.IsNullOrWhiteSpace(filepath))
            throw new ArgumentException($"File path cannot be empty or null." + nameof(filepath));

        _filepath = filepath;
    }

    public UserFeatures GetFeatures(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException($"Api key provided cannot be empty. {nameof(apiKey)}");

        var json = File.ReadAllText(_filepath);

        List<UserFeatures>? userFeatures;

        try
        {           
            userFeatures = JsonSerializer.Deserialize<List<UserFeatures>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (userFeatures == null || userFeatures.Count == 0)
                throw new InvalidOperationException($"Provided JSON cannot be empty.");

        }

        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to deserialize Json.", ex);
        }


        Dictionary<string, List<FeatureFlag>> entries = new();

        try
        {
            entries = userFeatures.ToDictionary(x => x.ApiKey, y => y.Features);
        }

        catch (ArgumentException)
        {
            // We'd log it here.
            throw;
        }

        if (entries == null || entries.Count == 0)
            throw new InvalidOperationException("");
        
        // If key does not exist, throw exception
        if (!entries.TryGetValue(apiKey, out var features))
            throw new KeyNotFoundException($"Api key provided is not found. {nameof(apiKey)}");

        else
            return new(apiKey, entries[apiKey]);
    }

    public bool HasFeature(string apiKey, string featureName)
    {
        var userFeatures = GetFeatures(apiKey);

        // If it contains feature, return true.
        return userFeatures.Features.Any(x => x.Name == featureName);
    }
}