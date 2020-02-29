using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{

    public interface Transport
    {
        
    }

    public class MqttTransport : Transport
    {
        
    }
    
    public abstract class MegaD
    {
        private readonly IPAddress address;
        private readonly string password;
        private readonly Dictionary<ushort, Port> ports = new Dictionary<ushort, Port>();

        protected MegaD(string address, string password)
        {
            this.address = IPAddress.Parse(address);
            this.password = password;
        }

        public string BaseUrl => $"http://{address}/{password}/";

        public bool Add(Port port)
        {
            if (!ports.TryAdd(port.Number, port)) return false;
            port.MegaD = this;
            return true;

        }
        public string DumpState()
        {
            var sb= new StringBuilder();
            sb.AppendLine($"Host: {address}. Pass *** ");
            foreach (var portsValue in ports.Values)
            {
                sb.AppendLine($"  {portsValue}");
            }

            return sb.ToString();
        }
        

        public async Task<bool> Update()
        {
            foreach (var port in ports.Values)
                if (!await port.UpdateValue())
                    return false;

            return true;
        }
    }
}