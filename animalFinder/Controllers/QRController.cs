using System;
using System.Net;
using animalFinder.DTO.API;
using animalFinder.Enum;
using animalFinder.Exception;
using animalFinder.Mapper;
using animalFinder.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace animalFinder.Controllers
{
    [ApiController]
    [Route("api/qr")]
    public class QRController : ControllerBase
    {
        private readonly IPrivateProfileService _privateProfileService;
        private readonly IAnimalService _animalService;

        public QRController(IPrivateProfileService privateProfileService, IAnimalService animalService)
        {
            _privateProfileService = privateProfileService;
            _animalService = animalService;
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id, [FromQuery(Name = "lat")] float lat, [FromQuery(Name = "lng")] float lng)
        {
            try
            {
                var registeredAnimal = _privateProfileService.GetById(new Guid(id));
                if (registeredAnimal.AnimalId == null)
                {
                    throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
                }

                var animal = _animalService.Edit((Guid) registeredAnimal.AnimalId, new DTO.Service.Coordinate() { Latitude = lat, Longitude = lng });

                return Ok(AnimalMapper.StoA(animal));
            }
            catch (ApiException exp)
            {
                return StatusCode(
                    exp.GetStatusCode(),
                    Error.CreateBuilder().SetErrorMessage(exp.GetErrorMessage()).Build()
                    );
            }
            catch
            {
                return BadRequest();
            }
        }

        //for downloading the qr use the file controller
    }
}