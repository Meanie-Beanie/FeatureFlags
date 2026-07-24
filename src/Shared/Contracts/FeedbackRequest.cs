using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts;
public record FeedbackRequest
{
    public string Message { get; init; } = string.Empty;
}
