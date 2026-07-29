
using API.Entities;
using Shared.Contracts;
using System.Text.Json;

namespace API.UnitTests.Services;

public class JsonFeatureStore
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

        Dictionary<string, List<FeatureFlag>> entries = new();

        try
        {           
            var userFeatures = JsonSerializer.Deserialize<List<UserFeatures>>(json);

            if (userFeatures == null)
                throw new InvalidOperationException($"Provided JSON cannot be empty.");

            entries = userFeatures.ToDictionary(x => x.ApiKey, y => y.Features);
        }

        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to deserialize Json.", ex);
        }

        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"Given Json is empty." + ex);
        }

        catch (Exception ex)
        {
            throw new Exception("Something went wrong." + ex);
        }


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