using System.ComponentModel.DataAnnotations;

namespace SistemaMottuAPI.DTO
{
    public class LogAlteracaoCreateDto
    {
        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "O ID da moto é obrigatório.")]
        public int IdMoto { get; set; }

        [Required(ErrorMessage = "O tipo de ação é obrigatório.")]
        [StringLength(10, ErrorMessage = "O tipo de ação não pode exceder 10 caracteres.")]
        public string TipoAcao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo alterado é obrigatório.")]
        [StringLength(50, ErrorMessage = "O campo alterado não pode exceder 50 caracteres.")]
        public string CampoAlterado { get; set; } = string.Empty;

        public string? ValorAntigo { get; set; }
        public string? ValorNovo { get; set; }
    }

    public class LogAlteracaoUpdateDto
    {
        public int? IdUsuario { get; set; }
        public int? IdMoto { get; set; }

        [StringLength(10, ErrorMessage = "O tipo de ação não pode exceder 10 caracteres.")]
        public string? TipoAcao { get; set; }

        [StringLength(50, ErrorMessage = "O campo alterado não pode exceder 50 caracteres.")]
        public string? CampoAlterado { get; set; }

        public string? ValorAntigo { get; set; }
        public string? ValorNovo { get; set; }
    }

    public class LogAlteracaoResponseDto
    {
        public int IdLog { get; set; }
        public int IdUsuario { get; set; }
        public int IdMoto { get; set; }
        public DateTime DataHora { get; set; }
        public string TipoAcao { get; set; } = string.Empty;
        public string CampoAlterado { get; set; } = string.Empty;
        public string? ValorAntigo { get; set; }
        public string? ValorNovo { get; set; }
    }
}