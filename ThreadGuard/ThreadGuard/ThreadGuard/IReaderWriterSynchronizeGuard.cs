using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadGuard
{
    public  interface  IReaderWriterSynchronizeGuard:  ISynchronizeGuard
    {
        /// <summary>
        /// Acquire write lock
        /// </summary>
        /// <returns></returns>
         IDisposable AcquireWrite();

        /// <summary>
        /// Release write lock
        /// </summary>
         void ReleaseWrite();

        /// <summary>
        /// Acquire Read lock
        /// </summary>
        /// <returns></returns>
         IDisposable AcquireRead();

        /// <summary>
        /// Release Read lock
        /// </summary>
         void ReleaseRead();


        /// <summary>
        /// Is the write lock acquired
        /// </summary>
         bool IsAcquiredWrite { get;  }

        /// <summary>
        /// Is the read lock acquired
        /// </summary>
         bool IsAcquiredRead { get; }


    }
}
