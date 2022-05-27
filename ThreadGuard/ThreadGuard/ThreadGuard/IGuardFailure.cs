using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    public interface IGuardFailure
    {
        void Fail(object obj, string message);
    }
}
