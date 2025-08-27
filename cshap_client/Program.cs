using cshap_client.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cshap_client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PacketCommandMapping.InitCommandMappingFromFile("gen/message_command_mapping.json");
            return;
        }
    }
}
