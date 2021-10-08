using System;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using Claunia.PropertyList;

namespace ipa_file_info
{
    class Program
    {
        private static string InfoPlistRegex = @"^Payload\/.*\.app\/Info.plist$";
        private static Regex PathMatch = new Regex(InfoPlistRegex);

        // static NSObject getVal(this NSDictionary dict, string key) {
        //     return dict.ObjectForKey
        // }

        static void Main(string[] args)
        {
            Console.WriteLine("Init.");

            var path = args.Length > 0 ? args[0] : @"C:\tmp\test.ipa";

            Console.WriteLine("Name\t\tID\t\t\tMin iOS\tSDK\t\tApp Version");

            using (var archive = ZipFile.OpenRead(path)) {
                var infoPlist = archive.Entries.Where(entry => PathMatch.Match(entry.FullName).Success).FirstOrDefault();

                // Now open the stream. Have to cast to dict.
                var plist = (NSDictionary)PropertyListParser.Parse(infoPlist.Open());

                // Keys to read:
                // CFBundleName (string); app name
                // CFBundleIdentifier (string); bundle ID
                // DTSDKName (string); sdk built with?
                // MinimumOSVersion (string); minimum iOS version (e.g. 9.3)
                var appName    = plist.ObjectForKey("CFBundleName")?.ToString();
                var bundleId   = plist.ObjectForKey("CFBundleIdentifier")?.ToString();
                var sdkVersion = plist.ObjectForKey("DTSDKName")?.ToString();
                var minIos     = plist.ObjectForKey("MinimumOSVersion")?.ToString();

                // Below don't seem to be in all of 'em?
                // CFBundleShortVersionString (string); string in settings(?)
                // UISupportedDevices (Array of strings); devices allowed
                // UIRequiredDeviceCapabilities (array of strings); reqd architecture (e.g. armv7)
                var version    = plist.ObjectForKey("CFBundleShortVersionString")?.ToString();
                var supportedDevices = ((NSArray)plist.ObjectForKey("UISupportedDevices"))?.GetArray();
                var requiredDeviceCapabilities = ((NSArray)plist.ObjectForKey("UIRequiredDeviceCapabilities"))?.GetArray();

                // Print 'em
                Console.WriteLine($"{appName}\t{bundleId}\t{minIos}\t{sdkVersion}\t{version}");

                // Close.

                Console.WriteLine("Done.");
            }
        }
    }
}
