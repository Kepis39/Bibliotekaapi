using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bibliotekaapi.Models;

public partial class Reader
{
    public int TicketNumber { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Phone { get; set; }

    [JsonIgnore]
    public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
}
