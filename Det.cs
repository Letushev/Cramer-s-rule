using Parcs;
using System.Threading;

namespace NewMatrixModule
{
    public class Det : IModule
    {
        public void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {
            Matrix[] m = (Matrix [])info.Parent.ReadObject(typeof(Matrix[]));
            double[] dets = new double[m.Length];

            for (int i = 0; i < m.Length; i++)
            {
                dets[i] = m[i].Det(token);
            }

            info.Parent.WriteObject(dets);
        }
    }
}
