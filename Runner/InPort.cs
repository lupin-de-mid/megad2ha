namespace ConsoleApp1
{
    public class InPort : Port
    {
        private ushort count;

        public InPort(ushort number) : base(number)
        {
        }

        public bool State { get; protected set; }
        protected override string PortType => "In";

        protected override bool UpdateImpl(string value)
        {
            var data = value.Split("/");
            if (data.Length == 2)
            {
                State = data[0].Equals("ON");
                return ushort.TryParse(data[1], out count) && (State || data[0].Equals("OFF"));
            }

            return false;
        }

        public override string ToString()
        {
            return $"{base.ToString()}={(State ? 1 : 0)}={count}";
        }
    }
}