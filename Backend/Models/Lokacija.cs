using System.ComponentModel.DataAnnotations;

namespace eBarber.Models;

public class Lokacija
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Naslov { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? Mesto { get; set; }
    
    public TimeSpan? DelovniCasOd { get; set; }
    public TimeSpan? DelovniCasDo { get; set; }
    
    public int? FrizerId { get; set; }
    public Frizer? Frizer { get; set; }
    
    public ICollection<Termin>? Termini { get; set; }
}