using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using eBarber.Models;

namespace eBarber.Data;

public class BarberContext : IdentityDbContext<ApplicationUser>
{
    public BarberContext(DbContextOptions<BarberContext> options)
        : base(options)
    {
    }

    public DbSet<Frizer> Frizerji { get; set; }
    public DbSet<Lokacija> Lokacije { get; set; }
    public DbSet<Termin> Termini { get; set; }
    public DbSet<Ocena> Ocene { get; set; }
    public DbSet<Sporocilo> Sporocila { get; set; }
    public DbSet<OkvirniTermin> OkvirniTermini { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<PrijavaTermina> PrijaveTerminov { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Frizer>()
            .HasOne(f => f.User)
            .WithOne(u => u.Frizer)
            .HasForeignKey<Frizer>(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Lokacija>()
            .HasOne(l => l.Frizer)
            .WithMany(f => f.Lokacije)
            .HasForeignKey(l => l.FrizerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Termin>()
            .HasOne(t => t.Stranka)
            .WithMany(u => u.Termini)
            .HasForeignKey(t => t.StrankaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Termin>()
            .HasOne(t => t.Lokacija)
            .WithMany(l => l.Termini)
            .HasForeignKey(t => t.LokacijaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ocena>()
            .HasOne(o => o.Termin)
            .WithOne(t => t.Ocena)
            .HasForeignKey<Ocena>(o => o.TerminId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ocena>()
            .HasOne(o => o.Stranka)
            .WithMany(u => u.Ocene)
            .HasForeignKey(o => o.StrankaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Sporocilo>()
            .HasOne(s => s.Posiljatelj)
            .WithMany(u => u.PosljanaSporocila)
            .HasForeignKey(s => s.PosiljateljId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Sporocilo>()
            .HasOne(s => s.Prejemnik)
            .WithMany(u => u.PrejemljenaSporocila)
            .HasForeignKey(s => s.PrejemnikId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<OkvirniTermin>()
            .HasOne(ot => ot.Lokacija)
            .WithMany()
            .HasForeignKey(ot => ot.LokacijaId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<PrijavaTermina>()
            .HasOne(pt => pt.OkvirniTermin)
            .WithMany(ot => ot.Prijave)
            .HasForeignKey(pt => pt.OkvirniTerminId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<PrijavaTermina>()
            .HasOne(pt => pt.Uporabnik)
            .WithMany()
            .HasForeignKey(pt => pt.UprabnikId)
            .OnDelete(DeleteBehavior.Restrict);
   
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}