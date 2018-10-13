# Set-ExecutionPolicy Unrestricted 

$clang = '"C:/LLVM/bin/clang-cl.exe"'
$clangxx = '"C:/Program\ Files/LLVM/bin/clang++.exe"'
$make = '"C:/MinGW/bin/mingw32-make.exe"'
$generator = '"MinGW Makefiles"'
$buildType = 'Debug'
$path = '.'
$buildPath = './build'


# https://www.powershellgallery.com/packages/WebKitDev/0.1.12/Content/Functions%5CInvoke-CMakeBuild.ps1

# Remove temp files

Remove-Item build -Recurse -Force 
Remove-Item CMakeFiles -Recurse -Force
Remove-Item CMakeCache.txt -Force
Remove-Item cmake_install.cmake -Force
Remove-Item Makefile -Force

$genArgs = @('-G')
$genArgs += @('"MinGW Makefiles"');

$genArgs += ('-DCMAKE_BUILD_TYPE={0}' -f $buildType);
$genArgs += ('-DCMAKE_C_COMPILER={0}' -f $clang);
$genArgs += ('-DCMAKE_MAKE_PROGRAM={0}' -f $make);

$genArgs += ('-B {0}' -f $buildPath);
$genArgs += ('{0}' -f $path);

# Create the generate call
$genCall = ('cmake {0}' -f ($genArgs -Join ' '));

# Write-Host $genCall
Invoke-Expression $genCall

# $buildCall = ('cmake {0}' -f ($buildArgs -Join ' '));

#Write-Host $buildCall
#Invoke-Expression $buildCall

# Call cmake 
#Write-Host $clang
#cmake -G $generator -DCMAKE_BUILD_TYPE=DEBUG -DCMAKE_C_COMPILER=$clang . 

# "-DCMAKE_C_COMPILER=$clang -DCMAKE_MAKE_PROGRAMM=$make -DCMAKE_BUILD_TYPE=Debug . " 