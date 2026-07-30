using API.Interfaces;
using API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.UnitTests.Services;

/*
 * The functionality of the feedback system is out of the scope of this project but we'll add this test just here for test coverage and if future expansion happens. 
*/ 

public class FeedbackServiceTests
{
    [Fact]
    public void Send_FeedbackSentSuccesfully_DoesNotThrowException()
    {
        string message = "Test";

        var SUT = new FeedbackService();
        SUT.Send(message);
    }
}
