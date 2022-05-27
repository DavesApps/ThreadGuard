using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    /// <summary>
    /// GuardedVar creates a variable of type T that will be protected by the guard implementing the synchronize interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GuardedVar<T>
    {
        protected T _value;
        private ISynchronizeGuard _synchronizeContext;
        private UInt64 _contextVersion;
        IGuardFailure _guardFailure; 

        /// <summary>
        /// Initialize a guard variable, associate with an acquire checker
        /// </summary>
        /// <param name="guard">The guard checker to be used</param>
        public GuardedVar(ISynchronizeGuard context) :this(context,GlobalGuardFailure.FailInterface, default(T))
        {
            
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The synchronization guard</param>
        /// <param name="guardFailure">The guardfailure interface</param>
        public GuardedVar(ISynchronizeGuard context,IGuardFailure guardFailure) : this(context, guardFailure, default(T))
        {


        }

        public GuardedVar(ISynchronizeGuard context, IGuardFailure guardFailure, T defaultValue)
        {
            if (guardFailure != null)
                _guardFailure = guardFailure;
            else
                _guardFailure = GlobalGuardFailure.FailInterface;

            //This is only needed if we support guards that can't use a context sometimes.  Remove if we won't support
            CheckIfGuardSupportsMultithreadedSynchronization(context);

            _synchronizeContext = context;
            _value = defaultValue;


            if (context.IsAcquired)
                _contextVersion = _synchronizeContext.ContextVersion; //ContextConstants.DEFAULTVERSIONCONTEXTVAR;
            else
                _contextVersion = ContextConstants.DEFAULTVERSIONCONTEXTVAR;
        }

        protected virtual void CheckIfGuardSupportsMultithreadedSynchronization(ISynchronizeGuard context)
        {
            if (context is IContextSupport)
            {
                if (!((IContextSupport)context).IsSupportingConext)
                    throw new GuardFailureException("The guard does not support protecting variables from multithreaded access with a context.");
            }
        }

        /// <summary>
        /// Initialize a guard variable associate with an acquire checker and set a default value
        /// </summary>
        /// <param name="guard"></param>
        /// <param name="defaultValue">Value assigned unguarded since the object has not been available for use yet</param>
        public GuardedVar(ISynchronizeGuard context, T defaultValue) : this(context, GlobalGuardFailure.FailInterface, defaultValue)
        {
            
        }

        public UInt64 ContextVersion
        {
            get => _contextVersion;
        }

        /// <summary>
        /// ReadOnlyValue  makes read intention obvious.  Must have exclusive or read guard acquired
        /// </summary>
        public virtual T ReadOnlyValue
        {
            get
            {
                if (!_synchronizeContext.IsAcquired  && !(_synchronizeContext as IReaderWriterSynchronizeGuard).IsAcquiredRead)
                    _guardFailure.Fail(this, "Set ThreadContextVar failed.  Guard: " + _synchronizeContext.ToString() + " was not acquired.");


                return _value;
            }
        }


        /// <summary>
        /// The value for this variable which can be read or written to
        /// </summary>
        public virtual T Value
        {
            get
            {

                if (!_synchronizeContext.IsAcquired)
                    _guardFailure.Fail(this, "Set ThreadContextVar failed.  Guard: " + _synchronizeContext.ToString() + " was not acquired.");


                return _value;
            }

            set
            {
                if (!_synchronizeContext.IsAcquired)
                    _guardFailure.Fail(this, "Set ThreadContextVar failed.  Guard: " + _synchronizeContext.ToString() + " was not acquired.");

                _contextVersion = _synchronizeContext.ContextVersion;

                _value = value;

            }
        }

        /// <summary>
        /// GetValueNoGuard - return value with no guard check
        /// </summary>
        /// <returns></returns>
        public virtual T GetUnguardedValue()
        {
            return _value;
        }

        /// <summary>
        /// SetUnguardedValue  sets the value without checking if the guard has been acquired
        /// </summary>
        /// <param name="value">The value to set</param>
        public virtual void SetUnguardedValue(T value)
        {
            _value = value;
        }


        /// <summary>
        /// SynchronizeContext gets the guard
        /// </summary>
        public virtual ISynchronizeGuardContext SynchronizeContext
        {
            get { return _synchronizeContext; }
        }

        /// <summary>
        /// Protected property for derived classes to get the guard interface
        /// </summary>
        protected ISynchronizeGuard Guard
        {
            get { return _synchronizeContext;  } 
            
        }

        /// <summary>
        /// Get the current guard failure class
        /// </summary>
        protected IGuardFailure GuardFailure
        {
            get { return _guardFailure; }
        }

    }

    }
