using System;
using System.Collections.Generic;

namespace Bibliotekaapi.Models;

public partial class ThematicCatalog
{
    public int ThemeCode { get; set; }

    public string? ThemeName { get; set; }

    public virtual ICollection<BookCatalog> BookCatalogs { get; set; } = new List<BookCatalog>();

    public virtual ICollection<Copy> Copies { get; set; } = new List<Copy>();
}
