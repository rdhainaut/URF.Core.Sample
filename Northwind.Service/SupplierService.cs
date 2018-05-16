using Northwind.Data.Models;
using URF.Core.Abstractions.Trackable;
using URF.Core.Services;

namespace Northwind.Service
{
    public class SupplierService : Service<Suppliers>, ISupplierService
    {
        public SupplierService(ITrackableRepository<Suppliers> repository) : base(repository)
        {
        }
    }
}
