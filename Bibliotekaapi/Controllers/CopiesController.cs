using Bibliotekaapi.Models;
using Bibliotekaapi.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bibliotekaapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CopiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CopiesController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Copy>>> GetCopies()
    {
        return await _context.Copies
            .Include(c => c.Book)
            .Include(c => c.ThemeCodeNavigation)
            .ToListAsync();
    }
    [HttpGet("{inventoryNumber}")]
    public async Task<ActionResult<Copy>> GetCopy(string inventoryNumber)
    {
        var copy = await _context.Copies
            .Include(c => c.Book)
            .Include(c => c.ThemeCodeNavigation)
            .FirstOrDefaultAsync(c => c.InventoryNumber == inventoryNumber);

        if (copy == null) return NotFound("Экземпляр не найден");
        return copy;
    }
    [HttpPost]
    public async Task<ActionResult<Copy>> PostCopy(Copy copy)
    {
        var bookExists = await _context.Books.AnyAsync(b => b.BookId == copy.BookId);
        if (!bookExists)
            return BadRequest("Ошибка: Каждый экземпляр должен соответствовать существующей книге (book_id).");

        _context.Copies.Add(copy);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCopy), new { inventoryNumber = copy.InventoryNumber }, copy);
    }
    [HttpPut("{inventoryNumber}")]
    public async Task<IActionResult> PutCopy(string inventoryNumber, Copy copy)
    {
        if (inventoryNumber != copy.InventoryNumber) return BadRequest();

        _context.Entry(copy).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{inventoryNumber}")]
    public async Task<IActionResult> DeleteCopy(string inventoryNumber)
    {
        var copy = await _context.Copies.FindAsync(inventoryNumber);
        if (copy == null) return NotFound();

        _context.Copies.Remove(copy);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}