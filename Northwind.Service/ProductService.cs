using Northwind.Data.Models;
using URF.Core.Abstractions.Trackable;
using URF.Core.Services;
using System.Linq;

namespace Northwind.Service
{
    public class ProductService : Service<Products>, IProductService
    {
        public ProductService(ITrackableRepository<Products> repository) : base(repository)
        {
        }

        public decimal CalculateStockCashValue()
        {
            return this.Repository.Queryable().Sum(p => p.UnitPrice * p.UnitsInStock).Value;
        }
    }
}
