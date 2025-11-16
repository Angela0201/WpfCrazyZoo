using System;
using System.Collections.Generic;

namespace CrazyZoo.Domain.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        void Add(T item);
        void Remove(T item);
        T Find(Func<T, bool> predicate);
    }
}
