using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace KestrelPureOwin
{
    public class OwinHeaderDictionary : IDictionary<string, string[]>
    {
        public OwinHeaderDictionary(IHeaderDictionary headers)
        {
            Headers = headers;
        }

        private IHeaderDictionary Headers { get; }

        public int Count => Headers.Count;

        public bool IsReadOnly => Headers.IsReadOnly;

        public ICollection<string> Keys => Headers.Keys;

        public ICollection<string[]> Values => Headers.Values.Select(x => x.ToArray()).ToList();

        public string[] this[string key]
        {
            get { return Headers[key].ToArray(); }
            set { Headers[key] = value; }
        }

        public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
        {
            return Headers.Select(Convert).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, string[]> item)
        {
            Headers.Add(Convert(item));
        }

        public void Clear()
        {
            Headers.Clear();
        }

        public bool Contains(KeyValuePair<string, string[]> item)
        {
            return Headers.Contains(Convert(item));
        }

        public void CopyTo(KeyValuePair<string, string[]>[] array, int arrayIndex)
        {
            foreach (var header in Headers)
            {
                array[arrayIndex++] = Convert(header);
            }
        }

        public bool Remove(KeyValuePair<string, string[]> item)
        {
            return Headers.Remove(Convert(item));
        }

        public void Add(string key, string[] value)
        {
            Headers.Add(new KeyValuePair<string, StringValues>(key, value));
        }

        public bool ContainsKey(string key)
        {
            return Headers.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return Headers.Remove(key);
        }

        public bool TryGetValue(string key, out string[] value)
        {
            StringValues stringValues;
            if (Headers.TryGetValue(key, out stringValues))
            {
                value = stringValues.ToArray();
                return true;
            }

            value = null;
            return false;
        }

        private static KeyValuePair<string, StringValues> Convert(KeyValuePair<string, string[]> item)
        {
            return new KeyValuePair<string, StringValues>(item.Key, item.Value);
        }

        private static KeyValuePair<string, string[]> Convert(KeyValuePair<string, StringValues> item)
        {
            return new KeyValuePair<string, string[]>(item.Key, item.Value);
        }
    }
}
