namespace CityInfo.API.Models
{
    public class CityDTO
    {
        /*
         * What is accepted or returned from an API is not the same as the entities used
         * by the underlying datastore.
         */
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty; // the value is "" when you  make a new object

        public string? Description { get; set; }

        public int NumberOfPointsOfInterest
        {
            get
            {
                return PointsOfInterest.Count;
            }
        }

        public ICollection<PointOfInterestDTO> PointsOfInterest { get; set; } 
            = new List<PointOfInterestDTO>(); // always initialize a collection to avoid null reference



    }
}
