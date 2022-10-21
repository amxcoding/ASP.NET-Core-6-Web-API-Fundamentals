using CityInfo.API.DBContext;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext cityInfoContext;

        public CityInfoRepository(CityInfoContext cityInfoContext)
        {
            this.cityInfoContext = cityInfoContext ?? throw new ArgumentNullException(nameof(cityInfoContext));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await cityInfoContext.Cities
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // overloading
        public async Task<(IEnumerable<City>, PaginationMetaData)> // Tuple, allows us to return multiple values easily
            GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = cityInfoContext.Cities as IQueryable<City>;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery) || (a.Description != null && a.Description.Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();
            var paginationMetaData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection
                .OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1)) // Go to a certain page by skipping page amount of data       add paging last on filtered and searched collection
                .Take(pageSize) // return the page size amount of data
                .ToListAsync();

            return (collectionToReturn, paginationMetaData);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await cityInfoContext.Cities
                    .Include(c => c.PointsOfInterests)
                    .Where(c => c.Id == cityId)
                    .FirstOrDefaultAsync();
            }
            return await cityInfoContext.Cities
                .Where(c => c.Id == cityId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await cityInfoContext.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await cityInfoContext.PointsOfInterests
                .Where(p => p.Id == pointOfInterestId && p.CityId == cityId)
                .FirstOrDefaultAsync(); 
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await cityInfoContext.PointsOfInterests
                .Where(c => c.Id == cityId)
                .ToListAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);

            if (city != null)
            {
                city.PointsOfInterests.Add(pointOfInterest);
            }

        }

        public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
        {
            return await cityInfoContext.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }


        public async Task<bool> SaveChangesAsync()
        {
            return (await cityInfoContext.SaveChangesAsync() >= 1);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            cityInfoContext.PointsOfInterests.Remove(pointOfInterest);
        }
    }
}
