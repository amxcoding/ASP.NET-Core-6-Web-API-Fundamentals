using AutoMapper;
using CityInfo.API.Infrastructure;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController] // Adds additional features needed for building api. However not necessary.
    [Authorize(Policy = "MustBeFromAntwerp")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // !! If you change classname the uri will change too!
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository cityInfoRepository;
        private readonly IMapper mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            this.cityInfoRepository = cityInfoRepository ?? throw new ArgumentException(nameof(cityInfoRepository));
            this.mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        }

        // ActionResult of IActionResult can also be used as return type;
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDTO>>> 
            GetCities([FromQuery(Name ="filteronname")] string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10) // you can manually set the query string name,
                                                                      // the query string must be nullable
        {
            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }

            var (citiesEntities, paginationMetaData) = await cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            var result = mapper.Map<IEnumerable<CityWithoutPointsOfInterestDTO>>(citiesEntities);
            return Ok(result); // and empty collection is also valid response.
        }

        /// <summary>
        /// Get a city by id
        /// </summary>
        /// <param name="id"> The id of the city to get</param>
        /// <param name="includePointsOfInterest">Wheter or not to include the points of interest</param>
        /// <returns>An IActionResult</returns>
        /// <response code="200">Returns the requested city</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        [HttpGet("{id}")] // --> api/cities/1
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false) // return IActionResult more generic
                                                                                               // type if controller can return multiple types
        {
            var cityToReturn = await cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

            if (cityToReturn == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                var withPointsOfInterest = mapper.Map<CityDTO>(cityToReturn);
                return Ok(withPointsOfInterest); // TODO: mapping does not include PointsOfInterest
            }
            var withoutPointsOfInterest = mapper.Map<CityWithoutPointsOfInterestDTO>(cityToReturn);
            return Ok(withoutPointsOfInterest);
        }

    }
}

/*
 * Use binding source attributes when default behavior needs to be overriden.
 * Passing Data to the API.
 *      - [FromBody]: The body should come from the request body. Inferred for complex types;
 *      - [FromForm]: Form data in the request body. Inferred for action parameters of type IFormFile and IFormFileCollection;
 *      - [FromHeader]: Data comes from request header;
 *      - [FromQuery]: Query string parameters. Inferred for any other action parameters;
 *      - [FromRoute]: Route data from the current request. Inferred for any action parameter name matching a parameter in the route template;
 *      - [FromService]: The service injected as action parameter;
 *      
 * Example:
 * public ActionResult<CityDTO> GetCity([FromRoute] int cityId) {} // by default ASP.NET attempts to use the complex object model binder 
 * 
 * The [ApiController] attribute changes the rules to better fit APIs
 * 
 */