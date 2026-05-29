using Microsoft.AspNetCore.Mvc;
using api_clase.DTOs;
using api_clase.Models;
using api_clase.Services;
using AutoMapper;

namespace api_clase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly JwtService _jwtService;
        private readonly IMapper _mapper;

        public AuthController(
            ICustomerRepository customerRepository,
            JwtService jwtService,
            IMapper mapper)
        {
            _customerRepository = customerRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        /// <summary>
        /// POST: api/auth/login - Autentica un usuario y devuelve un token JWT
        /// </summary>
        [HttpPost("login")]
        public ActionResult<AuthResponseDTO> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = _customerRepository.GetByEmail(loginDTO.Email);

            if (customer == null || customer.Password != loginDTO.Password)
                return Unauthorized(new { message = "Email o contraseña incorrectos" });

            if (!customer.IsActive)
                return Unauthorized(new { message = "Cuenta desactivada" });

            var token = _jwtService.GenerateToken(customer);

            return Ok(new AuthResponseDTO
            {
                Token = token,
                Name = customer.Name,
                Email = customer.Email,
                Role = customer.Role,
                CustomerId = customer.Id
            });
        }

        /// <summary>
        /// POST: api/auth/register - Registra un nuevo cliente
        /// </summary>
        [HttpPost("register")]
        public ActionResult<AuthResponseDTO> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Comprobar si el email ya existe
            var existing = _customerRepository.GetByEmail(registerDTO.Email);
            if (existing != null)
                return Conflict(new { message = "Ya existe una cuenta con ese email" });

            var customer = new Customer(registerDTO.Name, registerDTO.Email, registerDTO.Password, "CLIENT");
            _customerRepository.Add(customer);

            var token = _jwtService.GenerateToken(customer);

            return CreatedAtAction(nameof(Login), new AuthResponseDTO
            {
                Token = token,
                Name = customer.Name,
                Email = customer.Email,
                Role = customer.Role,
                CustomerId = customer.Id
            });
        }
    }
}
