namespace ConsoleApp1
{
    public class Ds2413OutPort : Port
    {
        private bool aValue;
        private bool bValue;

        public Ds2413OutPort(ushort number) : base(number)
        {
        }

        public bool AValue => aValue;

        public bool BValue => bValue;

        public override string PortType => "DS2413";

        protected override bool UpdateImpl(string value)
        {
            var data = value.Split('/');
            if (data.Length == 2)
                if (data[0].IsValidOnOff(out aValue))
                    if (data[1].IsValidOnOff(out bValue))
                        return true;

            return false;
        }

        public override string ToString()
        {
            return $"{base.ToString()}={aValue.To10()}:{bValue.To10()}";
        }
    }
}