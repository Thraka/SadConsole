using BenchmarkDotNet.Running;

namespace SadConsole.PerformanceTests
{
    /// <summary>
    /// Provide a Console-interface to allow selection of what performance tests you want to run.
    /// </summary>
	class Program
	{
        private static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
