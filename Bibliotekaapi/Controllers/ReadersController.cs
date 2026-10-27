using Bibliotekaapi.Models;
using Bibliotekaapi.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bibliotekaapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReadersController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReadersController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reader>>> GetReaders()
    {
        return await _context.Readers
            .Include(r => r.Issues)
            .ToListAsync();
    }
    [HttpGet("{ticketNumber}")]
    public async Task<ActionResult<Reader>> GetReader(int ticketNumber)
    {
        var reader = await _context.Readers
            .Include(r => r.Issues)
            .FirstOrDefaultAsync(r => r.TicketNumber == ticketNumber);

        if (reader == null) return NotFound("Читатель не найден");
        return reader;
    }
    [HttpGet("debtors")]
    public async Task<IActionResult> GetDebtors()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var debtors = await _context.Issues
            .Include(i => i.TicketNumberNavigation)
            .Include(i => i.InventoryNumberNavigation)
                .ThenInclude(c => c.Book)
            .Where(i =>
                (i.ActualReturn == null && i.ReturnDate < today) ||
                (i.ActualReturn != null && i.ActualReturn > i.ReturnDate))
            .Select(i => new
            {
                TicketNumber = i.TicketNumber,
                FullName = i.TicketNumberNavigation.FullName,
                Phone = i.TicketNumberNavigation.Phone,
                BookTitle = i.InventoryNumberNavigation.Book.Title,
                InventoryNumber = i.InventoryNumber,
                IssueDate = i.IssueDate,
                ReturnDate = i.ReturnDate,
                ActualReturn = i.ActualReturn
            })
            .ToListAsync();

        return Ok(debtors);
    }
    [HttpPost]
    public async Task<ActionResult<Reader>> PostReader(Reader reader)
    {
        _context.Readers.Add(reader);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetReader), new { ticketNumber = reader.TicketNumber }, reader);
    }
    [HttpPut("{ticketNumber}")]
    public async Task<IActionResult> PutReader(int ticketNumber, Reader reader)
    {
        if (ticketNumber != reader.TicketNumber) return BadRequest();

        _context.Entry(reader).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{ticketNumber}")]
    public async Task<IActionResult> DeleteReader(int ticketNumber)
    {
        var reader = await _context.Readers.FindAsync(ticketNumber);
        if (reader == null) return NotFound();

        _context.Readers.Remove(reader);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
