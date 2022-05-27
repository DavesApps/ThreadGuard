using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadGuard;

namespace ThreadGuardTest
{
    /// <summary>
    /// TestGuardFailure used for testing guard failures
    /// </summary>
    class TestGuardFailure : IGuardFailure
    {
        private bool _called = false;
        private object _obj = null;
        private string _message;

        public bool Called {
            get => _called;
            private set => _called = value;
        }

        public object Obj
        {
            get => _obj;
            private set => _obj = value;
        }

        public string Message
        {
            get => _message;
            private set => _message = value;
        }


        public void Fail(object obj, string message)
        {
            Called = true;
            Obj = obj;
            Message = message;
            throw new InvalidOperationException("Fail");
        }
    }
}
