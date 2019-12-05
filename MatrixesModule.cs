using Parcs;
using System;
using System.IO;
using System.Threading;

namespace NewMatrixModule
{
    using log4net;

    public class MatrixesModule : MainModule
    {
        private const string fileName = "result.txt";

        private static readonly ILog _log = LogManager.GetLogger(typeof(MatrixesModule));

        private static CommandLineOptions options;

        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            options = new CommandLineOptions();

            if (args != null)
            {
                if (!CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    throw new ArgumentException($@"Cannot parse the arguments. Possible usages: {options.GetUsage()}");
                }
            }

            (new MatrixesModule()).RunModule(options);
        }

        public override void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {
            int n = options.dimension;

            Matrix a = new Matrix(n, n, true);
            Matrix b = new Matrix(n, 1, true);
            a.WriteToFile("a.txt");
            b.WriteToFile("b.txt");

            int pointsNum = options.PointsNum;

            _log.InfoFormat("Starting Cramer`s rule Module on {0} points", pointsNum);

            var points = new IPoint[pointsNum];
            var channels = new IChannel[pointsNum];
            for (int i = 0; i < pointsNum; ++i)
            {
                points[i] = info.CreatePoint();
                channels[i] = points[i].CreateChannel();
                points[i].ExecuteClass("NewMatrixModule.Det");
            }

            DateTime time = DateTime.Now;
            _log.Info("Waiting for a result...");

            double mainDet = a.Det();
            int partsCount = n / pointsNum;
            
            for (int i = 0; i < pointsNum; i++)
            {
                Matrix[] arr = new Matrix[partsCount];

                for (int j = 0; j < partsCount; j++)
                {
                    Matrix m = new Matrix(n, n);
                    m.Assign(a);
                    m.FillSubMatrix(b, 0, i + j);
                    arr[j] = m;
                }

                channels[i].WriteObject(arr);
            }

            LogSendingTime(time);
            double[] dets = new double [n];

            for (int i = 0; i < pointsNum; i++)
            {
                double[] channelDets = channels[i].ReadObject<double[]>();
                for (int j = 0; j < partsCount; j++)
                {
                    dets[i + j] = channelDets[j];
                }
            }

            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = dets[i] / mainDet;
            }

            using (var writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < n; i++)
                {
                    writer.Write(result[i]);
                    writer.WriteLine();
                }
            }


            LogResultFoundTime(time);
            Console.ReadKey();
        }

        private static void LogResultFoundTime(DateTime time)
        {
            _log.InfoFormat(
                "Result found: time = {0}, saving the result to the file {1}",
                Math.Round((DateTime.Now - time).TotalSeconds, 3),
                fileName);
        }

        private static void LogSendingTime(DateTime time)
        {
            _log.InfoFormat("Sending finished: time = {0}", Math.Round((DateTime.Now - time).TotalSeconds, 3));
        }
    }
}
