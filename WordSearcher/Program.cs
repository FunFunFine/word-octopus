using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordSearcher
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                // TODO: add warning message
                return 1;
            }

            var serverUri = args[0];
            var authKey = args[1];
            var api = new Api(serverUri, authKey);
            api.Connect();
            var field = api.MoveRight();
            try
            {
                api.SendWords(new[] {"шпора"});
            }
            catch
            {
                ////
            }

            api.GetStats();


            api.Close();
            Console.Read();
            return 0;

        }
    }
}
