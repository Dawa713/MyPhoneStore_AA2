using System.ComponentModel.DataAnnotations;

namespace api_clase.DTOs
{
    /// <summary>
    /// DTO para mostrar una compra en las respuestas de la API.
    /// Incluye datos del teléfono y cliente sin exponer objetos completos.
    /// </summary>
    public class PurchaseDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int PhoneId { get; set; }
        public string PhoneBrand { get; set; } = string.Empty;
        public string PhoneModel { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO para crear una nueva compra.
    /// </summary>
    public class CreatePurchaseDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int PhoneId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Quantity { get; set; }
    }
}
