using System.ComponentModel.DataAnnotations;

namespace eBarber.Models;

public class Sporocilo
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(2000)]
    public string Vsebina { get; set; } = string.Empty;
    
    public DateTime CasPosiljanja { get; set; } = DateTime.Now;
    
    public bool Prebrano { get; set; } = false;
    
    [Required]
    public string PosiljateljId { get; set; } = string.Empty;
    public ApplicationUser? Posiljatelj { get; set; }
    
    [Required]
    public string PrejemnikId { get; set; } = string.Empty;
    public ApplicationUser? Prejemnik { get; set; }
}