using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api_clase.Models;
using api_clase.Services;
using api_clase.DTOs;
using AutoMapper;

namespace api_clase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todas las rutas de compras requieren estar autenticado
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPhoneRepository _phoneRepository;
        private readonly IMapper _mapper;

        public PurchasesController(
            IPurchaseRepository purchaseRepository,
            ICustomerRepository customerRepository,
            IPhoneRepository phoneRepository,
            IMapper mapper)
        {
            _purchaseRepository = purchaseRepository;
            _customerRepository = customerRepository;
            _phoneRepository = phoneRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// GET: api/purchases - Obtiene todas las compras (solo ADMIN)
        /// </summary>
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public ActionResult<IEnumerable<PurchaseDTO>> GetAll()
        {
            var purchases = _purchaseRepository.GetAll();
            return Ok(_mapper.Map<IEnumerable<PurchaseDTO>>(purchases));
        }

        /// <summary>
        /// GET: api/purchases/{id} - Obtiene una compra por ID
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<PurchaseDTO> GetById(int id)
        {
            var purchase = _purchaseRepository.GetById(id);
            if (purchase == null)
                return NotFound(new { message = $"Compra con ID {id} no encontrada" });

            return Ok(_mapper.Map<PurchaseDTO>(purchase));
        }

        /// <summary>
        /// GET: api/purchases/customer/{customerId} - Compras de un cliente concreto
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public ActionResult<IEnumerable<PurchaseDTO>> GetByCustomer(int customerId)
        {
            var customer = _customerRepository.GetById(customerId);
            if (customer == null)
                return NotFound(new { message = $"Cliente con ID {customerId} no encontrado" });

            var purchases = _purchaseRepository.GetByCustomerId(customerId);
            return Ok(_mapper.Map<IEnumerable<PurchaseDTO>>(purchases));
        }

        /// <summary>
        /// GET: api/purchases/search/byStatus?status=COMPLETED - Filtra por estado
        /// </summary>
        [HttpGet("search/byStatus")]
        public ActionResult<IEnumerable<PurchaseDTO>> GetByStatus([FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new { message = "El estado es requerido" });

            var purchases = _purchaseRepository.GetByStatus(status);
            return Ok(_mapper.Map<IEnumerable<PurchaseDTO>>(purchases));
        }

        /// <summary>
        /// GET: api/purchases/search/byDate?from=2024-01-01&to=2024-12-31 - Filtra por rango de fechas
        /// </summary>
        [HttpGet("search/byDate")]
        public ActionResult<IEnumerable<PurchaseDTO>> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (from > to)
                return BadRequest(new { message = "La fecha 'from' no puede ser mayor que 'to'" });

            var purchases = _purchaseRepository.GetByDateRange(from, to);
            return Ok(_mapper.Map<IEnumerable<PurchaseDTO>>(purchases));
        }

        /// <summary>
        /// POST: api/purchases - Crea una nueva compra
        /// </summary>
        [HttpPost]
        public ActionResult<PurchaseDTO> Create([FromBody] CreatePurchaseDTO createDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validar que el cliente existe
            var customer = _customerRepository.GetById(createDTO.CustomerId);
            if (customer == null)
                return NotFound(new { message = $"Cliente con ID {createDTO.CustomerId} no encontrado" });

            // Validar que el teléfono existe
            var phone = _phoneRepository.GetById(createDTO.PhoneId);
            if (phone == null)
                return NotFound(new { message = $"Teléfono con ID {createDTO.PhoneId} no encontrado" });

            try
            {
                var purchase = _mapper.Map<Purchase>(createDTO);
                var created = _purchaseRepository.Add(purchase);
                var responseDTO = _mapper.Map<PurchaseDTO>(created);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, responseDTO);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/purchases/{id}/cancel - Cancela una compra y devuelve el stock
        /// </summary>
        [HttpPut("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            var purchase = _purchaseRepository.GetById(id);
            if (purchase == null)
                return NotFound(new { message = $"Compra con ID {id} no encontrada" });

            if (purchase.Status == "CANCELLED")
                return BadRequest(new { message = "La compra ya está cancelada" });

            _purchaseRepository.Cancel(id);
            return Ok(new { message = "Compra cancelada correctamente. Stock devuelto." });
        }
    }
}
