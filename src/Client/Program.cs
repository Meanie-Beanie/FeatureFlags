using Client;
using Client.Interfaces;
using Client.Services.FeatureFlag;
using Client.Services.UserInterface;

IUserInterface userInterface = new ConsoleInterface();
IFeatureFlagService featureFlagService = new FeatureFlagService();

App app = new App(userInterface, featureFlagService);
await app.RunAsync();

