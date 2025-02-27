using Microsoft.AspNetCore.Identity;

namespace WebConcessionariasVeiculos.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public int ConcessionariaId { get; set; }
        public Concessionaria Concessionaria { get; set; }
        public int FabricanteId { get; set; }
        public Fabricante Fabricante { get; set; }
        public int VeiculoId { get; set; }
        public Veiculo Veiculo { get; set; }
        public string NomeCliente { get; set; }
        public string CPF { get; set; }
        public string Telefone { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal PrecoVenda { get; set; }
        public string NumeroProtocolo { get; set; }
        public bool Ativo { get; set; } = true;
    }

   // public class Usuario : IdentityUser
    //{
    //    public string NomeCompleto { get; set; }
     //   public string Cargo { get; set; }
    //}
}
