using Client.Interfaces;

namespace Client.Services;
public class ConsoleInterface : IUserInterface
{
    public string? GetInput()
        => Console.ReadLine();

    public void ShowMessage(string? value)
        => Console.WriteLine(value);
}
