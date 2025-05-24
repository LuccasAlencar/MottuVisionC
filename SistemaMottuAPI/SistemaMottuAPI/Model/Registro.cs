using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaMottuAPI.Model;

public class Registro
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdRegistro { get; set; }
    
    [Required]
    public int IdMoto { get; set; }
    
    [Required]
    public int IdUsuario { get; set; }
    
    public int? IdReconhecimento { get; set; }
    
    public DateTime DataHora { get; set; } = DateTime.Now;
    
    [Required]
    [StringLength(10)]
    public string Tipo { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    public string ModoRegistro { get; set; } = string.Empty;
    
    [ForeignKey("IdMoto")]
    public virtual Moto? Moto { get; set; }
    
    [ForeignKey("IdUsuario")]
    public virtual Usuario? Usuario { get; set; }
    
    [ForeignKey("IdReconhecimento")]
    public virtual Reconhecimento? Reconhecimento { get; set; }
}