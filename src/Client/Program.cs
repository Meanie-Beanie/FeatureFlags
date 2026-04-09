using Client;
using Client.Interfaces;

IUserInterface userInterface = new ConsoleInterface();
IFeatureFlagService featureFlagService = new FeatureFlagService();

App app = new App(userInterface, featureFlagService);
await app.RunAsync();

