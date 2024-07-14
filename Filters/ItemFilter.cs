using Neocore.Common;

namespace Neocore.Filters;

public class ItemFilter
{
    public int? VendorId { get; set; }
    public float? MaxPrice { get; set; }
    public bool? HasMultilpleVendors { get; set; }

    public void Apply(QueryBuilder builder)
    {
        //builder.Where("($maxPrice IS NULL OR i.price <= $maxPrice)", "maxPrice", MaxPrice);

        //if (VendorId.HasValue)
        //{
        //    builder.Where("($vendorId IS NULL OR (v.id = $vendorId AND v IS NOT NULL))", "vendorId", VendorId);
        //}

        //if (HasMultilpleVendors.HasValue)
        //{
        //    builder.With("i, COUNT(DISTINCT v) as vendorCount")
        //        .Where("($hasMultipleVendors IS NULL OR " +
        //            "($hasMultipleVendors = true AND vendorCount > 1) OR " +
        //            "($hasMultipleVendors = false AND vendorCount <= 1))", 
        //            "hasMultipleVendors", HasMultilpleVendors);
        //}


        if (MaxPrice.HasValue)
        {
            builder.With($"{Aliases.Item}, {Aliases.Vendor}")
                .Where($"{Aliases.Item}.price <= $maxPrice", "maxPrice", MaxPrice.Value);
        }
        if (VendorId.HasValue)
        {
            builder.With($"{Aliases.Item}, {Aliases.Vendor}")
                .Where($"{Aliases.Vendor}.id = $vendorId", "vendorId", VendorId.Value);
        }
        else if (HasMultilpleVendors.HasValue && HasMultilpleVendors.Value)
        {
            builder.With($"{Aliases.Item}, COUNT(DISTINCT {Aliases.Vendor}) as cdv")
                .Where("cdv > 1");
        }
    }
}
