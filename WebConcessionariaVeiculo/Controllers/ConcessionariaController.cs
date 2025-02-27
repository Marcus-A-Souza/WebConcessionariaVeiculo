using WebConcessionariasVeiculos.Data;
using WebConcessionariasVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebConcessionariasVeiculos.Controllers;
[Authorize(Roles = "Gerente")]
public class ConcessionariaController : Controller
{
    private readonly DbConcessionariaContext _context;

    public ConcessionariaController(DbConcessionariaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Concessionarias.Where(c => c.Ativo).ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Concessionaria concessionaria)
    {
        if (ModelState.IsValid)
        {
            if (await _context.Concessionarias.AnyAsync(c => c.Nome == concessionaria.Nome))
            {
                ModelState.AddModelError("Nome", "O nome da concessionária já existe.");
                return View(concessionaria);
            }

            if (concessionaria.CapacidadeMaxima <= 0)
            {
                ModelState.AddModelError("CapacidadeMaxima", "A capacidade máxima deve ser um número positivo.");
                return View(concessionaria);
            }

            _context.Add(concessionaria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(concessionaria);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var concessionaria = await _context.Concessionarias.FindAsync(id);
        if (concessionaria == null || !concessionaria.Ativo)
        {
            return NotFound();
        }
        return View(concessionaria);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Concessionaria concessionaria)
    {
        if (id != concessionaria.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(concessionaria);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Concessionarias.Any(c => c.Id == concessionaria.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(concessionaria);
    }

    public async Task<IActionResult> Details(int id)
    {
        var concessionaria = await _context.Concessionarias.FirstOrDefaultAsync(c => c.Id == id && c.Ativo);
        if (concessionaria == null)
        {
            return NotFound();
        }
        return View(concessionaria);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var concessionaria = await _context.Concessionarias.FindAsync(id);
        if (concessionaria != null)
        {
            concessionaria.Ativo = false;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
