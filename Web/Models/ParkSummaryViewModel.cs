using System;
using System.Collections.Generic;

namespace MVCWebApp.Models
{

    public class Park
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Area { get; set; }
        public string Facilities { get; set; }
    }

    public class ParkViewModel
    {
        public string SelectedCountry { get; set; }
        public string SelectedState { get; set; }
        public string SelectedCity { get; set; }
        public List<string> Countries { get; set; }
        public List<string> States { get; set; }
        public List<string> Cities { get; set; }
        public List<Park> Parks { get; set; }
        public int PageSize { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / PageSize);
        public string SortColumn { get; set; } = "Id";
        public string SortOrder { get; set; } = "asc";
    }

    public class LocationService
    {
        public static List<string> GetCountries()
        {
            return new List<string> { "USA" };
        }

        public static List<string> GetStates(string country)
        {
            if (country == "USA")
            {
                return new List<string>
                {
                    "Alabama", "Alaska", "Arizona", "Arkansas", "California",
                    "Colorado", "Connecticut", "Delaware", "Florida", "Georgia"
                };
            }
            return new List<string>();
        }

        public static List<string> GetCities(string state)
        {
            Dictionary<string, List<string>> stateCities = new Dictionary<string, List<string>>
            {
                { "Alabama", new List<string> { "Birmingham", "Montgomery", "Mobile", "Huntsville" } },
                { "Alaska", new List<string> { "Anchorage", "Fairbanks", "Juneau", "Sitka" } },
                { "Arizona", new List<string> { "Phoenix", "Tucson", "Mesa", "Chandler" } },
                { "Arkansas", new List<string> { "Little Rock", "Fort Smith", "Fayetteville", "Springdale" } },
                { "California", new List<string> { "Los Angeles", "San Francisco", "San Diego", "Sacramento" } },
                { "Colorado", new List<string> { "Denver", "Colorado Springs", "Aurora", "Fort Collins" } },
                { "Connecticut", new List<string> { "Bridgeport", "New Haven", "Hartford", "Stamford" } },
                { "Delaware", new List<string> { "Wilmington", "Dover", "Newark", "Middletown" } },
                { "Florida", new List<string> { "Miami", "Orlando", "Tampa", "Jacksonville" } },
                { "Georgia", new List<string> { "Atlanta", "Savannah", "Augusta", "Columbus" } }
            };

            if (stateCities.ContainsKey(state))
            {
                return stateCities[state];
            }
            return new List<string>();
        }
    }

    public class ParkService
    {
        private static List<Park> _allParks = new List<Park>
        {
            new Park { Id = 1, Name = "Sample1 National Park", Area = 2219791, Facilities = "Camping, Hiking, Visitor Center" },
            new Park { Id = 2, Name = "Sample2 Canyon National Park", Area = 1217403, Facilities = "Camping, Rafting, Museum" },
            new Park { Id = 3, Name = "Sample3 National Park", Area = 747956, Facilities = "Camping, Rock Climbing, Trails" },
            new Park { Id = 4, Name = "Sample4 National Park", Area = 146597, Facilities = "Camping, Shuttle Service, Ranger Programs" },
            new Park { Id = 5, Name = "Sample5 National Park", Area = 922651, Facilities = "Camping, Beach Access, Hot Springs" },
            new Park { Id = 6, Name = "Sample6 Water National Park", Area = 522419, Facilities = "Camping, Historic Buildings, Waterfalls" },
            new Park { Id = 7, Name = "Sample7 National Park", Area = 49075, Facilities = "Camping, Carriage Roads, Beach Access" },
            new Park { Id = 8, Name = "Sample8 National Park", Area = 1013322, Facilities = "Camping, Boating, Horseback Riding" },
            new Park { Id = 9, Name = "Sample9 National Park", Area = 1508939, Facilities = "Boating, Bird Watching, Airboat Tours" },
            new Park { Id = 10, Name = "Sample10 Tree National Park", Area = 790636, Facilities = "Camping, Rock Climbing, Stargazing" },
            new Park { Id = 11, Name = "Sample11 National Park", Area = 76680, Facilities = "Camping, Hiking, Photography" },
            new Park { Id = 12, Name = "Sample12 National Park", Area = 404063, Facilities = "Camping, Cave Tours, Nature Programs" },
            new Park { Id = 13, Name = "Sample13 Rainier National Park", Area = 236381, Facilities = "Camping, Visitor Center, Guided Tours" },
            new Park { Id = 14, Name = "Sample14 Mountain National Park", Area = 265795, Facilities = "Camping, Hiking, Wildlife Viewing" },
            new Park { Id = 15, Name = "Sample15 National Park", Area = 199173, Facilities = "Camping, Skyline Drive, Picnic Areas" }
        };

        // Dictionary mapping location to parks (for demonstration purposes)
        private static Dictionary<string, List<int>> _locationParkMap = new Dictionary<string, List<int>>
        {
            // California
            { "California_Los Angeles", new List<int> { 3, 10, 12 } },
            { "California_San Francisco", new List<int> { 5, 13 } },
            { "California_San Diego", new List<int> { 10, 11 } },
            { "California_Sacramento", new List<int> { 3, 12 } },
            
            // Florida
            { "Florida_Miami", new List<int> { 9 } },
            { "Florida_Orlando", new List<int> { 9, 15 } },
            { "Florida_Tampa", new List<int> { 9 } },
            { "Florida_Jacksonville", new List<int> { 9, 6 } },
            
            // Colorado
            { "Colorado_Denver", new List<int> { 14, 11 } },
            { "Colorado_Colorado Springs", new List<int> { 14 } },
            { "Colorado_Aurora", new List<int> { 14 } },
            { "Colorado_Fort Collins", new List<int> { 14, 8 } },
            
            // Default for other locations
            { "default", new List<int> { 1, 2, 3, 4, 5 } }
        };

        public static List<Park> GetParks(string country, string state, string city, string sortColumn, string sortOrder, int page, int pageSize, out int totalItems)
        {
            var parks = new List<Park>();

            // Create location key to match with our dictionary
            string locationKey = $"{state}_{city}";

            // Try to get parks for specific location
            if (_locationParkMap.ContainsKey(locationKey))
            {
                var parkIds = _locationParkMap[locationKey];
                parks = _allParks.Where(p => parkIds.Contains(p.Id)).ToList();
            }
            else
            {
                // If no specific location mapping, check if we have a mapping for just the state
                string stateKey = state + "_";
                var stateKeys = _locationParkMap.Keys.Where(k => k.StartsWith(stateKey));

                if (stateKeys.Any())
                {
                    // Get all parks mapped to this state
                    var parkIds = new HashSet<int>();
                    foreach (var key in stateKeys)
                    {
                        parkIds.UnionWith(_locationParkMap[key]);
                    }
                    parks = _allParks.Where(p => parkIds.Contains(p.Id)).ToList();
                }
                else
                {
                    // If still no mapping, use default list
                    parks = _locationParkMap["default"].Select(id => _allParks.First(p => p.Id == id)).ToList();
                }
            }

            totalItems = parks.Count;

            // Apply sorting
            parks = ApplySorting(parks, sortColumn, sortOrder);

            // Apply paging
            return parks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private static List<Park> ApplySorting(List<Park> parks, string sortColumn, string sortOrder)
        {
            switch (sortColumn.ToLower())
            {
                case "id":
                    return sortOrder.ToLower() == "asc"
                        ? parks.OrderBy(p => p.Id).ToList()
                        : parks.OrderByDescending(p => p.Id).ToList();
                case "name":
                    return sortOrder.ToLower() == "asc"
                        ? parks.OrderBy(p => p.Name).ToList()
                        : parks.OrderByDescending(p => p.Name).ToList();
                case "area":
                    return sortOrder.ToLower() == "asc"
                        ? parks.OrderBy(p => p.Area).ToList()
                        : parks.OrderByDescending(p => p.Area).ToList();
                case "facilities":
                    return sortOrder.ToLower() == "asc"
                        ? parks.OrderBy(p => p.Facilities).ToList()
                        : parks.OrderByDescending(p => p.Facilities).ToList();
                default:
                    return parks;
            }

        }

    }
}
