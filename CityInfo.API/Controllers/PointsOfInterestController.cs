using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Infrastructure;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    /*
     * Paths can indicate a hierarchy of subresources (/contacts/22/addresses), 
     * but API designers should avoid complex structures that require more than two levels of nesting.
     * 
     * Paths that end with a resource name (and typically no trailing slash) are used to list multiple items 
     * (/files) or create items without specifying an identifier.
     * 
     * For individual resources, include resource identifiers in the path, not the query (/contacts/22 instead of /contacts?id=22).
     * 
     * https://blog.stoplight.io/crud-api-design
     */

    [ApiController]
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/cities/{cityId}/[controller]")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _localMailService;
        private readonly ICityInfoRepository repository;
        private readonly IMapper mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, 
            IMailService localMailService, ICityInfoRepository repository, IMapper mapper)
        {
            this._logger = logger ?? throw new ArgumentException(nameof(logger)); // useful when you use a different container than the default
            this._localMailService = localMailService ?? throw new ArgumentException(nameof(localMailService));
            this.repository = repository;
            this.mapper = mapper;
            ;
            //HttpContext.RequestServices.GetServices(); To directly request a service without injection only when no injection possible
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDTO>>> GetPointsOfInterest(int cityId)
        {
            var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value; // beter if we had an id 

            if (!await repository.CityNameMatchesCityId(cityName, cityId))
            {
                return Forbid();
            }

            if (!await repository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest");
                return NotFound();
            }

            var pointsOfInterest = await repository.GetPointsOfInterestForCityAsync(cityId); // TODO: returns only one item
            var pointsOfInterestDTO = mapper.Map<IEnumerable<PointOfInterestDTO>>(pointsOfInterest);

            return Ok(pointsOfInterestDTO);
        }


        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDTO>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            try
            {

                if (!await repository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"City with city id {cityId} was not found.");
                    return NotFound();
                }

                var pointOfInterest = await repository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

                if (pointOfInterest == null)
                {
                    return NotFound();
                }

                var pointOfInterestDTO = mapper.Map<PointOfInterestDTO>(pointOfInterest);

                return Ok(pointOfInterestDTO);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex);
                // because we catch the exception we need to return manually a 500 error;
                // This will go back to the consumer of the API, dont expose implementation details
                // Never write out a stacktrace in a production environment
                return StatusCode(500, "An error occured during your request.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDTO>> CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDTO pointOfInterest) // [FromBody] is default here not needed to add
        {

            /*
             * Modelstate is a dictionary containing both the state of the model and model binding validation.
             * If a rule is applied to a model in as data annotation then it can be checked if the rules all adhered.
             * If not then Modelstate.IsValid will return false. Is also false when an invalid value for a property type is passed in.
             * 
             * Note thanks to de ApiController atributte is the below code note needed.
             */
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            if (!await repository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = mapper.Map<PointOfInterest>(pointOfInterest);
            await repository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);
            bool succes = await repository.SaveChangesAsync();
            
            var created = mapper.Map<PointOfInterestDTO>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",// here you point to the name of the action GetPointOfInterest (what is the same als the action)
                new // this information helps to setup the route which is used in the action GetPointOfInterest
                {
                    cityId = cityId,
                    pointOfInterestId = created.Id
                },
                created
                );

        }


        [HttpPut("{pointofInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDTO pointOfInterest)
        {
            // validation of input happens on dto level

            // check if city exists 
            if (!await repository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            // check if to be updated item exits
            var pointOfInterestEntity = await repository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var updated = mapper.Map(pointOfInterest, pointOfInterestEntity); // auto mapper will override destination object with source object
            await repository.SaveChangesAsync();

            return NoContent();
        }


        /*
         * A JSON patch document is a list of operations like add, remove, replace, delete etc. 
         * that have to be applied to the resource allowing for partial updates.
         * Nuget Mircrosoft.AspNetCore.JsonPatch --> Dependency on Newtonsoft.JSON
         * 
         * Patch documentation https://www.rfc-editor.org/rfc/rfc6902
         * Example
         *    [ // an array of operations
                 { "op": "test", "path": "/a/b/c", "value": "foo" },  ->> op = operation name, replace or add or remove
                 { "op": "remove", "path": "/a/b/c" }, 
                 { "op": "add", "path": "/a/b/c", "value": [ "foo", "bar" ] },   --> path signifies the path to the property for example /name
                 { "op": "replace", "path": "/a/b/c", "value": 42 }, --> value signifies the new value for a property such as new name value
                 { "op": "move", "from": "/a/b/c", "path": "/a/b/d" },
                 { "op": "copy", "from": "/a/b/d", "path": "/a/b/e" }
               ]
         * 
         * 
         * Patching can be used to manipulate arrays and copy or move values etc.
         */
        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdateDTO> patchDocument) // the request for a patch is a json patch document
        {
            // If you use one DTO for update and create etc. remeber to check that patch doesn't try to update the id
            // check if city exists 
            if (!await repository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            // check if to be updated item exits
            var pointOfInterestEntity = await repository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = mapper.Map<PointOfInterestForUpdateDTO>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState); // by providing the modelstate any errors will make the modelstate invalid

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // validate the model if required fields etc are still met
            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            // Update
            mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            await repository.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            // check if city exists 
            if (!await repository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            // check if to be updated item exits
            var pointOfInterestEntity = await repository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            repository.DeletePointOfInterest(pointOfInterestEntity);
            await repository.SaveChangesAsync();

            _localMailService.Send("Point of interest deleted", $"{pointOfInterestId}");
            return NoContent();

        }
    }
}
