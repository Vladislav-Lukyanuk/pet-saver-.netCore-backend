using DAL.Entity;
using DAL.Enum;
using System;
using System.Collections.Generic;

namespace DAL.Provider.Interface
{
    public interface IAnimalDataProvider
    {
        IEnumerable<Animal> GetAll(Status status, ushort skip, ushort count);
        IEnumerable<Animal> GetAllByUserId(string userId, Status status, ushort skip, ushort count);
        Animal Get(Guid id);
        Animal Add(Animal animal);
        Animal Update(Animal animal);
        Guid Remove(Animal animal);
    }
}
