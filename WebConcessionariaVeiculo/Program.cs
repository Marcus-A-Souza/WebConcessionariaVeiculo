using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebConcessionariasVeiculos.Models;
using WebConcessionariasVeiculos.Data;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do banco de dados
builder.Services.AddDbContext<DbConcessionariaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") + ";TrustServerCertificate=True"));

// Configura��o do Identity
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<DbConcessionariaContext>()
.AddDefaultTokenProviders();

// Habilita autentica��o e autoriza��o
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AcessoNegado";
});

// Adicionando servi�os ao container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Criar roles e usu�rio administrador automaticamente ao iniciar a aplica��o
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<Usuario>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    try
    {
        await IdentitySeeder.SeedRolesAndUsersAsync(userManager, roleManager);
        Console.WriteLine("Usu�rios e pap�is criados/inicializados com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao criar usu�rios e roles: {ex.Message}");
    }

    // Redefinir a senha do usu�rio administrador
    var user = await userManager.FindByEmailAsync("adm@empresa.com");
    if (user != null)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, "123456");

        if (result.Succeeded)
        {
            Console.WriteLine("Senha redefinida com sucesso!");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Erro ao redefinir senha: {error.Description}");
            }
        }
    }

}
// Criar roles e usu�rio administrador automaticamente ao iniciar a aplica��o
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<Usuario>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    try
    {
        await IdentitySeeder.SeedRolesAndUsersAsync(userManager, roleManager);
        Console.WriteLine("Usu�rios e pap�is criados/inicializados com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao criar usu�rios e roles: {ex.Message}");
    }

    // Redefinir a senha do usu�rio administrador
    var user = await userManager.FindByEmailAsync("vendedor@empresa.com");
    if (user != null)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, "123456");

        if (result.Succeeded)
        {
            Console.WriteLine("Senha redefinida com sucesso!");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Erro ao redefinir senha: {error.Description}");
            }
        }
    }

}

// Configura��o do pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Necess�rio para login funcionar
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
