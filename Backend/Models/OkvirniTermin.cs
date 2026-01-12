using System.ComponentModel.DataAnnotations;

namespace eBarber.Models;

public class OkvirniTermin
{
    public int Id { get; set; }
    
    [Required]
    public DateTime ZacetekCasa { get; set; }
    
    [Required]
    public DateTime KonecCasa { get; set; }
    
    [Required]
    public int LokacijaId { get; set; }
    public Lokacija? Lokacija { get; set; }
    
    [Required]
    [Range(1, 50)]
    public int MaksimalnoUporabnikov { get; set; } = 10;
    
    [StringLength(500)]
    public string? Opis { get; set; }
    
    public bool JeAktiven { get; set; } = true;
    
    public ICollection<PrijavaTermina>? Prijave { get; set; }
}