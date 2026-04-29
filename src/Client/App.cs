using Client.Interfaces;

namespace Client;

public class App
{
    private readonly IUserInterface _userInterface;
    private readonly IFeatureAccessService _featureAccessService;

    public App(IUserInterface console, IFeatureAccessService featureAccessService)
    {
        _userInterface = console;
        _featureAccessService = featureAccessService;
    }

    public async Task<int> RunAsync()
    {
        var isAuthenticated = await AuthenticateAsync();
        
        SelectFeature();

        return 0;
    }

    private async Task<bool> AuthenticateAsync()
    {
        while (true)
        {
            _userInterface.ShowMessage("Please enter your Api key. Press escape to quit.");
            var input = _userInterface.GetInput();

            /*
             * Note:
             * Currently ties implementation directly to ConsoleKey so abstraction is required at some point.
            */ 
            if (input == ConsoleKey.Escape.ToString())
                return false;

            // Loop until user gives a correct key.
            if (string.IsNullOrEmpty(input))
                continue;

            var userAuth = await _featureAccessService.RequestAccessAsync(input);

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
    private void SelectFeature()
    {
        _userInterface.ShowMessage("Select one of the following features by pressing a number:");

        for (int i = 0; i < _featureAccessService.EnabledFeatures.Count; i++)
        {
            _userInterface.ShowMessage($"{i + 1}. {_featureAccessService.EnabledFeatures[i]}");
        }

        while (true)
        {
            if (int.TryParse(_userInterface.GetInput(), out var selection))
            {
                if (selection > _featureAccessService.EnabledFeatures.Count + 1 && selection > 0)
                {
                    _userInterface.ShowMessage("Select a feature from the list.");
                    continue;
                }

                _featureAccessService.HasFeature(_featureAccessService.EnabledFeatures[selection]);

            }
        }

    }
}