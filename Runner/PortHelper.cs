using System.Net.NetworkInformation;

namespace ConsoleApp1
{
    public static class PortHelper
    {
        public static bool IsValidOnOff(this string v, out bool val)
        {
            val = false;
            if (v.Equals("ON"))
            {
                val = true;
                return true;
            }

            return v.Equals("OFF");
        }
        
        public static string To10(this bool state)
        {
            return (state ? 1 : 0).ToString();
        }
    }
}