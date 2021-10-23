using BenchmarkDotNet.Running;

namespace SadConsole.PerformanceTests
{
	class Program
	{
        private static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
