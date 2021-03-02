using System.Threading.Tasks;

namespace DAL
{
    public interface IUnitOfWork
    {
        T Get<T>();
        int Commit();
        Task<int> CommitAsync();
    }
}
