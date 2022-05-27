using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadGuard;

namespace ThreadGuardTest
{
    [TestClass]
    public class GuardedVarUnitTests
    {
        [TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        public void TestGuardedVarContextVersion()
        {
            MonitorGuard mgc = new MonitorGuard();
            GuardedVar<bool> tcv = new GuardedVar<bool>(mgc);

            using (mgc.Acquire())
            {
                tcv.Value = true;
                Assert.IsTrue(mgc.ContextVersion == tcv.ContextVersion, "ContextVersion's should match since we assigned a value within the context");
            }

        }

        /// <summary>
        /// TestGuardedVarDefaultValue - assign default value and check value after acquire
        /// </summary>
        [TestMethod]
        public void TestGuardedVarDefaultValue()
        {
            MonitorGuard mgc = new MonitorGuard();
            GuardedVar<int> tcv = new GuardedVar<int>(mgc, 89740538);

            using (mgc.Acquire())
            {

                Assert.IsTrue(tcv.Value == 89740538, "Default value should match.");
            }

        }

        /// <summary>
        /// UseGuardedVarWithoutGuardThrowsInvalidOperationException negative test to confirm exception when accessing
        /// guarded variable without the guard being acquired.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardFailureException))]
        public void GuardedVarAccesswithoutAcquireThrowsInvalidOperationException()
        {
            MonitorGuard _mgc = new MonitorGuard();
            GuardedVar<bool> _tcv = new GuardedVar<bool>(_mgc);

            _tcv.Value = false; // failure throws InvalidOperationException since the guard was not acquired


        }

        /// <summary>
        /// GuardedVarAccesWithoutAcquireMutexInvalidOperationException - write to MutexGuard without acquire should throw exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardFailureException))]
        public void GuardedVarAccesWithoutAcquireMutexInvalidOperationException()
        {
            MutexGuard _mutexGuard = new MutexGuard();
            GuardedVar<bool> _tcvMutexGuard = new GuardedVar<bool>(_mutexGuard);
            _tcvMutexGuard.Value = false;

        }

        /// <summary>
        /// GuardedVarAccesWithoutAcquireReaderWriterLockInvalidOperationException - ReaderWriterLockSlimGuard check write without acquire should throw exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardFailureException))]
        public void GuardedVarAccesWithoutAcquireReaderWriterLockInvalidOperationException()
        {
            ReaderWriterLockSlimGuard _guard = new ReaderWriterLockSlimGuard();
            GuardedVar<bool> _tcv = new GuardedVar<bool>(_guard);
            _tcv.Value = false;

        }

        /// <summary>
        /// GuardedVarAccesWithoutAcquireSemaphoreGuardInvalidOperationException - Semaphore Guard write without acquire should throw exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardFailureException))]
        public void GuardedVarAccesWithoutAcquireSemaphoreGuardInvalidOperationException()
        {
            SemaphoreGuard _guard = new SemaphoreGuard(1,1);
            GuardedVar<bool> _tcv = new GuardedVar<bool>(_guard);
            _tcv.Value = false;

        }

        /// <summary>
        /// GuardedVarAccesWithoutAcquireSemaphoreSlimGuardInvalidOperationException - write to semaphore protected variable without acquiring should throw exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardFailureException))]
        public void GuardedVarAccesWithoutAcquireSemaphoreSlimGuardInvalidOperationException()
        {
            SemaphoreSlimGuard _guard = new SemaphoreSlimGuard(1, 1);
            GuardedVar<bool> _tcv = new GuardedVar<bool>(_guard);
            _tcv.Value = false;

        }

        /// <summary>
        /// TestGuardedVolatileVar - confirm that writing to GuardedVolatileVarBool and GuardedVolatileVarInt work on the same thread
        /// </summary>
        [TestMethod]
        public void TestGuardedVolatileVar()
        {
            VolatileGuard vg = new VolatileGuard(Thread.CurrentThread.ManagedThreadId);
            GuardedVolatileVarBool _tcvBool = new GuardedVolatileVarBool(vg);
            GuardedVolatileVarInt _tcvInt = new GuardedVolatileVarInt(vg);

            bool testRead = _tcvBool.Value;
            Assert.IsFalse(testRead);

            _tcvBool.Value = true; ;
            Assert.IsTrue(_tcvBool.Value);

            int testReadInt = _tcvInt.Value;
            Assert.AreEqual(testReadInt, 0);



        }

        /// <summary>
        /// Check the one shot write mode for GuardedVolatileVarBool and GuardedVolatileVarInt
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGuardedVolatileVarOneWriteMode()
        {
            GuardedVolatileVarBool _tcvBool = new GuardedVolatileVarBool(true);

            GuardedVolatileVarInt _tcvInt = new GuardedVolatileVarInt(true);

            //Write once should work
            _tcvInt.Value = 1;
            _tcvBool.Value = true;

            //should work since we allow resetting the same value from the writer thread
            _tcvInt.Value = 1;
            _tcvBool.Value = true;

            bool gotexception = false;
            try
            {
                _tcvBool.Value = false; // Exception since we won't allow changing the value with the one shot mode
            }
            catch (InvalidOperationException)
            {
                gotexception = true;
            }

            Assert.IsTrue(gotexception);


            _tcvInt.Value = 2; // will throw exception since second write of different value (test runner will catch this exception and check)


        }

        /// <summary>
        /// TestSingleThreadWriteForVolatileVarBool check if write to GuardedVolatileVarBool fails when done from a thread different than the thread it was instantiated on
        /// </summary>
        [TestMethod]
        public void TestSingleThreadWriteForVolatileVarBool()
        {
            GuardedVolatileVarBool _tcvBool = new GuardedVolatileVarBool(true);
            bool failed = false;

            _tcvBool.Value = true;


            Thread thread1 = new Thread(() =>
            {
                try
                {
                    _tcvBool.Value = false; //should fail
                }
                catch (InvalidOperationException)
                {
                    failed = true;
                }

            });
            thread1.Start();
            thread1.Join();
            Assert.IsTrue(failed);
        }


        /// <summary>
        /// TestSingleThreadWriteForVolatileVarInt check if write to GuardedVolatileVarInt fails when done from a thread different than the thread it was instantiated on
        /// </summary>
        [TestMethod]
        public void TestSingleThreadWriteForVolatileVarInt()
        {
            GuardedVolatileVarInt _tcvBool = new GuardedVolatileVarInt(true);
            bool failed = false;

            _tcvBool.Value = 1;


            Thread thread1 = new Thread(() =>
            {
                try
                {
                    _tcvBool.Value = 2; //should fail
                }
                catch (InvalidOperationException)
                {
                    failed = true;
                }

            });
            thread1.Start();
            thread1.Join();
            Assert.IsTrue(failed);
        }

    }
}
