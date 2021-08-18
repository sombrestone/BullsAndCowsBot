using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullsAndCowsGame
{
    interface IRepository<T>
    {
        /*Task<IEnumerable<T>> GetAll();
        Task<T> Get(long id);*/
        IEnumerable<T> GetAll();
        T Get(long id);
        void Create(T item);
        void Update(T item);
        void Delete(long id);
        void Save();
    }
}
