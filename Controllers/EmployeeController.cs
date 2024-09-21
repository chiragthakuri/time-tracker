using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;

namespace TimeTracking.Conrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private TimeTrackingDbContext ctx;

        public EmployeeController(TimeTrackingDbContext context)
        {
            ctx= context;
        }
        // GET: api/<EmployeeController>
        [HttpGet]
        [ProducesResponseType<IEnumerable<Resources.Employee>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var response = await ctx.Employees.ProjectToType<Employee>().ToListAsync();
            return Ok(response);
        }

        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        [ProducesResponseType<Resources.Employee>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var dbEmployee = await ctx.Employees.FindAsync(id);

              if (dbEmployee == null)
                {
                    return NotFound(); // Return 404 if employee not found
                }

            var response = dbEmployee.Adapt<Resources.Employee>();

            return Ok(response);
        }

        // POST api/<EmployeeController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
