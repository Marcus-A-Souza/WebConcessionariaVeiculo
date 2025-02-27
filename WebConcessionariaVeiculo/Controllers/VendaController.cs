using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using WebConcessionariasVeiculos.Data;
using WebConcessionariasVeiculos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebConcessionariasVeiculos.Controllers
{
    [Authorize(Roles = "Vendedor")] // Apenas vendedores podem acessar
    public class VendaController : Controller
    {
        private readonly DbConcessionariaContext _context;

        public VendaController(DbConcessionariaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vendas = await _context.Vendas
                .Include(v => v.Veiculo)
                .Include(v => v.Fabricante)
                .Include(v => v.Concessionaria)
                .Where(v => v.Ativo)
                .ToListAsync();
            return View(vendas);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Concessionarias = new SelectList(
                await _context.Concessionarias.Where(c => c.Ativo).ToListAsync(),
                "Id",
                "Nome"
            );

            ViewBag.Fabricantes = new SelectList(
                await _context.Fabricantes.Where(f => f.Ativo).ToListAsync(),
                "Id",
                "Nome"
            );

            ViewBag.Veiculos = new SelectList(
                await _context.Veiculos.Where(v => v.Ativo).Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = v.Modelo
                }).ToListAsync(),
                "Value",
                "Text"
            );

            return View();
            var venda = new Venda
            {
                DataVenda = DateTime.Now // Inicializa a data corretamente
            };
            return View(venda);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Venda venda)
        {
            if (!ModelState.IsValid)
            {
                // Verificar se já existe uma venda ativa com este CPF
                if (await _context.Vendas.AnyAsync(v => v.CPF == venda.CPF && v.Ativo))
                {
                    ModelState.AddModelError("CPF", "Já existe uma venda ativa com este CPF.");
                    return ReloadCreateView(venda);
                }

                // Garantir que a data da venda não seja futura
                if (venda.DataVenda > DateTime.Now)
                {
                    ModelState.AddModelError("DataVenda", "A data da venda não pode ser no futuro.");
                    return ReloadCreateView(venda);
                }

                // Verificar se o preço da venda não é maior que o preço do veículo
                var veiculo = await _context.Veiculos.FindAsync(venda.VeiculoId);
                if (veiculo != null && venda.PrecoVenda > veiculo.Preco)
                {
                    ModelState.AddModelError("PrecoVenda", "O preço da venda não pode ser maior que o preço do veículo.");
                    return ReloadCreateView(venda);
                }

                // Geração de número de protocolo único
                venda.NumeroProtocolo = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
                _context.Add(venda);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return ReloadCreateView(venda);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var venda = await _context.Vendas.FindAsync(id);
            if (venda == null || !venda.Ativo)
            {
                return NotFound();
            }
            return View(venda);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Venda venda)
        {
            if (id != venda.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Vendas.Any(v => v.Id == venda.Id))
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
            return View(venda);
        }

        public async Task<IActionResult> Details(int id)
        {
            var venda = await _context.Vendas
             .Include(v => v.Veiculo)
             .ThenInclude(v => v.Fabricante) // Inclui o Fabricante corretamente
             .Include(v => v.Concessionaria)
             .FirstOrDefaultAsync(v => v.Id == id && v.Ativo);
            if (venda == null)
            {
                return NotFound();
            }
            return View(venda);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var venda = await _context.Vendas.FindAsync(id);
            if (venda != null)
            {
                venda.Ativo = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Método para recarregar ViewBag caso o formulário tenha erro
        private IActionResult ReloadCreateView(Venda venda)
        {
            ViewBag.Concessionarias = new SelectList(_context.Concessionarias.Where(c => c.Ativo).ToList(), "Id", "Nome");
            ViewBag.Fabricantes = new SelectList(_context.Fabricantes.Where(f => f.Ativo).ToList(), "Id", "Nome");
            ViewBag.Veiculos = new SelectList(_context.Veiculos.Where(v => v.Ativo).Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = v.Modelo
            }).ToList(), "Value", "Text");

            return View("Create", venda);
        }
    }
}
