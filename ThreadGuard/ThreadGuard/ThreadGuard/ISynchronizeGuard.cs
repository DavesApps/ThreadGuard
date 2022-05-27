using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    public interface ISynchronizeGuard : IIsAcquired, ISynchronizeGuardContext
    {
        IDisposable Acquire();
        void Release();
    }
}
