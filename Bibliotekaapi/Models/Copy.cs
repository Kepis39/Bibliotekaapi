using System;
using System.Collections.Generic;

namespace Bibliotekaapi.Models;

public partial class Copy
{
    public string InventoryNumber { get; set; } = null!;

    public string BookId { get; set; } = null!;

    public int? ThemeCode { get; set; }

    public int? CopyCount { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();

    public virtual ThematicCatalog? ThemeCodeNavigation { get; set; }
}
