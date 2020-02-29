namespace ConsoleApp1
{
    public class OutPort : Port
    {
        private bool state;

        public OutPort(ushort number) : base(number)
        {
        }

        public bool State
        {
            get => state;
            protected set => state = value;
        }

        protected override string PortType => "Out";


        protected override bool UpdateImpl(string value)
        {
            return PortHelper.IsValidOnOff(value, out state);
        }

        public override string ToString()
        {
            return $"{base.ToString()}={(State ? 1 : 0)}";
        }
    }
}