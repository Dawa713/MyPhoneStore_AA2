# 📋 Guía de Defensa — Frontend Vue 3 (Entorno Cliente)

---

## ¿Qué hace el frontend?

Aplicación web que consume la API PhoneStore. Permite a los usuarios ver el catálogo de teléfonos, comprarlos, gestionar su historial y, si son administradores, gestionar el inventario y los clientes.

---

## ✅ Cumplimiento de requisitos obligatorios

### Tecnologías usadas

| Requisito | Implementación |
|-----------|---------------|
| Vue 3 | `vue@3` con Composition API |
| TypeScript | Todos los `.vue` y `.ts` usan tipos |
| Vue Router | Navegación entre páginas con guards |
| Pinia | 3 stores: `auth`, `phones`, `purchases` + `ui` |
| Framework UI | **PrimeVue** (DataTable, Button, Toast, Tag, ConfirmDialog...) |
| Validación de formularios | **VeeValidate + Yup** |
| Sin `alert()` nativo | Reemplazados por `Toast` y `ConfirmDialog` de PrimeVue |

---

### Vistas y layouts

**3 layouts distintos:**

- `DefaultLayout` — NavBar + Footer. Para páginas públicas y usuario logueado.
- `AuthLayout` — Solo contenido centrado, sin header ni footer. Para login y registro.
- `AdminLayout` — Header de admin + sidebar con navegación + footer de admin.

**Vistas:**

| Ruta | Vista | Layout | Acceso |
|------|-------|--------|--------|
| `/` | HomeView | Default | Público |
| `/login` | LoginView | Auth | Público |
| `/register` | RegisterView | Auth | Público |
| `/my-purchases` | MyPurchasesView | Default | Autenticado |
| `/admin/phones` | AdminPhonesView | Admin | Solo ADMIN |
| `/admin/customers` | AdminCustomersView | Admin | Solo ADMIN |
| `/admin/purchases` | AdminPurchasesView | Admin | Solo ADMIN |

---

### Componentes

- `NavBar.vue` — Navegación principal. Muestra secciones distintas según si el usuario está logueado o es admin. Resalta la ruta activa con `router-link-active`.
- `AppFooter.vue` — Pie de página con enlaces y año.
- `PhoneCard.vue` — Tarjeta de un teléfono. **Separado del listado**: `HomeView` solo hace el `v-for`, `PhoneCard` se encarga de pintarlo.
- Layouts (`DefaultLayout`, `AuthLayout`, `AdminLayout`) — Plantillas que envuelven las vistas.

---

### Stores Pinia

**`auth.ts`** — Estado de autenticación:
- Guarda `token`, `userName`, `userRole`, `customerId` en `localStorage`
- `isLoggedIn` y `isAdmin` son `computed` derivados del token
- Métodos: `login()`, `register()`, `logout()`

**`phones.ts`** — Estado de teléfonos:
- `phones[]` lista reactiva
- `fetchAll()`, `fetchByBrand()`, `fetchByPrice()` — obtención desde API
- `add()`, `edit()`, `remove()` — persistencia en backend

**`purchases.ts`** — Estado de compras:
- `fetchMine(customerId)` — compras del usuario
- `fetchAll()` — todas las compras (admin)
- `add()`, `cancel()` — persistencia en backend

**`ui.ts`** — Estado de interfaz:
- `sidebarOpen` — controla el sidebar del admin
- `toggleSidebar()`, `setLoading()`

---

### Validación con VeeValidate + Yup

En `LoginView`, `RegisterView` y `AdminPhonesView`:

```typescript
// Esquema de validación declarativo con Yup
const schema = yup.object({
  email: yup.string().required('El email es obligatorio').email('Email inválido'),
  password: yup.string().required().min(6, 'Mínimo 6 caracteres'),
})

// VeeValidate conecta el esquema con los campos del formulario
const { defineField, handleSubmit, errors } = useForm({ validationSchema: schema })
```

Los errores se muestran en tiempo real bajo cada campo. El formulario no se envía si hay errores.

---

### Comunicación con la API

Toda la comunicación está centralizada en `src/services/api.ts`:
- Usa **axios** con una instancia configurada con la URL base
- Un **interceptor** añade automáticamente el token JWT a todas las peticiones
- Las funciones son simples: `getPhones()`, `createPurchase(data)`, etc.
- Los stores llaman a estas funciones, no las vistas directamente

