using System.Data;

namespace Erp.WmsConnector.Shared.Infrastructure.Database;

public static class EnumerableExtentions
{
    public static DataTable ToDataTable<T>(this IEnumerable<T> items, params string[] excludeProperties)
    {
        HashSet<string> excludedProperties = [.. excludeProperties];
        var type = typeof(T);
        var allProperties = type.GetProperties();

        // Order properties by inheritance hierarchy: base class properties first
        var properties = allProperties
            .OrderBy(p => GetInheritanceDepth(p.DeclaringType!))
            .ToList();

        var table = new DataTable();

        foreach (var property in properties)
        {
            if (excludedProperties.Contains(property.Name))
            {
                continue;
            }

            Type propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            table.Columns.Add(property.Name, propertyType);
        }

        foreach (T item in items)
        {
            DataRow row = table.NewRow();

            foreach (var property in properties)
            {
                if (excludedProperties.Contains(property.Name))
                {
                    continue;
                }

                row[property.Name] = property.GetValue(item) ?? DBNull.Value;
            }

            table.Rows.Add(row);
        }

        return table;
    }

    private static int GetInheritanceDepth(Type type)
    {
        Type varyBaseType = typeof(object);
        int depth = 0;
        while (type.BaseType != null && type.BaseType != varyBaseType)
        {
            depth++;
            type = type.BaseType;
        }

        return depth;
    }
}
