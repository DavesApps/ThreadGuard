using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ThreadGuard
{
    /// <summary>
    /// MutexGuard wrapper around Monitor class
    /// </summary>
    public class MutexGuard : ISynchronizeGuard, IDisposable, ISynchronizeGuardContext, IIsAcquired
    {
        
        IGuardFailure _guardFailure = GlobalGuardFailure.FailInterface;

        ThreadLocal<bool> _isAcquired = new ThreadLocal<bool>();
        private UInt64 _contextVersion = ContextConstants.DEFAULTVERSIONGUARD;
        private Mutex _mutext = null;

        private int _referenceCount = 0;

        //default constructor
        public MutexGuard()
        {
            _mutext = new Mutex();
        }

        /// <summary>
        /// Provide an alternate guard failure interface to handle failures differently if desired.
        /// </summary>
        /// <param name="failure"></param>
        public MutexGuard(IGuardFailure failure)
        {
            _guardFailure = failure;
            
        }

        /// <summary>
        /// Acquire
        /// </summary>
        /// <returns></returns>
        public IDisposable Acquire()
        {
            _mutext.WaitOne();

            if (_isAcquired.Value != true)
            {
                _contextVersion++;
                _isAcquired.Value = true;
            }

            _referenceCount++;  // handle re-entrant lock support

            return this;
        }

        /// <summary>
        /// Release
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

                if (_referenceCount == 0)
                    _isAcquired.Value = false;

                _mutext.ReleaseMutex();



            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Release();
        }

        /// <summary>
        /// IsAcquired
        /// </summary>
        public bool IsAcquired { get { return _isAcquired.Value; } }

        /// <summary>
        /// ContextVersion
        /// </summary>
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
        /// 
        /// </summary>
        ~MutexGuard()
        {
            _isAcquired.Dispose();
        }
    }

}
