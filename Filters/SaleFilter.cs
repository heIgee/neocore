using Neocore.Common;

namespace Neocore.Filters;

public class SaleFilter: IFilter
{
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public int? EmployeeId { get; set; }
    public int? CustomerId { get; set; }
    public string? ItemType { get; set; }

    public void Apply(QueryBuilder builder)
    {
        builder.With($"{Al.Sale}, {Al.Employee}, {Al.Customer}, {Al.SoldItemList}");
        
        if (DateFrom.HasValue)
        {
            builder.Where($"{Al.Sale}.date >= date($dateFrom)", "dateFrom", DateFrom.Value);
        }
        
        if (DateTo.HasValue)
        {
            builder.Where($"{Al.Sale}.date <= date($dateTo)", "dateTo", DateTo.Value);
        }
        
        if (EmployeeId.HasValue)
        {
            builder.Where($"{Al.Employee}.id = $employeeId", "employeeId", EmployeeId.Value);
        }
            
        if (CustomerId.HasValue)
        {
            builder.Where($"{Al.Customer}.id = $customerId", "customerId", CustomerId.Value);
        }
        
        if (!string.IsNullOrEmpty(ItemType))
        {
            builder.Where($"ANY(soldItem IN {Al.SoldItemList} WHERE toLower(soldItem.item.type) CONTAINS toLower($itemType))", "itemType", ItemType);
        }
    }
}
