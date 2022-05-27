using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadGuard;

namespace ThreadGuardTest
{
    [TestClass]
    public class MonitorGuardUnitTests
    {

        /// <summary>
        /// Verify MonitorGuardAcquire and release called
        /// </summary>
        [TestMethod]
        public void VerifyMonitorGuardAcquireRelease()
        {
            MonitorGuard mgc = new MonitorGuard();
            using (mgc.Acquire())
            {
                Assert.IsTrue(mgc.IsAcquired, "Should be true after acquired.");
            }

            Assert.IsFalse(mgc.IsAcquired, "Should be false after released.");


        }

        GuardedVar<int> SimulateFindItem(ISynchronizeGuard context, GuardedVar<int> item)
        {
            GuardedVar<int> foundItem1 = new GuardedVar<int>(context, item.Value);

            return foundItem1;
        }

        /// <summary>
        /// MonitorGuardAcquireSimulationTest  check a more typical usage
        /// No exceptions should be triggered
        /// </summary>
        [TestMethod]
        public void MonitorGuardAcquireSimulationTest()
        {
            MonitorGuard mgc = new MonitorGuard();
            GuardedVar<bool> tcv = new GuardedVar<bool>(mgc);
            int fi = 0;
            using (mgc.Acquire())
            {
                tcv.Value = true;
                bool myval2 = tcv.Value;
                GuardedVar<int> foundItem = SimulateFindItem(mgc, new GuardedVar<int>(mgc, 1));
                fi = foundItem.Value;
            }
        }



        /// <summary>
        /// MonitorGuardTestGuardFailureInterface
        /// </summary>
        [TestMethod]
        public void MonitorGuardTestGuardFailureInterface()
        {
            TestGuardFailure tgf = new TestGuardFailure();
            MonitorGuard _mgc = new MonitorGuard();
            GuardedVar<bool> _tcv = new GuardedVar<bool>(_mgc, tgf);

            InvalidOperationException ioe = null;
            try
            {
                _tcv.Value = false; // Will call the IGuardFailure on tgf
            }
            catch(InvalidOperationException e)
            {
                ioe = e;
            }

            Assert.IsNotNull(ioe, "Should have caught exception");
            Assert.IsTrue(tgf.Called, "Should have been called from setting the GuardedVar outside of a lock");
            Assert.IsTrue(tgf.Message != null, "Should have a message set");
            Assert.IsNotNull(tgf.Obj , "Should equal the guarded var - check for not null since it's a struct");

        }

    }
}
