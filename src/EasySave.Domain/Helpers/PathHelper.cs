using System.Runtime.InteropServices;
using System.Text;

public static class PathHelper
{
    // Import the Windows API to resolve network drive connections
    [DllImport("mpr.dll")]
    private static extern int WNetGetConnection(
        string localName,
        StringBuilder remoteName,
        ref int length);

    // Converts a local path (C:\... or Z:\...) into a Universal Naming Convention (UNC) path.
    public static string ToUncPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return path;
        if (path.StartsWith(@"\\")) return path; // If the path is already in UNC format, return as is

        // Normalize the path to handle relative paths and get the root
        string fullPath = Path.GetFullPath(path);
        string root = Path.GetPathRoot(fullPath); 

        var stringbuilder = new StringBuilder(512);
        int size = stringbuilder.Capacity;

        //See if the root (ex: "Z:") corresponds to a mapped network drive
        int result = WNetGetConnection(root.Substring(0, 2), stringbuilder, ref size);

        if (result == 0) 
        {
            // Case 1 : It's a network drive => Replace the letter with the server path
            string uncRoot = stringbuilder.ToString();
            return Path.Combine(uncRoot, fullPath.Substring(root.Length));
        }

        // Case 2: It is a local disk (ex: C:). => manually builds the UNC path via the administrative share
        string machineName = Environment.MachineName;
        string driveLetter = root.Substring(0, 1); // "C"

        return $@"\\{machineName}\{driveLetter}$\{fullPath.Substring(root.Length)}";
    }
}
