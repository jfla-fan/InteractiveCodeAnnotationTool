using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CodeAnnotationTool.NotionProvider
{
    /// <summary>
    /// Provide access to cached storage.
    /// Underlying json file has the following structure:
    /// 
    /// </summary>
    internal class CachedNotionProvider : INotionProvider
    {
        private readonly string _storagePath;
        private IDictionary<string, List<NotionInfo>> _notionMap;

        public CachedNotionProvider(string storagePath)
        {
            _storagePath = storagePath;

            if (!File.Exists(_storagePath))
            {
                _notionMap = new Dictionary<string, List<NotionInfo>>();
            }
            else
            {
                Refresh();
            }
        }

        public IEnumerable<NotionInfo> GetNotions(string key)
        {
            if (!_notionMap.ContainsKey(key))
            {
                throw new NotionProviderException($"Can't find key {key}.");
            }

            return _notionMap[key];
        }

        public IEnumerable<NotionInfo> FindNotions(string key)
        {
            return _notionMap.TryGetValue(key, out var notions) ? notions : new List<NotionInfo>();
        }

        public int RemoveNotion(Predicate<NotionInfo> predicate)
        {
            var notionsToRemove = _notionMap.SelectMany(n => n.Value)
                                                          .Where(n => predicate(n))
                                                          .GroupBy(n => n.Key)
                                                          .ToList();

            foreach (var group in notionsToRemove)
            {
                foreach (var notion in group)
                {
                    _notionMap[group.Key].Remove(notion);
                }
            }

            return notionsToRemove.Count;
        }

        public bool SaveNotion(NotionInfo notionInfo)
        {
            if (!_notionMap.ContainsKey(notionInfo.Key))
            {
                _notionMap[notionInfo.Key] = new List<NotionInfo>();
            }

            _notionMap[notionInfo.Key].Add(notionInfo);

            return true;
        }

        public bool ExistsNotion(Predicate<NotionInfo> predicate)
        {
            return _notionMap.Values.Any(notions => notions.Any(notion => predicate(notion)));
        }

        public void Refresh()
        {
            if (!File.Exists(_storagePath))
            {
                throw new NotionProviderException($"File {_storagePath} was removed, can't find it.");
            }

            string json = File.ReadAllText(_storagePath);
            _notionMap = JsonConvert.DeserializeObject<Dictionary<string, List<NotionInfo>>>(json);
        }

        public void Flush()
        {
            string json = JsonConvert.SerializeObject(_notionMap);
            File.WriteAllText(_storagePath, json);
        }

        public void Dispose()
        {
            Flush();
            Debug.WriteLine("CachedNotionProvider: disposed.");
        }
    }
}
