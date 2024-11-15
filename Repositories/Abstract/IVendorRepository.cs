﻿using Neocore.Models;
using Neocore.ViewModels;

namespace Neocore.Repositories.Abstract;

public interface IVendorRepository
{
    Task<IEnumerable<Vendor>> FindAll();
    Task<IEnumerable<VendorSummary>> FindAllWithSummary();
    Task<Vendor?> FindById(int id);
    //Task<IEnumerable<Employee>> FindByItemType(string productType);
}