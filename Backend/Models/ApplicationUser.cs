using Microsoft.AspNetCore.Identity;

namespace eBarber.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? City { get; set; }
    public DateTime? DateCreated { get; set; }
    
    public Frizer? Frizer { get; set; }
    public ICollection<Termin>? Termini { get; set; }
    public ICollection<Ocena>? Ocene { get; set; }
    public ICollection<Sporocilo>? PosljanaSporocila { get; set; }
    public ICollection<Sporocilo>? PrejemljenaSporocila { get; set; }

    public int TotalBookings { get; set; } = 0;
    public int CompletedBookings { get; set; } = 0;
    public int CancelledBookings { get; set; } = 0;
    public decimal AverageRating { get; set; } = 0;
    
    public decimal ReliabilityScore => 
        TotalBookings > 0 ? (decimal)CompletedBookings / TotalBookings : 0;
}