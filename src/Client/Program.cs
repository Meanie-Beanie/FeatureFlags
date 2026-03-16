using Client;
using Client.Interfaces;
using Client.Services;

HttpClient httpClient = new HttpClient(); // Config later

IUserInterface userInterface = new ConsoleInterface();
IFeatureFlagService featureFlagService = new FeatureFlagService(httpClient);
IFeatureAccessService featureAccesService = new FeatureAccessService(featureFlagService);

App app = new App(userInterface, featureAccesService);
await app.RunAsync();

