using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Constants
{
    internal static class GameConstants
    {
        public const string Level1 = "Elementary";
        public const string Level2 = "Enthusiast";
        public const string Level3 = "Adept";
        public const string Level4 = "Advanced";
        public const string Level5 = "Proficient";
        public const string Level6 = "Professional";

        public static string getLevelTitle(int level)
        {
            switch (level)
            {
                case 1:
                    return Level1;
                case 2:
                    return Level2;
                case 3:
                    return Level3;
                case 4:
                    return Level4;
                case 5:
                    return Level5;
                case 6:
                    return Level6;
                default:
                    return Level1;
            }
        }

    }
}
