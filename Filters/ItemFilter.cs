using Neocore.Common;

namespace Neocore.Filters;

public class ItemFilter : IFilter
{
    public int? VendorId { get; set; }
    public float? MaxPrice { get; set; }
    public bool? HasMultilpleVendors { get; set; }

    public void Apply(QueryBuilder builder)
    {
        if (MaxPrice.HasValue)
        {
            builder.With($"{Al.Item}, {Al.Vendor}")
                .Where($"{Al.Item}.price <= $maxPrice", "maxPrice", MaxPrice.Value);
        }
        if (VendorId.HasValue)
        {
            builder.With($"{Al.Item}, {Al.Vendor}")
                .Where($"{Al.Vendor}.id = $vendorId", "vendorId", VendorId.Value);
        }
        else if (HasMultilpleVendors.HasValue && HasMultilpleVendors.Value)
        {
            builder.With($"{Al.Item}, COUNT(DISTINCT {Al.Vendor}) as cdv")
                .Where("cdv > 1");
        }
    }
}
