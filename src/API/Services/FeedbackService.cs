using API.Interfaces;

namespace API.Services;

public class FeedbackService : IFeedbackService
{
    public void Send(string message)
    {
        Console.WriteLine("Sending message: " + message);
    }
}
