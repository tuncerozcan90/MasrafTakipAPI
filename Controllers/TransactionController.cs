using MasrafTakipAPI.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransactionsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets a list of all transactions.
    /// </summary>
    /// <returns>A list of transactions.</returns>
    /// <response code="200">Returns the list of transactions.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
    {
        return await _context.Transactions.ToListAsync();
    }

    /// <summary>
    /// Gets a specific transaction by id.
    /// </summary>
    /// <param name="id">The id of the transaction.</param>
    /// <returns>The transaction with the specified id.</returns>
    /// <response code="200">Returns the transaction with the specified id.</response>
    /// <response code="404">If the transaction is not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetTransaction(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);

        if (transaction == null)
        {
            return NotFound();
        }

        return transaction;
    }

    /// <summary>
    /// Updates an existing transaction.
    /// </summary>
    /// <param name="id">The id of the transaction to update.</param>
    /// <param name="transaction">The updated transaction data.</param>
    /// <returns>No content if the update is successful.</returns>
    /// <response code="204">No content if the update is successful.</response>
    /// <response code="400">If the id in the URL does not match the id in the request body.</response>
    /// <response code="404">If the transaction with the specified id is not found.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
    {
        if (id != transaction.Id)
        {
            return BadRequest();
        }

        _context.Entry(transaction).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TransactionExists(id))
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
    /// Creates a new transaction.
    /// </summary>
    /// <param name="transaction">The transaction to create.</param>
    /// <returns>The created transaction.</returns>
    /// <response code="201">Returns the created transaction.</response>
    [HttpPost]
    public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
    }

    /// <summary>
    /// Deletes a specific transaction by id.
    /// </summary>
    /// <param name="id">The id of the transaction to delete.</param>
    /// <returns>No content if the deletion is successful.</returns>
    /// <response code="204">No content if the deletion is successful.</response>
    /// <response code="404">If the transaction with the specified id is not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TransactionExists(int id)
    {
        return _context.Transactions.Any(e => e.Id == id);
    }
}
