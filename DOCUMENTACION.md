# PhoneStore — Documento Informativo

## Nombre de la aplicación

**PhoneStore**

---

## Propósito

PhoneStore es una API REST para la gestión de una tienda de teléfonos móviles. Permite a los clientes consultar el catálogo, registrarse, autenticarse y realizar compras. Los administradores pueden gestionar el inventario de teléfonos, los clientes registrados y el historial completo de compras.

---

## Características

### Gestión de teléfonos
- Catálogo público con búsqueda por marca y rango de precio
- Alta, modificación y baja lógica de teléfonos (solo ADMIN)
- Control de stock: se reduce automáticamente al realizar una compra y se restaura al cancelarla

### Gestión de clientes
- Registro de nuevos clientes con validación de datos
- Autenticación mediante JWT con roles (ADMIN / CLIENT)
- Listado y desactivación de clientes (solo ADMIN)

### Gestión de compras
- Registro de compras vinculando cliente y teléfono
- Historial personal de compras por cliente
- Cancelación de compras con devolución automática de stock
- Búsqueda por estado (COMPLETED / CANCELLED) y por rango de fechas (solo ADMIN)

### Arquitectura
- **Patrón Repository**: separa la lógica de negocio del acceso a datos
- **DTOs**: los datos sensibles (contraseñas) nunca se exponen en las respuestas
- **AutoMapper**: conversión automática entre entidades y DTOs
- **Migraciones EF Core**: el esquema de base de datos se gestiona mediante código
- **Swagger UI**: documentación interactiva de todos los endpoints

### Seguridad
- Autenticación con JSON Web Tokens (JWT, HMAC-SHA256)
- Autorización por roles en cada endpoint
- CORS configurado para el frontend Vue

### Infraestructura
- Contenerización con Docker (API en puerto 7859, MariaDB en puerto 9597)
- Orquestación con Docker Compose
- Migraciones y datos iniciales aplicados automáticamente al arrancar

---

## Modelo de datos

| Entidad | Atributos principales | Relaciones |
|---------|----------------------|------------|
| **Customer** | Id, Name, Email, Password, Role, CreatedAt, IsActive | 1 cliente → N compras |
| **Phone** | Id, Brand, Model, Price, Stock, ReleaseDate, IsActive | 1 teléfono → N compras |
| **Purchase** | Id, CustomerId, PhoneId, Quantity, TotalPrice, PurchaseDate, Status, IsActive | N compras → 1 cliente, 1 teléfono |

---

## Tecnologías utilizadas

| Componente | Tecnología |
|-----------|-----------|
| Framework | ASP.NET Core 10 |
| Lenguaje | C# |
| Base de datos | MariaDB 11 |
| ORM | Entity Framework Core 9 + Pomelo |
| Autenticación | JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer) |
| Mapeo | AutoMapper 12 |
| Documentación API | Swagger / OpenAPI (Swashbuckle) |
| Contenedores | Docker + Docker Compose |
| Frontend (opcional) | Vue 3 + TypeScript + PrimeVue |
