# Set-ExecutionPolicy Unrestricted 

$generator = 'MinGW Makefiles'

$buildType = 'Debug'
$path = '..'

Remove-Item CMakeFiles -Recurse -Force
Remove-Item CMakeCache.txt -Force

$genArgs = @('-G "{0}"' -f $generator )
$genArgs += ('-DCMAKE_BUILD_TYPE={0}' -f $buildType);
$genArgs += '-DCMAKE_VERBOSE_MAKEFILE=FALSE';
$genArgs += ('{0}' -f $path);

# Create the generate call
$genCall = ('cmake {0}' -f ($genArgs -Join ' '));
Invoke-Expression $genCall;

Invoke-Expression 'cmake --build . --target LbELI_cli -- -j 2'

if( -not $? )
{
	$msg = $Error[0].Exception.Message
	"Encountered error during cmake build. Error Message is $msg. Please check." 
	exit
}

Copy-Item ".\LbwELI.Demo.dll" "/LockbaseDemo/Lockbase" -Force