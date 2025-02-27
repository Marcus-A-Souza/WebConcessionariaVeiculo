using Microsoft.AspNetCore.Identity;

namespace WebConcessionariasVeiculos.Models
{
    public class Usuario : IdentityUser
    {
        public string NomeCompleto { get; set; } // Adiciona novos campos ao Identity
    }
}
