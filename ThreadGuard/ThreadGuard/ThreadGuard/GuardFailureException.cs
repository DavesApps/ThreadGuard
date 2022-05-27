using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    /// <summary>
    /// GuardFailureException - exception that is thrown when there is a guard failure exception
    /// </summary>
    public  class GuardFailureException :InvalidOperationException
    {
        public GuardFailureException(string message) : base(message)
        {

        }
    }
}
