using System.ComponentModel.DataAnnotations;

namespace eBarber.Models;

public class Frizer
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Ime { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Priimek { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? Email { get; set; }
    
    [StringLength(20)]
    public string? Telefon { get; set; }
    
    [Range(0, 5)]
    public double PovprecnaOcena { get; set; } = 0;
    
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    
    public ICollection<Lokacija>? Lokacije { get; set; }
}