namespace ConsoleApp1
{
    public class OutPwm : OutPort
    {
        public OutPwm(ushort number) : base(number)
        {
        }

        public override string PortType => "Pwm";
        public byte Level { get; private set; }

        protected override bool UpdateImpl(string value)
        {
            var result = Level;
            if (byte.TryParse(value, out result))
            {
                State = result > 0;
                Level = result;
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"{base.ToString()}={Level}";
        }
    }
}