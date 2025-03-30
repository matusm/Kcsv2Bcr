Kcsv2Bcr - Keyence CSV to BCR Converter
=======================================

A standalone command line tool that converts files produced by the [Keyence](https://www.keyence.com) VK-X3000 3D Surface Profiler .
The converted GPS data files are formatted according to ISO 25178-7, ISO 25178-71 and EUNA 15178 (BCR). All files are ASCII (text) files, the (currently deprecated) option to produce binary files is not implemented. The data produced with the surface profiler must be exported as a CSV file, other formats can not be used with this software

## Command Line Usage:  

```
Kcsv2Bcr inputfile [outputfile] [options]
```

## Options:  

`--quiet (-q)` : Quiet mode. No screen output (except for errors).

`--comment` : User supplied string to be included in the metadata.

`--strict` : Disable large (>65535) field dimension and other goodies.

`--iso` : Force output file to be ISO 25178-71:2012 compliant (not recommended, Gwyddion will currently ignore metadata of this format).

`--mask (-m)` : Replace (mask) missing or invalid data points with a neutral value. Supported values are:

0: do nothing (invalid points stay invalid)

1: replace all invalid points by 0

2: replace all invalid points by the minimum hight value 

3: replace all invalid points by the maximum hight value 

4: replace all invalid points by the average hight value 

5: replace all invalid points by the central hight value 

### Effect of the `--strict` command line option

* Field dimensions are restricted to 65535 at most;

* The ManufacID is trimmed to 10 characters;

* Invalid data points are coded by the string `BAD` instead of `NaN`.

### Caveats

Due to limmited example data the app works only for specific cases. The keywords in the file to be converted must be in German language. The app will not work with data produced on a computer with different locale. The decimal separator must be the dot on the line, however.

## Installation

If you do not want to build the application from the source code you can use the released binaries. Just copy the .exe and the .dll files to a directory of your choice. This direcory should be included in the user's PATH variable.

## Dependencies  
At.Matus.StatisticPod :  https://github.com/matusm/At.Matus.StatisticPod  

Bev.IO.BcrWriter: https://github.com/matusm/Bev.IO.BcrWriter 

Bev.UI.ConsoleUI: https://github.com/matusm/Bev.UI.ConsoleUI

CommandLineParser: https://github.com/commandlineparser/commandline 
