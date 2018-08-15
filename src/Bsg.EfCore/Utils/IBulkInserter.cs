namespace Bsg.EfCore.Utils
{
    using System;
    using System.Collections.Generic;

    public interface IBulkInserter<in TClass> : IDisposable
    {
        int InsertedCount();

        void ResetInsertedCount();

        void Insert(IEnumerable<TClass> items);

        void Insert(TClass item);

        void Queue(TClass item, bool autoFlush);

        void Flush();
    }
}