# Aufsetzen Entwicklung Maschine

Hier wird beschrieben mit welchen Tools und Maschinen die Entwicklung stattfindet. Hintergrund ist das das Setup Entwicklerübergreifend möglichst homogen ist. 

Folgende Schritte müssen gemacht werden um das Entwicklungssystem aufzusetzen. 

## MacOS 

Benötigte Software 

XCode 11.3 CommandLines Tools installieren
(clang version 11.0.0 (clang-1100.0.33.16))

`/Applications/Xcode.app/Contents/Developer/usr/bin/xcodebuild -version`
`gcc --version`

CMake 3.15.6-Darwin installieren
`sudo "/Applications/CMake.app/Contents/bin/cmake-gui" --install`

`cmake --version` 

dotnet core 3.1.0 installieren

Clion Installieren (2019.3.2)

NodeJS installieren (v10.16.3)
`node --version`

yarn installieren (v1.17.3)
`yarn --version`

### SourceTree (optional)



## Windows VM

At first you need to install Chocolatey.  

### Win7 32-bit

1. Chrome V69.0.3497.100 
    `choco install googlechrome`

1. Powershell V6.1.0 win x86  
    `choco install powershell-core`

1. NodeJS V10.16.3  
    `choco install nodejs --version 10.16.3`

1. Git 2.19.1 32-bit  
    `choco install git`

1. LLVM 6.0.1 win 32bit  
    `choco install llvm --version 6.0.1 -y` 

1. CMake 3.12.3 win 32bit  
    `choco install cmake`

1. Visual Studio 2015 CE - 14.0.23197.0  
    `choco install visualstudio2015community --version 14.0.23107.0`

### Win10 64-bit

1. Chrome V70.0.3538.77 (64-bit) 
    `choco install googlechrome -y`

1. Powershell V6.1.0 win (64-bit)
    `choco install powershell-core -y`

1. NodeJS V10.16.3 (64-bit)  
    `choco install nodejs --version 8.12.0 -y`

1. Yarn V1.22.5 
    `choco install yarn --version 1.22.5 -y`

1. Git 2.19.1 64-bit  
    `choco install git --version 2.19.1 -y `

1. OpenSSH 7.7.2.1 64bit
    `choco install openssh --version 7.7.2.1 -y`

1. Sourcetree Tool 7.7.2.1 64bit
    manual install from `www.sourcetreeapp.com`

1. Visual Studio Code 1.28.2 64bit
    `choco install vscode --version 1.28.2 -y`

1. LLVM 6.0.1 win 64bit  
    `choco install llvm --version 7.0.0 -y` 

1. CMake 3.12.3 win 64bit  
    `choco install cmake --version 3.12.2 -y --installargs 'ADD_CMAKE_TO_PATH=System'`

1. MinGW 5.3.0  32&64bit  
    `choco install mingw --version 5.3.0 -y` 

### SetUp Access to git-repo

At first we need to create a private key and insert the corresponding public key to your GitHub Account. 

```
ssh-keygen -t rsa -b 4096 -C "sb@koertner-muth.com"
cat .ssh/id_rsa.pub
```

Also we need to config our git for the correct author and eMail string. Replace the user.name and user.email with your identity. 

```
git config --global user.name "Captain Future"
git config --global user.email sb@koertner-muth.com
```

Config `Sourcetree` under `Tools\Options\General\SSH Client Connection` to use the OpenSSH private-key under `.ssh/id_rsa`.


Do a clone of `git@github.com:CapitanFuture/LockbaseELI.git` to destination directory `C:\Users\SB\Projects\LockbaseELI`. Advanced Option mark checkbox `Recurse submodules`. 



 

