using WebConcessionariasVeiculos.Data;
using WebConcessionariasVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebConcessionariasVeiculos.Controllers
{
    [Authorize(Roles = "Gerente")]

    [Route("Fabricante")] // Definição explícita de rota para evitar conflito
    public class FabricanteController : Controller
    {
        private readonly DbConcessionariaContext _context;

        public FabricanteController(DbConcessionariaContext context)
        {
            _context = context;
        }

        // Definição explícita de rota para evitar ambiguidade
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Fabricantes.Where(f => f.Ativo).ToListAsync());
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(Fabricante fabricante)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Fabricantes.AnyAsync(f => f.Nome == fabricante.Nome))
                {
                    ModelState.AddModelError("Nome", "O nome do fabricante já existe.");
                    return View(fabricante);
                }

                if (fabricante.AnoFundacao > System.DateTime.Now.Year)
                {
                    ModelState.AddModelError("AnoFundacao", "O ano de fundação deve estar no passado.");
                    return View(fabricante);
                }

                _context.Add(fabricante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fabricante);
        }

        [HttpGet]
        [Route("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var fabricante = await _context.Fabricantes.FindAsync(id);
            if (fabricante == null || !fabricante.Ativo)
            {
                return NotFound();
            }
            return View(fabricante);
        }

        [HttpPost]
        [Route("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, Fabricante fabricante)
        {
            if (id != fabricante.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fabricante);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Fabricantes.Any(f => f.Id == fabricante.Id))
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
            return View(fabricante);
        }

        [HttpGet]
        [Route("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var fabricante = await _context.Fabricantes.FirstOrDefaultAsync(f => f.Id == id && f.Ativo);
            if (fabricante == null)
            {
                return NotFound();
            }
            return View(fabricante);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var fabricante = await _context.Fabricantes.FindAsync(id);
            if (fabricante == null)
            {
                return NotFound();
            }
            return View(fabricante);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fabricante = await _context.Fabricantes.FindAsync(id);
            if (fabricante != null)
            {
                fabricante.Ativo = false; // Deleção lógica
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
