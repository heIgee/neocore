using Neocore.Common;

namespace Neocore.Filters;

public class ContractFilter
{
    public int? VendorId { get; set; }
    public DateOnly? DeliveryDateFrom { get; set; }

    public void Apply(QueryBuilder builder)
    {
        if (VendorId.HasValue)
        {
            builder.With($"{Aliases.Contract}, {Aliases.Vendor}")
                .Where($"{Aliases.Vendor}.id = $vendorId", "vendorId", VendorId.Value);
        }

        if (DeliveryDateFrom.HasValue)
        {
            builder.With($"{Aliases.Contract}, {Aliases.Vendor}")
                .Where($"{Aliases.Contract}.deliveryDate >= $deliveryDateFrom", "deliveryDateFrom", DeliveryDateFrom.Value);
        }
    }
}
