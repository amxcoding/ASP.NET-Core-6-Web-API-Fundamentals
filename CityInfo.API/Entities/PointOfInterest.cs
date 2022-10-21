using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }        

        [MaxLength(200)]
        public string Description { get; set; }

        /*
         * To signify the relation between city and point of interest there are two ways.
         * Note a city can have multiple points of interest. Hence there is a one to many relationship here.
         * Thus every point of interest containts the city id which it belongs to.
         * 
         * We want to navigate in the object graph from a point of interest to the parent city.
         * We need a property to refer to that parent city. And we need to state what the foreign key property will be.
         * 
         * Convention based:
         * By convention a relationship will be created when there is a navigation property discovered.
         * A property is considered a navigation property if the property cannot be mapped to a scalar by the current database provider.
         * These relationship will always target the primary key of the property. Here City Id.
         * When you use the conventional method you dont need to explicitly state the foreign key.
         * However it is good for clearity.
         * 
         * Explicitly stated:
         * [ForeignKey("CityId")] 
         * public int CityId { get; set; }
         * 
         */

        [ForeignKey("CityId")] // not necessary when the name contains the object it refers to
        public int CityId { get; set; }
        public City? City { get; set; } // navigation property

        public PointOfInterest(string name)
        {
            Name = name;
        }
        


    }
}