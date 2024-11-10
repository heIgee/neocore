using Neocore.Common;

namespace Neocore.Filters;

public interface IFilter
{
    void Apply(QueryBuilder builder);
}
