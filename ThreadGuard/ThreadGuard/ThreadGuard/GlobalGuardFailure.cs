using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    /// <summary>
    /// GlobalGuardFailure used to store an instance of an object used to handle failures from Guard objects.
    /// Changing FailInterface to a new object - will allow any new Guard objects created afterwards to use that new failure interface.
    /// For example you might want unguarded detection to fail during debug sessions differently than production.  Guard classes usually provide a way 
    /// to override this if desired.
    /// </summary>
    static class GlobalGuardFailure
    {
        static public IGuardFailure FailInterface = new GuardFailure();
    }
}
