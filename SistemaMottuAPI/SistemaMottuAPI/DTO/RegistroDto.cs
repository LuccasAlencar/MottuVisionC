using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SistemaMottuAPI.DTO;

public class RegistroDto
{
    [Required(ErrorMessage = "ID da moto é obrigatório")]
    public int IdMoto { get; set; }
    
    [Required(ErrorMessage = "ID do usuário é obrigatório")]
    public int IdUsuario { get; set; }
    
    public int? IdReconhecimento { get; set; }
    
    [Required(ErrorMessage = "Tipo é obrigatório")]
    [StringLength(10, ErrorMessage = "Tipo deve ter no máximo 10 caracteres")]
    public string Tipo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Modo de registro é obrigatório")]
    [StringLength(10, ErrorMessage = "Modo de registro deve ter no máximo 10 caracteres")]
    public string ModoRegistro { get; set; } = string.Empty;
}