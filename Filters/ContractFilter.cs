using Neocore.Common;

namespace Neocore.Filters;

public class ContractFilter : IFilter
{
    public int? VendorId { get; set; }
    public DateOnly? DeliveryDateFrom { get; set; }

    public void Apply(QueryBuilder builder)
    {
        if (VendorId.HasValue)
        {
            builder.With($"{Al.Contract}, {Al.Vendor}")
                .Where($"{Al.Vendor}.id = $vendorId", "vendorId", VendorId.Value);
        }

        if (DeliveryDateFrom.HasValue)
        {
            builder.With($"{Al.Contract}, {Al.Vendor}")
                .Where($"{Al.Contract}.deliveryDate >= $deliveryDateFrom", "deliveryDateFrom", DeliveryDateFrom.Value);
        }
    }
}
