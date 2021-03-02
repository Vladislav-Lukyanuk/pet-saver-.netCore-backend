using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace DAL.Provider
{
    public abstract class DataProvider<T> : IDisposable where T : class, new()
    {
        /// <summary>
        /// Gets or sets the context
        /// </summary>
        private Context Context { get; set; }

        /// <summary>
        /// Initializes a nes instance of the <see cref="DataProvider{T}" /> class
        /// </summary>
        /// <param name="context">The context</param>
        protected DataProvider(Context context)
        {
            Context = context;
        }

        #region Dispose

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Return if the context is null.
            if (Context == null)
            {
                return;
            }

            // Dispose of the context.
            try
            {
                Context.Dispose();
            }
            catch
            {
            }
        }

        #endregion

        #region Methods

        #region public

        /// <summary>
        /// Create the empty object in database
        /// </summary>
        /// <returns></returns>
        public T GetNew()
        {
            return Context.Set<T>().Add(new T()).Entity;
        }

        /// <summary>
        ///     Add the specified entity.
        /// </summary>
        /// <param name="entity"> The entity. </param>
        public void Add(T entity)
        {
            Context.Add(entity);
        }

        /// <summary>
        ///     Remove the specified entity.
        /// </summary>
        /// <param name="entity"> The entity. </param>
        public void Remove(T entity)
        {
            Context.Remove(entity);
        }

        #endregion

        #region private

        private static InternalEntityEntry GetInternalEntityEntry(EntityEntry entityEntry)
        {
            var internalEntry = (InternalEntityEntry)entityEntry
                .GetType()
                .GetProperty("InternalEntry", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(entityEntry);

            return internalEntry;
        }

        #endregion

        #region protected

        protected static string GetCurrentValueByForeignKey(EntityEntry entityEntry, IProperty prop)
        {
            var navProperty =
                entityEntry.Navigations.FirstOrDefault(n => n.Metadata.ForeignKey.Properties.Contains(prop));

            if (navProperty == null)
            {
                return Convert.ToString(entityEntry.CurrentValues[prop]);
            }

            if (!navProperty.IsLoaded)
            {
                navProperty.Load();
            }

            var currentValue = navProperty.CurrentValue;

            return currentValue == null
                ? ""
                : Convert.ToString(currentValue.GetType().GetProperty("Name")?.GetValue(currentValue, null));
        }

        protected virtual void Update(T entity)
        {
            Context.Update(entity);
        }

        /// <summary>
        ///     Gets all entities.
        /// </summary>
        /// <param name="asNoTracking"> if set to <c>true</c> [as no tracking]. </param>
        /// <returns> </returns>
        protected virtual IEnumerable<T> GetAll(bool asNoTracking)
        {
            // Return the value.
            return asNoTracking
                ? Context.Set<T>().AsNoTracking().ToList()
                : Context.Set<T>().ToList();
        }

        /// <summary>
        ///     Gets all entities.
        /// </summary>
        /// <returns> </returns>
        protected virtual IEnumerable<T> GetAll()
        {
            return GetAll(false);
        }

        /// <summary>
        ///     Gets all entities.
        /// </summary>
        /// <param name="orderBy">The order by clause.</param>
        /// <param name="orderByAscending"> if set true - order by ASCENDING otherwise order by DESCENDING. </param>
        /// <param name="asNoTracking"> if set to <c>true</c> [as no tracking]. </param>
        /// <returns> </returns>
        protected virtual IEnumerable<T> GetAll(
            Func<T, object> orderBy, bool orderByAscending = true, bool asNoTracking = false)
        {
            // Return the value.
            var rs = asNoTracking ? Context.Set<T>().AsNoTracking().ToList() : Context.Set<T>().ToList();

            return orderBy == null
                ? rs
                : orderByAscending
                    ? rs.OrderBy(orderBy).ToList()
                    : rs.OrderByDescending(orderBy).ToList();
        }

        /// <summary>
        ///     Gets all entities including the specified properties.
        /// </summary>
        /// <param name="includeProperties"> The include properties. </param>
        /// <returns> </returns>
        protected virtual IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Context.Set<T>();

            // Return the value.
            return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        /// <summary>
        ///     Gets the a list of entities according the where.
        /// </summary>
        /// <param name="where"> The where. </param>
        /// <param name="asNoTracking"> if set to <c>true</c> [as no tracking]. </param>
        /// <returns> </returns>
        protected virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where, bool asNoTracking = false)
        {
            // Return the value.
            return asNoTracking
                ? Context.Set<T>().AsNoTracking().Where(where).ToList()
                : Context.Set<T>().Where(where).ToList();
        }

        /// <summary>
        ///     Gets the many objects including the properties sent.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        protected virtual IEnumerable<T> GetManyIncluding(
            Expression<Func<T, bool>> where, bool asNoTracking = false,
            params Expression<Func<T, object>>[] includeProperties)
        {
            return GetManyIncluding(where, null, asNoTracking, true, includeProperties);
        }

        /// <summary>
        ///     Gets the many objects including the properties sent.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="orderByAscending"> if set true - order by ASCENDING otherwise order by DESCENDING. </param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        protected virtual IEnumerable<T> GetManyIncluding(
            Expression<Func<T, bool>> where,
            Expression<Func<T, object>> orderBy,
            bool asNoTracking = false,
            bool orderByAscending = true,
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            var rs = asNoTracking
                ? query.AsNoTracking().Where(where)
                : query.Where(where);

            if (orderBy != null)
            {
                rs = orderByAscending
                    ? rs.OrderBy(orderBy)
                    : rs.OrderByDescending(orderBy);
            }

            // Return the value.
            return rs.ToList();
        }

        /// <summary>
        ///     Gets the many objects including the properties sent.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <param name="includePath">The include properties.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="orderByAscending"> if set true - order by ASCENDING otherwise order by DESCENDING. </param>
        /// <returns></returns>
        protected virtual IEnumerable<T> GetManyIncluding(
            Expression<Func<T, bool>> where,
            Expression<Func<T, object>> orderBy,
            EntityPath<T> includePath,
            bool asNoTracking = false,
            bool orderByAscending = true)
        {
            IQueryable<T> query = Context.Set<T>();
            if (includePath != null)
            {
                query = includePath.Apply(query);
            }

            var rs = asNoTracking
                ? query.AsNoTracking().Where(where)
                : query.Where(where);

            if (orderBy != null)
            {
                rs = orderByAscending
                    ? rs.OrderBy(orderBy)
                    : rs.OrderByDescending(orderBy);
            }

            // Return the value.
            return rs.ToList();
        }

        /// <summary>
        ///     Gets the many objects including the properties sent.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="distinct">The distinct condition</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        protected virtual IEnumerable<T> GetManyDistinctIncluding(
            Expression<Func<T, bool>> where, Expression<Func<T, object>> distinct,
            bool asNoTracking = false,
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            // Return the value.
            return asNoTracking
                ? query.AsNoTracking().Where(where).GroupBy(distinct).Select(grp => grp.First()).ToList()
                : query.Where(where).GroupBy(distinct).Select(grp => grp.First()).ToList();
        }

        /// <summary>
        ///     Gets a lists of entities paginated according the where.
        /// </summary>
        /// <param name="where"> The where. </param>
        /// <param name="orderBy"> The order by. </param>
        /// <param name="pageIndex"> Index of the page. </param>
        /// <param name="pageSize"> Size of the page. </param>
        /// <param name="totalRecords"> The total records. </param>
        /// <param name="asNoTracking"> if set to <c>true</c> [as no tracking]. </param>
        /// <param name="orderByAscending"> if set true - order by ASCENDING otherwise order by DESCENDING. </param>
        /// <param name="includeProperties"> The include properties. </param>
        /// <returns> </returns>
        protected virtual IEnumerable<T> GetManyPaginated(
            Expression<Func<T, bool>> where, Expression<Func<T, object>> orderBy, int pageIndex,
            int pageSize,
            out int totalRecords, bool asNoTracking = false, bool orderByAscending = true,
            params Expression<Func<T, object>>[] includeProperties)
        {
            var query = Context.Set<T>().AsQueryable();

            totalRecords = query.Where(where).Count();

            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            var rs = asNoTracking
                ? query.AsNoTracking().Where(where)
                : query.Where(where);

            return orderByAscending
                ? rs.OrderBy(orderBy).Skip(pageSize * pageIndex).Take(pageSize).ToList()
                : rs.OrderByDescending(orderBy).Skip(pageSize * pageIndex).Take(pageSize).ToList();
        }

        /// <summary>
        ///     Gets a lists of entities paginated according the where.
        /// </summary>
        /// <param name="where"> The where. </param>
        /// <param name="orderBy"> The order by. </param>
        /// <param name="pageIndex"> Index of the page. </param>
        /// <param name="pageSize"> Size of the page. </param>
        /// <param name="totalRecords"> The total records. </param>
        /// <param name="asNoTracking"> if set to <c>true</c> [as no tracking]. </param>
        /// <param name="orderByAscending"> if set true - order by ASCENDING otherwise order by DESCENDING. </param>
        /// <param name="includePath"> The include properties. </param>
        /// <returns> </returns>
        protected virtual IEnumerable<T> GetManyPaginated(
            Expression<Func<T, bool>> where, Expression<Func<T, object>> orderBy, int pageIndex,
            int pageSize,
            out int totalRecords, bool asNoTracking = false, bool orderByAscending = true,
            EntityPath<T> includePath = null)
        {
            var query = Context.Set<T>().AsQueryable();

            totalRecords = query.Where(where).Count();

            if (includePath != null)
            {
                query = includePath.Apply(query);
            }

            var rs = asNoTracking
                ? query.AsNoTracking().Where(where)
                : query.Where(where);

            return orderByAscending
                ? rs.OrderBy(orderBy).Skip(pageSize * pageIndex).Take(pageSize).ToList()
                : rs.OrderByDescending(orderBy).Skip(pageSize * pageIndex).Take(pageSize).ToList();
        }

        protected virtual IEnumerable<T> GetManyTop(
            Expression<Func<T, bool>> where, Expression<Func<T, object>> orderBy,
            int pageSize = 10,
            bool asNoTracking = false, bool orderByAscending = true,
            params Expression<Func<T, object>>[] includeProperties)
        {
            var query = Context.Set<T>().AsQueryable();

            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            // Return the value.
            var rs = asNoTracking
                ? query.AsNoTracking().Where(where)
                : query.Where(where);

            return orderByAscending
                ? rs.OrderBy(orderBy).Take(pageSize).ToList()
                : rs.OrderByDescending(orderBy).Take(pageSize).ToList();
        }

        /// <summary>
        ///     Gets the entity according the where.
        /// </summary>
        /// <param name="where"> Expression for the where. </param>
        /// <param name="asNoTracking"> if set to <c>true</c> [as no tracking]. </param>
        /// <returns> </returns>
        protected virtual T Get(Expression<Func<T, bool>> where, bool asNoTracking = false)
        {
            return asNoTracking
                ? Context.Set<T>().AsNoTracking().Where(where).FirstOrDefault()
                : Context.Set<T>().Where(where).FirstOrDefault();
        }

        /// <summary>
        ///     Gets the entity according the where.
        /// </summary>
        /// <param name="where"> Expression for the where. </param>
        /// <param name="orderBy"> Order by operator.</param>
        /// <param name="orderByAscending">if set true - order by ASCENDING otherwise order by DESCENDING.</param>
        /// <param name="asNoTracking"> if set to <c>true</c> [as no tracking]. </param>
        /// <returns> </returns>
        protected virtual T Get(
            Expression<Func<T, bool>> where,
            Expression<Func<T, object>> orderBy,
            bool orderByAscending = true,
            bool asNoTracking = false)
        {
            var rs = asNoTracking
                ? Context.Set<T>().AsNoTracking().Where(where)
                : Context.Set<T>().Where(where);

            rs = orderByAscending
                ? rs.OrderBy(orderBy)
                : rs.OrderByDescending(orderBy);

            // Return the value.
            return rs.FirstOrDefault();
        }

        /// <summary>
        ///     Gets the entity according the where, including the properties received.
        /// </summary>
        /// <param name="where"> Expression for the where. </param>
        /// <param name="asNoTracking">If set to <c>true</c> [as no tracking].</param>
        /// <param name="includeProperties"> The include properties. </param>
        /// <returns> </returns>
        protected virtual T GetIncluding(
            Expression<Func<T, bool>> where, bool asNoTracking, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return asNoTracking
                ? query.AsNoTracking().Where(where).ToList().FirstOrDefault()
                : query.Where(where).ToList().FirstOrDefault();
        }

        /// <summary>
        ///     Gets the entity according the where, including the properties received.
        /// </summary>
        /// <param name="where"> Expression for the where. </param>
        /// <param name="asNoTracking">If set to <c>true</c> [as no tracking].</param>
        /// <param name="includePath"> The include properties. </param>
        /// <returns> </returns>
        protected virtual T GetIncluding(
            Expression<Func<T, bool>> where, bool asNoTracking, EntityPath<T> includePath)
        {
            IQueryable<T> query = Context.Set<T>();
            query = includePath.Apply(query);

            return asNoTracking
                ? query.AsNoTracking().Where(where).ToList().FirstOrDefault()
                : query.Where(where).ToList().FirstOrDefault();
        }

        /// <summary>
        ///     Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="where"> Expression for the where. </param>
        /// <returns> </returns>
        protected virtual bool GetAny(Expression<Func<T, bool>> where)
        {
            return Context.Set<T>().Where(where).Any();
        }

        /// <summary>
        ///     Gets the count according the where.
        /// </summary>
        /// <param name="where"> Expression for the where. </param>
        /// <returns> </returns>
        protected virtual int GetCount(Expression<Func<T, bool>> where)
        {
            return Context.Set<T>().Where(where).Count();
        }

        /// <summary>
        ///     Commits the pending changes.
        /// </summary>
        protected virtual int Commit()
        {
            return Context.SaveChanges();
        }

        #endregion

        #endregion
    }
}