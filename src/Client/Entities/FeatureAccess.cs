using Shared.Contracts;

namespace Client.Entities;
public sealed class FeatureAccess
{
    public bool IsAuthorized { get; set; } = false;

    public string? ErrorMessage { get; set; }

    // Tied to Shared.Contracts. If done properly, would have it's own mapping to decouple it from infastructure layer.
    // But this is a small scale project and this makes everything more complex and difficult
    public List<string> Features { get; set; } = new();
}
