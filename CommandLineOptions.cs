using Parcs.Module.CommandLine;

namespace NewMatrixModule
{
    using CommandLine;

    public class CommandLineOptions : BaseModuleOptions
    {
        [Option("n", Required = true, HelpText = "Number of variables.")]
        public int dimension { get; set; }
        [Option("p", Required = true, HelpText = "Number of points.")]
        public int PointsNum { get; set; }        
    }
}
