

cmake_minimum_required(VERSION 3.6)

project(HelloWorld C)

file(
    WRITE main.c
        "#include <stdio.h>\n"
        "int main() { printf( \"Hello World! \" ); return 0; }\n"
)

add_executable(${PROJECT_NAME} main.c)

## mkdir build 
## cd build 
## cmake -G"Visual Studio 14 2015" -T"LLVM-vs2014" ..
## cmake --build .
## debug/HelloWorld.exe

