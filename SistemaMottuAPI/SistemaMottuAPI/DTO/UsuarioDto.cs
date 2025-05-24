using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SistemaMottuAPI.DTO;

public class UsuarioDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email é obrigatório")]
    [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(60, ErrorMessage = "Senha deve ter no máximo 60 caracteres")]
    public string Senha { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "ID do cargo é obrigatório")]
    public int IdCargo { get; set; }
    
    [StringLength(3, ErrorMessage = "Ativo deve ter no máximo 3 caracteres")]
    public string? Ativo { get; set; } = "Sim";
}