using System.ComponentModel.DataAnnotations;

namespace SistemaMottuAPI.DTO
{
    public class CameraCreateDto
    {
        [Required(ErrorMessage = "A localização da câmera é obrigatória.")]
        [StringLength(100, ErrorMessage = "A localização não pode exceder 100 caracteres.")]
        public string Localizacao { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "O status não pode exceder 20 caracteres.")]
        public string Status { get; set; } = "ativo"; 
    }

    public class CameraUpdateDto
    {
        [StringLength(100, ErrorMessage = "A localização não pode exceder 100 caracteres.")]
        public string? Localizacao { get; set; }

        [StringLength(20, ErrorMessage = "O status não pode exceder 20 caracteres.")]
        public string? Status { get; set; }
    }

    public class CameraResponseDto
    {
        public int IdCamera { get; set; }
        public string Localizacao { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime UltimaVerificacao { get; set; }
    }
}