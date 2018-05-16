#region

using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Data.Models;
using Northwind.Service;
using TrackableEntities.Common.Core;
using Urf.Core.Abstractions;

#endregion

namespace Northwind.Api.OData
{
    public class SuppliersController : ODataController
    {
        private readonly ISupplierService _supplierService;
        private readonly IUnitOfWork _unitOfWork;

        public SuppliersController(
            ISupplierService supplierService,
            IUnitOfWork unitOfWork)
        {
            _supplierService = supplierService;
            _unitOfWork = unitOfWork;
        }

        // e.g. GET odata/Suppliers
        [EnableQuery]
        public IQueryable<Suppliers> Get() => _supplierService.Queryable();

        // e.g.  GET odata/Customers(37)
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var supplier = await _supplierService.FindAsync(key);

            if (supplier == null)
                return NotFound();

            return Ok(supplier);
        }
    }
}