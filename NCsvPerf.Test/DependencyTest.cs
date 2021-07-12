using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Knapcode.NCsvPerf
{
    public class DependencyTest
    {
        [Fact]
        public void AllReferencesAreRelease()
        {
            foreach (var assemblyName in typeof(Program).Assembly.GetReferencedAssemblies())
            {
                var assembly = Assembly.Load(assemblyName);
                var isDebug = assembly.GetCustomAttributes<DebuggableAttribute>().Any(x => x.IsJITOptimizerDisabled);
                Assert.False(isDebug, $"Assembly '{assemblyName}' is not compiled as Release.");
            }
        }
    }
}
