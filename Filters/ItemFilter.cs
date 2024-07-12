using Neocore.Common;

namespace Neocore.Filters;

public class ItemFilter
{
    public int? VendorId { get; set; }
    public float? MaxPrice { get; set; }
    public bool? HasMultilpleVendors { get; set; }

    public void Apply(QueryBuilder builder)
    {
        if (VendorId.HasValue)
        {
            builder.Where($"{Aliases.Vendor}.id = $vendorId", "vendorId", VendorId.Value);
        }
        if (MaxPrice.HasValue)
        {
            builder.Where($"{Aliases.Item}.price <= $maxPrice", "maxPrice", MaxPrice.Value);
        }
        if (HasMultilpleVendors.HasValue && HasMultilpleVendors.Value)
        {
        builder.With($"{Aliases.Item}, COUNT(DISTINCT {Aliases.Vendor}) as cdv")
            .Where("cdv > 1");
        }
    }
}
