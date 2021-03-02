using animalFinder.DTO.Service;
using animalFinder.Enum;
using animalFinder.Exception;
using animalFinder.Mapper;
using animalFinder.Service.Interface;
using DAL;
using DAL.Enum;
using DAL.Provider.Interface;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using animalFinder.Constant;
using animalFinder.Object;

namespace animalFinder.Service
{
    public class PrivateProfileService : IPrivateProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRegisteredAnimalProvider _registeredAnimalProvider;
        private readonly IAnimalDataProvider _animalDataProvider;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;
        private readonly QRCodeGenerator _qrGenerator; 
        public PrivateProfileService(IUnitOfWork unitOfWork, IFileService fileService, IEmailService emailService)
        {
            _fileService = fileService;
            _emailService = emailService;
            _qrGenerator = new QRCodeGenerator();
            _unitOfWork = unitOfWork;
            _registeredAnimalProvider = unitOfWork.Get<IRegisteredAnimalProvider>();
            _animalDataProvider = unitOfWork.Get<IAnimalDataProvider>();
            _userDataProvider = unitOfWork.Get<IUserDataProvider>();
        }

        public IEnumerable<RegisteredAnimal> GetWithPagination(string userId, ushort skip, ushort count)
        {
            var rAnimals = _registeredAnimalProvider.GetAll(userId, skip, count);
            if (rAnimals == null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }

            return RegisteredAnimalMapper.DtoS(rAnimals);
        }
        public RegisteredAnimal GetById(string userId, Guid id)
        {
            var animal = _registeredAnimalProvider.Get(id);
            if (animal == null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }
            if (!animal.UserId.Equals(userId))
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
            }

            return RegisteredAnimalMapper.DtoS(animal);
        }

        public RegisteredAnimal GetById(Guid id)
        {
            var animal = _registeredAnimalProvider.Get(id);
            if (animal == null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }

            return RegisteredAnimalMapper.DtoS(animal);
        }

        public RegisteredAnimal Add(string userId, RegisteredAnimal animal)
        {
            animal.UploadDate = DateTimeOffset.Now;
            var databaseLayerAnimal = RegisteredAnimalMapper.StoD(animal);
            var addedAnimal = _registeredAnimalProvider.Add(databaseLayerAnimal);

            QRCodeData qrCodeData = _qrGenerator.CreateQrCode(addedAnimal.Id.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            var qrId = _fileService.Upload(userId, _fileService.AsJpeg(qrCodeImage), "image/jpeg");

            addedAnimal.QR = qrId.ToString();
            addedAnimal.UserId = userId;
            _registeredAnimalProvider.Update(addedAnimal);

            _unitOfWork.Commit();

            return RegisteredAnimalMapper.DtoS(addedAnimal);
        }

        public Guid Remove(string userId, Guid rAnimalId)
        {
            var rAnimal = _registeredAnimalProvider.Get(rAnimalId);
            if (rAnimal == null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }
            if (!rAnimal.UserId.Equals(userId))
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
            }

            var removedRAnimalId = _registeredAnimalProvider.Remove(rAnimal);

            _unitOfWork.Commit();

            return removedRAnimalId;
        }

        public RegisteredAnimal Edit(string userId, Guid animalId, RegisteredAnimal animal)
        {
            var rAnimal = _registeredAnimalProvider.Get(animalId);
            if (rAnimal == null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }
            if (!rAnimal.UserId.Equals(userId))
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
            }

            rAnimal.Image = animal.Image;
            rAnimal.Name = animal.Name;

            var updatedAnimal = _registeredAnimalProvider.Update(rAnimal);

            _unitOfWork.Commit();

            return RegisteredAnimalMapper.DtoS(updatedAnimal);
        }

        public RegisteredAnimal MarkAs(string userId, Guid rAnimalId, Status status, string title, string description, Coordinate coordinates)
        {
            if (status.Equals(Status.Found)) 
            {
                var foundRAnimal = _registeredAnimalProvider.Get(rAnimalId);
                if (foundRAnimal == null)
                {
                    throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
                }
                if (!foundRAnimal.UserId.Equals(userId))
                {
                    throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
                }

                //remove from animal list
                _animalDataProvider.Remove(foundRAnimal.Animal);
                
                //remove link
                foundRAnimal.Animal = null;
                _registeredAnimalProvider.Update(foundRAnimal);

                _unitOfWork.Commit();
                
                return RegisteredAnimalMapper.DtoS(foundRAnimal);
            }

            var lostRAnimal = _registeredAnimalProvider.Get(rAnimalId);
            if (lostRAnimal == null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }
            if (!lostRAnimal.UserId.Equals(userId))
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
            }

            var animal = new DAL.Entity.Animal()
            {
                Image = lostRAnimal.Image,
                Title = string.IsNullOrEmpty(title) ? lostRAnimal.Name : title,
                Description = string.IsNullOrEmpty(description) ? "" : description,
                UploadDate = DateTimeOffset.Now,
                Status = Status.Lost,
                Coordinates = new List<DAL.Entity.Coordinate>() { CoordinateMapper.StoD(coordinates) }
            };

            lostRAnimal.Animal = animal;

            var updatedAnimal = _registeredAnimalProvider.Update(lostRAnimal);

            _unitOfWork.Commit();

            return RegisteredAnimalMapper.DtoS(updatedAnimal);
        }

        public void SendToMail(string userId, Guid rAnimalId)
        {
            var animal = _registeredAnimalProvider.Get(rAnimalId);
            if (animal == null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }
            if (!animal.UserId.Equals(userId))
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
            }

            var user = _userDataProvider.Get(userId);
            var file = _fileService.Get(new Guid(animal.QR));
            _emailService.Generate(user.Email, "QRCode", new EmailObject { }, MailConstant.QR_CODE, new [] { file });
        }
    }
}
