//using CityInfo.API.Models;

//namespace CityInfo.API.Infrastructure
//{
//    /*
//     * Thread safe singleton. Note is not lazy
//     * For lazy and thread safe use:
//        public sealed class Singleton
//        {
//            private static readonly Lazy<Singleton> lazy =
//                new Lazy<Singleton>(() => new Singleton());

//            public static Singleton Instance { get { return lazy.Value; } }

//            private Singleton()
//            {
//            }
//        }
//     */
//    public class CitiesDataStore
//    {
//        private static CitiesDataStore _instance = null;
//        private static readonly object _lock = new object();
//        public List<CityDTO> Cities { get; set; }

//        private CitiesDataStore()
//        {
//            Cities = new List<CityDTO>() {
//                    new CityDTO() {
//                        Id = 1,
//                        Name = "New York City",
//                        Description = "The one with that big park",
//                        PointsOfInterest = new List<PointOfInterestDTO>()
//                        {
//                            new PointOfInterestDTO()
//                            {
//                                Id = 1,
//                                Name = "Central Park",
//                                Description = "Most visited park"

//                            },
//                            new PointOfInterestDTO()
//                            {
//                                Id = 2,
//                                Name = "Central Park",
//                                Description = "Most visited park"

//                            },

//                        }
//                    },
//                    new CityDTO() {
//                        Id = 2,
//                        Name = "Antwerp",
//                        Description = "Antwerp",
//                        PointsOfInterest = new List<PointOfInterestDTO>()
//                        {
//                            new PointOfInterestDTO()
//                            {
//                                Id = 1,
//                                Name = "Central Park",
//                                Description = "Most visited park"

//                            },
//                            new PointOfInterestDTO()
//                            {
//                                Id = 2,
//                                Name = "Central Park",
//                                Description = "Most visited park"

//                            },

//                        }
//                    },
//                    new CityDTO() {
//                        Id = 3,
//                        Name = "Paris",
//                        Description = "Paris",
//                        PointsOfInterest = new List<PointOfInterestDTO>()
//                        {
//                            new PointOfInterestDTO()
//                            {
//                                Id = 1,
//                                Name = "Central Park",
//                                Description = "Most visited park"

//                            },
//                            new PointOfInterestDTO()
//                            {
//                                Id = 2,
//                                Name = "Central Park",
//                                Description = "Most visited park"

//                            },

//                        }
//                    }

//                };

//        }

//        public static CitiesDataStore Instance
//        {
//            get
//            {
//                lock (_lock)
//                {
//                    if (_instance == null)
//                    {
//                        _instance = new CitiesDataStore();
//                    }
//                    return _instance;
//                }
//            }
//        }




//    }
//}
