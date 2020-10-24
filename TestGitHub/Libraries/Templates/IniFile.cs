using System.Runtime.InteropServices;
using System.Text;

namespace TestGitHub.Libraries.Templates
{
    public class IniFile
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        public IniFile(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; private set; }

        public string GetString(string section, string key, string defaultValue = "")
        {
            var sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, defaultValue, sb, (uint)sb.Capacity, FilePath);
            return sb.ToString();
        }

        public bool WriteString(string section, string key, string value)
        {
            return WritePrivateProfileString(section, key, value, FilePath);
        }
    }
}
