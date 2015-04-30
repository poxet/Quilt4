using System;

namespace Quilt4.MongoDBRepository
{
    class RequestDeleteEntityEventArgs : EventArgs
    {
        private readonly string _collection;
        private readonly Guid _id;

        public RequestDeleteEntityEventArgs(string collection, Guid id)
        {
            _collection = collection;
            _id = id;
        }

        public string Collection { get { return _collection; } }
        public Guid Id { get { return _id; } }
    }
}