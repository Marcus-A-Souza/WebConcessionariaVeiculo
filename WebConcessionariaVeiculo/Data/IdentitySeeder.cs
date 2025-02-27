using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using WebConcessionariasVeiculos.Models;

namespace WebConcessionariasVeiculos.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndUsersAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            try
            {
                // Criar os papéis (Roles) se não existirem
                if (!await roleManager.RoleExistsAsync("Gerente"))
                    await roleManager.CreateAsync(new IdentityRole("Gerente"));

                if (!await roleManager.RoleExistsAsync("Vendedor"))
                    await roleManager.CreateAsync(new IdentityRole("Vendedor"));

                // Criar o usuário administrador (Gerente) caso não exista
                var adminUser = await userManager.FindByEmailAsync("adm@empresa.com");
                if (adminUser == null)
                {
                    var newAdmin = new Usuario
                    {
                        UserName = "Adm",
                        Email = "adm@empresa.com",
                        NomeCompleto = "Administrador",
                        EmailConfirmed = true
                    };

                    var createUser = await userManager.CreateAsync(newAdmin, "123456");
                    if (createUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdmin, "Gerente");
                        Console.WriteLine("Usuário Administrador criado com sucesso!");
                    }
                    else
                    {
                        foreach (var error in createUser.Errors)
                        {
                            Console.WriteLine($"Erro ao criar usuário Administrador: {error.Description}");
                        }
                    }
                }

                // Criar um usuário vendedor caso não exista
                var vendedorUser = await userManager.FindByEmailAsync("vendedor@empresa.com");
                if (vendedorUser == null)
                {
                    var newVendedor = new Usuario
                    {
                        UserName = "Vendedor",
                        Email = "vendedor@empresa.com",
                        NomeCompleto = "Vendedor da Concessionária",
                        EmailConfirmed = true
                    };

                    var createVendedor = await userManager.CreateAsync(newVendedor, "123456");
                    if (createVendedor.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newVendedor, "Vendedor");
                        Console.WriteLine("Usuário Vendedor criado com sucesso!");
                    }
                    else
                    {
                        foreach (var error in createVendedor.Errors)
                        {
                            Console.WriteLine($"Erro ao criar usuário Vendedor: {error.Description}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar papéis e usuários: {ex.Message}");
            }
        }
    }
}
