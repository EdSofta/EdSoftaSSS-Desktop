using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.Utility
{
    internal class BaseOperation
    {

        /// <summary>
        /// Values the specified c.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>System.Int32.</returns>
        private static int val(char c)
        {
            if (c >= '0' && c <= '9')
                return (int)c - (int)'0';
            else
                return (int)c - (int)'A' + 10;
        }

        // Function to convert a  
        // number from given base  
        // 'b' to decimal 

        /// <summary>
        /// To the deci.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="b_ase">The b ase.</param>
        /// <returns>System.Int64.</returns>
        public static long toDeci(string str, int b_ase)
        {
            int len = str.Length;
            long power = 1; // Initialize  
            // power of base 
            long num = 0; // Initialize result 
            int i;

            // Decimal equivalent is  
            // str[len-1]*1 + str[len-1] * 
            // base + str[len-1]*(base^2) + ... 
            for (i = len - 1; i >= 0; i--)
            {
                // A digit in input number  
                // must be less than  
                // number's base 
                if (val(str[i]) >= b_ase)
                {
                    return -1;
                }

                num += val(str[i]) * power;
                power = power * b_ase;
            }

            return num;
        }

        /// <summary>
        /// Res the value.
        /// </summary>
        /// <param name="num">The number.</param>
        /// <returns>System.Char.</returns>
        private static char reVal(long num)
        {
            if (num >= 0 && num <= 9)
                return (char)(num + 48);
            else
                return (char)(num - 10 + (int)'A');
        }

        // Function to convert a given decimal number 
        // to a base 'base' and 

        /// <summary>
        /// Froms the deci.
        /// </summary>
        /// <param name="base1">The base1.</param>
        /// <param name="inputNum">The input number.</param>
        /// <returns>System.String.</returns>
        public static string fromDeci(int base1, long inputNum)
        {
            string s = "";

            // Convert input number is given  
            // base by repeatedly dividing it 
            // by base and taking remainder 
            while (inputNum > 0)
            {
                s += reVal(inputNum % base1);
                inputNum /= base1;
            }
            char[] res = s.ToCharArray();

            // Reverse the result 
            Array.Reverse(res);
            return new String(res);
        }

    }
}
