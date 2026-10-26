using EUMA.Data;
using EUMA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EUMA.Controllers;

public class CategoriasController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index() => View(await context.Categorias.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync());
    public async Task<IActionResult> Details(int? id)
    {
        var item = await context.Categorias.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : View(item);
    }
    public async Task<IActionResult> Create()
    {
        var item = new Categoria(); return View(item);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Descripcion")] Categoria item)
    {
        await Validar(item);
        if (ModelState.IsValid) { context.Add(item); if (await Guardar()) return RedirectToAction(nameof(Index)); }
        return View(item);
    }
    public async Task<IActionResult> Edit(int? id)
    {
        var item = await context.Categorias.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        return View(item);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] Categoria item)
    {
        if (id != item.Id) return NotFound();
        var actual = await context.Categorias.FindAsync(id);
        if (actual is null) return NotFound();
        await Validar(item);
        if (ModelState.IsValid)
        {
            actual.Nombre = item.Nombre;
            actual.Descripcion = item.Descripcion;
            if (await Guardar()) return RedirectToAction(nameof(Index));
        }
        return View(item);
    }
    public async Task<IActionResult> Delete(int? id)
    {
        var item = await context.Categorias.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : View(item);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await context.Categorias.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        if (await context.Productos.AnyAsync(x => x.CategoriaId == id)) { ModelState.AddModelError("", "No se puede eliminar una categoría con productos asociados."); return View("Delete", item); }
        context.Categorias.Remove(item);
        if (await Guardar()) return RedirectToAction(nameof(Index));
        return View("Delete", item);
    }
    private async Task Validar(Categoria item)
    {
        item.Nombre = (item.Nombre ?? "").Trim();
        if (item.Nombre.Length < 3) ModelState.AddModelError("Nombre", "Ingresá al menos tres caracteres.");
        if (await context.Categorias.AnyAsync(x => x.Nombre == item.Nombre && x.Id != item.Id)) ModelState.AddModelError("Nombre", "La categoría ya existe.");
    }
    private async Task<bool> Guardar()
    {
        try { await context.SaveChangesAsync(); return true; }
        catch (DbUpdateConcurrencyException) { ModelState.AddModelError("", "El registro cambió o fue eliminado. Volvé a cargar la página."); }
        catch (DbUpdateException) { ModelState.AddModelError("", "No se pudo guardar. Revisá los datos únicos y las relaciones y volvé a intentar."); }
        return false;
    }
}
