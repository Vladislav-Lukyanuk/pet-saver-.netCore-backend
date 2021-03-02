using System;
using DAL.Enum;
using Microsoft.AspNetCore.Mvc;
using animalFinder.Mapper;
using animalFinder.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using animalFinder.DTO.API;
using System.Security.Claims;

namespace animalFinder.Controllers
{
    [ApiController]
    [Route("api/found")]
    public class FoundController : Controller
    {
        private readonly IAnimalService _animalService;

        public FoundController(IAnimalService animalService)
        {
            _animalService = animalService;
        }

        [HttpGet]
        public IActionResult GetWithPagination(
            [FromQuery(Name = "skip")] ushort skip,
            [FromQuery(Name = "count")] ushort count,
            [FromQuery(Name = "lat")] float lat,
            [FromQuery(Name = "lng")] float lng,
            [FromQuery(Name = "radius")] short radius
        )
        {
            try
            {
                var animals = AnimalMapper.StoA(
                    _animalService.GetWithPagination(Status.Found, skip, count, lat, lng, radius)
                );
                return Ok(animals);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("/by-user/{id}")]
        [Authorize]
        public IActionResult GetWithPaginationByUserId(
            [FromQuery(Name = "skip")] ushort skip,
            [FromQuery(Name = "count")] ushort count
        )
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;

                var animals = AnimalMapper.StoA(
                    _animalService.GetWithPaginationByUserId(userId, Status.Found, skip, count)
                );
                return Ok(animals);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                var animal = AnimalMapper.StoA(_animalService.GetById(new Guid(id)));
                return Ok(animal);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add([FromBody] Animal animal)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;

                var sAnimal = AnimalMapper.AtoS(animal);
                sAnimal.Status = Status.Found;
                sAnimal.UserId = userId;

                var addedAnimal = AnimalMapper.StoA(_animalService.Add(sAnimal));
                return Ok(addedAnimal);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(string id)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;

                var removedAnimalId = _animalService.Remove(userId, new Guid(id));

                return Ok(removedAnimalId);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
