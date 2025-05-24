using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SistemaMottuAPI.DTO;

public class MotoDto
{
    [Required(ErrorMessage = "Placa é obrigatória")]
    [StringLength(7, ErrorMessage = "Placa deve ter exatamente 7 caracteres")]
    public string Placa { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Marca é obrigatória")]
    [StringLength(50, ErrorMessage = "Marca deve ter no máximo 50 caracteres")]
    public string Marca { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Modelo é obrigatório")]
    [StringLength(50, ErrorMessage = "Modelo deve ter no máximo 50 caracteres")]
    public string Modelo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Cor é obrigatória")]
    [StringLength(20, ErrorMessage = "Cor deve ter no máximo 20 caracteres")]
    public string Cor { get; set; } = string.Empty;
    
    [StringLength(3, ErrorMessage = "Presente deve ter no máximo 3 caracteres")]
    public string Presente { get; set; } = "Não";
    
    [StringLength(255, ErrorMessage = "Imagem de referência deve ter no máximo 255 caracteres")]
    public string? ImagemReferencia { get; set; }
}