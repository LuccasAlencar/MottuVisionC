using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaMottuAPI.Model;

public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdUsuario { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(60)]
    public string Senha { get; set; } = string.Empty;
    
    [Required]
    public int IdCargo { get; set; }
    
    [StringLength(3)]
    public string Ativo { get; set; } = "Sim";
    
    [ForeignKey("IdCargo")]
    public virtual Cargo? Cargo { get; set; }
    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();
    public virtual ICollection<LogAlteracao> LogsAlteracoes { get; set; } = new List<LogAlteracao>();
}