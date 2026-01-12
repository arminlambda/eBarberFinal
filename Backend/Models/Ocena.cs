using System.ComponentModel.DataAnnotations;

namespace eBarber.Models;

public class Ocena
{
    public int Id { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int Stevilka { get; set; }
    
    [StringLength(1000)]
    public string? Komentar { get; set; }
    
    public DateTime DatumOcene { get; set; } = DateTime.Now;
    
    [Required]
    public int TerminId { get; set; }
    public Termin? Termin { get; set; }
    
    [Required]
    public string StrankaId { get; set; } = string.Empty;
    public ApplicationUser? Stranka { get; set; }
}