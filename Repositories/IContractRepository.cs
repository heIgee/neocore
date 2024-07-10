﻿using Neocore.Filters;
using Neocore.Models;

namespace Neocore.Repositories;
public interface IContractRepository
{
    Task<IEnumerable<Contract>> FindAll();
    Task<IEnumerable<Contract>> FindByFilter(ContractFilter filter);
    Task<Contract> FindById(int id);
    Task<IEnumerable<Contract>> FindByVendor(int vendorId);
}