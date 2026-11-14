# EUMA · Tienda online de esencias

Trabajo práctico grupal (Grupo 6): una tienda online pequeña para vender los productos de EUMA, tomando como modelo su catálogo online. Los clientes ven el catálogo, arman un carrito, compran y pagan (pago simulado) y consultan su pedido. El personal de la tienda administra productos, categorías, ventas y clientes.

## Tecnologías

- ASP.NET Core MVC con .NET 10 y C#.
- Entity Framework Core 10 con SQL Server LocalDB (Code First, con migraciones).
- Vistas Razor, HTML5, CSS con Bootstrap y JavaScript (jQuery Validation para validar formularios).
- Sesión de ASP.NET Core para guardar el carrito de cada visitante.

## Cómo ejecutarlo

1. Abrir `EUMA.slnx` en Visual Studio.
2. Crear la base: en la *Consola del Administrador de paquetes* ejecutar `Update-Database`.
   Desde la terminal también se puede: `.\EUMA\.tools\dotnet-ef.exe database update --project EUMA`.
3. Cargar los datos de ejemplo ejecutando `EUMA/BaseDeDatos/DatosIniciales.sql` sobre la base `CEUMA`
   (desde Visual Studio, SSMS o `sqlcmd -S "(localdb)\MSSQLLocalDB" -d CEUMA -i EUMA\BaseDeDatos\DatosIniciales.sql`).
4. Opcional, después del anterior: `EUMA/BaseDeDatos/PedidosDePrueba.sql` agrega clientes y pedidos para probar la administración.
   Los dos scripts se pueden ejecutar varias veces sin duplicar datos.
5. Ejecutar con F5. La cadena de conexión es `ConexionEUMA`, en `EUMA/appsettings.json`.

## Funcionalidades

| Funcionalidad | Página | Controlador |
|---|---|---|
| Ver catálogo (categorías y búsqueda) | `/` | `HomeController.Index` |
| Ver detalle del producto | `/Home/Detalle/{id}` | `HomeController.Detalle` |
| Carrito de compras | `/Carrito` | `CarritoController` |
| Comprar y pagar (pago simulado) | `/Compra/Finalizar` y `/Compra/Pago/{id}` | `CompraController` |
| Consultar mi pedido | `/MiPedido` | `MiPedidoController` |
| Productos: alta, modificación y baja lógica | `/Productos` | `ProductosController` |
| Categorías: alta, modificación y baja | `/Categorias` | `CategoriasController` |
| Ventas: pedidos y cambio de estado | `/Ventas` | `VentasController` |
| Clientes: consulta | `/Clientes` | `ClientesController` |

La tienda usa el layout `_Layout.cshtml` y la administración usa `_LayoutAdmin.cshtml` (se elige con un `_ViewStart.cshtml` en cada carpeta de administración). La administración no tiene login: queda fuera del alcance de esta versión.

## Estructura

- `EUMA/Models`: clases del negocio (Categoria, Producto, Cliente, Pedido, DetallePedido), el carrito (Carrito, ItemCarrito, que viven en la sesión) y el formulario de compra (DatosCompra).
- `EUMA/Data/AppDbContext.cs`: el contexto de Entity Framework, con las reglas de la base.
- `EUMA/Migrations`: las migraciones que crean y actualizan la base.
- `EUMA/Controllers` y `EUMA/Views`: un controlador y una carpeta de vistas por funcionalidad.
- `EUMA/BaseDeDatos`: scripts de datos de ejemplo.
- `EUMA/wwwroot`: archivos estáticos. Las imágenes de los productos van en `wwwroot/img` y en el producto se carga solo el nombre del archivo.

## Notas

- Los precios se cargan con punto decimal (por ejemplo `12.50`).
- Estados de un pedido: Pendiente de pago → Pagado → Enviado → Entregado (envío a domicilio) o Pendiente de pago → Pagado → Entregado (retiro en el local). Un pedido pendiente de pago o pagado se puede cancelar: el stock vuelve.
- El stock se descuenta al confirmar la compra.

## Cómo trabajamos con Git

- Repositorio: https://github.com/33206858/PNT1, rama `master`.
- Antes de empezar: `git pull`. Si hay migraciones nuevas, `Update-Database`.
- Trabajamos de a uno por vez: el que termina hace commit y push, y avisa al otro.
- Las migraciones las crea una sola persona; el otro solo hace `pull` y `Update-Database`.
