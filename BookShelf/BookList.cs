using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace BookShelf
{
    class BookList : List<Book>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
