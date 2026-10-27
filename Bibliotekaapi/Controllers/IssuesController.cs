using Bibliotekaapi.Models;
using Bibliotekaapi.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bibliotekaapi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IssuesController : ControllerBase
{
    private readonly AppDbContext _context;

    public IssuesController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Issue>>> GetIssues()
    {
        return await _context.Issues
            .Include(i => i.TicketNumberNavigation)
            .Include(i => i.InventoryNumberNavigation)
            .ToListAsync();
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Issue>> GetIssue(int id)
    {
        var issue = await _context.Issues
            .Include(i => i.TicketNumberNavigation)
            .Include(i => i.InventoryNumberNavigation)
            .FirstOrDefaultAsync(i => i.IssueId == id);

        if (issue == null) return NotFound();
        return issue;
    }
    [HttpPost("issue-book")]
    public async Task<IActionResult> IssueBook(int issueId, int ticketNumber, string inventoryNumber, int days = 14)
    {
        var readerExists = await _context.Readers.AnyAsync(r => r.TicketNumber == ticketNumber);
        var copyExists = await _context.Copies.AnyAsync(c => c.InventoryNumber == inventoryNumber);

        if (!readerExists || !copyExists)
            return BadRequest("Читатель или экземпляр книги не найдены в базе.");

        var today = DateOnly.FromDateTime(DateTime.Today);
        var issue = new Issue
        {
            IssueId = issueId,
            TicketNumber = ticketNumber,
            InventoryNumber = inventoryNumber,
            IssueDate = today,
            ReturnDate = today.AddDays(days),
            ActualReturn = null
        };

        _context.Issues.Add(issue);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Книга выдана читателю", issue });
    }
    [HttpPut("return-book/{id}")]
    public async Task<IActionResult> ReturnBook(int id)
    {
        var issue = await _context.Issues.FindAsync(id);
        if (issue == null) return NotFound("Запись о выдаче не найдена.");

        issue.ActualReturn = DateOnly.FromDateTime(DateTime.Today);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Книга принята от читателя", issue });
    }
    [HttpPost]
    public async Task<ActionResult<Issue>> PostIssue(Issue issue)
    {
        _context.Issues.Add(issue);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetIssue), new { id = issue.IssueId }, issue);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> PutIssue(int id, Issue issue)
    {
        if (id != issue.IssueId) return BadRequest();

        _context.Entry(issue).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIssue(int id)
    {
        var issue = await _context.Issues.FindAsync(id);
        if (issue == null) return NotFound();

        _context.Issues.Remove(issue);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}