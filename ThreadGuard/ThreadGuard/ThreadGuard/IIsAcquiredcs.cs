using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    public interface IIsAcquired
    {
        bool IsAcquired { get; }
    }
}
