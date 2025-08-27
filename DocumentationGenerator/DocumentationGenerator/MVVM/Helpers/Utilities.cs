using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationGenerator.MVVM.Helpers
{
    public static class Utilities
    {

        public static byte HexToByte(string hex)
        {
            if (hex.Length != 2)
                throw new ArgumentException("Hex string must be 2 characters long.");

            int high = HexCharToInt(hex[0]);
            int low = HexCharToInt(hex[1]);

            return (byte)((high << 4) + low);
        }

        private static int HexCharToInt(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;

            throw new ArgumentException("Invalid hex character: " + c);
        }

    }
}
