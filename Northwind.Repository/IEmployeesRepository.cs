using Northwind.Data.Models;
using System.Linq;
using URF.Core.Abstractions.Trackable;

// Example: extending IRepository<TEntity> and/or ITrackableRepository<TEntity>, scope: application-wide across all IRepositoryX<TEntity>
namespace Northwind.Repository
{
    public interface IEmployeesRepository : ITrackableRepository<Employees>
    {
        IQueryable<Employees> Get25YearWorkingEmployees();
    }
}