using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    /// <summary>
    /// Class implementation to handle Guard Failures
    /// </summary>
    class GuardFailure : IGuardFailure
    {
        public void Fail(object obj, string message)
        {
            throw new GuardFailureException(message);
        }
    }
}
