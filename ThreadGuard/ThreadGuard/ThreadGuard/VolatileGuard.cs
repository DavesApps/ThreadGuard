using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    /// <summary>
    /// VolatileGuard class must be created with the threadID of the one thread that is allowed to write to variables protected by this guard.
    /// Any other thread will be allowed to read.  To be used with GuardedVolatileVarBool, GuardedVolatileVarInt etc. (those classes enforce the one write thread)
    /// </summary>
    public class VolatileGuard : ISynchronizeGuard, IDisposable, ISynchronizeGuardContext, IIsAcquired, IContextSupport
    {

        IGuardFailure _guardFailure = GlobalGuardFailure.FailInterface;

        
        private UInt64 _contextVersion = ContextConstants.DEFAULTVERSIONGUARD;
        private int _threadID;

        bool _supportsContext = false;

        //default constructor
        public VolatileGuard(int threadID)
        {
            _threadID= threadID;
                       
        }

        

        /// <summary>
        /// Provide an alternate guard failure interface to handle failures differently if desired.
        /// </summary>
        /// <param name="failure"></param>
        public VolatileGuard(IGuardFailure failure,int threadID) : this(threadID)
        {
            _guardFailure = failure;

        }

        public IDisposable Acquire()
        {
            

            return this;
        }

        public void Release()
        {
            
        }

        public void Dispose()
        {
            Release();
        }

        /// <summary>
        /// Returns acquired if we are currently on the one thread allowed to write
        /// </summary>
        public bool IsAcquired { get { return Thread.CurrentThread.ManagedThreadId==_threadID; } }

        public UInt64 ContextVersion
        {
            get
            {
                return _contextVersion;
            }
        }

        /// <summary>
        /// Checks to see if it supports context
        /// </summary>
        public bool IsSupportingConext
        {
            get { return _supportsContext; }
        }
     

    }


}
