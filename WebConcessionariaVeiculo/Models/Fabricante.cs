namespace WebConcessionariasVeiculos.Models
{
    public class Fabricante
    {
        public int Id { get; set; }
        public string Nome { get; set; } // Máximo 100 caracteres, único
        public string PaisOrigem { get; set; } // Máximo 50 caracteres
        public int AnoFundacao { get; set; } // Ano válido no passado
        public string Website { get; set; } // Validação de URL
        public bool Ativo { get; set; } = true; // Deleção lógica

       

    }

}
