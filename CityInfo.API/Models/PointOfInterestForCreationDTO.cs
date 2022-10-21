using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace CityInfo.API.Models
{
    /*
     * Is is a good idea to use different models for create update and read.
     */
    public class PointOfInterestForCreationDTO
    {
        /*
        * From security first point of view it is important to always validate your objects.
        * 
        * For small project validation used below is good. For large project to keep validation seperated from models use:
        * FluentValidation -> https://docs.fluentvalidation.net/en/latest/
        */
        [Required(ErrorMessage ="Name is required.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
