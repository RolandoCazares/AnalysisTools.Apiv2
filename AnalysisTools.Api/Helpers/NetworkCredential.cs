using System.Runtime.InteropServices;

namespace analysistools.api.Helpers
{
    /// <summary>
    /// Class that allows the system to login to external shared file systems from other servers or computers.
    /// Optical stations and ticker servers use this system.
    /// </summary>
    public class NetworkCredential
    {
        private static readonly Lazy<NetworkCredential> lazy = new Lazy<NetworkCredential>(() => new NetworkCredential());
        public static NetworkCredential Instance { get => lazy.Value; }

        private NetworkCredential()
        {
            _ShareName = string.Empty;
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2A(ref structNetResource pstNetRes, string psPassword, string psUsername, int piFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct structNetResource
        {
            public int iScope;
            public int iType;
            public int iDisplayType;
            public int iUsage;
            public string sLocalName;
            public string sRemoteName;
            public string sComment;
            public string sProvider;
        }

        private const int RESOURCETYPE_DISK = 0x1;

        //Standard	
        private const int CONNECT_INTERACTIVE = 0x00000008;
        private const int CONNECT_PROMPT = 0x00000010;
        private const int CONNECT_UPDATE_PROFILE = 0x00000001;
        //IE4+
        private const int CONNECT_REDIRECT = 0x00000080;
        //NT5 only
        private const int CONNECT_COMMANDLINE = 0x00000800;
        private const int CONNECT_CMD_SAVECRED = 0x00001000;

        private string _ShareName;
        public string ShareName
        {
            get { return (_ShareName); }
            set { _ShareName = value; }
        }

        public void LogIn(string IpAddress, string Username, string Password)
        {
            this.ShareName = "\\\\" + IpAddress;
            structNetResource stNetRes = new structNetResource();
            stNetRes.iScope = 2;
            stNetRes.iType = 0;
            stNetRes.iDisplayType = 3;
            stNetRes.iUsage = 1;
            stNetRes.sRemoteName = _ShareName;
            stNetRes.sLocalName = "";
            int i = WNetAddConnection2A(ref stNetRes, Password, Username, 1);
            if (i > 0) { /*throw new System.ComponentModel.Win32Exception(i);*/ }
        }
    }
}