---

### Navegación protegida (Route Guards)

```typescript
router.beforeEach((to) => {
  const auth = useAuthStore()
  if (to.meta.requiresAuth && !auth.isLoggedIn) return { name: 'login' }
  if (to.meta.requiresAdmin && !auth.isAdmin) return { name: 'home' }
})
```

Si intentas acceder a `/admin` sin ser admin, te redirige al inicio. Si intentas `/my-purchases` sin estar logueado, te manda al login.

---

### Docker

El frontend se conteneriza en dos etapas:
1. **Build**: imagen Node 20 ejecuta `npm run build` → genera la carpeta `dist/`
2. **Serve**: imagen Nginx sirve los archivos estáticos de `dist/`

`nginx.conf` usa `try_files $uri /index.html` para que Vue Router gestione todas las rutas (sin esto, recargar `/admin` daría 404).

---

## Preguntas probables del profesor

**P: ¿Qué es la Composition API?**
R: La forma moderna de escribir lógica en Vue 3. En lugar de `data()`, `methods`, `computed` separados (Options API), todo se define dentro de `setup()` con funciones como `ref()`, `computed()`, `onMounted()`. Es más flexible y facilita reutilizar lógica.

**P: ¿Para qué sirve Pinia?**
R: Para tener estado compartido entre componentes. Sin Pinia, si dos componentes necesitan la lista de teléfonos, cada uno haría su propia petición a la API. Con Pinia, la lista se obtiene una vez y cualquier componente la usa. También persiste el token en localStorage para que el login sobreviva recargas.

**P: ¿Qué diferencia hay entre `ref` y `reactive`?**
R: `ref` sirve para valores primitivos (string, number, boolean) y se accede con `.value`. `reactive` sirve para objetos. En la práctica, `ref` también puede contener objetos y es más predecible.

**P: ¿Qué es un interceptor de axios?**
R: Código que se ejecuta antes de cada petición (o respuesta). El nuestro lee el token del localStorage y lo añade al header `Authorization`. Así no hay que añadirlo manualmente en cada llamada a la API.

**P: ¿Por qué separas PhoneCard del v-for?**
R: Principio de responsabilidad única. `HomeView` es responsable de "qué datos mostrar y en qué orden". `PhoneCard` es responsable de "cómo se ve un teléfono". Si cambia el diseño de la tarjeta, solo tocas `PhoneCard`. Si cambia la lógica del listado, solo tocas `HomeView`.

**P: ¿Qué es VeeValidate y por qué no validar con `if` en el submit?**
R: VeeValidate gestiona el estado de validación de forma reactiva. Yup define las reglas de forma declarativa y legible. Validar manualmente con `if` dentro del submit no muestra errores en tiempo real mientras el usuario escribe, y mezcla la lógica de validación con la lógica de negocio.

**P: ¿Cómo funciona el layout dinámico en App.vue?**
R: Cada ruta declara `meta.layout: 'auth'|'admin'|'default'`. `App.vue` tiene un `computed` que devuelve el componente de layout según ese valor, y usa `<component :is="currentLayout">` para renderizarlo. Así login no tiene header/footer y admin tiene su propio panel.

**P: ¿Para qué sirve nginx.conf con `try_files`?**
R: Vue Router es client-side: las rutas como `/admin` no existen en el servidor, solo en el navegador. Si Nginx recibe una petición a `/admin` directamente (recarga de página), buscaría un archivo que no existe y devolvería 404. Con `try_files $uri /index.html`, siempre devuelve el `index.html` y Vue Router se encarga del resto.

**P: ¿Por qué `.env.production` para la URL de la API?**
R: En desarrollo la API corre en `localhost:5149`. En Docker la API corre en `localhost:7959`. Con la variable de entorno `VITE_API_URL` se cambia la URL según el entorno sin tocar el código. Vite lee `.env.production` durante el `npm run build`.

**P: ¿Qué es gitflow?**
R: Una metodología de trabajo con Git. Hay una rama `main` (producción, estable) y una rama `develop` (desarrollo activo). Las nuevas funcionalidades se crean en ramas `feature/nombre-feature` que luego se fusionan en `develop`. Cuando `develop` está estable, se fusiona en `main` con una etiqueta de versión.
