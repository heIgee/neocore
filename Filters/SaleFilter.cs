using Neocore.Common;

namespace Neocore.Filters;

public class SaleFilter: IFilter
{
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public string? ItemType { get; set; }
    public int? EmployeeId { get; set; }

    public void Apply(QueryBuilder builder)
    {
        // Always need to keep track of all nodes and relationships we're filtering on
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
        
        if (!string.IsNullOrEmpty(ItemType))
        {
            // Need to match the type of items in the sold items list
            builder.Where($"ANY(item IN {Al.SoldItemList} WHERE item.type = $itemType)", "itemType", ItemType);
        }
    }


    //if (VendorId.HasValue)
    //{
    //    builder.With($"{Al.Contract}, {Al.Vendor}")
    //        .Where($"{Al.Vendor}.id = $vendorId", "vendorId", VendorId.Value);
    //}

    //if (DeliveryDateFrom.HasValue)
    //{
    //    builder.With($"{Al.Contract}, {Al.Vendor}")
    //        .Where($"{Al.Contract}.deliveryDate >= $deliveryDateFrom", "deliveryDateFrom", DeliveryDateFrom.Value);
    //}
}
