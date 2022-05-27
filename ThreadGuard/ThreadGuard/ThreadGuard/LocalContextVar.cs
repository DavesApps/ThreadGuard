using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    /// <summary>
    /// LocalContextVar creates a variable of type T that will be protected by the guard implementing the synchronize interface.
    /// Currently this class is experimental  It is not recommended to use at this time.  There are no tests for this.  Its not clear that it
    /// adds value yet.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LocalContextVar<T>
    {
        private T _value = default(T);
        private ISynchronizeGuard _guard;
        private IGuardFailure _guardFailure = GlobalGuardFailure.FailInterface;
        private UInt64 _contextVersion;

        /// <summary>
        /// Initialize a guard variable, associate with an acquire checker
        /// </summary>
        /// <param name="guard">The guard checker to be used</param>
        public LocalContextVar(ISynchronizeGuard guard) :this(guard,GlobalGuardFailure.FailInterface)
        {
            //_guard = guard;
        }

        /// <summary>
        /// Initialize a guard variable, associate with an acquire checker
        /// </summary>
        /// <param name="guard">The guard checker to be used</param>
        public LocalContextVar(ISynchronizeGuard guard,IGuardFailure guardFailure) : this(guard,default(T),guardFailure)
        {
            //_guard = guard;
        }

        /// <summary>
        /// Initialize a guard variable associate with an acquire checker and set a default value
        /// </summary>
        /// <param name="guard"></param>
        /// <param name="defaultValue"></param>
        public LocalContextVar(ISynchronizeGuard guard, T defaultValue) : this( guard, defaultValue,GlobalGuardFailure.FailInterface)
        {
            
        }

        /// <summary>
        /// Initialize a guard variable associate with an acquire checker and set a default value
        /// </summary>
        /// <param name="guard"></param>
        /// <param name="defaultValue"></param>
        public LocalContextVar(ISynchronizeGuard guard, T defaultValue,IGuardFailure guardFailure)
        {
            _guard = guard;
            _value = defaultValue;
            _guardFailure = guardFailure;

            if (guard.IsAcquired)
                _contextVersion = _guard.ContextVersion; //ContextConstants.DEFAULTVERSIONCONTEXTVAR;
            else
                _contextVersion = ContextConstants.DEFAULTVERSIONCONTEXTVAR;

        }


        /// <summary>
        /// The value for this variable which can be read or written to
        /// </summary>
        public T Value
        {
            get
            {
                CheckGuard();
                if (_contextVersion != _guard.ContextVersion)
                    _guardFailure.Fail(this, "Context version variable was set with doesn't match current guard context version.");

                return _value;
            }

            set
            {
                CheckGuard();
                _contextVersion = _guard.ContextVersion;
                _value = value;

            }
        }

        /// <summary>
        /// SetValueNoGuard sets the value with no guard check
        /// </summary>
        /// <param name="value"></param>
        public void SetValueNoGuard(T value)
        {
            _value = value;
        }

        /// <summary>
        /// GetValueNoGuard - return value with no guard check
        /// </summary>
        /// <returns></returns>
        public T GetValueNoGuard()
        {
            return _value;
        }

        public ISynchronizeGuard Guard
        {
            get { return _guard; }
        }

        private void CheckGuard()
        {
            if (!_guard.IsAcquired)
                _guardFailure.Fail(this, "Guard was not acquired operation. Type:" + typeof(T).ToString());
                
        }
    }
}
