using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Interfaces;
public interface IFeature
{
    public bool CanUse { get; }
    Task<bool> ExecuteAsync(CancellationToken cancellationToken = default);
}
