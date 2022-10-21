using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        /*
         * IQueryable<> vs IEnumerable<>:
         * The consumer can keep building on the IQueryable. For example, order by class, where class etc.
         * However you are leaking persistence related logic out of the repository. Which violates the purpose of the pattern.
         */

        /*
         * The purpose of async code is freeing up threads so 
         * they can be used for other tasks, which improves the scalability of you application.
         * 
         * It is not for perfomance improvement per se. Although it has a positive effect on the performance.
         * 
         * Threading is usefull when using IO opertations. 
         */
        Task<IEnumerable<City>> GetCitiesAsync();

        Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);

        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest); // can return null

        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);

        Task<bool> CityExistsAsync(int cityId);

        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);

        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

        Task<bool> SaveChangesAsync(); // when you add a new item, to an existing object save it

        void DeletePointOfInterest(PointOfInterest pointOfInterest); // deleting just like adding is an in memory operation not an i/o operation

        Task<bool> CityNameMatchesCityId(string? cityName, int cityId);


    }
}
