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
    public class PointOfInterestProfile : Profile
    {
        public PointOfInterestProfile()
        {
            CreateMap<PointOfInterest, PointOfInterestDTO>();
            CreateMap<PointOfInterestForCreationDTO, PointOfInterest>();
            CreateMap<PointOfInterestForUpdateDTO, PointOfInterest>();
            CreateMap<PointOfInterest, PointOfInterestForUpdateDTO>();
        }

    }
}
