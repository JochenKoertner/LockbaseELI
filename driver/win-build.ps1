# Set-ExecutionPolicy Unrestricted 

$clang = '"C:/LLVM/bin/clang-cl.exe"'
$clangxx = '"C:/Program\ Files/LLVM/bin/clang++.exe"'
$make = '"C:/MinGW/bin/mingw32-make.exe"'
$generator = 'MinGW Makefiles'
# generator = 'Visual Studio 14 2015'

$buildType = 'Debug'
$path = '..'


# https://www.powershellgallery.com/packages/WebKitDev/0.1.12/Content/Functions%5CInvoke-CMakeBuild.ps1

# Remove temp file

# call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" x86


Remove-Item CMakeFiles -Recurse -Force
Remove-Item CMakeCache.txt -Force

$genArgs = @('-G "{0}"' -f $generator )
# $genArgs += ('-T "{0}"' -f "LLVM-vs2014");

$genArgs += ('-DCMAKE_BUILD_TYPE={0}' -f $buildType);
# $genArgs += '-DPAHO_WITH_SSL=FALSE';
# $genArgs += '-DPAHO_BUILD_DOCUMENTATION=FALSE';
# $genArgs += '-DPAHO_BUILD_SAMPLES=FALSE';
$genArgs += '-DCMAKE_VERBOSE_MAKEFILE=FALSE';

$genArgs += ('{0}' -f $path);

# Create the generate call
$genCall = ('cmake {0}' -f ($genArgs -Join ' '));

# Write-Host $genCall
Invoke-Expression $genCall;

$buildCall = 'cmake --build . --target LbELI_cli -- -j 2';

# Write-Host $buildCall
Invoke-Expression $buildCall

# Call cmake 
#Write-Host $clang
#cmake -G $generator -DCMAKE_BUILD_TYPE=DEBUG -DCMAKE_C_COMPILER=$clang . 

# "-DCMAKE_C_COMPILER=$clang -DCMAKE_MAKE_PROGRAMM=$make -DCMAKE_BUILD_TYPE=Debug . " 

if( -not $? )
{
    $msg = $Error[0].Exception.Message
    "Encountered error during cmake build. Error Message is $msg. Please check." 
    exit
}

Copy-Item ".\LbwELI.Demo.dll" "/LockbaseDemo/Lockbase" -Force