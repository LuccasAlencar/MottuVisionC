using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaMottuAPI.Model;

public class Camera
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdCamera { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Localizacao { get; set; } = string.Empty;
    
    [StringLength(20)]
    public required string Status { get; set; }
    
    public DateTime UltimaVerificacao { get; set; } = DateTime.Now;
    
    public virtual ICollection<Reconhecimento> Reconhecimentos { get; set; } = new List<Reconhecimento>();
}