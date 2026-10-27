using Bibliotekaapi.Models;
using Bibliotekaapi.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bibliotekaapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookCatalogsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookCatalogsController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookCatalog>>> GetBookCatalogs()
    {
        return await _context.BookCatalogs
            .Include(bc => bc.Book)
            .Include(bc => bc.ThemeCodeNavigation)
            .ToListAsync();
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<BookCatalog>> GetBookCatalog(int id)
    {
        var bookCatalog = await _context.BookCatalogs
            .Include(bc => bc.Book)
            .Include(bc => bc.ThemeCodeNavigation)
            .FirstOrDefaultAsync(bc => bc.CatalogId == id);

        if (bookCatalog == null) return NotFound();
        return bookCatalog;
    }
    [HttpPost]
    public async Task<ActionResult<BookCatalog>> PostBookCatalog(BookCatalog bookCatalog)
    {
        var bookExists = await _context.Books.AnyAsync(b => b.BookId == bookCatalog.BookId);
        var themeExists = await _context.ThematicCatalogs.AnyAsync(t => t.ThemeCode == bookCatalog.ThemeCode);

        if (!bookExists || !themeExists)
            return BadRequest("Указанная книга или тематический каталог не существуют.");

        _context.BookCatalogs.Add(bookCatalog);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBookCatalog), new { id = bookCatalog.CatalogId }, bookCatalog);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBookCatalog(int id, BookCatalog bookCatalog)
    {
        if (id != bookCatalog.CatalogId) return BadRequest();

        _context.Entry(bookCatalog).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBookCatalog(int id)
    {
        var bookCatalog = await _context.BookCatalogs.FindAsync(id);
        if (bookCatalog == null) return NotFound();

        _context.BookCatalogs.Remove(bookCatalog);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}