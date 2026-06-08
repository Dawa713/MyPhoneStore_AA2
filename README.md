# 📱 PhoneStore — API + Frontend

**Proyecto DAW 2º - Entorno Servidor + Entorno Cliente**

---

## 🔑 Credenciales de prueba

| Usuario | Email | Contraseña | Rol |
|---------|-------|-----------|-----|
| Juan Pérez | `juan@email.com` | `password123` | ADMIN |
| María García | `maria@email.com` | `pass1234` | CLIENT |
| Carlos López | `carlos@email.com` | `secure456` | CLIENT |

---

## 🐳 Lanzar con Docker

```bash
git clone -b develop https://github.com/Dawa713/aa2dwec.git
cd aa2dwec

# Linux/Mac
docker-compose build && docker-compose up

# Windows PowerShell
docker-compose build; docker-compose up
```

| Servicio | URL |
|----------|-----|
| Frontend | http://localhost |
| API / Swagger | http://localhost:7859/swagger |
| Base de datos | localhost:9597 |

---

## 📋 Descripción

**PhoneStore** es una tienda de teléfonos móviles con API REST y frontend Vue 3.

- **API**: ASP.NET Core 10, Entity Framework Core, MariaDB, JWT
- **Frontend**: Vue 3, TypeScript, PrimeVue, Pinia, VeeValidate

### Funcionalidades

- Catálogo público de teléfonos con búsqueda por marca y precio
- Registro e inicio de sesión con JWT
- Compra de teléfonos con historial personal
- Panel de administración: gestión de teléfonos, clientes y compras

---

## 🏗️ Estructura del proyecto

```
aa2dwec/
├── Controllers/        # Endpoints REST (Auth, Phones, Customers, Purchases)
├── Models/             # Entidades (Customer, Phone, Purchase)
├── DTOs/               # Objetos de transferencia de datos
├── Services/           # Repositorios e interfaces
├── Mappings/           # AutoMapper
├── Migrations/         # Migraciones EF Core
├── Dockerfile          # Imagen de la API
├── docker-compose.yml  # Orquestación completa
└── aa2-frontend/       # Proyecto Vue 3
    ├── src/
    │   ├── views/      # Páginas (Home, Login, Register, Admin, MyPurchases)
    │   ├── components/ # NavBar, AppFooter, PhoneCard
    │   ├── layouts/    # DefaultLayout, AuthLayout, AdminLayout
    │   ├── stores/     # Pinia: auth, phones, purchases, ui
    │   └── services/   # API client (axios)
    └── Dockerfile      # Imagen del frontend (Nginx)
```

---

## 🚀 Desarrollo local (sin Docker)

### API

```bash
# Requisitos: .NET 10, MariaDB corriendo en localhost:3306
cd aa2dwec
dotnet restore
dotnet ef database update
dotnet run
# API disponible en http://localhost:5149
```

### Frontend

```bash
cd aa2dwec/aa2-frontend
npm install
npm run dev
# Frontend disponible en http://localhost:5173
```

---

## 🔧 Tecnologías

| Capa | Tecnología |
|------|-----------|
| Backend | ASP.NET Core 10, C# |
| Base de datos | MariaDB 11, Entity Framework Core |
| Autenticación | JWT (HMAC-SHA256) |
| Frontend | Vue 3, TypeScript, Vite |
| UI | PrimeVue |
| Estado | Pinia |
| Validación | VeeValidate + Yup |
| Contenedores | Docker, Docker Compose |

---

## 📡 Endpoints principales

```
POST   /api/auth/login                Obtener token JWT
POST   /api/auth/register             Registrar cuenta

GET    /api/phones                    Listar teléfonos (público)
GET    /api/phones/{id}               Teléfono por ID (público)
GET    /api/phones/search/byBrand     Buscar por marca (público)
GET    /api/phones/search/byPrice     Buscar por precio (público)
POST   /api/phones                    Crear teléfono [ADMIN]
PUT    /api/phones/{id}               Actualizar teléfono [ADMIN]
DELETE /api/phones/{id}               Baja lógica de teléfono [ADMIN]
POST   /api/phones/{id}/purchase      Comprar teléfono [autenticado]

GET    /api/purchases                 Todas las compras [ADMIN]
GET    /api/purchases/{id}            Compra por ID [autenticado]
GET    /api/purchases/customer/{id}   Compras de un cliente [autenticado]
GET    /api/purchases/search/byStatus Filtrar por estado [autenticado]
GET    /api/purchases/search/byDate   Filtrar por rango de fechas [autenticado]
POST   /api/purchases                 Crear compra [autenticado]
PUT    /api/purchases/{id}/cancel     Cancelar compra (devuelve stock) [autenticado]

GET    /api/customers                 Listar clientes [ADMIN]
GET    /api/customers/{id}            Cliente por ID [ADMIN]
POST   /api/customers                 Crear cliente [ADMIN]
PUT    /api/customers/{id}            Actualizar cliente [ADMIN]
DELETE /api/customers/{id}            Baja lógica de cliente [ADMIN]
```

---

## 📝 Ramas Git

- `main` — producción
- `develop` — desarrollo activo
