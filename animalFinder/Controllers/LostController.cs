using System;
using animalFinder.Mapper;
using animalFinder.Service.Interface;
using DAL.Enum;
using Microsoft.AspNetCore.Mvc;

namespace animalFinder.Controllers
{
    [ApiController]
    [Route("api/lost")]
    public class LostController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        public LostController(IAnimalService animalService)
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
                var animals = AnimalMapper.StoA(_animalService.GetWithPagination(Status.Lost, skip, count, lat, lng, radius));
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
    }
}
