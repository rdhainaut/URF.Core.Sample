using Northwind.Data.ComplexTypes;
using Northwind.Data.Models;
using Northwind.Repository;
using System.Linq;
using URF.Core.Services;

namespace Northwind.Service
{
    public class EmployeeService : Service<Employees>, IEmployeeService
    {
        private IEmployeesRepository _repository;
        private IEmployeeContactRepository _contactRepository;

        public EmployeeService(IEmployeesRepository repository, IEmployeeContactRepository contactRepository) : base(repository)
        {
            _repository = repository;
            _contactRepository = contactRepository;
        }

        public IQueryable<Employees> Get25YearWorkingEmployees()
        {
            return _repository.Get25YearWorkingEmployees();
        }

        public IQueryable<EmployeeContacts> GetEmployeeContacts()
        {
            return _contactRepository.GetEmployeeContacts();
        }
    }
}