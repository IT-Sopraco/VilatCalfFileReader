using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace VSM.RUMA.SRV.FILEREADER
{
    public delegate void MultiValueDictionaryChanged(object sender, NotifyCollectionChangedEventArgs e);
    public delegate void MultiValueDictionaryKeyAdded(object sender, CollectionChangeEventArgs e);

    public class MultiValueDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {

        public void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                Add(key, new List<TValue>());
                if (KeyAdded != null) { KeyAdded(this, new CollectionChangeEventArgs(CollectionChangeAction.Add, key)); }
            }
            this[key].Add(value);
            if (ValueAdded != null) { ValueAdded(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,value)); }

        }

        public event MultiValueDictionaryChanged ValueAdded;

        public event MultiValueDictionaryKeyAdded KeyAdded;

        public List<TValue> GetValues(TKey key)
        {
            return this[key];
        }

        public IEnumerable<TValue> GetAllValues()
        {
            foreach (var keyValPair in this)
                foreach (var val in keyValPair.Value)
                    yield return val;
        }

    }

}
