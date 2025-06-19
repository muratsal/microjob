using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Shared.Caching
{
    public class DownloadCache
    {
        private static List<CacheItem> _cacheList;
        private static int maxCacheCount = 20;
        public static void AddCacheWithKey(string key, object value, DocumentType docType)
        {
            if (_cacheList == null)
            {
                _cacheList = new List<CacheItem>();
            }

            removeOldCaches();

            _cacheList.Add(new CacheItem
            {
                Key = key,
                Value = value,
                DocType = docType,
                CacheDate = DateTime.Now
            });

        }

        public static CacheItem GetCacheItemByKey(string key)
        {
            return _cacheList.FirstOrDefault(x => x.Key == key);
        }

        private static void removeOldCaches()
        {
            _cacheList = _cacheList.OrderByDescending(x => x.CacheDate).Take(maxCacheCount).ToList();
        }

    }

    public class CacheItem
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public DateTime CacheDate { get; set; }
        public DocumentType DocType { get; set; }

    }

    public enum DocumentType
    {
        EXCEL,
        PDF
    }
}
