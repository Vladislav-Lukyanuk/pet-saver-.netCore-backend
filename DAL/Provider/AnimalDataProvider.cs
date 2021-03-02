using DAL.Entity;
using DAL.Enum;
using DAL.Provider.Interface;
using System;
using System.Collections.Generic;

namespace DAL.Provider
{
    public class AnimalDataProvider : DataProvider<Animal>, IAnimalDataProvider
    {
        private readonly Context Context;

        public AnimalDataProvider(Context context) : base(context)
        {
            Context = context;
        }

        public IEnumerable<Animal> GetAll(Status status, ushort skip, ushort count)
        {
            var page = skip / count;

            return GetManyPaginated(a => a.Status.Equals(status), a => a.UploadDate, page, count, out var total, false, false, a => a.Coordinates);
        }
        public IEnumerable<Animal> GetAllByUserId(string userId, Status status, ushort skip, ushort count)
        {
            var page = skip / count;

            return GetManyPaginated(a => a.UserId.Equals(userId) && a.Status.Equals(status), a => a.UploadDate, page, count, out var total, false, false, a => a.Coordinates);
        }

        public Animal Get(Guid id) 
        {
            return GetIncluding(a => a.Id.Equals(id), false, a => a.Coordinates);
        }

        public new Animal Add(Animal animal)
        {
            base.Add(animal);
            return animal;
        }

        public new Animal Update(Animal animal)
        {
            base.Update(animal);
            return animal;
        }

        public new Guid Remove(Animal animal)
        {
            base.Remove(animal);
            return animal.Id;
        }
    }
}
