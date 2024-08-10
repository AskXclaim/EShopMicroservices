using System.Reflection;
using Carter;

namespace BuildingBlocks.Utilities;

public static class CarterModule
{
   public static List<Type> GetICarterModules(Assembly assembly)
    {
        var types = assembly.GetTypes();
        var modules = types
            .Where(t =>
                !t.IsAbstract &&
                typeof(ICarterModule).IsAssignableFrom(t)
                && (t.IsPublic || t.IsNestedPublic)
            ).ToList();
        return modules;
    }
}