using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ThreadGuard
{
    /// <summary>
    /// SemaphoreSlimGuard wrapper around SemaphoreSlim class
    /// </summary>
    public class SemaphoreSlimGuard : ISynchronizeGuard, IDisposable, ISynchronizeGuardContext, IIsAcquired, IContextSupport
    {

        IGuardFailure _guardFailure = GlobalGuardFailure.FailInterface;

        ThreadLocal<bool> _isAcquired = new ThreadLocal<bool>();
        private UInt64 _contextVersion = ContextConstants.DEFAULTVERSIONGUARD;
        private Semaphore _semaphore = null;
        bool _supportsContext = false;

        //default constructor
        public SemaphoreSlimGuard(int initialCount, int maximumCount)
        {
            if (initialCount == 1 && maximumCount == 1)
                _supportsContext = true;

            _semaphore = new Semaphore(initialCount, maximumCount);
        }

        /// <summary>
        /// Provide an alternate guard failure interface to handle failures differently if desired.
        /// </summary>
        /// <param name="failure"></param>
        public SemaphoreSlimGuard(IGuardFailure failure)
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
        /// True if supporting context only happens if count is 1 for semaphore
        /// </summary>
        public bool IsSupportingConext
        {
            get { return _supportsContext; }
        }

        ~SemaphoreSlimGuard()
        {
            _isAcquired.Dispose();
        }


    }
}
