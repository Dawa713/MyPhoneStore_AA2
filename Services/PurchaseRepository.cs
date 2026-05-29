using api_clase.Models;
using api_clase.Data;
using Microsoft.EntityFrameworkCore;

namespace api_clase.Services
{
    /// <summary>
    /// Implementación del repositorio de compras usando Entity Framework Core.
    /// Al crear una compra, también reduce el stock del teléfono.
    /// </summary>
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly ApplicationDbContext _context;

        public PurchaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Purchase> GetAll()
        {
            // Include carga las relaciones (Customer y Phone) junto con la compra
            return _context.Purchases
                .Include(p => p.Customer)
                .Include(p => p.Phone)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();
        }

        public IEnumerable<Purchase> GetByCustomerId(int customerId)
        {
            return _context.Purchases
                .Include(p => p.Customer)
                .Include(p => p.Phone)
                .Where(p => p.CustomerId == customerId && p.IsActive)
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();
        }

        public IEnumerable<Purchase> GetByPhoneId(int phoneId)
        {
            return _context.Purchases
                .Include(p => p.Customer)
                .Include(p => p.Phone)
                .Where(p => p.PhoneId == phoneId && p.IsActive)
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();
        }

        public IEnumerable<Purchase> GetByDateRange(DateTime from, DateTime to)
        {
            return _context.Purchases
                .Include(p => p.Customer)
                .Include(p => p.Phone)
                .Where(p => p.PurchaseDate >= from && p.PurchaseDate <= to && p.IsActive)
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();
        }

        public IEnumerable<Purchase> GetByStatus(string status)
        {
            return _context.Purchases
                .Include(p => p.Customer)
                .Include(p => p.Phone)
                .Where(p => p.Status == status.ToUpper() && p.IsActive)
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();
        }

        public Purchase? GetById(int id)
        {
            return _context.Purchases
                .Include(p => p.Customer)
                .Include(p => p.Phone)
                .FirstOrDefault(p => p.Id == id && p.IsActive);
        }

        public Purchase Add(Purchase purchase)
        {
            // Calcular precio total basado en el precio actual del teléfono
            var phone = _context.Phones.FirstOrDefault(p => p.Id == purchase.PhoneId && p.IsActive);
            if (phone == null)
                throw new InvalidOperationException("Teléfono no encontrado o inactivo");

            if (phone.Stock < purchase.Quantity)
                throw new InvalidOperationException($"Stock insuficiente. Disponible: {phone.Stock}");

            purchase.TotalPrice = phone.Price * purchase.Quantity;

            // Reducir stock del teléfono
            phone.Stock -= purchase.Quantity;
            _context.Phones.Update(phone);

            _context.Purchases.Add(purchase);
            _context.SaveChanges();

            // Recargar con relaciones para devolverlo completo
            return GetById(purchase.Id)!;
        }

        public void Cancel(int id)
        {
            var purchase = _context.Purchases
                .Include(p => p.Phone)
                .FirstOrDefault(p => p.Id == id);

            if (purchase != null && purchase.Status != "CANCELLED")
            {
                // Devolver el stock al cancelar
                if (purchase.Phone != null)
                {
                    purchase.Phone.Stock += purchase.Quantity;
                    _context.Phones.Update(purchase.Phone);
                }

                purchase.Status = "CANCELLED";
                _context.Purchases.Update(purchase);
                _context.SaveChanges();
            }
        }
    }
}
