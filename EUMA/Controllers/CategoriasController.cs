using EUMA.Data;
using EUMA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EUMA.Controllers;

public class CategoriasController : Controller
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Categorias.OrderBy(x => x.Nombre).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        var item = await _context.Categorias.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    public IActionResult Create()
    {
        var item = new Categoria();
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Descripcion")] Categoria item)
    {
        await Validar(item);
        if (ModelState.IsValid)
        {
            _context.Categorias.Add(item);
            if (await Guardar())
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var item = await _context.Categorias.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] Categoria item)
    {
        if (id != item.Id)
        {
            return NotFound();
        }
        var actual = await _context.Categorias.FindAsync(id);
        if (actual == null)
        {
            return NotFound();
        }
        await Validar(item);
        if (ModelState.IsValid)
        {
            actual.Nombre = item.Nombre;
            actual.Descripcion = item.Descripcion;
            if (await Guardar())
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var item = await _context.Categorias.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Categorias.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        if (await _context.Productos.AnyAsync(x => x.CategoriaId == id))
        {
            ModelState.AddModelError("", "No se puede eliminar una categoría con productos asociados.");
            return View("Delete", item);
        }
        _context.Categorias.Remove(item);
        if (await Guardar())
        {
            return RedirectToAction(nameof(Index));
        }
        return View("Delete", item);
    }

    private async Task Validar(Categoria item)
    {
        item.Nombre = (item.Nombre ?? "").Trim();
        if (await _context.Categorias.AnyAsync(x => x.Nombre == item.Nombre && x.Id != item.Id))
        {
            ModelState.AddModelError("Nombre", "La categoría ya existe.");
        }
    }

    private async Task<bool> Guardar()
    {
        try
        {
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Los cambios se guardaron correctamente.";
            return true;
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "No se pudo guardar. Revisá los datos únicos y las relaciones y volvé a intentar.");
            TempData["Error"] = "No se pudieron guardar los cambios.";
        }
        return false;
    }
}
