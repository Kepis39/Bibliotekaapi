using System;
using System.Collections.Generic;

namespace Bibliotekaapi.Models;

public partial class Issue
{
    public int IssueId { get; set; }

    public int TicketNumber { get; set; }

    public string InventoryNumber { get; set; } = null!;

    public DateOnly IssueDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public DateOnly? ActualReturn { get; set; }

    public virtual Copy InventoryNumberNavigation { get; set; } = null!;

    public virtual Reader TicketNumberNavigation { get; set; } = null!;
}
