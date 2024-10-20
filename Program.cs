using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct DEVMODE
    {
        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmNup;
        public int dmDisplayFrequency;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

    const int ENUM_CURRENT_SETTINGS = -1;
    const int CDS_UPDATEREGISTRY = 0x01;
    const int DISP_CHANGE_SUCCESSFUL = 0;

    static DEVMODE GetCurrentSettings()
    {
        DEVMODE dm = new DEVMODE();
        dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
        if (!EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm))
        {
            throw new Exception("Unable to enumerate display settings.");
        }
        return dm;
    }

    static void ChangeResolution(int width, int height)
    {
        DEVMODE dm = new DEVMODE();
        dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
        if (!EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm))
        {
            throw new Exception("Unable to enumerate display settings.");
        }

        dm.dmPelsWidth = width;
        dm.dmPelsHeight = height;
        dm.dmFields = 0x00080000 | 0x00100000; // DM_PELSWIDTH | DM_PELSHEIGHT

        int result = ChangeDisplaySettings(ref dm, CDS_UPDATEREGISTRY);
        if (result != DISP_CHANGE_SUCCESSFUL)
        {
            throw new Exception($"Resolution change failed with error code: {result}");
        }
    }

    static bool IsProcessRunning(string processName)
    {
        foreach (Process process in Process.GetProcesses())
        {
            if (process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            // Display current resolution
            try
            {
                DEVMODE currentSettings = GetCurrentSettings();
                Console.WriteLine($"Current resolution: {currentSettings.dmPelsWidth}x{currentSettings.dmPelsHeight}");
                Console.WriteLine("Usage: ChangeResolution.exe -x <width> -y <height> -p <optional program path> -w (wait for program to start)");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return;
        }

        int width = 0, height = 0;
        string programPath = null;
        bool waitForProgram = false;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-x":
                    width = int.Parse(args[++i]);
                    break;
                case "-y":
                    height = int.Parse(args[++i]);
                    break;
                case "-p":
                    programPath = args[++i];
                    break;
                case "-w":
                    waitForProgram = true;
                    break;
                default:
                    Console.WriteLine("Usage: ChangeResolution.exe -x <width> -y <height> -p <optional program path> -w (wait for program to start)");
                    return;
            }
        }

        if (width == 0 || height == 0)
        {
            Console.WriteLine("Usage: ChangeResolution.exe -x <width> -y <height> -p <optional program path> -w (wait for program to start)");
            return;
        }

        try
        {
            DEVMODE currentSettings = GetCurrentSettings();
            Console.WriteLine($"Current resolution: {currentSettings.dmPelsWidth}x{currentSettings.dmPelsHeight}");

            if (waitForProgram && programPath != null)
            {
                string processName = System.IO.Path.GetFileNameWithoutExtension(programPath);
            
                while (true)
                {
                    Console.WriteLine($"Waiting for program: {programPath}");
                    Console.WriteLine("Press Ctrl+C to exit waiting mode.");

                    // Wait for the process to start
                    while (!IsProcessRunning(processName))
                    {
                        System.Threading.Thread.Sleep(1000); // Check every second
                    }

                    // Change resolution when the process starts
                    ChangeResolution(width, height);
                    Console.WriteLine($"Resolution changed to {width}x{height} before {processName} started.");

                    // Wait for the process to exit
                    while (IsProcessRunning(processName))
                    {
                        System.Threading.Thread.Sleep(1000); // Wait until the process exits
                    }

                    // Revert to original resolution after the process exits
                    ChangeResolution(currentSettings.dmPelsWidth, currentSettings.dmPelsHeight);
                    Console.WriteLine("Resolution reverted to original settings.");
                }
            }
            else if (programPath != null)
            {
                ChangeResolution(width, height);
                Console.WriteLine($"Resolution changed to {width}x{height}.");
                Console.WriteLine($"Launching program: {programPath}");

                using (Process process = Process.Start(programPath))
                {
                    process.WaitForExit();
                }

                // Revert to original resolution
                ChangeResolution(currentSettings.dmPelsWidth, currentSettings.dmPelsHeight);
                Console.WriteLine("Resolution reverted to original settings.");
            }
            else
            {
                ChangeResolution(width, height);
                Console.WriteLine($"Resolution changed to {width}x{height}.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
