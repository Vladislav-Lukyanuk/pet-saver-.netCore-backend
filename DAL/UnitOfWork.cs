using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context context;
        private readonly IServiceProvider serviceProvider;

        public UnitOfWork(Context context, IServiceProvider serviceProvider)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
        }

        public int Commit()
        {
            return context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await context.SaveChangesAsync();
        }

        public T Get<T>()
        {
            var result = serviceProvider.GetService<T>();

            if (result == null)
            {
                throw new ApplicationException($"Unknown service {typeof(T)}");
            }

            return (T)result;
        }
    }
}
