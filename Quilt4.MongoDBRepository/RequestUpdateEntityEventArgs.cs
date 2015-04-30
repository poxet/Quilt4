using System;

namespace Quilt4.MongoDBRepository
{
    class RequestUpdateEntityEventArgs : EventArgs
    {
        private readonly string _collection;
        private readonly object _item;

        public RequestUpdateEntityEventArgs(string collection, object item)
        {
            _collection = collection;
            _item = item;
        }

        public string Collection { get { return _collection; } }
        public object Item { get { return _item; } }
    }
}