using System;
using System.Collections.Generic;

namespace Server.Repository
{
    public interface IRepository<T> : IDisposable where T : class
    {
        List<T> GetElementList(int id);
        T GetElement(int id);
        void Create(T item);
        void Update(T item);
        void Delete(int id);
        void Save();
    }
}
