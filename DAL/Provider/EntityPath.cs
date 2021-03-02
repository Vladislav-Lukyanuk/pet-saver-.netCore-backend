using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Provider
{
    public abstract class EntityPath<T> where T : class
    {
        private readonly List<Func<IQueryable<T>, IQueryable<T>>> _actions = new List<Func<IQueryable<T>, IQueryable<T>>>();

        protected EntityPath() { }

        protected EntityPath(Func<IQueryable<T>, IQueryable<T>> action)
        {
            _actions.Add(action);
        }

        protected void Add(Func<IQueryable<T>, IQueryable<T>> path)
        {
            _actions.Add(path);
        }

        public IQueryable<T> Apply(IQueryable<T> query)
        {
            foreach (var action in _actions)
            {
                query = action(query);
            }

            return query;
        }
    }
}
