using animalFinder.DTO.Service;
using DAL.Enum;
using System;
using System.Collections.Generic;

namespace animalFinder.Service.Interface
{
    public interface IAnimalService
    {
        IEnumerable<Animal> GetWithPagination(Status status, ushort skip, ushort count, float userPointLat, float userPointLng, short radius);
        IEnumerable<Animal> GetWithPaginationByUserId(string userId, Status status, ushort skip, ushort count);
        Animal GetById(Guid id);
        Animal Add(Animal animal);
        Animal Edit(Guid id, Coordinate coordinates);
        Guid Remove(string userId, Guid id);
    }
}
