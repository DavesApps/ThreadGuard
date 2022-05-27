using System;
using System.Threading;

namespace ThreadGuard
{
    /// <summary>
    /// MonitorGuard wrapper around Monitor class
    /// </summary>
    public class MonitorGuard : ISynchronizeGuard, IDisposable, ISynchronizeGuardContext, IIsAcquired
    {
        private object _lockobj = new object();
        IGuardFailure _guardFailure = GlobalGuardFailure.FailInterface;

        ThreadLocal<bool> _isAcquired = new ThreadLocal<bool>();
        private UInt64 _contextVersion = ContextConstants.DEFAULTVERSIONGUARD;

        private int _referenceCount = 0;

        //default constructor
        public MonitorGuard() { }

        /// <summary>
        /// Provide an alternate guard failure interface to handle failures differently if desired.
        /// </summary>
        /// <param name="failure"></param>
        public MonitorGuard(IGuardFailure failure)
        {
            _guardFailure = failure;
        }


        /// <summary>
        /// acquire the lock (supports re-entrant)
        /// </summary>
        /// <returns></returns>
        public IDisposable Acquire()
        {
            Monitor.Enter(_lockobj);

            if (_isAcquired.Value!=true)
                _contextVersion++;

            _isAcquired.Value = true;

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
                
                //_contextVersion++;
                _referenceCount--;    //re-entrant lock support
                if (_referenceCount < 0)
                {
                    _guardFailure.Fail(this, "Reference count <0 - released more than acquired.");
                    _referenceCount = 0;
                }


                if (Monitor.IsEntered(_lockobj))  //only exit if reference count=0;
                {
                    if (_referenceCount == 0 )
                        _isAcquired.Value = false;

                    Monitor.Exit(_lockobj);
                }

                
                

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
        ~MonitorGuard()
        {
            _isAcquired.Dispose();
        }
    }
}
