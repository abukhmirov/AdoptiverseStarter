using AdoptiverseAPI.DataAccess;
using AdoptiverseAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdoptiverseAPI.Controllers
{
    // This will be the default route - controller gets replaced with the name of the controller.
    // "/api/shelters"
    [Route("/api/[controller]")]
    [ApiController]
    public class SheltersController : ControllerBase
    {
        private AdoptiverseApiContext _context;

        public SheltersController(AdoptiverseApiContext context)
        {
            _context = context;
        }





        // GET "/api/shelters"
        [HttpGet]
        public ActionResult<IEnumerable<Shelter>> GetShelters()
        {
            return _context.Shelters;

            // We could manually set the response code like this:
            //Response.StatusCode = 200;

            // API endpoints should return JSON, we are creating a new JSON result with our list of books.

        }



        // GET "/api/shelters/:shelterId"
        [HttpGet("{shelterId}")]
        public ActionResult<Shelter> GetShelter(int shelterId)
        {
            var shelter = _context.Shelters.Find(shelterId);

            if (shelter is null)
            {
                return NotFound();
            }
            Response.StatusCode = 200;
            return shelter;
            
            
        }



        //Create
        [HttpPost]
        public ActionResult CreateShelter(Shelter shelter)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return NotFound();
            }
            _context.Shelters.Add(shelter);
            _context.SaveChanges();

            //var savedShelter = _context.Shelters.First();

            Response.StatusCode = 201;
            return new JsonResult(shelter);
        }

        [HttpPut("{shelterId}")]
        public ActionResult UpdateShelter(int shelterId, Shelter shelter)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest();
            }

            Shelter existingShelter = _context.Shelters.Find(shelterId);

            if (existingShelter == null)
            {
                return NotFound();
            }

            existingShelter.Name = shelter.Name;
            existingShelter.Rank = shelter.Rank;
            existingShelter.City = shelter.City;
            existingShelter.FosterProgram = shelter.FosterProgram;


            _context.SaveChanges();


            //Response.StatusCode = 204;
            return new JsonResult(existingShelter);
        }
    }
}