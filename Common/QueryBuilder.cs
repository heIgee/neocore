using System.Text;

namespace Neocore.Common;

public class QueryBuilder
{
    private readonly StringBuilder _query = new();
    private readonly Dictionary<string, object> _parameters = [];

    private bool _whereAdded = false;
    private bool _matchAdded = false;
    private bool _returnAdded = false;

    public QueryBuilder Match(string match)
    {
        if (_matchAdded)
            throw new InvalidOperationException($"MATCH clause has already been added. Query: {_query}");

        _query.Append($" MATCH {match}");
        _matchAdded = true;
        return this;
    }

    public QueryBuilder Where(string condition, string? paramName = null, object? paramValue = null)
    {
        _query.Append(_whereAdded ? " AND" : " WHERE");
        _query.Append($" {condition}");
        if (!string.IsNullOrEmpty(paramName) && paramValue is not null)
            _parameters[paramName] = paramValue;
        _whereAdded = true;
        return this;
    }

    public QueryBuilder With(string withClause)
    {
        _query.Append($" WITH {withClause}");
        _whereAdded = false;
        return this;
    }

    public QueryBuilder OptionalMatch(string match)
    {
        _query.Append($" OPTIONAL MATCH {match}");
        return this;
    }

    public QueryBuilder Return(string returnClause)
    {
        if (_returnAdded)
            throw new InvalidOperationException($"RETURN clause has already been added. Query: {_query}");

        _query.Append($" RETURN {returnClause}");
        _returnAdded = true;
        return this;
    }

    public (string Query, Dictionary<string, object> Parameters) Build()
    {
        return (_query.ToString().Trim(), _parameters);
    }
}
