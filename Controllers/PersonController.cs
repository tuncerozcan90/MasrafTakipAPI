using MasrafTakipAPI.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PersonsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PersonsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets a list of all persons.
    /// </summary>
    /// <returns>A list of persons.</returns>
    /// <response code="200">Returns the list of persons.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
    {
        return await _context.Persons.Include(p => p.Transactions).ToListAsync();
    }

    /// <summary>
    /// Gets a specific person by id.
    /// </summary>
    /// <param name="id">The id of the person.</param>
    /// <returns>The person with the specified id.</returns>
    /// <response code="200">Returns the person with the specified id.</response>
    /// <response code="404">If the person is not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<Person>> GetPerson(int id)
    {
        var person = await _context.Persons.Include(p => p.Transactions).FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
        {
            return NotFound();
        }

        return person;
    }

    /// <summary>
    /// Creates a new person.
    /// </summary>
    /// <param name="person">The person to create.</param>
    /// <returns>The created person.</returns>
    /// <response code="201">Returns the created person.</response>
    [HttpPost]
    public async Task<ActionResult<Person>> PostPerson(Person person)
    {
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
    }

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    /// <param name="id">The id of the person to update.</param>
    /// <param name="person">The updated person data.</param>
    /// <returns>No content if the update is successful.</returns>
    /// <response code="204">No content if the update is successful.</response>
    /// <response code="400">If the id in the URL does not match the id in the request body.</response>
    /// <response code="404">If the person with the specified id is not found.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPerson(int id, Person person)
    {
        if (id != person.Id)
        {
            return BadRequest();
        }

        _context.Entry(person).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PersonExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific person by id.
    /// </summary>
    /// <param name="id">The id of the person to delete.</param>
    /// <returns>No content if the deletion is successful.</returns>
    /// <response code="204">No content if the deletion is successful.</response>
    /// <response code="404">If the person with the specified id is not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson(int id)
    {
        var person = await _context.Persons.FindAsync(id);
        if (person == null)
        {
            return NotFound();
        }

        _context.Persons.Remove(person);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Gets the total spending for a specific person.
    /// </summary>
    /// <param name="id">The id of the person.</param>
    /// <returns>The total spending of the person.</returns>
    /// <response code="200">Returns the total spending of the person.</response>
    /// <response code="404">If the person with the specified id is not found.</response>
    [HttpGet("{id}/totalspending")]
    public async Task<ActionResult<decimal>> GetTotalSpending(int id)
    {
        var totalSpending = await _context.Transactions
            .Where(t => t.PersonId == id)
            .SumAsync(t => t.Amount);

        return totalSpending;
    }

    private bool PersonExists(int id)
    {
        return _context.Persons.Any(e => e.Id == id);
    }
}
