namespace Simple.Indicators;

using System;
using System.Linq;

public static class TableHelper
{
    static ITable[]? allTables = null;

    private static void initializeTableNames()
    {
        var tInterface = typeof(ITable);
        var asm = tInterface.Assembly;

        var allTableTypes = asm.GetTypes()
                               .Where(tInterface.IsAssignableFrom)
                               .Where(t => t.IsClass && !t.IsInterface)
                               .ToArray();

        allTables = allTableTypes.Select(t => (ITable)Activator.CreateInstance(t)).ToArray();
    }
    public static ITable[] GetAllTables()
    {
        if (allTables == null) initializeTableNames();

        return allTables!;
    }
    public static ITable? GetTableByName(string name)
    {
        if (allTables == null) initializeTableNames();

        return allTables.FirstOrDefault(t => t.GetType().FullName == name);
    }

}
