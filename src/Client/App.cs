using Client.Features;
using Client.Interfaces;

namespace Client;

public class App
{
    private readonly IUserInterface _userInterface;
    private readonly IFeatureAccessService _featureAccessService;
    
    // This will be gone later.
    private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7078")};

    public App(IUserInterface console, IFeatureAccessService featureAccessService)
    {
        _userInterface = console;
        _featureAccessService = featureAccessService;
    }

    public async Task<int> RunAsync()
    {
        var isAuthenticated = await AuthenticateAsync();

        if (_featureAccessService.EnabledFeatures.Count == 0)
        {
            _userInterface.ShowMessage("User has no access to services. Please, contact customer service.");
            _userInterface.GetInput();
            return 0;
        }    
        
        while (true)
        {
            await SelectFeature();
            _userInterface.ShowMessage("Press any key to continue or  0 to shut down.");
            
            if(int.TryParse(_userInterface.GetInput(), out var input))
            {
                if (input == 0)
                    return 0;
            }
        }
    }

    private async Task<bool> AuthenticateAsync()
    {
        while (true)
        {
            _userInterface.ShowMessage("Please enter your Api key. Press escape to quit.");
            var userApiKey = _userInterface.GetInput();

            /*
             * Note:
             * Currently ties implementation directly to ConsoleKey so abstraction is required at some point.
            */ 
            if (userApiKey == ConsoleKey.Escape.ToString())
                return false;

            // Loop until user gives a correct key.
            if (string.IsNullOrEmpty(userApiKey))
                continue;

            var userAuth = await _featureAccessService.RequestAvailableServicesAsync(userApiKey);

            if (!userAuth.IsAuthorized)
            {
                _userInterface.ShowMessage("Api key provided is not authorized. Please enter a valid key.");
                continue;
            }

            else
                return userAuth.IsAuthorized;
        }
    }


    /*
     * Due to the time constraints, tied to the console app. 
     * 
     * My decision to make features as simple strings are biting me in the butt.
     * It would be so easy to overload HasFeature -method by doing a system that checks it via id's
     * but I foolish went against my own better judgement.
    */
    private async Task SelectFeature()
    {
        _userInterface.ShowMessage("Select one of the following features by pressing a number:");

        for (int i = 0; i < _featureAccessService.EnabledFeatures.Count; i++)
        {
            _userInterface.ShowMessage($"{i + 1}. {_featureAccessService.EnabledFeatures[i]}");
        }

        _userInterface.ShowMessage("Select a feature from the list.");

        while (true)
        {
            if (int.TryParse(_userInterface.GetInput(), out var selection))
            {
                if (selection > _featureAccessService.EnabledFeatures.Count + 1 && selection > 0)
                {
                    continue;
                }

                // We are simply going to manually launch it because I FORGOT to apply this feature. It works for a prototyping though.
                FeedbackFeature feedbackFeature = new(_featureAccessService, _userInterface, _httpClient);
                await feedbackFeature.ExecuteAsync();
            }
        }
    }
}