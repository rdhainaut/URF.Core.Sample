using Microsoft.EntityFrameworkCore;
using Northwind.Data.Models;
using System.Linq;
using URF.Core.EF.Trackable;

namespace Northwind.Repository
{
    public class EmployeesRepository : TrackableRepository<Employees>, IEmployeesRepository
    {
        public EmployeesRepository(DbContext context) : base(context)
        {

        }

        public IQueryable<Employees> Get25YearWorkingEmployees()
        {
            return this.Context
                .Set<Employees>()
                .FromSql(@"SELECT * FROM[northwind].[dbo].[Employees] WHERE HireDate < DateAdd(yy, -25, GETDATE())")
                .AsNoTracking();
        }
    }
}