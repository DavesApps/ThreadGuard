using System;
using System.Threading;
using ThreadGuard;

namespace ThreadGuardTest
{
    /// <summary>
    /// FakeGuard wrapper around  class used for testing only
    /// </summary>
    public class FakeGuard : ISynchronizeGuard, IDisposable, ISynchronizeGuardContext
    {
        private object _lockobj = new object();

        ThreadLocal<bool> _isAcquired = new ThreadLocal<bool>();
        private UInt64 _contextVersion = ContextConstants.DEFAULTVERSIONGUARD;

        public IDisposable Acquire()
        {
            
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
                    throw new InvalidOperationException("Can't retrieve the context version when the guard has not been acquired.");

                return _contextVersion;
            }
        }
        ~FakeGuard()
        {
            _isAcquired.Dispose();
        }
    }
}
