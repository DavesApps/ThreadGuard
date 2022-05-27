using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    /// <summary>
    /// IContextSupport - to tell if supports context
    /// </summary>
    public interface IContextSupport
    {
        bool IsSupportingConext { get; }
    }
}
