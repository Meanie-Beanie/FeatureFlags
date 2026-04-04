using Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.UserInterface;
public class ConsoleInterface : IUserInterface
{
    public string? GetInput()
        => Console.ReadLine();

    public void ShowMessage(string? value)
        => Console.Write(value);
}
