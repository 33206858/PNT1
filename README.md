# EUMA — avance MVC con Entity Framework

Avance del 26/10/2026. El ejemplo Estudiante/Deporte de clase se adapta al dominio del TP mediante Producto/Origen, Categoria y AppDbContext.

## Implementado

- Modelos con Data Annotations, relaciones, índices únicos y restricciones SQL existentes.
- CRUD MVC de Productos y Categorías: Index, Create, Edit, Details y Delete, con formularios parciales compartidos. Código escrito siguiendo el patrón del scaffolding; no se ejecutó el generador de Visual Studio.
- Productos: código único incluyendo inactivos; categoría existente; precio positivo; stock no negativo; origen válido. La baja es lógica mediante Activo=false; Edit permite reactivar.
- Categorías: nombre único y eliminación bloqueada si existen productos asociados, incluidos inactivos.
- POST con antiforgery y lista explícita de propiedades aceptadas. Se valida en servidor además de la validación del navegador.
- ValidateNever sobre Producto.Categoria evita exigir una entidad de navegación en el formulario; el servidor verifica CategoriaId contra la base.
- Límites decimales interpretados con cultura invariante. Formularios con cultura en-US para alinear el punto decimal de HTML/jQuery y el model binding; interfaz en español. Ingresar precios como 1234.56, sin separadores de miles.
- Dependencias EF Core fijadas en 10.0.14 y System.Security.Cryptography.Pkcs en 9.0.21. La auditoría original detectó PKCS 9.0.11 vulnerable; no se atribuye esa corrección a Copilot.

## Ejecutar

Abrir EUMA.slnx en Visual Studio con .NET 10 y SQL Server LocalDB. Verificar ConexionEUMA en EUMA/appsettings.json.

En la consola del Administrador de paquetes, seleccionar EUMA y ejecutar `Update-Database` para aplicar la migración Inicial en una instalación nueva. Esta entrega no modifica el esquema ni necesita una migración adicional. El paquete EF Tools ya está referenciado; el comando CLI dotnet ef requiere instalar su herramienta por separado.

Desde esta carpeta:

```powershell
dotnet restore EUMA.slnx
dotnet build EUMA.slnx --no-restore
dotnet run --project EUMA/EUMA.csproj
```

Abrir las rutas /Categorias y /Productos o usar el menú. Crear primero una categoría.

## Pruebas manuales de aceptación

1. Crear una categoría con nombre de al menos tres caracteres; verificar listado y detalle.
2. Intentar duplicar el nombre; debe mostrarse un error sin insertar.
3. Crear un producto con código único, categoría, presentación, origen, precio 1234.56 y stock 5. Comprobar el importe persistido.
4. Intentar código duplicado, precio cero, stock negativo, nombre vacío y categoría inexistente. Deben rechazarse también mediante POST directo.
5. Editar producto y verificar persistencia. Probar un enum fuera de rango mediante POST directo.
6. Desactivar producto: debe seguir en la administración como inactivo; reactivarlo desde Editar.
7. Intentar eliminar su categoría: debe bloquearse mientras tenga productos.
8. Crear una categoría sin productos y eliminarla; no debe aparecer en el listado.
9. Probar identificadores inexistentes y discrepancia entre ruta e Id del formulario: respuesta 404.
10. Enviar POST sin token antiforgery: respuesta 400.

## Alcance pendiente

Carrito, confirmación de pedidos, descuento de stock, gestión de clientes, autenticación/autorización y catálogo público con filtro Activo siguen pendientes. El listado Productos es de administración e incluye inactivos. No publicar esta administración abierta en Internet. El documento funcional Word existente se conserva; este archivo documenta el avance técnico.

## Commits sugeridos

Revisar `git diff` y separar: corrección de dependencias; CRUD y validación de productos; CRUD de categorías y navegación; documentación y pruebas. Cada integrante debe revisar y confirmar sus cambios con su propia identidad. Los cambios se agrupan en commits locales por tarea; la publicación en GitHub se realiza por separado.

## Verificación ejecutada

- Compilación final: cero errores y cero advertencias.
- Auditoría NuGet con dependencias transitivas: no se detectaron paquetes vulnerables en los orígenes consultados el 26/10/2026.
- Conexión a LocalDB confirmada: cinco productos y una categoría existentes.
- HTTP 200: listados y altas de productos/categorías; detalle, edición y confirmación de baja de un producto existente.
- HTTP 404: identificadores inexistentes en detalle/edición de productos y baja de categorías.
- POST de producto vacío con precio/stock negativos, categoría inexistente y enum inválido: HTTP 200 con siete mensajes de validación, sin guardar.
- POST sin token antiforgery: HTTP 400.
- No se modificaron registros existentes ni se ejecutaron altas/bajas válidas como parte de estas comprobaciones. Esas operaciones requieren completar las pruebas manuales anteriores.

## Trabajo entre dos integrantes

Repositorio: https://github.com/33206858/PNT1. La rama principal actual se llama master.

1. El propietario invita al compañero desde Settings > Collaborators. Cada uno usa su propia cuenta e identidad Git.
2. Cada uno clona el repositorio, restaura los paquetes y ejecuta Update-Database contra su propia LocalDB. Las migraciones comparten el esquema; los datos se comparten mediante un script de datos iniciales revisado, no copiando archivos MDF/LDF.
3. Antes de una tarea, actualizar master con `git pull --ff-only` y crear una rama, por ejemplo `git switch -c feature/busqueda-productos`. El pull presupone un árbol limpio; si master diverge, revisar los commits y resolver la integración sin forzar.
4. Hacer commits pequeños con archivos específicos, por ejemplo `git add EUMA/Controllers/ProductosController.cs` y `git commit -m "feat: agregar búsqueda de productos"`.
5. Publicar la rama con `git push -u origin feature/busqueda-productos`, abrir un pull request hacia master y pedir revisión al compañero. Integrar después de compilar y probar.
6. Ambos actualizan master al comenzar la siguiente tarea. Evitar trabajar simultáneamente en la misma funcionalidad y coordinar las migraciones: uno genera la migración, el otro actualiza su código y aplica Update-Database.

Una base Azure compartida sincroniza datos, no archivos de código. Usarla para integración o demostración; las bases locales permiten probar sin interferir con el compañero. Solo una persona aplica migraciones a la base compartida una vez integrado el cambio. No guardar contraseñas en appsettings.json ni en Git: usar User Secrets o ConnectionStrings__ConexionEUMA como variable de entorno. Tener una base en Azure no publica la web: alojar la aplicación es una decisión separada.

La oferta Azure SQL consultada el 26/10/2026 incluye por base 100000 vCore-segundos, 32 GB de datos y 32 GB de backups por mes durante la vigencia de la suscripción. Elegir explícitamente la oferta gratuita y la pausa al agotar el cupo; la modalidad que continúa funcionando puede facturar excedentes. La pausa por agotamiento impide acceder hasta el siguiente mes. Cerrar SSMS al terminar para permitir la pausa por inactividad. La compatibilidad depende de la suscripción; Students Starter no es compatible.

Referencias: https://learn.microsoft.com/en-us/azure/azure-sql/database/free-tier y https://docs.github.com/en/repositories/creating-and-managing-repositories/access-to-repositories

## Nombre del proyecto

La solución y el proyecto se llaman EUMA: abrir EUMA.slnx. El código utiliza el namespace EUMA y la configuración ConexionEUMA. La conexión conserva Database=CEUMA para seguir usando la base local existente con sus datos; el nombre físico de la base es independiente del proyecto. No hace falta renombrar la base ni generar migraciones por este cambio.
