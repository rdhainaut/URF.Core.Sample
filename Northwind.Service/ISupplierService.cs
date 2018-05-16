using System;
using System.Linq.Expressions;
using Northwind.Data.Models;
using URF.Core.Abstractions.Services;

namespace Northwind.Service
{
    public interface ISupplierService : IService<Suppliers>
    {
    }
}