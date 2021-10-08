using System;
using System.IO;
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

        private static string getInfoForIpa(string path) {
            try
            {
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
                    return $"{appName}\t{bundleId}\t{minIos}\t{sdkVersion}\t{version}";
                }
            } catch (Exception e) {
                return $"Failed to process '{path}'; {e.Message}";
            }
        }

        // Useful links:
        // PList lib: https://github.com/claunia/plist-cil

        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("IPA Info Util, by GlitechedGoat\nPlease provide an IPA file, or folder to find IPA files in.");
                return;
            }


            // Check if path is directory or not.
            // https://stackoverflow.com/a/1395226
            try {
                var path = args[0] ?? string.Empty;
                FileAttributes attr = File.GetAttributes(path);

                Console.WriteLine("Name\t\tID\t\t\tMin iOS\tSDK\t\tApp Version");

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    Console.WriteLine($"Processing directory: {path}");

                    // Search directory, then run for each.
                    var files = Directory.GetFiles(path, "*.ipa", SearchOption.AllDirectories);

                    foreach (var file in files) {
                        Console.WriteLine(getInfoForIpa(file));
                    }
                }
                else
                {
                    // Run for single file.
                    Console.WriteLine(getInfoForIpa(path));
                }
            } catch (System.IO.FileNotFoundException e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
