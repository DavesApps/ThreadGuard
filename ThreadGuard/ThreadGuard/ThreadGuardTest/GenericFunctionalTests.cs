using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadGuard;

namespace ThreadGuardTest
{
    [TestClass]
    public class GenericFunctionTests
    {
        
        const int MAXLOOPTHREADCOUNT = 200000;
        const int BUSYLOOPCOUNT = 1000;


        /// <summary>
        /// GenericMonitorGuardVarTestsWithReentrancy - Basic thread synchronization test uses the guard as a standard synchronization object
        /// </summary>
        [TestMethod]
        public void GenericMonitorGuardVarTestsWithReentrancy()
        {
            List<ISynchronizeGuard> guards = new List<ISynchronizeGuard>();

            guards.Add(new MonitorGuard());




            Thread thread1 = null, thread2 = null;

            foreach (ISynchronizeGuard guard in guards)
            {
                bool error1 = false, error2 = false;
                bool done1 = false, done2 = false;
                Exception e1 = null, e2 = null;

                GuardedVar<int> _guardVar = new GuardedVar<int>(guard);

                thread1 = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    try
                    {
                        for (int count = 0; count < MAXLOOPTHREADCOUNT; count++)
                        {
                            using (guard.Acquire())
                            {
                                int temp = _guardVar.Value;
                                int temp2 = 0;

                                guard.Acquire();  //test -re-entrancy
                                guard.Release();

                                _guardVar.Value++;

                                for (int x = 0; x < BUSYLOOPCOUNT; x++) //busy loop
                                    temp2++;

                                Assert.IsTrue(temp + 1 == _guardVar.Value, "Should be true as value should not be changed during our busy loop. Type" + guard.GetType().Name);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        error1 = true;
                        e1 = e;
                    }

                    done1 = true;

                });

                thread2 = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    try
                    {
                        for (int count = 0; count < MAXLOOPTHREADCOUNT; count++)
                        {
                            using (guard.Acquire())
                            {
                                int temp = _guardVar.Value;
                                int temp2 = 0;
                                _guardVar.Value++;

                                for (int x = 0; x < BUSYLOOPCOUNT; x++) //busy loop
                                    temp2++;


                                Assert.IsTrue(temp + 1 == _guardVar.Value, "Should be true as value should not be changed during our busy loop. Type:" + guard.GetType().Name);

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        error2 = true;
                        e2 = e;
                    }

                    done2 = true;
                });

                

                thread1.Start();
                thread2.Start();

               
                thread1.Join();
                thread2.Join();
               

                Assert.IsFalse(error1, "Guard:" + guard.GetType().Name + " Exception:" + (e1 == null ? "None" : e1.ToString()));
                Assert.IsFalse(error2, "Guard:" + guard.GetType().Name + " Exception:" + (e2 == null ? "None" : e2.ToString()));

                Assert.IsTrue(done1 && done2, "Guard:" + guard.GetType().Name + " thread did not complete");
                               
            } //end for loop




        }



        /// <summary>
        /// GenericGuardVarTests - Basic thread synchronization test uses the guard as a standard synchronization object
        /// </summary>
        [TestMethod]
        public void GenericGuardVarTests()
        {
            List<ISynchronizeGuard> guards = new List<ISynchronizeGuard>();

            guards.Add(new MonitorGuard());
            guards.Add(new MutexGuard());
            guards.Add(new SemaphoreGuard(1,1));            //only works with context for this config
            guards.Add(new SemaphoreSlimGuard(1, 1));       // only works with context for this config
            guards.Add(new SpinLockGuard());
            guards.Add(new ReaderWriterLockSlimGuard());    // variable context will only work within the write lock

            Thread thread1 = null, thread2 = null, thread3 = null;

            foreach (ISynchronizeGuard guard in guards)
            {
                bool error1 = false, error2 = false, error3 = false;
                bool done1 = false, done2 = false, done3 = false;
                Exception e1 = null, e2 = null, e3 = null;

                GuardedVar<int> _guardVar = new GuardedVar<int>(guard);

                thread1= new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    try
                    {
                        for (int count = 0; count < MAXLOOPTHREADCOUNT; count++)
                        {
                            using (guard.Acquire())
                            {
                                int temp = _guardVar.Value;
                                int temp2 = 0;
                                _guardVar.Value++;

                                for (int x = 0; x < BUSYLOOPCOUNT; x++) //busy loop
                                    temp2++;

                                Assert.IsTrue(temp + 1 == _guardVar.Value, "Should be true as value should not be changed during our busy loop. Type" + guard.GetType().Name);
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        error1 = true;
                        e1 = e;
                    }

                    done1 = true;

                });

                thread2=new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    try
                    {
                        for (int count = 0; count < MAXLOOPTHREADCOUNT; count++)
                        {
                            using (guard.Acquire())
                            {
                                int temp = _guardVar.Value;
                                int temp2 = 0;
                                _guardVar.Value++;

                                for (int x = 0; x < BUSYLOOPCOUNT; x++) //busy loop
                                    temp2++;


                                Assert.IsTrue(temp + 1 == _guardVar.Value, "Should be true as value should not be changed during our busy loop. Type:" + guard.GetType().Name);

                            }
                        }
                    }
                    catch(Exception e)
                    {
                        error2 = true;
                        e2 = e;
                    }

                    done2 = true;
                });

                if (guard is IReaderWriterSynchronizeGuard)
                    thread3 = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        IReaderWriterSynchronizeGuard readerWriterGuard = guard as IReaderWriterSynchronizeGuard;

                        try
                        {
                            for (int count = 0; count < MAXLOOPTHREADCOUNT; count++)
                            {
                                using (readerWriterGuard.AcquireRead())
                                {
                                    int temp = _guardVar.ReadOnlyValue;
                                    int temp2 = 0;


                                    for (int x = 0; x < BUSYLOOPCOUNT; x++) //busy loop
                                        temp2++;


                                    Assert.IsTrue(temp == _guardVar.ReadOnlyValue, "Should be true as value should not be changed during our busy loop. Type:" + guard.GetType().Name);

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            error3 = true;
                            e3 = e;
                        }

                        done3 = true;
                    });

                thread1.Start();
                thread2.Start();

                if (guard is IReaderWriterSynchronizeGuard)
                    thread3.Start();

                thread1.Join();
                thread2.Join();
                if (guard is IReaderWriterSynchronizeGuard)
                    thread3.Join();

                Assert.IsFalse(error1, "Guard:" + guard.GetType().Name + " Exception:" + (e1 == null ? "None" : e1.ToString()));
                Assert.IsFalse(error2, "Guard:" + guard.GetType().Name + " Exception:" + (e2 == null ? "None" : e2.ToString()));

                if (guard is IReaderWriterSynchronizeGuard)
                    Assert.IsFalse(error3, "Guard:" + guard.GetType().Name + " Exception:" + (e3 == null ? "None" : e3.ToString()));

                Assert.IsTrue(done1 && done2, "Guard:" + guard.GetType().Name + " thread did not complete");

                if (guard is IReaderWriterSynchronizeGuard)
                    Assert.IsTrue(done3, "Guard:" + guard.GetType().Name + " thread did not complete");


            } //end for loop


            

        }


        
        
    }
}
