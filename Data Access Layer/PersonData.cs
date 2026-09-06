using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using GameZoneBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data_Access_Layer;

public class PersonData : BaseData
{
    public PersonData(IConfiguration config) : base(config) { }

    public async Task<Person?> GetPersonByEmailAsync(string Email)
    {
        using var context = CreateDbContext();
        var person = await context.People
            .AsNoTracking()
            .Include(p => p.Grade)
            .Include(p => p.User)
                .ThenInclude(u => u.Status)
            .Include(p => p.Employee)
                .ThenInclude(e => e.Status)
            .FirstOrDefaultAsync(p => p.Email == Email);
        return person;
    }
    public async Task<int?> GetPersonIdByEmailAsync(string Email)
    {
        using var context = CreateDbContext();
        var personId = await context.People
    .AsNoTracking()
    .Where(p => p.Email == Email)
    .Select(p => (int?)p.PersonId)
    .SingleOrDefaultAsync();
        return personId;
    }
    async public Task<Person?> AddNewPerson(Person person)
    {
        using var context = CreateDbContext();
        await context.People.AddAsync(person);
        await context.SaveChangesAsync();
        return person;
    }
}
