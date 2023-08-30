using AdoptiverseAPI.DataAccess;
using AdoptiverseAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;

namespace AdoptiverseEndpointTests
{
    public class AdoptiverseEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AdoptiverseEndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }






        [Fact]
        public async void GetShelters_ReturnsListOfShelters()
        {

            Shelter shelter1 = new Shelter {CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = true, Rank = 1, City = "Denver", Name = "Dumb Friends League"};

            Shelter shelter2 = new Shelter { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = true, Rank = 2, City = "Houston", Name = "Houston Humane Society" };

            List<Shelter> shelters = new() { shelter1, shelter2 };

            AdoptiverseApiContext context = GetDbContext();

            HttpClient client = _factory.CreateClient();
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            HttpResponseMessage response = await client.GetAsync("/api/shelters");
            string content = await response.Content.ReadAsStringAsync();

            // The method ObjectToJson is defined below
            string expected = ObjectToJson(shelters);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);

        }




        [Fact]
        public async void GetShelter_ReturnsShelter()
        {
            //Arrange
            Shelter shelter1 = new Shelter { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = true, Rank = 1, City = "Denver", Name = "Dumb Friends League" };

            Shelter shelter2 = new Shelter { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = true, Rank = 2, City = "Houston", Name = "Houston Humane Society" };

            List<Shelter> shelters = new() { shelter1, shelter2 };

            AdoptiverseApiContext context = GetDbContext();
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();

            //Act

            HttpResponseMessage response = await client.GetAsync("/api/shelters/1");
            string content = await response.Content.ReadAsStringAsync();

            Shelter expectedShelter = shelters.First(b => b.Id == 1); // Get the shelter with ID 1 from the list
            string expectedContent = ObjectToJson(expectedShelter);

            //Assert
            response.EnsureSuccessStatusCode();

            Assert.Equal(expectedContent, content);

        }




        [Fact]
        public async void PostShelter_CreatesShelterInDb()
        {
            // Create fresh database
            AdoptiverseApiContext context = GetDbContext();

            // Set up and send the request
            HttpClient client = _factory.CreateClient();

            var jsonString = "{\"createdAt\": \"2023-08-29T12:00:00.000Z\", \"updatedAt\": \"2023-08-29T12:05:00.000Z\", \"fosterProgram\": true, \"rank\": 1, \"city\": \"San Francisco\", \"name\": \"Happy Paws\"}";

            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/shelters", requestContent);

            // Get the first (and should be only) shelter from the db
            var newShelter = context.Shelters.First();

            Assert.Equal("Created", response.StatusCode.ToString());
            Assert.Equal(201, (int)response.StatusCode);
            Assert.Equal("Happy Paws", newShelter.Name);
        }








        // This method helps us create an expected value. We can use the Newtonsoft JSON serializer to build the string that we expect.  Without this helper method, we would need to manually create the expected JSON string.

        private string ObjectToJson(object obj)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
               // NamingStrategy = new SnakeCaseNamingStrategy()
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            });

            return json;
        }






        private AdoptiverseApiContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AdoptiverseApiContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new AdoptiverseApiContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
    }
}