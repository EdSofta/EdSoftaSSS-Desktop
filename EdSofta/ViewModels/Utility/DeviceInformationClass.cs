using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Constants;

namespace EdSofta.ViewModels.Utility
{
    internal class DeviceInformationClass
    {
        /// <summary>
        /// The system identifier
        /// </summary>
        private static string SystemID = string.Empty;
        /// <summary>
        /// The product identifier
        /// </summary>
        private static string ProductID = string.Empty;


        /// <summary>
        /// Gets the product key.
        /// </summary>
        /// <returns>System.String.</returns>
        internal static string GenerateProductKey()
        {
            var longSystemId = BaseOperation.toDeci(Value().Substring(0, 12), 32);
            var longProductId = BaseOperation.toDeci(Keys.ProductCode, 32);
            var sumId = BaseOperation.fromDeci(32, longProductId + longSystemId);
            var generatedKey = replaceInString(sumId);
            ProductID = padProductKey(generatedKey);
            //SystemID = replaceInString(fromDeci(32, toDeci(Value().Substring(0, 12), 32)));
            return ProductID;
        }

        public static string padProductKey(string key)
        {
            if (key.Length == 12) return key;

            string newKey = key;
            if (key.Length < 12)
            {
                var filler = 12 - key.Length;
                for (int i = 0; i < filler; i++)
                {
                    newKey = $"{newKey}C";
                }
            }
            else
            {
                return newKey.Substring(0, 12);
            }

            return newKey;
        }


        /// <summary>
        /// Replaces the in string.
        /// </summary>
        /// <param name="initialString">The initial string.</param>
        /// <returns>System.String.</returns>
        public static string replaceInString(string initialString)
        {

            var allChar = initialString.ToCharArray().Select(x => isSpecialChar(x) ? replaceSpecialChar(x) : x);
            return string.Join("", allChar);
        }

        /// <summary>
        /// Restores the in string.
        /// </summary>
        /// <param name="initialString">The initial string.</param>
        /// <returns>System.String.</returns>
        public static string restoreInString(string initialString)
        {

            var allChar = initialString.ToCharArray().Select(x => x = isSpecialChar(x) ? restoreSpecialChar(x) : x);
            return string.Join("", allChar);
        }

        /// <summary>
        /// Determines whether [is special character] [the specified character].
        /// </summary>
        /// <param name="Char">The character.</param>
        /// <returns><c>true</c> if [is special character] [the specified character]; otherwise, <c>false</c>.</returns>
        private static bool isSpecialChar(char Char)
        {
            if (Char == '1' || Char == '0' || Char == 'I' || Char == 'O' || Char == 'W' || Char == 'X' || Char == 'Y' || Char == 'Z')
                return true;
            else return false;
        }

        /// <summary>
        /// Replaces the special character.
        /// </summary>
        /// <param name="Char">The character.</param>
        /// <returns>System.Char.</returns>
        private static char replaceSpecialChar(char Char)
        {
            char character;
            switch (Char)
            {
                case '1':
                    character = 'W';
                    break;
                case '0':
                    character = 'X';
                    break;
                case 'O':
                    character = 'Y';
                    break;
                case 'I':
                    character = 'Z';
                    break;
                default:
                    return Char;
            }

            return character;
        }

        /// <summary>
        /// Restores the special character.
        /// </summary>
        /// <param name="Char">The character.</param>
        /// <returns>System.Char.</returns>
        private static char restoreSpecialChar(char Char)
        {
            char character;
            switch (Char)
            {
                case 'W':
                    character = '1';
                    break;
                case 'X':
                    character = '0';
                    break;
                case 'Y':
                    character = 'O';
                    break;
                case 'Z':
                    character = 'I';
                    break;
                default:
                    return character = Char;
            }

            return character;
        }

        /// <summary>
        /// Values this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string Value()
        {
            if (string.IsNullOrEmpty(SystemID))
            {
                SystemID = GetHash("CPU >> " + cpuId() + "\nBIOS >> " + biosId() + "\nBASE >> " + baseId()
                                     //+"\nDISK >> "+ diskId() + "\nVIDEO >> " + videoId() +"\nMAC >> "+ macId()
                                     );
            }
            return SystemID;
        }

        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.String.</returns>
        private static string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetBase32HexString(sec.ComputeHash(bt));
        }



        /// <summary>
        /// Gets the base32 hexadecimal string.
        /// </summary>
        /// <param name="bt">The bt.</param>
        /// <returns>System.String.</returns>
        private static string GetBase32HexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 31;
                n2 = (n >> 4) & 31;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                //if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }


        #region Hardware Information

        /// <summary>
        /// Identifiers the specified WMI class.
        /// </summary>
        /// <param name="wmiClass">The WMI class.</param>
        /// <param name="wmiProperty">The WMI property.</param>
        /// <param name="wmiMustBeTrue">The WMI must be true.</param>
        /// <returns>System.String.</returns>
        private static string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Identifiers the specified WMI class.
        /// </summary>
        /// <param name="wmiClass">The WMI class.</param>
        /// <param name="wmiProperty">The WMI property.</param>
        /// <returns>System.String.</returns>
        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Cpus the identifier.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string cpuId()
        {

            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "")
            {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (retVal == "")
                {
                    retVal = identifier("Win32_Processor", "Name");
                    if (retVal == "")
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }

        /// <summary>
        /// Bioses the identifier.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string biosId()
        {
            return identifier("Win32_BIOS", "Manufacturer")
            + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + identifier("Win32_BIOS", "IdentificationCode")
            + identifier("Win32_BIOS", "SerialNumber")
            + identifier("Win32_BIOS", "ReleaseDate")
            + identifier("Win32_BIOS", "Version");
        }

        /// <summary>
        /// Bases the identifier.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string baseId()
        {
            return identifier("Win32_BaseBoard", "Model")
            + identifier("Win32_BaseBoard", "Manufacturer")
            + identifier("Win32_BaseBoard", "Name")
            + identifier("Win32_BaseBoard", "SerialNumber");
        }

        #endregion Hardware Information



        #region OS Information

        public static string getOSInfo()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else
                            operatingSystem = "8.1";
                        break;
                    case 10:
                        operatingSystem = "10";
                        break;
                    default:
                        break;
                }
            }
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.


            //if (operatingSystem != "")
            //{
            //    //Got something.  Let's prepend "Windows" and get more info.
            //    operatingSystem = "Windows " + operatingSystem;
            //    //See if there's a service pack installed.
            //    if (os.ServicePack != "")
            //    {
            //        //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
            //        operatingSystem += " " + os.ServicePack;
            //    }
            //    //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
            //    //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            //}


            //Return the information we've gathered.
            return operatingSystem;
        }

        public static bool IsWindows10()
        {
            var os = getOSInfo();
            var value = 0;
            int.TryParse(os, out value);
            return value == 10;
        }

        #endregion OS Information



        public static string getRunningVersion()
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return $"App Version: {version}";
                //return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                var version = new Version(1,0);
                return $"App Version: {version}";
            }
        }
    }
}
