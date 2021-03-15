# Test task
The project is written using the .NET 5.0.

## Generate test file
To generate test file it is necessary to run the following command from the repository base folder:
> dotnet run --project .\src\FileSorter.Generator\ -c Release <output file path> <size in MB> [<MinNumberPartLength:MaxNumberPartLength:MinLineLength:MaxLineLength:DuplicateNthStringPart>]

There is an optional parameter to configure line settings:
- MinNumberPartLength (default value is **1**)
- MaxNumberPartLength (default value is **10**)
- MinLineLength (default value is **15**)
- MaxLineLength (default value is **1024**)
- DuplicateNthStringPart (default value is **100**)

Example:
> dotnet run --project .\src\FileSorter.Generator\ -c Release c:\temp\src.txt 10240 1:15:20:1024:100

## Sort file
To sort file it is necessary to run the following command from the repository base folder:
> dotnet run --project .\src\FileSorter.Sorter\ -c Release <source file path> [<target file path>]

`The 'target file path' is optional parameter. By default the application generates file near the source file with the '_sorted' suffix.`
