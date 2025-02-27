using WebConcessionariasVeiculos.Data;
using WebConcessionariasVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebConcessionariasVeiculos.Controllers
{
    [Authorize(Roles = "Gerente")] // Permite acesso a Gerentes e Vendedores
    public class VeiculoController : Controller
    {
        private readonly DbConcessionariaContext _context;

        public VeiculoController(DbConcessionariaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var veiculos = await _context.Veiculos
               .Include(v => v.Fabricante) // Carregar o fabricante junto com o veículo
               .Where(v => v.Ativo)
               .ToListAsync();
            return View(veiculos);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Fabricantes = new SelectList(await _context.Fabricantes.ToListAsync(), "Id", "Nome");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Veiculo veiculo)
        {
            if (!ModelState.IsValid)
            {
                if (await _context.Veiculos.AnyAsync(v => v.Modelo == veiculo.Modelo))
                {
                    ModelState.AddModelError("Modelo", "Já existe um veículo com esse modelo.");
                    return View(veiculo);
                }

                _context.Add(veiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(veiculo);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null || !veiculo.Ativo)
            {
                return NotFound();
            }
            return View(veiculo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Veiculos.Any(v => v.Id == veiculo.Id))
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
            return View(veiculo);
        }

        public async Task<IActionResult> Details(int id)
        {
            var veiculo = await _context.Veiculos
                .Include(v => v.Fabricante) // Carrega os dados do Fabricante
                .FirstOrDefaultAsync(v => v.Id == id && v.Ativo);

            if (veiculo == null)
            {
                return NotFound();
            }

            return View(veiculo);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo != null)
            {
                veiculo.Ativo = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
