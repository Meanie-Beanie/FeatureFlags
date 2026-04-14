using Client;
using Client.Interfaces;
using Client.Services;

HttpClient httpClient = new HttpClient(); // Config later

IUserInterface userInterface = new ConsoleInterface();
IFeatureFlagService featureFlagService = new FeatureFlagService(httpClient);

App app = new App(userInterface, featureFlagService);
await app.RunAsync();

