namespace WebConcessionariasVeiculos.Models
{
    public class Concessionaria
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public int CapacidadeMaxima { get; set; }
        public bool Ativo { get; set; } = true; // Deleção lógica

       
    }
}
