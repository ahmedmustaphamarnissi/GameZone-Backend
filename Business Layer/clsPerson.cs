using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer;
using GameZoneBack.Models;
using Microsoft.Extensions.Configuration;
using Models.Data.enums;
using Models.DTO;

namespace Business_Layer
{
    public class clsPerson : BaseService
    {
        public clsPerson(IConfiguration config) : base(config) { }
        async public Task<Person?> GetPersonByEmailAsync(string Email)
        {
            var personData = new PersonData(_config);
            var person = await personData.GetPersonByEmailAsync(Email);
            
            return person;
        }

        async public Task<Person?> AddNewPersonAsync(PersonDTO personData)
        {

            var per = new Person
            {
                FirstName = personData.FirstName,
                LastName = personData.LastName,
                Email = personData.Email,
                CountryId = personData.CountryId,
                DateOfBirth = personData.DateOfBirth,
                GradeId = (int)PersonGrade.Customer
            };
            var Data = new PersonData(_config);
            Person? person = await Data.AddNewPerson(per);
            if (person == null)
                throw new ArgumentException("person is not added");
            return person;
        }

        public async Task<int?> GetPersonIdByEmailAsync(string Email)
        {
            var Data = new PersonData(_config);
            return await Data.GetPersonIdByEmailAsync(Email);
        }

    }
}
