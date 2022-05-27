using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    /// <summary>
    /// SemaphoreGuard wrapper around Monitor class
    /// </summary>
    public class SemaphoreGuard : ISynchronizeGuard, IDisposable, ISynchronizeGuardContext, IIsAcquired, IContextSupport
    {

        IGuardFailure _guardFailure = GlobalGuardFailure.FailInterface;

        ThreadLocal<bool> _isAcquired = new ThreadLocal<bool>();
        private UInt64 _contextVersion = ContextConstants.DEFAULTVERSIONGUARD;
        private Semaphore _semaphore = null;

        bool _supportsContext = false;

        //default constructor
        public SemaphoreGuard(int initialCount,int maximumCount)
        {
            if (initialCount == 1 && maximumCount == 1)
                _supportsContext = true;

            _semaphore = new Semaphore(initialCount, maximumCount);
        }

        public SemaphoreGuard(int initialCount, int maximumCount, IGuardFailure failure) : this(initialCount,maximumCount)
        {
            _guardFailure = failure;

        }

        /// <summary>
        /// Provide an alternate guard failure interface to handle failures differently if desired.
        /// </summary>
        /// <param name="failure"></param>
        public SemaphoreGuard(IGuardFailure failure)
        {
            _guardFailure = failure;
            
        }

        public IDisposable Acquire()
        {
            _semaphore.WaitOne();
            _isAcquired.Value = true;
            _contextVersion++;

            return this;
        }

        public void Release()
        {
            if (_isAcquired.Value)
            {
                _isAcquired.Value = false;
                _contextVersion++;

                _semaphore.Release();

            }
        }

        public void Dispose()
        {
            Release();
        }

        public bool IsAcquired { get { return _isAcquired.Value; } }

        public UInt64 ContextVersion
        {
            get
            {
                if (_isAcquired.Value == false)
                    _guardFailure.Fail(this, "Can't retrieve the context version when the guard has not been acquired.");


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

        ~SemaphoreGuard()
        {
            _isAcquired.Dispose();
        }

        
    }


}
