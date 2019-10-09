using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace SearchEngine
{
    //Credit: https://docs.microsoft.com/en-us/dotnet/api/system.threading.readerwriterlockslim?view=netframework-4.8
    public class SyncedUrlQueue
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private Dictionary<string, QueueEntry> innerCache = new Dictionary<string, QueueEntry>();

        public int Count
        { get { return innerCache.Count; } }

        public QueueEntry Read(string key)
        {
            cacheLock.EnterReadLock();
            try
            {
                return innerCache[key];
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        public bool ContainsKey(string key)
        {
            cacheLock.EnterReadLock();
            try
            {
                return innerCache.ContainsKey(key);
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        public Dictionary<string, QueueEntry>.KeyCollection Keys(bool CopyInstance = false)
        {
            cacheLock.EnterReadLock();
            try
            {
                if (CopyInstance)
                {
                    return new Dictionary<string, QueueEntry>.KeyCollection(innerCache);
                }   
                else
                    return innerCache.Keys;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        public Dictionary<string, QueueEntry>.ValueCollection Values(bool CopyInstance = false)
        {
            cacheLock.EnterReadLock();
            try
            {
                if (CopyInstance)
                {
                    return new Dictionary<string, QueueEntry>.ValueCollection(innerCache);
                }
                else
                    return innerCache.Values;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        public void Add(string key, QueueEntry value)
        {
            cacheLock.EnterWriteLock();
            try
            {
                innerCache.Add(key, value);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public bool AddWithTimeout(string key, QueueEntry value, int timeout)
        {
            if (cacheLock.TryEnterWriteLock(timeout))
            {
                try
                {
                    innerCache.Add(key, value);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public AddOrUpdateStatus AddOrUpdate(string key, QueueEntry value)
        {
            cacheLock.EnterUpgradeableReadLock();
            try
            {
                QueueEntry result = null;
                if (innerCache.TryGetValue(key, out result))
                {
                    if (result == value)
                    {
                        return AddOrUpdateStatus.Unchanged;
                    }
                    else
                    {
                        cacheLock.EnterWriteLock();
                        try
                        {
                            innerCache[key] = value;
                        }
                        finally
                        {
                            cacheLock.ExitWriteLock();
                        }
                        return AddOrUpdateStatus.Updated;
                    }
                }
                else
                {
                    cacheLock.EnterWriteLock();
                    try
                    {
                        innerCache.Add(key, value);
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                    return AddOrUpdateStatus.Added;
                }
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
        }

        public void Delete(string key)
        {
            cacheLock.EnterWriteLock();
            try
            {
                innerCache.Remove(key);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public enum AddOrUpdateStatus
        {
            Added,
            Updated,
            Unchanged
        };

        ~SyncedUrlQueue()
        {
            if (cacheLock != null) cacheLock.Dispose();
        }
    }
}
