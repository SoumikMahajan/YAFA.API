using Dapper;
using System.Data;

namespace Yafa.Api.Extension;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class NullableDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly?>
{
    public override DateOnly? Parse(object value)
    {
        if (value is null || value is DBNull)
            return null;

        return DateOnly.FromDateTime((DateTime)value);
    }

    public override void SetValue(IDbDataParameter parameter, DateOnly? value)
    {
        parameter.Value = value.HasValue
            ? value.Value.ToDateTime(TimeOnly.MinValue)
            : DBNull.Value;

        parameter.DbType = DbType.Date;
    }
}
