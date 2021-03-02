using DAL.Entity;
using System;

namespace DAL.Provider.Interface
{
    public interface IFileDataProvider
    {
        File Get(Guid id);
        File Add(File file);
    }
}
