using Elfie.Serialization;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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
            if(!ModelState.IsValid)
            {
                return Problem("Invalid employee request", statusCode: StatusCodes.Status400BadRequest);
            }
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
        [ProducesResponseType<Resources.Employee>(StatusCodes.Status201Created)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] Resources.Employee value){
            if(!ModelState.IsValid)
                {
                    return Problem("Invalid employee request", statusCode: StatusCodes.Status400BadRequest);
                }

            try{
            
            var dbEmployee = value.Adapt<Models.Employee>();

            await ctx.Employees.AddAsync(dbEmployee);

            await ctx.SaveChangesAsync();

            var response = dbEmployee.Adapt<Resources.Employee>();

            return CreatedAtAction(nameof(Get), new {id = response.Id}, response);

            }  catch(Exception ex) {
                    return Problem("Problem persisting employee resource", statusCode: StatusCodes.Status500InternalServerError);
            
            }
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        [ProducesResponseType<Resources.Employee>(StatusCodes.Status201Created)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromBody] Resources.Employee value, int id)
        {
            if(!ModelState.IsValid)
                {
                    return Problem("Invalid employee request", statusCode: StatusCodes.Status400BadRequest);
                }

            try{
            
                var dbEmployee = value.Adapt<Models.Employee>();

                ctx.Entry<Models.Employee>(dbEmployee).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                await ctx.SaveChangesAsync();

                return NoContent();

            }  catch(DbUpdateConcurrencyException dbex) {
                var dbEmployee = ctx.Employees.Find(id);
                if(dbEmployee==null){
                    return NotFound();
                } else {
                    return Problem("Problem persisting employee resource", statusCode: StatusCodes.Status500InternalServerError);
                }
            }  catch(Exception ex) {
                    return Problem("Problem persisting employee resource", statusCode: StatusCodes.Status500InternalServerError);
            
            }
        }

        // PATCH api/<EmployeeController>/5
        [HttpPatch("{id}")]
        [ProducesResponseType<Resources.Employee>(StatusCodes.Status201Created)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch([FromBody] JsonPatchDocument<Resources.Employee> value, int id)
        {
            if(!ModelState.IsValid)
                {
                    return Problem("Invalid employee request", statusCode: StatusCodes.Status400BadRequest);
                }

            try{
            
                var dbEmployee = await ctx.Employees.FindAsync(id);

                 if(dbEmployee==null){
                    return NotFound();
                } 

                var employee = dbEmployee.Adapt<Resources.Employee>();

                value.ApplyTo(employee, ModelState);

                var patchedEmployee = employee.Adapt<Models.Employee>();

                ctx.Entry<Models.Employee>(dbEmployee).CurrentValues.SetValues(patchedEmployee); 

                
                await ctx.SaveChangesAsync();

                return NoContent();

            }  catch(DbUpdateConcurrencyException dbex) {
                var dbEmployee = ctx.Employees.Find(id);
                if(dbEmployee==null){
                    return NotFound();
                } else {
                return Problem("Problem persisting employee resource", statusCode: StatusCodes.Status500InternalServerError);
                }
            }  catch(Exception ex) {
                    return Problem("Problem persisting employee resource", statusCode: StatusCodes.Status500InternalServerError);
            
            }
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType<Resources.Employee>(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {

            try{
            
                var dbEmployee = await ctx.Employees.FindAsync(id);

                if(dbEmployee==null){
                    return NotFound();
                }

                ctx.Employees.Remove(dbEmployee);
                await ctx.SaveChangesAsync();
                return NoContent();
                
            } catch(Exception Ex){
                return Problem("Problem deleting employee resource", statusCode: StatusCodes.Status500InternalServerError);
            
            }

        }
    }
}
