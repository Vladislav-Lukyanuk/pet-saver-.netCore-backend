using animalFinder.DTO.Service;
using DAL.Enum;
using System;
using System.Collections.Generic;

namespace animalFinder.Service.Interface
{
    public interface IPrivateProfileService
    {
        IEnumerable<RegisteredAnimal> GetWithPagination(string userId, ushort skip, ushort count);
        RegisteredAnimal GetById(string userId, Guid id);
        RegisteredAnimal GetById(Guid id);
        RegisteredAnimal Add(string userId, RegisteredAnimal animal);
        Guid Remove(string userId, Guid rAnimalId);
        RegisteredAnimal Edit(string userId, Guid animalId, RegisteredAnimal animal);
        RegisteredAnimal MarkAs(string userId, Guid rAnimalId, Status status, string title, string description, Coordinate coordinates);
        void SendToMail(string userId, Guid rAnimalId);
    }
}
