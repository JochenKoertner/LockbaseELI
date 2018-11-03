# Cross Compile Paho.mqtt.c Library 

$generator = 'Ninja'
#$generator = 'MinGW Makefiles'

$buildType = 'Release'
$path = '../paho.mqtt.c'
$toolchain = 'C:/Users/SB/Projects/LockbaseELI/driver/paho.mqtt.c/cmake/toolchain.win32.cmake'

Remove-Item CMakeFiles -Recurse -Force
Remove-Item CMakeCache.txt -Force

$genArgs = @('-G "{0}"' -f $generator )
$genArgs += ('-DCMAKE_BUILD_TYPE={0}' -f $buildType);
$genArgs += '-DPAHO_WITH_SSL=FALSE';
$genArgs += '-DPAHO_BUILD_DOCUMENTATION=FALSE';
$genArgs += '-DPAHO_BUILD_STATIC=TRUE';
$genArgs += '-DPAHO_BUILD_SAMPLES=FALSE';
$genArgs += '-DPAHO_ENABLE_TESTING=FALSE';
$genArgs += '-DCMAKE_VERBOSE_MAKEFILE=TRUE';
$genArgs += '-DLIBS_SYSTEM="ws2_32 crypt32 RpcRT4"';
# $genArgs += ('-DCMAKE_TOOLCHAIN_FILE={0}' -f $toolchain);


$genArgs += ('{0}' -f $path);

# Create the generate call
$genCall = ('cmake {0}' -f ($genArgs -Join ' '));

#Write-Host $genCall
Invoke-Expression $genCall;

$buildCall = 'cmake --build .';

#Write-Host $buildCall
Invoke-Expression $buildCall

