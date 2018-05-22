using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Northwind.Service;
using Urf.Core.Abstractions;

namespace Northwind.Api.OData
{
    public class EmployeesController : ODataController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeesController(
            IEmployeeService employeeService,
            IUnitOfWork unitOfWork)
        {
            _employeeService = employeeService;
            _unitOfWork = unitOfWork;
        }

        // GET odata/Employees/Default.Get25YearWorkingEmployees
        public IActionResult Get25YearWorkingEmployees()
        {
            return Ok(_employeeService.Get25YearWorkingEmployees());
        }

        // GET odata/Employees/Default.GetEmployeeContacts
        public IActionResult GetEmployeeContacts()
        {
            return Ok(_employeeService.GetEmployeeContacts());
        }
    }
}
