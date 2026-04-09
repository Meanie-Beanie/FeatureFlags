using Client.Interfaces;

namespace Client;

public class App
{
    private readonly IUserInterface _userInterface;
    private readonly IFeatureFlagService _featureFlagService;

    public App(IUserInterface console, IFeatureFlagService featureFlagsService)
    {
        _userInterface = console;
        _featureFlagService = featureFlagsService;
    }

    public async Task<int> RunAsync()
    {
        return 0;
    }

    private async Task<IEnumerable<object>> AuthenticateAsync()
    {
        throw new NotImplementedException();
    }
}