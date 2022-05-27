using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    /// <summary>
    /// ReaderWriterLockSlimGuard wrapper around Monitor class
    /// </summary>
    public class ReaderWriterLockSlimGuard : IDisposable, ISynchronizeGuardContext, IIsAcquired, IReaderWriterSynchronizeGuard
    {

        IGuardFailure _guardFailure = GlobalGuardFailure.FailInterface;

        ThreadLocal<bool> _isAcquireWriteLock = new ThreadLocal<bool>(false);
        ThreadLocal<bool> _isAcquireReadLock = new ThreadLocal<bool>(false);

        private UInt64 _contextVersion = ContextConstants.DEFAULTVERSIONGUARD;
        private ReaderWriterLockSlim _readerWriterLock = null;

        //default constructor
        public ReaderWriterLockSlimGuard()
        {
            _readerWriterLock = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Provide an alternate guard failure interface to handle failures differently if desired.
        /// </summary>
        /// <param name="failure"></param>
        public ReaderWriterLockSlimGuard(IGuardFailure failure) : this()
        {
            _guardFailure = failure;

        }


        /// <summary>
        /// Make the default acquire a write lock so it acks like a regular lock in the default case
        /// </summary>
        /// <returns></returns>
        public IDisposable Acquire()
        {
            return AcquireWrite();
        }

        /// <summary>
        /// Release Write Lock is the default behaviour for release
        /// </summary>
        public void Release()
        {
            ReleaseWrite();
        }

        /// <summary>
        /// Acquire write lock
        /// </summary>
        /// <returns></returns>
        public IDisposable AcquireWrite()
        {
            _readerWriterLock.EnterWriteLock();
            _isAcquireWriteLock.Value = true;
            _contextVersion++;


            return this;

        }


        /// <summary>
        /// Release write lock
        /// </summary>
        public void ReleaseWrite()
        {
            if (_isAcquireWriteLock.Value)
            {
                _isAcquireWriteLock.Value = false;
                _contextVersion++;

                _readerWriterLock.ExitWriteLock();

            }

        }

        /// <summary>
        /// Acquire Read lock
        /// </summary>
        /// <returns></returns>
        public IDisposable AcquireRead(bool upgradeable=false)
        {
            if (upgradeable)
                _readerWriterLock.EnterUpgradeableReadLock();
            else
                _readerWriterLock.EnterReadLock();
            _isAcquireReadLock.Value = true;
            //_contextVersion++;

            

            return this;
        }


        /// <summary>
        /// Release Read lock
        /// </summary>
        public void ReleaseRead(bool upgradeable=false)
        {
            if (_isAcquireReadLock.Value)
            {
                _isAcquireWriteLock.Value = false;
                //_contextVersion++;

                if (upgradeable)
                    _readerWriterLock.ExitUpgradeableReadLock();
                else
                    _readerWriterLock.ExitReadLock();

            }
        }

        public void Dispose()
        {
            if (_isAcquireReadLock.Value)
                ReleaseRead();
            else if (_isAcquireWriteLock.Value)
                ReleaseWrite();
        }

        public IDisposable AcquireRead()
        {
            return AcquireRead(false);
        }

        public void ReleaseRead()
        {
            ReleaseRead(false);
        }

        /// <summary>
        /// Is the Exclusive lock Acquired Write lock value
        /// </summary>
        public bool IsAcquired { get { return _isAcquireWriteLock.Value; } }

        /// <summary>
        /// Is the write lock acquired
        /// </summary>
        public bool IsAcquiredWrite { get { return _isAcquireWriteLock.Value; } }

        /// <summary>
        /// Is the read lock acquired
        /// </summary>
        public bool IsAcquiredRead { get { return _isAcquireReadLock.Value; } }


        /// <summary>
        /// Returns the context version that is accurate for the write lock
        /// </summary>
        public UInt64 ContextVersion
        {
            get
            {
                if (_isAcquireWriteLock.Value == false)
                    _guardFailure.Fail(this, "Can't retrieve the context version when the guard has not been acquired.");


                return _contextVersion;
            }
        }
        ~ReaderWriterLockSlimGuard()
        {
            _isAcquireWriteLock.Dispose();
            _isAcquireReadLock.Dispose();
        }


    }


}
