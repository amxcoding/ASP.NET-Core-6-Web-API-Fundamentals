using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// to explicetly state how the id is generated. default is identity.
                                                             // Note this also depends on the database type.
                                                             // Some databases require you to explicityly state how the primary key is generated
        public int Id { get; set; } // Because the name contains the word Id, it is already the primary key so attribute key is not needed

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } // if we dont want a null value, initialize as empty string or add constructor

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<PointOfInterest> PointsOfInterests { get; set; } 
            = new List<PointOfInterest>(); // initialize as an empty list to avoid null exception problems

        public City(string name)
        {
            Name = name;
        }
    }
}
