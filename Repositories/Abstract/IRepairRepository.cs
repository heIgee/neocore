using Neocore.Models;
using Neocore.ViewModels;

namespace Neocore.Repositories.Abstract;
public interface IRepairRepository
{
    Task Add(RepairExtended repairX, int? oldId = null);
    Task Delete(int id);
    Task<IEnumerable<Repair>> FindAll();
    Task<IEnumerable<RepairExtended>> FindAllExtended();
    Task<Repair?> FindById(int id);
    Task<RepairExtended?> FindExtendedById(int id);
    Task Update(int id, RepairExtended repairX);
}