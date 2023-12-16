using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly DataContext _dc;

        public CityController(DataContext dc)
        {
            _dc = dc;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var cities = _dc.Cities.ToList();
            return Ok(cities);
        }
    }
}
