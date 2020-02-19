using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.IO;

namespace PersistentCollections
{
    public class PersistentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> dictionary;
        private readonly string path;

        public PersistentDictionary(string path)
        {
            this.path = path;
            if (File.Exists(path))
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(File.ReadAllText(path));
            }
            else
            {
                dictionary = new Dictionary<TKey, TValue>();
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)dictionary).CopyTo(array, index);
        }

        public object SyncRoot
        {
            get { return ((ICollection)dictionary).SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return ((ICollection)dictionary).IsSynchronized; }
        }

        public bool Contains(object key)
        {
            return ((IDictionary)dictionary).Contains(key);
        }

        public void Add(object key, object value)
        {
            lock (SyncRoot)
            {
                ((IDictionary)dictionary).Add(key, value);
                WriteToFile();
            }
        }

        public void Remove(object key)
        {
            lock (SyncRoot)
            {
                ((IDictionary)dictionary).Remove(key);
                WriteToFile();
            }
        }

        public object this[object key]
        {
            get { return dictionary[(TKey)key]; }
            set
            {
                lock (SyncRoot)
                {
                    dictionary[(TKey)key] = (TValue)value;
                    WriteToFile();
                }
            }
        }

        public bool IsFixedSize
        {
            get { return ((IDictionary)dictionary).IsFixedSize; }
        }

        public void Add(TKey key, TValue value)
        {
            lock (SyncRoot)
            {
                dictionary.Add(key, value);
                WriteToFile();
            }
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                dictionary.Clear();
                WriteToFile();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue v;
            return (dictionary.TryGetValue(item.Key, out v) && v.Equals(item.Key));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary)
                .CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                lock (SyncRoot)
                {
                    dictionary.Remove(item.Key);
                    WriteToFile();
                }
                return true;
            }
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return dictionary.ContainsValue(value);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            dictionary.GetObjectData(info, context);
        }

        public void OnDeserialization(object sender)
        {
            dictionary.OnDeserialization(sender);
        }

        public bool Remove(TKey key)
        {
            bool result;

            lock (SyncRoot)
            {
                result = dictionary.Remove(key);
                WriteToFile();
            }

            return result;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public IEqualityComparer<TKey> Comparer
        {
            get { return dictionary.Comparer; }
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly { get; private set; }

        public TValue this[TKey key]
        {
            get { return dictionary[key]; }
            set
            {
                lock (SyncRoot)
                {
                    dictionary[key] = value;
                    WriteToFile();
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get { return dictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return dictionary.Values; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (SyncRoot)
            {
                dictionary.Add(item.Key, item.Value);
                WriteToFile();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void WriteToFile()
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(dictionary));
        }
    }
}
