using Bibliotekaapi.Models;
using Bibliotekaapi.Models.Data;
using Bibliotekaapi.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bibliotekaapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;

    public BooksController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        return await _context.Books
            .Include(b => b.Copies)
            .Include(b => b.BookCatalogs)
            .ToListAsync();
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(string id)
    {
        var book = await _context.Books
            .Include(b => b.Copies)
            .Include(b => b.BookCatalogs)
            .FirstOrDefaultAsync(b => b.BookId == id);

        if (book == null) return NotFound("Книга не найдена");
        return book;
    }
    [HttpGet("popular")]
    public async Task<IActionResult> GetPopularBooks([FromQuery] int top = 5)
    {
        var popular = await _context.Issues
            .Include(i => i.InventoryNumberNavigation)
                .ThenInclude(c => c.Book)
            .GroupBy(i => new
            {
                i.InventoryNumberNavigation.BookId,
                i.InventoryNumberNavigation.Book.Title,
                i.InventoryNumberNavigation.Book.FirstAuthor
            })
            .Select(g => new
            {
                BookId = g.Key.BookId,
                Title = g.Key.Title,
                FirstAuthor = g.Key.FirstAuthor,
                BorrowCount = g.Count()
            })
            .OrderByDescending(x => x.BorrowCount)
            .Take(top)
            .ToListAsync();

        return Ok(popular);
    }

    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, book);
    }
    [HttpPost("create-with-details")]
    public async Task<IActionResult> CreateBookWithMandatoryRelations(
        [FromBody] Book book,
        [FromQuery] string inventoryNumber,
        [FromQuery] int copyCount,
        [FromQuery] int themeCode,
        [FromQuery] int catalogId)
    {
        var themeExists = await _context.ThematicCatalogs.AnyAsync(t => t.ThemeCode == themeCode);
        if (!themeExists)
            return BadRequest("Указанный код темы (theme_code) не существует в тематическом каталоге.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Books.Add(book);

            var copy = new Copy
            {
                InventoryNumber = inventoryNumber,
                BookId = book.BookId,
                ThemeCode = themeCode,
                CopyCount = copyCount
            };
            _context.Copies.Add(copy);

            var bookCatalog = new BookCatalog
            {
                CatalogId = catalogId,
                BookId = book.BookId,
                ThemeCode = themeCode
            };
            _context.BookCatalogs.Add(bookCatalog);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = "Книга, обязательный экземпляр и привязка к каталогу успешно созданы", book });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest($"Ошибка при сохранении: {ex.Message}");
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(string id, Book book)
    {
        if (id != book.BookId) return BadRequest("Шифр книги не совпадает");

        _context.Entry(book).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(string id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
