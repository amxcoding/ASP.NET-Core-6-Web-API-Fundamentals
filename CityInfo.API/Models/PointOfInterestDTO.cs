using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo.API.Models
{
    public class PointOfInterestDTO
    {
        public int Id { get; set; } 

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

    }
}
