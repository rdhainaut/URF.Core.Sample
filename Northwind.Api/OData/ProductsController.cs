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

namespace Northwind.Api.OData
{
    public class ProductsController : ODataController
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(
            IProductService productService,
            IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
        }

        #region CRUDFunctions
        // e.g. GET odata/Products?$skip=2&$top=10
        //[EnableQuery(MaxTop = 20, PageSize = 5)]
        [EnableQuery]
        public IQueryable<Products> Get() => _productService.Queryable();

        // e.g.  GET odata/Products(37)
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _productService.FindAsync(key);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // e.g. PUT odata/Products(37)
        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] Products products)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (key != products.ProductId)
                return BadRequest();

            _productService.Update(products);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _productService.ExistsAsync(key))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // e.g. PUT odata/Products
        public async Task<IActionResult> Post([FromBody] Products products)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _productService.Insert(products);
            await _unitOfWork.SaveChangesAsync();

            return Created(products);
        }

        // e.g. PATCH, MERGE odata/Products(37)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<Products> product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _productService.FindAsync(key);
            if (entity == null)
                return NotFound();

            product.Patch(entity);
            _productService.Update(entity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _productService.ExistsAsync(key))
                    return NotFound();
                throw;
            }
            return Updated(entity);
        }

        // e.g. DELETE odata/Products(37)
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.DeleteAsync(key);

            if (!result)
                return NotFound();

            await _unitOfWork.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.NoContent);
        }
        #endregion

        #region Functions
        // GET odata/Products/Default.CalculateStockCashValue
        public IActionResult CalculateStockCashValue()
        {
            return Ok(_productService.CalculateStockCashValue());
        }
        #endregion

        #region Properties

        // GET odata/Products({key})/UnitPrice/$value
        public async Task<IActionResult> GetUnitPrice([FromODataUri] int key)
        {
            var product = await _productService.FindAsync(key);

            if (product == null)
                return NotFound();

            return Ok(product.UnitPrice);
        }

        // GET odata/Products({key})/Supplier
        [EnableQuery]
        [ODataRoute("Products({key})/Supplier")]
        public async Task<IActionResult> GetSupplierFromProduct([FromODataUri] int key)
        {
            var supplier = await _productService.Queryable().Where(p => p.ProductId == key).Select(p => p.Supplier).FirstOrDefaultAsync();

            if (supplier == default(Suppliers))
                return NotFound();

            return Ok(supplier);
        }
        #endregion
    }
}