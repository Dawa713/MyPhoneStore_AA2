using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_clase.Models
{
    /// <summary>
    /// Representa una compra realizada por un cliente.
    /// Relaciona Customer con Phone y guarda el historial de compras.
    /// </summary>
    [Table("Purchases")]
    public class Purchase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Clave foránea hacia Customer
        [Required]
        public int CustomerId { get; set; }

        // Clave foránea hacia Phone
        [Required]
        public int PhoneId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "COMPLETED"; // COMPLETED, CANCELLED, PENDING

        [Required]
        public bool IsActive { get; set; }

        // Propiedades de navegación (relaciones)
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [ForeignKey("PhoneId")]
        public Phone? Phone { get; set; }

        public Purchase()
        {
            PurchaseDate = DateTime.Now;
            IsActive = true;
            Status = "COMPLETED";
        }
    }
}
