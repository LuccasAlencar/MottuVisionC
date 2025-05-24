using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaMottuAPI.Model;

public class Reconhecimento
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdReconhecimento { get; set; }
    
    [Required]
    public int IdMoto { get; set; }
    
    [Required]
    public int IdCamera { get; set; }
    
    public DateTime DataHora { get; set; } = DateTime.Now;
    
    [Required]
    [Range(0, 1)]
    public decimal Precisao { get; set; }
    
    [Required]
    [StringLength(255)]
    public string ImagemCapturada { get; set; } = string.Empty;
    
    [Required]
    [Range(0, 1)]
    public decimal ConfiancaMinima { get; set; }
    
    [ForeignKey("IdMoto")]
    public virtual Moto? Moto { get; set; }
    
    [ForeignKey("IdCamera")]
    public virtual Camera? Camera { get; set; }
    
    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();
}