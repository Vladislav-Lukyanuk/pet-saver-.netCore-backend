using DAL.Entity;
using DAL.Provider.Interface;
using System;
using System.Collections.Generic;

namespace DAL.Provider
{
    public class RegisteredAnimalProvider: DataProvider<RegisteredAnimal>, IRegisteredAnimalProvider
    {
        private readonly Context Context;

        public RegisteredAnimalProvider(Context context) : base(context)
        {
            Context = context;
        }

        public IEnumerable<RegisteredAnimal> GetAll(string userId, ushort skip, ushort count)
        {
            var page = skip / count;
            return GetManyPaginated(a => a.UserId.Equals(userId), a => a.UploadDate, page, count, out var total, false, true, a => a.Animal, a => a.Animal.Coordinates);
        }

        public RegisteredAnimal Get(Guid id)
        {
            return GetIncluding(a => a.Id.Equals(id), false, a => a.Animal, a => a.Animal.Coordinates);
        }

        public new RegisteredAnimal Add(RegisteredAnimal rAnimal)
        {
            base.Add(rAnimal);
            return rAnimal;
        }

        public new Guid Remove(RegisteredAnimal rAnimal)
        {
            base.Remove(rAnimal);
            return rAnimal.Id;
        }

        public new RegisteredAnimal Update(RegisteredAnimal rAnimal)
        {
            base.Update(rAnimal);
            return rAnimal;
        }
    }
}
