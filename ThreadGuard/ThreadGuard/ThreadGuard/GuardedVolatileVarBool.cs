using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ThreadGuard
{


    /// <summary>
    /// GuardedVolatileVarBool 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GuardedVolatileVarBool : GuardedVar<bool>
        {
            private bool _oneWriteOnly = false;
            private bool _valueWritten = false;

        /// <summary>
        /// GuardedVolatileVarBool - default constructor creates our own VolatileGuard and uses the current thread as the only thread allowed to write the value
        /// </summary>
        public GuardedVolatileVarBool(bool oneWriteOnly=false) : this(new VolatileGuard(Thread.CurrentThread.ManagedThreadId), GlobalGuardFailure.FailInterface, false,oneWriteOnly)
            {

            }


            /// <summary>
            /// Initialize a guard variable, associate with an acquire checker
            /// </summary>
            /// <param name="guard">The guard checker to be used</param>
            public GuardedVolatileVarBool(ISynchronizeGuard context) : this(context, GlobalGuardFailure.FailInterface, false)
            {


            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="context">The synchronization guard</param>
            /// <param name="guardFailure">The guardfailure interface</param>
            public GuardedVolatileVarBool(ISynchronizeGuard context, IGuardFailure guardFailure) : this(context, guardFailure, false)
            {


            }

            public GuardedVolatileVarBool(ISynchronizeGuard context, IGuardFailure guardFailure, bool defaultValue, bool oneWriteOnly=false) : base(context, guardFailure, defaultValue)
            {
                if ((context as VolatileGuard)==null)
                    throw new InvalidOperationException("ISynchronizeGuard must be of type VolatileGuard");

                _oneWriteOnly = oneWriteOnly;
            }

            /// <summary>
            /// Initialize a guard variable associate with an acquire checker and set a default value
            /// </summary>
            /// <param name="guard"></param>
            /// <param name="defaultValue">Value assigned unguarded since the object has not been available for use yet</param>
            public GuardedVolatileVarBool(ISynchronizeGuard context, bool defaultValue) : this(context, GlobalGuardFailure.FailInterface, defaultValue)
            {

            }

            /// <summary>
            /// The value for this variable which can be read or written to
            /// </summary>
            public override bool Value
            {
                get
                {
                    return Volatile.Read(ref _value);

                }

                set
                {
                    if (!Guard.IsAcquired)
                        GuardFailure.Fail(this, "Set ThreadContextVar failed. Must be on thread specified in Guard.  Guard: " + Guard.ToString() + " was not acquired.");

                    if (_oneWriteOnly)
                    {
                       if (_valueWritten)
                       {
                            if (value!= _value)
                                throw new InvalidOperationException("Write once option is enabled.  Attempted second write.");
                        }
                       else _valueWritten = true;

                    }

                    Volatile.Write(ref _value, value);


                }
            }

        /// <summary>
        /// Makes it obvious the read intention this is just a redirect to reading Value
        /// </summary>
        public override bool ReadOnlyValue
        {
            get
            {       
                return Value;
            }
        }

        /// <summary>
        /// Empty to avoid the base class throwing an exception
        /// </summary>
        /// <param name="context"></param>
        protected override void CheckIfGuardSupportsMultithreadedSynchronization(ISynchronizeGuard context)
            {

            }


        /// <summary>
        /// GetValueNoGuard - return value with no guard check (Still reads with Volatile.Read)
        /// </summary>
        /// <returns></returns>
        public override bool GetUnguardedValue()
        {
            return Value;
        }

        /// <summary>
        /// SetUnguardedValue  sets the value without checking if the guard has been acquired.  Still writes using Volatile.Write
        /// </summary>
        /// <param name="value">The value to set</param>
        public override void SetUnguardedValue(bool value)
        {
            Volatile.Write(ref _value, value);
        }


        /// <summary>
        /// SynchronizeContext gets the guard
        /// </summary>
        public override ISynchronizeGuardContext SynchronizeContext
        {
            get { throw new NotImplementedException("Not supported for volatile Guarded Variables"); }
        }


    }
    
}
