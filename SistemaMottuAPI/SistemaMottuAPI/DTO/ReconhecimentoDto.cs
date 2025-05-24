using System.ComponentModel.DataAnnotations;

namespace SistemaMottuAPI.DTO
{
    public class ReconhecimentoCreateDto
    {
        [Required(ErrorMessage = "O ID da moto é obrigatório.")]
        public int IdMoto { get; set; }

        [Required(ErrorMessage = "O ID da câmera é obrigatório.")]
        public int IdCamera { get; set; }

        [Required(ErrorMessage = "A precisão é obrigatória.")]
        [Range(0, 1, ErrorMessage = "A precisão deve estar entre 0 e 1.")]
        public decimal Precisao { get; set; }

        [Required(ErrorMessage = "A imagem capturada é obrigatória.")]
        [StringLength(255, ErrorMessage = "O caminho da imagem não pode exceder 255 caracteres.")]
        public string ImagemCapturada { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confiança mínima é obrigatória.")]
        [Range(0, 1, ErrorMessage = "A confiança mínima deve estar entre 0 e 1.")]
        public decimal ConfiancaMinima { get; set; }
    }

    public class ReconhecimentoUpdateDto
    {
        public int? IdMoto { get; set; }
        public int? IdCamera { get; set; }

        [Range(0, 1, ErrorMessage = "A precisão deve estar entre 0 e 1.")]
        public decimal? Precisao { get; set; }

        [StringLength(255, ErrorMessage = "O caminho da imagem não pode exceder 255 caracteres.")]
        public string? ImagemCapturada { get; set; }

        [Range(0, 1, ErrorMessage = "A confiança mínima deve estar entre 0 e 1.")]
        public decimal? ConfiancaMinima { get; set; }
    }

    public class ReconhecimentoResponseDto
    {
        public int IdReconhecimento { get; set; }
        public int IdMoto { get; set; }
        public int IdCamera { get; set; }
        public DateTime DataHora { get; set; }
        public decimal Precisao { get; set; }
        public string ImagemCapturada { get; set; } = string.Empty;
        public decimal ConfiancaMinima { get; set; }
    }
}