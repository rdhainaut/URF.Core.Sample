using Northwind.Data.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Northwind.Repository
{
    public interface IEmployeeContactRepository
    {
        IQueryable<EmployeeContacts> GetEmployeeContacts();
    }
}