using System.ComponentModel.DataAnnotations;

namespace SistemaMottuAPI.DTO;

public class CargoDto
{
    [Required(ErrorMessage = "Nome do cargo é obrigatório")]
    [StringLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nível de permissão é obrigatório")]
    [Range(1, 5, ErrorMessage = "Nível de permissão deve ser entre 1 e 5")]
    public int NivelPermissao { get; set; }

    [Required(ErrorMessage = "Permissões são obrigatórias")]
    public string Permissoes { get; set; } = string.Empty; 
}