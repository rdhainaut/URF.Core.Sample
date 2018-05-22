using Northwind.Data.ComplexTypes;
using Northwind.Data.Models;
using System.Collections.Generic;
using System.Linq;
using URF.Core.Abstractions.Services;

namespace Northwind.Service
{
    public interface IEmployeeService : IService<Employees>
    {
        IQueryable<Employees> Get25YearWorkingEmployees();
        IQueryable<EmployeeContacts> GetEmployeeContacts();
    }
}