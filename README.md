# ipa-file-info
Simple util to read metadata from an ipa (iOS App package). As written, will put the following in the output:
- App name
- App bundle ID
- Binary name
- Minimum iOS version required
- SDK the App was built with
- App version
- Path where the IPA is located on the system

Note that some bundles may not have all of the above, and will have an empty field in such a case.

# Usage
Put the built exe in your `PATH` and pass it either
- A path to an IPA file, or
- A path to a directory (which will be recursively scanned for *.ipa files)

# Output
Default output is comma-delimited output to `STDOUT`, with errors written to `STDERR`.

I recommend redirecting the output to a file, e.g. `ipa-file-info.exe > info.csv`

# Building
Do the typical `dotnet restore && dotnet build`. If you'd like to package it as a release binary (single-file):

```dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true```