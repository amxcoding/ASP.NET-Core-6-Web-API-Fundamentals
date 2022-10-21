using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo.API.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            // add mapping configuration in the constructor
            CreateMap<City, CityWithoutPointsOfInterestDTO>(); // if a property does not exist it will be ignored
            CreateMap<City, CityDTO>(); // if a property does not exist it will be ignored
            

        }
    }
}
