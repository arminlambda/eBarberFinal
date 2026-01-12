using System.ComponentModel.DataAnnotations;

namespace eBarber.Models;

public class PrijavaTermina
{
    public int Id { get; set; }
    
    [Required]
    public int OkvirniTerminId { get; set; }
    public OkvirniTermin? OkvirniTermin { get; set; }
    
    [Required]
    public string UprabnikId { get; set; } = string.Empty;
    public ApplicationUser? Uporabnik { get; set; }
    
    public DateTime DatumPrijave { get; set; } = DateTime.Now;
    
    [StringLength(500)]
    public string? Opombe { get; set; }
    
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Potrjena"; 

    public bool JePrispel { get; set; } = false;
    public DateTime? CasOdjave { get; set; }
    public int? OcenaStranke { get; set; }  
}