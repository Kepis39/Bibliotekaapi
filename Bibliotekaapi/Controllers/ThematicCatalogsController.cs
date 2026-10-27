using Bibliotekaapi.Models;
using Bibliotekaapi.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bibliotekaapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ThematicCatalogsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ThematicCatalogsController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ThematicCatalog>>> GetThematicCatalogs()
    {
        return await _context.ThematicCatalogs
            .Include(t => t.BookCatalogs)
                .ThenInclude(bc => bc.Book)
            .ToListAsync();
    }
    [HttpGet("{themeCode}")]
    public async Task<ActionResult<ThematicCatalog>> GetThematicCatalog(int themeCode)
    {
        var catalog = await _context.ThematicCatalogs
            .Include(t => t.BookCatalogs)
                .ThenInclude(bc => bc.Book)
            .FirstOrDefaultAsync(t => t.ThemeCode == themeCode);

        if (catalog == null) return NotFound("Тема не найдена");
        return catalog;
    }

    [HttpPost]
    public async Task<ActionResult<ThematicCatalog>> PostThematicCatalog(ThematicCatalog catalog)
    {
        _context.ThematicCatalogs.Add(catalog);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetThematicCatalog), new { themeCode = catalog.ThemeCode }, catalog);
    }

    [HttpPut("{themeCode}")]
    public async Task<IActionResult> PutThematicCatalog(int themeCode, ThematicCatalog catalog)
    {
        if (themeCode != catalog.ThemeCode) return BadRequest();

        _context.Entry(catalog).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{themeCode}")]
    public async Task<IActionResult> DeleteThematicCatalog(int themeCode)
    {
        var catalog = await _context.ThematicCatalogs.FindAsync(themeCode);
        if (catalog == null) return NotFound();

        _context.ThematicCatalogs.Remove(catalog);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
