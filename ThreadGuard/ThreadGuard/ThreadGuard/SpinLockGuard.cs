using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ThreadGuard
{
    /// <summary>
    /// SpinLockGuard wrapper around Monitor class
    /// </summary>
    public class SpinLockGuard : ISynchronizeGuard, IDisposable, ISynchronizeGuardContext, IIsAcquired
    {
        
        private SpinLock _spinLock = new SpinLock();

        IGuardFailure _guardFailure = GlobalGuardFailure.FailInterface;

        ThreadLocal<bool> _isAcquired = new ThreadLocal<bool>();
        private UInt64 _contextVersion = ContextConstants.DEFAULTVERSIONGUARD;

        private int _referenceCount = 0;

        //default constructor
        public SpinLockGuard() { }

        /// <summary>
        /// Provide an alternate guard failure interface to handle failures differently if desired.
        /// </summary>
        /// <param name="failure"></param>
        public SpinLockGuard(IGuardFailure failure)
        {
            _guardFailure = failure;
        }


        /// <summary>
        /// acquire the lock (supports re-entrant)
        /// </summary>
        /// <returns></returns>
        public IDisposable Acquire()
        {
            bool acquired = false;
            _spinLock.Enter(ref acquired);

            if (_isAcquired.Value != true)
                _contextVersion++;

            _isAcquired.Value = acquired;

            _referenceCount++;  // handle re-entrant lock support



            return this;
        }

        /// <summary>
        /// Release() the lock
        /// </summary>
        public void Release()
        {
            if (_isAcquired.Value)
            {
                
                _isAcquired.Value = false;
                _contextVersion++;
                _spinLock.Exit();

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
        ~SpinLockGuard()
        {
            _isAcquired.Dispose();
        }
    }
}
