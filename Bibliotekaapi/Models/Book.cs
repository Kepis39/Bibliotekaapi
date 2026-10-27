using System;
using System.Collections.Generic;

namespace Bibliotekaapi.Models;

public partial class Book
{
    public string BookId { get; set; } = null!;

    public string? Title { get; set; }

    public string? FirstAuthor { get; set; }

    public string? Publisher { get; set; }

    public string? PlaceOfPublication { get; set; }

    public int? YearOfPublication { get; set; }

    public int? PageCount { get; set; }

    public float? Price { get; set; }

    public virtual ICollection<BookCatalog> BookCatalogs { get; set; } = new List<BookCatalog>();

    public virtual ICollection<Copy> Copies { get; set; } = new List<Copy>();
}
