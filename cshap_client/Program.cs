using cshap_client.game;
using cshap_client.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cshap_client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Client.Instance.Init();
            Client.Instance.m_Connection.Connect("127.0.0.1:10001");

            Thread thread = new Thread(inputCmd);
            thread.Start();
            Client.Instance.Run();
            Client.Instance.Shutdown();
        }

        static void inputCmd()
        {
            Console.WriteLine("input test cmd or 'exit' to exit");
            while (true)
            {
                string cmd = Console.ReadLine();
                cmd = cmd.TrimStart().TrimEnd();
                if (cmd == "exit" || cmd == "quit" || cmd == "stop")
                {
                    Client.Instance.IsRunning = false;
                    break;
                }
                else
                {
                    Console.WriteLine("cmd:" + cmd);
                    Client.Instance.RecvCommand(cmd);
                }
            }
            Console.WriteLine("exiting");
        }
    }
}
