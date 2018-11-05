 #!/bin/sh

# Remove the previous build completely
rm -rf * 

# Create a makefile 
cmake -G "Unix Makefiles" \
    -DCMAKE_BUILD_TYPE=Debug \
    -DPAHO_WITH_SSL=FALSE \
    -DPAHO_BUILD_DOCUMENTATION=FALSE \
    -DPAHO_BUILD_SAMPLES=FALSE \
    -DCMAKE_VERBOSE_MAKEFILE=TRUE \
    .. 

# perform the build
cmake --build .
