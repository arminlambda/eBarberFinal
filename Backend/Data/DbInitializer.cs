using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using eBarber.Models;

namespace eBarber.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<BarberContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Administrator", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = "admin@ebarber.si";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "eBarber",
                EmailConfirmed = true,
                DateCreated = DateTime.Now
            };
            
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Administrator");
            
            var frizer = new Frizer
            {
                Ime = "Admin",
                Priimek = "Frizer",
                Email = adminEmail,
                UserId = adminUser.Id
            };
            context.Frizerji.Add(frizer);
            await context.SaveChangesAsync();
        }
        if (!context.Lokacije.Any())
        {
            var frizer = await context.Frizerji.FirstOrDefaultAsync();
            if (frizer != null)
            {
                context.Lokacije.AddRange(
                    new Lokacija
                    {
                        Naslov = "Dom na Poljanski cesti",
                        Mesto = "Ljubljana",
                        DelovniCasOd = new TimeSpan(9, 0, 0),
                        DelovniCasDo = new TimeSpan(19, 0, 0),
                        FrizerId = frizer.Id
                    },
                    new Lokacija
                    {
                        Naslov = "Frankolovo 10A",
                        Mesto = "Frankolovo",
                        DelovniCasOd = new TimeSpan(10, 0, 0),
                        DelovniCasDo = new TimeSpan(18, 0, 0),
                        FrizerId = frizer.Id
                    }
                );
                await context.SaveChangesAsync();
            }
        }
        if (!context.OkvirniTermini.Any())
        {
            var lokacija1 = await context.Lokacije.FirstOrDefaultAsync();
            if (lokacija1 != null)
            {
                context.OkvirniTermini.AddRange(
                    new OkvirniTermin
                    {
                        ZacetekCasa = DateTime.Now.AddDays(7).Date.AddHours(17),
                        KonecCasa = DateTime.Now.AddDays(7).Date.AddHours(20),
                        LokacijaId = lokacija1.Id,
                        MaksimalnoUporabnikov = 5,
                        Opis = "Okvirni termin za striženje",
                        JeAktiven = true
                    },
                    new OkvirniTermin
                    {
                        ZacetekCasa = DateTime.Now.AddDays(10).Date.AddHours(14),
                        KonecCasa = DateTime.Now.AddDays(10).Date.AddHours(18),
                        LokacijaId = lokacija1.Id,
                        MaksimalnoUporabnikov = 8,
                        Opis = "Popoldanski termini",
                        JeAktiven = true
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
    
}