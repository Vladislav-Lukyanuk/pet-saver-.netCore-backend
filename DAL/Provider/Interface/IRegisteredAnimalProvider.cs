using DAL.Entity;
using System;
using System.Collections.Generic;

namespace DAL.Provider.Interface
{
    public interface IRegisteredAnimalProvider
    {
        IEnumerable<RegisteredAnimal> GetAll(string userId, ushort skip, ushort count);
        RegisteredAnimal Get(Guid id);

        RegisteredAnimal Add(RegisteredAnimal rAnimal);

        Guid Remove(RegisteredAnimal rAnimal);

        RegisteredAnimal Update(RegisteredAnimal rAnimal);
    }
}
