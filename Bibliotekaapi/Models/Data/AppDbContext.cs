using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Bibliotekaapi.Models.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookCatalog> BookCatalogs { get; set; }

    public virtual DbSet<Copy> Copies { get; set; }

    public virtual DbSet<Issue> Issues { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    public virtual DbSet<ThematicCatalog> ThematicCatalogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=app_db;Username=app;Password=app_password");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("books_pkey");

            entity.ToTable("books");

            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.FirstAuthor).HasColumnName("first_author");
            entity.Property(e => e.PageCount).HasColumnName("page_count");
            entity.Property(e => e.PlaceOfPublication).HasColumnName("place_of_publication");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Publisher).HasColumnName("publisher");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.YearOfPublication).HasColumnName("year_of_publication");
        });

        modelBuilder.Entity<BookCatalog>(entity =>
        {
            entity.HasKey(e => e.CatalogId).HasName("book_catalog_pkey");

            entity.ToTable("book_catalog");

            entity.Property(e => e.CatalogId)
                .ValueGeneratedNever()
                .HasColumnName("catalog_id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.ThemeCode).HasColumnName("theme_code");

            entity.HasOne(d => d.Book).WithMany(p => p.BookCatalogs)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("fk_catalog_book");

            entity.HasOne(d => d.ThemeCodeNavigation).WithMany(p => p.BookCatalogs)
                .HasForeignKey(d => d.ThemeCode)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_catalog_theme");
        });

        modelBuilder.Entity<Copy>(entity =>
        {
            entity.HasKey(e => e.InventoryNumber).HasName("copies_pkey");

            entity.ToTable("copies");

            entity.Property(e => e.InventoryNumber).HasColumnName("inventory_number");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.CopyCount).HasColumnName("copy_count");
            entity.Property(e => e.ThemeCode).HasColumnName("theme_code");

            entity.HasOne(d => d.Book).WithMany(p => p.Copies)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("fk_copies_book");

            entity.HasOne(d => d.ThemeCodeNavigation).WithMany(p => p.Copies)
                .HasForeignKey(d => d.ThemeCode)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_copies_theme");
        });

        modelBuilder.Entity<Issue>(entity =>
        {
            entity.HasKey(e => e.IssueId).HasName("issue_pkey");

            entity.ToTable("issue");

            entity.Property(e => e.IssueId)
                .ValueGeneratedNever()
                .HasColumnName("issue_id");
            entity.Property(e => e.ActualReturn).HasColumnName("actual_return");
            entity.Property(e => e.InventoryNumber).HasColumnName("inventory_number");
            entity.Property(e => e.IssueDate).HasColumnName("issue_date");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");
            entity.Property(e => e.TicketNumber).HasColumnName("ticket_number");

            entity.HasOne(d => d.InventoryNumberNavigation).WithMany(p => p.Issues)
                .HasForeignKey(d => d.InventoryNumber)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_issue_copy");

            entity.HasOne(d => d.TicketNumberNavigation).WithMany(p => p.Issues)
                .HasForeignKey(d => d.TicketNumber)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_issue_reader");
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.HasKey(e => e.TicketNumber).HasName("readers_pkey");

            entity.ToTable("readers");

            entity.Property(e => e.TicketNumber)
                .ValueGeneratedNever()
                .HasColumnName("ticket_number");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.Phone).HasColumnName("phone");
        });

        modelBuilder.Entity<ThematicCatalog>(entity =>
        {
            entity.HasKey(e => e.ThemeCode).HasName("thematic_catalogs_pkey");

            entity.ToTable("thematic_catalogs");

            entity.Property(e => e.ThemeCode)
                .ValueGeneratedNever()
                .HasColumnName("theme_code");
            entity.Property(e => e.ThemeName).HasColumnName("theme_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
