using System.Threading.Tasks;

namespace ConsoleApp1
{
    public abstract class Port
    {
        protected Port(ushort number)
        {
            Number = number;
        }

        public ushort Number { get; }

        protected abstract string PortType { get; }
        public MegaD MegaD { get; set; }

        public async Task<bool> UpdateValue()
        {
            var url = $"{MegaD.BaseUrl}?pt={Number}&cmd=get";
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