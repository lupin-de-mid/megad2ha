using System.Threading.Tasks;

namespace ConsoleApp1
{
    public abstract class Port
    {
        public Port(ushort number)
        {
            this.Number = number;
        }

        public ushort Number { get; }

        public abstract string PortType { get; }

        public async Task<bool> UpdateValue()
        {
            var url = $"http://192.168.2.11/sec/?pt={Number}&cmd=get";
            var value = await Program.GetUrl(url);
            return UpdateImpl(value);
        }

        protected abstract bool UpdateImpl(string value);

        public override string ToString()
        {
            return $"#{Number}={PortType}";
        }
    }
}