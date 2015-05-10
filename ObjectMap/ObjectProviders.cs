using System;
using System.Collections.ObjectModel;

namespace ObjectMap
{
    public class ObjectProviders : KeyedCollection<Type, ObjectProvider>
    {
        protected override Type GetKeyForItem(ObjectProvider item)
        {
            throw new NotImplementedException();
        }
    }
}