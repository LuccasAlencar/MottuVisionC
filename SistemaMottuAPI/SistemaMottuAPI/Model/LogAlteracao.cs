using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaMottuAPI.Model;

public class LogAlteracao
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdLog { get; set; }
    
    [Required]
    public int IdUsuario { get; set; }
    
    [Required]
    public int IdMoto { get; set; }
    
    public DateTime DataHora { get; set; } = DateTime.Now;
    
    [Required]
    [StringLength(10)]
    public string TipoAcao { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string CampoAlterado { get; set; } = string.Empty;
    
    public string? ValorAntigo { get; set; }
    
    public string? ValorNovo { get; set; }
    
    [ForeignKey("IdUsuario")]
    public virtual Usuario? Usuario { get; set; }
    
    [ForeignKey("IdMoto")]
    public virtual Moto? Moto { get; set; }
}