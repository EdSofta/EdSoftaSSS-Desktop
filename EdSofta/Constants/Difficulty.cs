using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Constants
{
    public static class Difficulty
    {
        public const string Easy = "Easy";
        public const string Medium = "Medium";
        public const string Hard = "Difficult";

        public static int Weight(string value)
        {
            switch (value)
            {
                case Easy:
                    return 1;
                case Medium:
                    return 2;
                case Hard:
                    return 3;
                default:
                    return 1;
            }
        }
    }
}
