using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Base.Helper;

public static class DllHelper
{
    private static Assembly[] _assembly = null!;

    public static Assembly[] GetHotfixAssembly()
    {
        return _assembly;
    }

    class HostAssemblyLoadContext : AssemblyLoadContext
    {
        public HostAssemblyLoadContext() : base(true)
        {
        }

        protected override Assembly? Load(AssemblyName name)
        {
            return null;
        }
    }

    private static HostAssemblyLoadContext? _context;

    public static void LoadAssembly<T>()
    {
        if (_context != null)
        {
            _context.Unload();
            _context = null;
        }

        var t = typeof(T);
        string currentAssemblyDirectory = Environment.CurrentDirectory;
        var modelName = t.Name.Split(".")[0];
        string dllPath = Path.Combine(currentAssemblyDirectory, $"{modelName}.Hotfix.dll");

        _context = new HostAssemblyLoadContext();
        var assemblyTemp = new[]
        {
            typeof(T).Assembly, //Model
            typeof(Game).Assembly, //Base
            _context.LoadFromAssemblyPath(dllPath) //Hotfix
        };
        _assembly = assemblyTemp;
    }
}