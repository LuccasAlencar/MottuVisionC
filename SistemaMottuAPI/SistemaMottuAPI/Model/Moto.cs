using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaMottuAPI.Model;

public class Moto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdMoto { get; set; }
    
    [Required]
    [StringLength(7)]
    public string Placa { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Marca { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Modelo { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Cor { get; set; } = string.Empty;
    
    [StringLength(3)]
    public string Presente { get; set; } = "Não";
    
    [StringLength(255)]
    public string? ImagemReferencia { get; set; }
    
    public virtual ICollection<LogAlteracao> LogsAlteracoes { get; set; } = new List<LogAlteracao>();
    
    public virtual ICollection<Reconhecimento> Reconhecimentos { get; set; } = new List<Reconhecimento>();
    
    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();
}