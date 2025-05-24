using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaMottuAPI.Model;

public class Cargo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdCargo { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 5)]
    public int NivelPermissao { get; set; }
    
    [Required]
    [Column(TypeName = "CLOB")] 
    public string Permissoes { get; set; } = string.Empty;
    
    public DateTime DataCadastro { get; set; } = DateTime.Now; 

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

}
