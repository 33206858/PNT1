using EUMA.Data;
using EUMA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EUMA.Controllers;

public class ProductosController : Controller
{
    private readonly AppDbContext _context;

    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Productos.Include(x => x.Categoria).OrderBy(x => x.Nombre).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        var item = await _context.Productos.Include(x => x.Categoria).FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    public async Task<IActionResult> Create()
    {
        var item = new Producto();
        await Opciones(item);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Codigo,Nombre,Descripcion,CategoriaId,Presentacion,Precio,Stock,Imagen")] Producto item)
    {
        item.Activo = true;
        await Validar(item);
        if (ModelState.IsValid)
        {
            _context.Productos.Add(item);
            if (await Guardar())
            {
                return RedirectToAction(nameof(Index));
            }
        }
        await Opciones(item);
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var item = await _context.Productos.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        await Opciones(item);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,Descripcion,CategoriaId,Presentacion,Precio,Stock,Imagen,Activo")] Producto item)
    {
        if (id != item.Id)
        {
            return NotFound();
        }
        var actual = await _context.Productos.FindAsync(id);
        if (actual == null)
        {
            return NotFound();
        }
        await Validar(item);
        if (ModelState.IsValid)
        {
            actual.Codigo = item.Codigo;
            actual.Nombre = item.Nombre;
            actual.Descripcion = item.Descripcion;
            actual.CategoriaId = item.CategoriaId;
            actual.Presentacion = item.Presentacion;
            actual.Precio = item.Precio;
            actual.Stock = item.Stock;
            actual.Imagen = item.Imagen;
            actual.Activo = item.Activo;
            if (await Guardar())
            {
                return RedirectToAction(nameof(Index));
            }
        }
        await Opciones(item);
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var item = await _context.Productos.Include(x => x.Categoria).FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Productos.Include(x => x.Categoria).FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        item.Activo = false;
        if (await Guardar())
        {
            return RedirectToAction(nameof(Index));
        }
        return View("Delete", item);
    }

    private async Task Validar(Producto item)
    {
        item.Nombre = (item.Nombre ?? "").Trim();
        item.Codigo = (item.Codigo ?? "").Trim();
        if (await _context.Productos.AnyAsync(x => x.Codigo == item.Codigo && x.Id != item.Id))
        {
            ModelState.AddModelError("Codigo", "El código ya existe, incluso entre los productos inactivos.");
        }
        if (!await _context.Categorias.AnyAsync(x => x.Id == item.CategoriaId))
        {
            ModelState.AddModelError("CategoriaId", "Seleccioná una categoría existente.");
        }
    }

    private async Task Opciones(Producto item)
    {
        ViewBag.CategoriaId = new SelectList(await _context.Categorias.OrderBy(x => x.Nombre).ToListAsync(), "Id", "Nombre", item.CategoriaId);
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
