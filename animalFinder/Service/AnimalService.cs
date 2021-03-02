using animalFinder.DTO.Service;
using animalFinder.Enum;
using animalFinder.Exception;
using animalFinder.Mapper;
using animalFinder.Service.Interface;
using DAL;
using DAL.Enum;
using DAL.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Net;

namespace animalFinder.Service
{
    public class AnimalService: IAnimalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAnimalDataProvider _animalDataProvider;
        private readonly IGeo _geo;
        public AnimalService(IUnitOfWork unitOfWork, IGeo geo) 
        {
            _unitOfWork = unitOfWork;
            _animalDataProvider = unitOfWork.Get<IAnimalDataProvider>();
            _geo = geo;
        }

        public IEnumerable<Animal> GetWithPagination(Status status, ushort skip, ushort count, float userPointLat, float userPointLng, short radius)
        {
            List<DAL.Entity.Animal> animals = (List<DAL.Entity.Animal>) _animalDataProvider.GetAll(status, skip, count);

            var filteredAnimals = animals.FindAll(a =>
            {
                DAL.Entity.Coordinate[] coordinates = new DAL.Entity.Coordinate[a.Coordinates.Count];
                a.Coordinates.CopyTo(coordinates, 0);
                var pointLat = coordinates[coordinates.Length - 1].Latitude;
                var pointLng = coordinates[coordinates.Length - 1].Longitude;
                
                return _geo.GetDistance(pointLat, pointLng, userPointLat, userPointLng) <= radius;
            });

            return AnimalMapper.DtoS(filteredAnimals);
        }

        public IEnumerable<Animal> GetWithPaginationByUserId(string userId, Status status, ushort skip, ushort count)
        {
            var animals = _animalDataProvider.GetAllByUserId(userId, status, skip, count);

            return AnimalMapper.DtoS(animals);
        }

        public Animal GetById(Guid id)
        {
            var animal = _animalDataProvider.Get(id);

            return AnimalMapper.DtoS(animal);
        }

        public Animal Add(Animal animal)
        {
            animal.UploadDate = DateTimeOffset.Now;
            var addedAnimal = _animalDataProvider.Add(AnimalMapper.StoD(animal));

            _unitOfWork.Commit();

            return AnimalMapper.DtoS(addedAnimal);
        }

        public Animal Edit(Guid id, Coordinate coordinates)
        {
            var animal = _animalDataProvider.Get(id);
            if (animal == null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }

            var animalCoordinates = animal.Coordinates;
            animalCoordinates.Add(CoordinateMapper.StoD(coordinates));
            animal.Coordinates = animalCoordinates;

            var updatedAnimal = _animalDataProvider.Update(animal);

            _unitOfWork.Commit();

            return AnimalMapper.DtoS(updatedAnimal);
        }

        public Guid Remove(string userId, Guid id)
        {
            var animal = _animalDataProvider.Get(id);

            if (animal == null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }
            if (!animal.UserId.Equals(userId))
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
            }

            var returnedId =_animalDataProvider.Remove(animal);

            _unitOfWork.Commit();

            return returnedId;
        }
    }
}
