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
            // 可选命令行参数 -account test -password 123
            var parsedArgs = ParseArguments(args);
            if (parsedArgs.TryGetValue("account", out object account))
            {
                Login.s_AccountName = account as string;
                if (parsedArgs.TryGetValue("password", out object password))
                {
                    Login.s_Password = password as string;
                }
                else
                {
                    Login.s_Password = Login.s_AccountName;
                }
            }

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

        static Dictionary<string, object> ParseArguments(string[] args)
        {
            var parameters = new Dictionary<string, object>();
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                // 处理标志参数 (如 -v, --verbose)
                if (arg.StartsWith("-"))
                {
                    string key = arg.TrimStart('-');
                    // 检查下一个参数是否是值（不是以-开头）
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        parameters[key] = args[i + 1];
                        i++; // 跳过下一个参数，因为我们已经处理了它
                    }
                    else
                    {
                        // 如果没有值，将其视为布尔标志
                        parameters[key] = true;
                    }
                }
                else
                {
                    // 处理位置参数（没有前缀的参数）
                    int position = parameters.Count(kv => !kv.Key.StartsWith("_")) + 1;
                    parameters[$"_{position}"] = arg;
                }
            }
            return parameters;
        }
    }
}
