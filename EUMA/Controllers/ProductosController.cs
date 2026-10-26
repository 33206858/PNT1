using EUMA.Data;
using EUMA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace EUMA.Controllers;

public class ProductosController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index() => View(await context.Productos.Include(x => x.Categoria).AsNoTracking().OrderBy(x => x.Nombre).ToListAsync());
    public async Task<IActionResult> Details(int? id)
    {
        var item = await context.Productos.Include(x => x.Categoria).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : View(item);
    }
    public async Task<IActionResult> Create()
    {
        var item = new Producto(); await Opciones(item); return View(item);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Codigo,Nombre,Descripcion,CategoriaId,Origen,Presentacion,Precio,Stock,Imagen,Activo")] Producto item)
    {
        await Validar(item);
        if (ModelState.IsValid) { context.Add(item); if (await Guardar()) return RedirectToAction(nameof(Index)); }
        await Opciones(item); return View(item);
    }
    public async Task<IActionResult> Edit(int? id)
    {
        var item = await context.Productos.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        await Opciones(item); return View(item);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,Descripcion,CategoriaId,Origen,Presentacion,Precio,Stock,Imagen,Activo")] Producto item)
    {
        if (id != item.Id) return NotFound();
        var actual = await context.Productos.FindAsync(id);
        if (actual is null) return NotFound();
        await Validar(item);
        if (ModelState.IsValid)
        {
            actual.Codigo = item.Codigo;
            actual.Nombre = item.Nombre;
            actual.Descripcion = item.Descripcion;
            actual.CategoriaId = item.CategoriaId;
            actual.Origen = item.Origen;
            actual.Presentacion = item.Presentacion;
            actual.Precio = item.Precio;
            actual.Stock = item.Stock;
            actual.Imagen = item.Imagen;
            actual.Activo = item.Activo;
            if (await Guardar()) return RedirectToAction(nameof(Index));
        }
        await Opciones(item); return View(item);
    }
    public async Task<IActionResult> Delete(int? id)
    {
        var item = await context.Productos.Include(x => x.Categoria).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : View(item);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await context.Productos.Include(x => x.Categoria).FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        item.Activo = false;
        if (await Guardar()) return RedirectToAction(nameof(Index));
        return View("Delete", item);
    }
    private async Task Validar(Producto item)
    {
        item.Nombre = (item.Nombre ?? "").Trim();
        if (item.Nombre.Length < 3) ModelState.AddModelError("Nombre", "Ingresá al menos tres caracteres.");
        item.Codigo = (item.Codigo ?? "").Trim();
        if (string.IsNullOrWhiteSpace(item.Codigo)) ModelState.AddModelError("Codigo", "Ingresá un código.");
        if (await context.Productos.AnyAsync(x => x.Codigo == item.Codigo && x.Id != item.Id)) ModelState.AddModelError("Codigo", "El código ya existe, incluso entre los productos inactivos.");
        if (!await context.Categorias.AnyAsync(x => x.Id == item.CategoriaId)) ModelState.AddModelError("CategoriaId", "Seleccioná una categoría existente.");
    }
    private async Task Opciones(Producto item)
    {
        ViewBag.CategoriaId = new SelectList(await context.Categorias.AsNoTracking()
            .OrderBy(x => x.Nombre).ToListAsync(), "Id", "Nombre", item.CategoriaId);
    }
    private async Task<bool> Guardar()
    {
        try { await context.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { ModelState.AddModelError("", "El registro cambió o fue eliminado. Volvé a cargar la página."); }
        catch (DbUpdateException) { ModelState.AddModelError("", "No se pudo guardar. Revisá los datos únicos y las relaciones y volvé a intentar."); }
        return false;
    }
}
