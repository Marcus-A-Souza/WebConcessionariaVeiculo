using WebConcessionariasVeiculos.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebConcessionariasVeiculos.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Modelo { get; set; }
        public int AnoFabricacao { get; set; }
        public decimal Preco { get; set; }
        public TipoVeiculo Tipo { get; set; }
        public string Descricao { get; set; }
        public int FabricanteId { get; set; }
        public Fabricante Fabricante { get; set; }
        public bool Ativo { get; set; } = true;


    }
    public enum TipoVeiculo
    {
        Carro,
        Moto,
        Caminhao
    }

}
