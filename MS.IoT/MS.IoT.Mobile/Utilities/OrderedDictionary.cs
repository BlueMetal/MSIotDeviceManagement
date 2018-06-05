using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{

    /// <summary>
    /// This class is based on one by Marc Clifton
    /// https://www.codeproject.com/Articles/12858/Generic-Keyed-List
    /// </summary>


    public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {

        private Dictionary<TKey, TValue> objectTable = new Dictionary<TKey, TValue>();
        private List<KeyValuePair<TKey, TValue>> objectList = new List<KeyValuePair<TKey, TValue>>();


        /// <summary>
        /// Get an unordered list of keys.
        /// This collection refers back to the keys in the original Dictionary.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return objectTable.Keys; }
        }

        /// <summary>
        /// Get an unordered list of values.
        /// This collection refers back to the values in the original Dictionary,
        /// so changes to the dictionary continue to be reflected in the key collection.
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return objectTable.Values; }
        }



        /// <summary>
        /// Get the ordered list of keys.
        /// This is a copy of the keys in the original Dictionary,
        /// so changes to the dictionary will not be reflected in the key collection.
        /// </summary>
        public List<TKey> OrderedKeys
        {
            get
            {
                List<TKey> retList = new List<TKey>();

                foreach (KeyValuePair<TKey, TValue> kvp in objectList)
                {
                    retList.Add(kvp.Key);
                }
                return retList;
            }
        }

        /// <summary>
        /// Get the ordered list of values.
        /// This is a copy of the values in the original Dictionary.
        /// </summary>
        public List<TValue> OrderedValues
        {
            get
            {
                List<TValue> retList = new List<TValue>();

                foreach (KeyValuePair<TKey, TValue> kvp in objectList)
                {
                    retList.Add(kvp.Value);
                }
                return retList;
            }
        }


        public void Clear()
        {
            objectTable.Clear();
            objectList.Clear();
        }


        /// <summary>
        /// Get/Set the value at the specified index.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <returns>The value.</returns>
        public KeyValuePair<TKey, TValue> this[int idx]
        {
            get
            {
                if (idx < 0 || idx >= Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                return objectList[idx];
            }
            set
            {
                if (idx < 0 || idx >= Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                objectList[idx] = value;
                objectTable[value.Key] = value.Value;
            }
        }

        /// <summary>
        /// Get/Set the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The associated value.</returns>
        public virtual TValue this[TKey key]
        {
            get { return objectTable[key]; }
            set
            {
                if (objectTable.ContainsKey(key))
                {
                    objectTable[key] = value;
                    objectList[IndexOf(key)] = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }



        /// <summary>
        /// Returns the key at the specified index.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <returns>The key at the index.</returns>
        public TKey GetKey(int idx)
        {
            if (idx < 0 || idx >= Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return objectList[idx].Key;
        }

        /// <summary>
        /// Returns the value at the specified index.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <returns>The value at the index.</returns>
        public TValue GetValue(int idx)
        {
            if (idx < 0 || idx >= Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return objectList[idx].Value;
        }


        /// <summary>
        /// Get the index of a particular key.
        /// </summary>
        /// <param name="key">The key to find the index of.</param>
        /// <returns>The index of the key, or -1 if not found.</returns>
        public int IndexOf(TKey key)
        {
            int ret = -1;
            for (int i = 0; i < objectList.Count; i++)
            {
                if (objectList[i].Key.Equals(key))
                {
                    ret = i;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Given the key-value pair, find the index.
        /// </summary>
        /// <param name="kvp">The key-value pair.</param>
        /// <returns>The index, or -1 if not found.</returns>
        public int IndexOf(KeyValuePair<TKey, TValue> kvp)
        {
            return IndexOf(kvp.Key);
        }



        /// <summary>
        /// Test if the KeyedList contains the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the key is found.</returns>
        public bool ContainsKey(TKey key)
        {
            return objectTable.ContainsKey(key);
        }

        /// <summary>
        /// Test if the KeyedList contains the key in the key-value pair.
        /// </summary>
        /// <param name="kvp">The key-value pair.</param>
        /// <returns>True if the key is found.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> kvp)
        {
            return objectTable.ContainsKey(kvp.Key);
        }


        /// <summary>
        /// Adds a key-value pair to the KeyedList.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The associated value.</param>
        public void Add(TKey key, TValue value)
        {
            objectTable.Add(key, value);
            objectList.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Adds a key-value pair to the KeyedList.
        /// </summary>
        /// <param name="kvp">The KeyValuePair instance.</param>
        public void Add(KeyValuePair<TKey, TValue> kvp)
        {
            Add(kvp.Key, kvp.Value);
        }


        /// <summary>
        /// Insert the key-value at the specified index.
        /// </summary>
        /// <param name="idx">The zero-based insert point.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Insert(int idx, TKey key, TValue value)
        {
            if ((idx < 0) || (idx > Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            objectTable.Add(key, value);
            objectList.Insert(idx, new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Insert the key-value pair at the specified index location.
        /// </summary>
        /// <param name="idx">The key.</param>
        /// <param name="kvp">The value.</param>
        public void Insert(int idx, KeyValuePair<TKey, TValue> kvp)
        {
            if ((idx < 0) || (idx > Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            objectTable.Add(kvp.Key, kvp.Value);
            objectList.Insert(idx, kvp);
        }



        /// <summary>
        /// Remove the entry.
        /// </summary>
        /// <param name="key">The key identifying the key-value pair.</param>
        /// <returns>True if removed.</returns>
        public bool Remove(TKey key)
        {
            bool found = objectTable.Remove(key);

            if (found)
            {
                objectList.RemoveAt(IndexOf(key));
            }

            return found;
        }

        /// <summary>
        /// Remove the key in the specified KeyValuePair instance. The Value
        /// property is ignored.
        /// </summary>
        /// <param name="kvp">The key-value identifying the entry.</param>
        /// <returns>True if removed.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> kvp)
        {
            return Remove(kvp.Key);
        }

        /// <summary>
        /// Remove the entry at the specified index.
        /// </summary>
        /// <param name="idx">The index to the entry to be removed.</param>
        public void RemoveAt(int idx)
        {
            if ((idx < 0) || (idx >= Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            objectTable.Remove(objectList[idx].Key);
            objectList.RemoveAt(idx);
        }



        /// <summary>
        /// Returns an ordered System.Collections KeyValuePair objects.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return objectList.GetEnumerator();
        }

        /// <summary>
        /// Returns an ordered KeyValuePair enumerator.
        /// </summary>
        IEnumerator<KeyValuePair<TKey, TValue>>
            IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return objectList.GetEnumerator();
        }



        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the number of entries in the KeyedList.
        /// </summary>
        public int Count
        {
            get { return objectList.Count; }
        }

        /// <summary>
        /// Attempt to get the value, given the key, without throwing an exception 
        /// if not found.
        /// </summary>
        /// <param name="key">The key identifying the entry.</param>
        /// <param name="val">The value, if found.</param>
        /// <returns>True if found.</returns>
        public bool TryGetValue(TKey key, out TValue val)
        {
            return objectTable.TryGetValue(key, out val);
        }

        /// <summary>
        /// Copy the entire key-value pairs to the KeyValuePair array, starting
        /// at the specified index of the target array. The array is populated 
        /// as an ordered list.
        /// </summary>
        /// <param name="kvpa">The KeyValuePair array.</param>
        /// <param name="idx">The position to start the copy.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] kvpa, int idx)
        {
            objectList.CopyTo(kvpa, idx);
        }


    }
}
