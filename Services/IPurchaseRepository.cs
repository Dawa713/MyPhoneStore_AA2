using api_clase.Models;

namespace api_clase.Services
{
    public interface IPurchaseRepository
    {
        IEnumerable<Purchase> GetAll();
        IEnumerable<Purchase> GetByCustomerId(int customerId);
        IEnumerable<Purchase> GetByPhoneId(int phoneId);
        IEnumerable<Purchase> GetByDateRange(DateTime from, DateTime to);
        IEnumerable<Purchase> GetByStatus(string status);
        Purchase? GetById(int id);
        Purchase Add(Purchase purchase);
        void Cancel(int id); // Cambia Status a CANCELLED
    }
}
