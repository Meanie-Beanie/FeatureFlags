namespace Client.Interfaces;

public interface IUserInterface
{
    void ShowMessage(string? value);
    string? GetInput();
}