using System;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace ipa_file_info
{
    class Program
    {
        private static string InfoPlistRegex = @"^Payload\/.*\.app\/Info.plist$";
        private static Regex PathMatch = new Regex(InfoPlistRegex);

        static void Main(string[] args)
        {
            Console.WriteLine("Init.");

            var path = @"C:\tmp\test.ipa";

            using (var archive = ZipFile.OpenRead(path)) {
                // var plists = archive.Entries.Where(entry => entry.Name.Equals("Info.plist"));
                var infoPlist = archive.Entries.Where(entry => PathMatch.Match(entry.FullName).Success).FirstOrDefault();

                // foreach (var entry in archive.Entries) {
                //     // Console.WriteLine($"Entry: {entry}");
                //     if (entry.FullName.EndsWith("Info.plist", StringComparison.Ordinal)) {
                //         Console.WriteLine($"Found Info.plist at {entry.FullName}");
                //         // TODO: read /only/ the Payload/Whatever.app/Info.plist file
                //         // Keys to read:
                //         // UISupportedDevices (Array of strings); devices allowed
                //         // DTSDKName (string); sdk built with?
                //         // UIRequiredDeviceCapabilities (array of strings); reqd architecture (e.g. armv7)
                //         // CFBundleName (string); app name
                //         // CFBundleShortVersionString (string); string in settings(?)
                //         // MinimumOSVersion (string); minimum iOS version (e.g. 9.3)
                //         // CFBundleIdentifier (string); bundle ID
                //     }
                // }
                Console.WriteLine("Done.");
            }
        }
    }
}
