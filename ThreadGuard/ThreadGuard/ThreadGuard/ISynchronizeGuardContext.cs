using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    public interface ISynchronizeGuardContext
    {
        UInt64 ContextVersion { get; }
    }
}
