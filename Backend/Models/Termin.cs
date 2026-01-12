using System.ComponentModel.DataAnnotations;

namespace eBarber.Models;

public class Termin
{
    public int Id { get; set; }
    
    [Required]
    public DateTime DatumInUra { get; set; }
    
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Čaka";
    
    [StringLength(500)]
    public string? Opombe { get; set; }
    
    public bool JeOkvirniTermin { get; set; } = false;
    
    public int? MaksimalnoUporabnikov { get; set; }
    
    [Required]
    public string StrankaId { get; set; } = string.Empty;
    public ApplicationUser? Stranka { get; set; }
    
    [Required]
    public int LokacijaId { get; set; }
    public Lokacija? Lokacija { get; set; }
    
    public Ocena? Ocena { get; set; }
}