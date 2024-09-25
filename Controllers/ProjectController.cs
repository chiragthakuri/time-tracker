using Elfie.Serialization;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TimeTracking.Models;

namespace TimeTracking.Conrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private TimeTrackingDbContext ctx;
        private readonly ILogger<ProjectController> _logger;
        public ProjectController(TimeTrackingDbContext context, ILogger<ProjectController> logger)
        {
            ctx = context;
            _logger = logger;
        }

        // GET api/<ProjectController>/
        [HttpGet]
        [ProducesResponseType<IEnumerable<Resources.Project>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
                var response = await ctx.Projects.ProjectToType<Resources.Project>().ToListAsync();
                return Ok(response);
        }

        // GET api/<ProjectController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Resources.Project), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
        try{    
            _logger.LogInformation("Fetching project with ID: {Id}", id);
            
            var projectEntity = await ctx.Projects.FindAsync(id);
            
            if (projectEntity == null)
            {
                _logger.LogWarning("Project with ID: {Id} not found.", id);
                return NotFound();
            }

            // Use your mapping logic here, assuming you have a method to map to Resources.Project
            var project = projectEntity.Adapt<Resources.Project>();

            return Ok(project);


            } catch (Exception ex){
                 _logger.LogError(ex, "An error occurred while fetching the project.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occured while fetching the projects!");
            }
        }

        // POST api/<ProjectController>
        [HttpPost]
        [ProducesResponseType(typeof(Resources.Project), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] Resources.Project value)
        {
            if(!ModelState.IsValid){
                return Problem("Invalid project request", statusCode: StatusCodes.Status400BadRequest);
            }

            try{

            var dbProject = value.Adapt<Models.Project>();

            await ctx.Projects.AddAsync(dbProject);

            await ctx.SaveChangesAsync();

            var response = dbProject.Adapt<Resources.Employee>();

            return CreatedAtAction(nameof(Get), new {id = response.Id}, response);

            } catch (Exception ex){
                _logger.LogError(ex, "Error persisting project resource");
                return Problem("problem persisting project resource", statusCode: StatusCodes.Status500InternalServerError);
            
            }

        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Resources.Project), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] Resources.Project value)
        {
            if(!ModelState.IsValid){
                return Problem("Invalid project request", statusCode: StatusCodes.Status400BadRequest);
            }
            
            try{

            var dbProject = value.Adapt<Models.Project>();
            
            ctx.Entry<Models.Project>(dbProject).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            await ctx.SaveChangesAsync();
           
            return NoContent();

            } catch (DbUpdateConcurrencyException dbex){
                var dbProject = ctx.Projects.Find(id);
                if(dbProject == null){
                return NotFound();
            } else {
                return Problem("Problem persiting employee resource", statusCode: StatusCodes.Status500InternalServerError);
            }

            }
            
            catch (Exception ex){
                _logger.LogError(ex, "Error persisting project resource");
                return Problem("problem persisting project resource", statusCode: StatusCodes.Status500InternalServerError);
            
            }

        }

        // PATCH api/<ProjectController>/5
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Resources.Project), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Resources.Project> value)
        {
            if(!ModelState.IsValid){
                return Problem("Invalid project request", statusCode: StatusCodes.Status400BadRequest);
            }
            
            try{

            var dbProject = await ctx.Projects.FindAsync(id);

            if(dbProject == null){
                return NotFound();
            }

            var project = dbProject.Adapt<Resources.Project>();

            value.ApplyTo(project, ModelState);

            var patchedProject = project.Adapt<Models.Employee>();

            ctx.Entry<Models.Project>(dbProject).CurrentValues.SetValues(patchedProject);

            await ctx.SaveChangesAsync();
           
            return NoContent();

            } catch (Exception ex){
                _logger.LogError(ex, "Error persisting project resource");
                return Problem("problem persisting project resource", statusCode: StatusCodes.Status500InternalServerError);
            
            }

        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Resources.Employee>(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try{

                var project = await ctx.Projects.FindAsync(id);

                if(project == null){
                    return NotFound();
                }

                ctx.Projects.Remove(project);
                await ctx.SaveChangesAsync();
                return NoContent();
                
            } catch (Exception ex) {
                return Problem("Error deleting the project", statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
