using System;
using System.Security.Claims;
using animalFinder.DTO.API;
using animalFinder.Exception;
using animalFinder.Mapper;
using animalFinder.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace animalFinder.Controllers
{
    [ApiController]
    [Route("api/private-profile")]
    [Authorize]
    public class PrivateProfileController : ControllerBase
    {
        private readonly IPrivateProfileService _privateProfileService;

        public PrivateProfileController(IPrivateProfileService privateProfileService)
        {
            _privateProfileService = privateProfileService;
        }
        [HttpGet]
        public IActionResult GetWithPagination(
            [FromQuery(Name = "skip")] ushort skip,
            [FromQuery(Name = "count")] ushort count
        )
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;
                var registeredAnimals = RegisteredAnimalMapper.StoA(_privateProfileService
                    .GetWithPagination(userId, skip, count));
                return Ok(registeredAnimals);
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

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;
                var registeredAnimal = RegisteredAnimalMapper.StoA(_privateProfileService.GetById(userId, new Guid(id)));
                return Ok(registeredAnimal);
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

        [HttpPost]
        public IActionResult Post([FromBody] RegisteredAnimal rAnimal)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;
                var addedAnimal = RegisteredAnimalMapper.StoA(_privateProfileService.Add(userId, RegisteredAnimalMapper.AtoS(rAnimal)));

                return Ok(addedAnimal);
            }
            catch (System.Exception s)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] RegisteredAnimal rAnimal)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;
                var updatedAnimal = RegisteredAnimalMapper.StoA(_privateProfileService.Edit(userId, new Guid(id), RegisteredAnimalMapper.AtoS(rAnimal)));

                return Ok(updatedAnimal);
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

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;
                var removedAnimalId = _privateProfileService.Remove(userId, new Guid(id));

                return Ok(removedAnimalId);
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

        [HttpGet("mark-as/{id}")]
        public IActionResult MarkAs(
            string id,
            [FromQuery(Name = "type")] int type,
            [FromQuery(Name = "title")] string title,
            [FromQuery(Name = "description")] string description,
            [FromQuery(Name = "lat")] float lat,
            [FromQuery(Name = "lng")] float lng
            )
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;
                var markedAnimal = _privateProfileService.MarkAs(
                    userId,
                    new Guid(id),
                    (DAL.Enum.Status) type,
                    title,
                    description,
                    CoordinateMapper.AtoS(new Coordinate() { Latitude = lat, Longitude = lng })
                    );

                return Ok(RegisteredAnimalMapper.StoA(markedAnimal));
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

        [HttpGet("send-to-mail/{id}")]
        public IActionResult SendToMail(string id)
        {
            try 
            { 
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;
                _privateProfileService.SendToMail(userId,new Guid(id));
                return Ok();
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
    }
}
