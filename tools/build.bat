
@ECHO OFF

echo:
echo ==========================
echo SrkSekvap package builder
echo ==========================
echo:

set currentDirectory=%CD%

cd ..
set rootDirectory=%CD%

if NOT EXIST build mkdir build
cd build
set outputDirectory=%CD%

cd %rootDirectory%
if NOT EXIST packages mkdir packages
cd packages
set packagesDirectory=%CD%

cd %currentDirectory%
set nuget=%CD%\..\tools\nuget.exe
set msbuild4="%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
set vincrement=%CD%\..\tools\Vincrement.exe


echo Check CLI apps
echo -----------------------------

cd %currentDirectory%

if not exist %nuget% (
 echo ERROR: nuget could not be found, verify path. exiting.
 echo Configured as: %nuget%
 pause
 exit
)

if not exist %msbuild4% (
 echo ERROR: msbuild 4 could not be found, verify path. exiting.
 echo Configured as: %msbuild4%
 pause
 exit
)

if not exist %vincrement% (
 echo ERROR: vincrement could not be found, verify path. exiting.
 echo Configured as: %vincrement%
 pause
 exit
)

echo Everything is fine.

echo:
echo Build solution
echo -----------------------------

pause

cd %rootDirectory%\src

set solutionDirectory=%CD%
%msbuild4% "SrkSekvap.sln" /p:Configuration=Release /nologo /verbosity:q /t:SrkSekvap

if not %ERRORLEVEL% == 0 (
 echo ERROR: build failed. exiting.
 cd %currentDirectory%
 pause
 exit
)
echo Done.

cd %currentDirectory%

echo:
echo Copy libs
echo -----------------------------

set pclFx=portable-net45+sl5+win8+wp8+wpa81

if NOT EXIST %outputDirectory%\lib        mkdir %outputDirectory%\lib
if NOT EXIST %outputDirectory%\lib\%pclFx%  mkdir %outputDirectory%\lib\%pclFx%

xcopy /Q /Y %solutionDirectory%\SrkSekvap\bin\Release\SrkSekvap.dll %outputDirectory%\lib\%pclFx%\
xcopy /Q /Y %solutionDirectory%\SrkSekvap\bin\Release\SrkSekvap.xml %outputDirectory%\lib\%pclFx%\

if NOT EXIST %outputDirectory%\images        mkdir %outputDirectory%\images

copy /Y %rootDirectory%\res\logo-200.png %outputDirectory%\images\icon.png

echo Done.


echo:
echo Increment version number
echo -----------------------------

set /p version=<%rootDirectory%\version.txt
echo Previous version: %version%

echo Hit return to continue...
pause 
%vincrement% -file=%rootDirectory%\version.txt 0.0.1 %rootDirectory%\version.txt
if not %ERRORLEVEL% == 0 (
 echo ERROR: vincrement existed with code %ERRORLEVEL%. exiting.
 cd %currentDirectory%
 pause
 exit
)

set /p version=<%rootDirectory%\version.txt
echo New version:      %version%



echo:
echo Build NuGet package
echo -----------------------------

echo - Did you update the release notes?
echo:
echo Hit return to continue...
pause 
cd %rootDirectory%
%nuget% pack %rootDirectory%\SrkSekvap.nuspec -BasePath %outputDirectory% -Version %version% -OutputDirectory %packagesDirectory%
echo Done.

cd %currentDirectory%


echo:
echo Push NuGet package
echo -----------------------------

echo Hit return to continue...
pause 
cd %packagesDirectory%
%nuget% push %packagesDirectory%\SrkSekvap.%version%.nupkg  -Source https://www.nuget.org/api/v2/package
echo Done.





cd %currentDirectory%
pause



