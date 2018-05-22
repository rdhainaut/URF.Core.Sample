using Microsoft.EntityFrameworkCore;
using Northwind.Data.ComplexTypes;
using System.Linq;

namespace Northwind.Repository
{
    public class EmployeeContactRepository : IEmployeeContactRepository
    {
        private DbContext _context;

        public EmployeeContactRepository(DbContext context)
        {
            _context = context;
        }

        public IQueryable<EmployeeContacts> GetEmployeeContacts()
        {
            return _context
                .Query<EmployeeContacts>()
                .FromSql(@"SELECT LastName, FirstName, HomePhone FROM[northwind].[dbo].[Employees]")
                .AsNoTracking();
        }
    }
}