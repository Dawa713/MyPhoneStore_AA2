# 📋 Guía de Defensa — API REST C# (Entorno Servidor)

---

## ¿Qué es la aplicación?

**PhoneStore** es una tienda de teléfonos móviles con API REST desarrollada en ASP.NET Core 10. Gestiona un catálogo de teléfonos, clientes registrados y el historial de compras.

---

## ✅ Cumplimiento de requisitos obligatorios

### 1. Arquitectura RESTful con CRUD y códigos HTTP

La API sigue los principios REST: cada recurso tiene su URL, los métodos HTTP indican la acción y los códigos de estado informan del resultado.

**Zona pública** (sin autenticación):
- `GET /api/phones` → lista teléfonos (200 OK)
- `GET /api/phones/{id}` → teléfono por ID (200 OK / 404 Not Found)
- `GET /api/phones/search/byBrand?brand=Apple` → búsqueda por marca
- `GET /api/phones/search/byPrice?minPrice=500&maxPrice=1000` → búsqueda por precio

**Zona privada** (requiere token JWT):
- `POST /api/auth/login` → obtener token
- `POST /api/auth/register` → registrar cliente
- `POST /api/purchases` → crear compra [CLIENT/ADMIN]
- `GET /api/purchases/customer/{id}` → mis compras [CLIENT/ADMIN]
- `PUT /api/purchases/{id}/cancel` → cancelar compra [CLIENT/ADMIN]

**Solo ADMIN**:
- `POST /api/phones` → crear teléfono (201 Created)
- `PUT /api/phones/{id}` → actualizar teléfono (200 OK / 404)
- `DELETE /api/phones/{id}` → desactivar teléfono (soft delete)
- `GET /api/customers` → listar todos los clientes
- `GET /api/purchases` → ver todas las compras

**Códigos de estado usados:**
| Código | Cuándo |
|--------|--------|
| 200 OK | Petición correcta con datos |
| 201 Created | Recurso creado (POST exitoso) |
| 400 Bad Request | Datos inválidos o stock insuficiente |
| 401 Unauthorized | Sin token o token inválido |
| 403 Forbidden | Token válido pero sin permisos |
| 404 Not Found | Recurso no existe |
| 409 Conflict | Email ya registrado |

---

### 2. Modelo de datos con ORM (Entity Framework Core)

**3 entidades con relaciones:**

```
Customer (1) ──────────────── (N) Purchase (N) ──────────────── (1) Phone
```

**Customer** (7 atributos):
- `Id` (int), `Name` (string), `Email` (string), `Password` (string)
- `Role` (string: ADMIN/CLIENT), `CreatedAt` (DateTime), `IsActive` (bool)

**Phone** (7 atributos):
- `Id` (int), `Brand` (string), `Model` (string), `Price` (decimal)
- `Stock` (int), `ReleaseDate` (DateTime), `IsActive` (bool)

**Purchase** (8 atributos):
- `Id` (int), `CustomerId` (int FK), `PhoneId` (int FK)
- `Quantity` (int), `TotalPrice` (decimal), `PurchaseDate` (DateTime)
- `Status` (string: COMPLETED/CANCELLED), `IsActive` (bool)

El acceso a BD se hace con **Entity Framework Core** (ORM) con **Pomelo** para MariaDB. Las migraciones generan y actualizan las tablas automáticamente.

---

### 3. Búsqueda y filtrado

**Zona pública (teléfonos):**
- Por marca: `GET /api/phones/search/byBrand?brand=Samsung`
- Por rango de precio: `GET /api/phones/search/byPrice?minPrice=500&maxPrice=900`

**Zona privada (compras):**
- Por estado: `GET /api/purchases/search/byStatus?status=COMPLETED`
- Por rango de fechas: `GET /api/purchases/search/byDate?from=2024-01-01&to=2024-12-31`

Ningún filtro usa `id` como campo de búsqueda. Todos devuelven listas ordenadas por fecha descendente.

---

### 4. Autenticación JWT con roles

**Flujo de autenticación:**
1. Cliente llama a `POST /api/auth/login` con email y contraseña
2. La API verifica credenciales en la BD
3. Si son correctas, genera un token JWT firmado con HMAC-SHA256
4. El token contiene: `id`, `email`, `role` (ADMIN o CLIENT), expiración (24h)
5. El cliente incluye el token en todas las peticiones: `Authorization: Bearer <token>`

**Protección por roles:**
```csharp
[Authorize(Roles = "ADMIN")]   // Solo administradores
[Authorize]                     // Cualquier usuario autenticado
// Sin atributo                 // Público
```

---

### 5. Contenedores Docker

```yaml
# docker-compose.yml
mariadb:  puerto 9597  (BD)
api:      puerto 7959  (API)
frontend: puerto 80    (Vue)
```

Arranque completo con un solo comando:
```bash
docker-compose build && docker-compose up
```

Las migraciones se aplican automáticamente al arrancar la API (`db.Database.Migrate()`).

---

## Preguntas probables del profesor

**P: ¿Qué es REST?**
R: Un estilo de arquitectura para APIs. Los principios clave son: URLs que representan recursos (`/api/phones`, no `/getPhones`), métodos HTTP con significado (GET=leer, POST=crear, PUT=actualizar, DELETE=borrar) y respuestas con códigos de estado estándar.

**P: ¿Por qué usas DTOs?**
R: Para controlar exactamente qué datos se exponen. Por ejemplo, `CustomerDTO` no incluye el `Password`. Si devolviera el objeto `Customer` completo, la contraseña viajaría en todas las respuestas.

**P: ¿Qué es el Repository Pattern?**
R: Separar la lógica de acceso a BD del controlador. El controlador llama a `_phoneRepository.GetAll()` sin saber si los datos vienen de una BD, un archivo o una caché. Facilita los tests y los cambios de BD.

**P: ¿Qué es la Inyección de Dependencias?**
R: En lugar de que el controlador cree `new PhoneRepository()`, ASP.NET lo inyecta automáticamente en el constructor. Se configura en `Program.cs` con `AddScoped<IPhoneRepository, PhoneRepository>()`. Scoped significa una instancia por petición HTTP.

**P: ¿Qué es un JWT?**
R: JSON Web Token. Es una cadena en tres partes (header.payload.signature) que codifica datos del usuario. La firma garantiza que nadie lo ha modificado. No se guarda en la BD — la API lo verifica recalculando la firma con su clave secreta.

**P: ¿Qué es el soft delete?**
R: En lugar de borrar el registro de la BD, se pone `IsActive = false`. Las consultas filtran por `IsActive = true`. Así se mantiene el historial y se puede recuperar.

**P: ¿Cómo funciona AutoMapper?**
R: Convierte automáticamente objetos de un tipo a otro. En lugar de escribir `phoneDTO.Brand = phone.Brand; phoneDTO.Model = phone.Model...` para cada campo, AutoMapper lo hace solo con `_mapper.Map<PhoneDTO>(phone)`.

**P: ¿Por qué MariaDB en lugar de SQL Server?**
R: MariaDB es open source, más ligero en Docker y compatible con MySQL. Entity Framework Core lo soporta a través del paquete Pomelo.

**P: ¿Qué hace `db.Database.Migrate()` al arrancar?**
R: Comprueba qué migraciones no se han aplicado y las ejecuta. En Docker, la primera vez crea todas las tablas y mete los datos iniciales (seed). Así no hay que ejecutar `dotnet ef database update` manualmente.
