cmake_minimum_required(VERSION 3.22)
project(modular_synth_backend)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
 
add_subdirectory(external_libraries)

file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/synthdefs
        DESTINATION
        ${CMAKE_CURRENT_BINARY_DIR})

set(SYNTHDEFS_FILE_LOCATION "${CMAKE_CURRENT_SOURCE_DIR}/synthdefs")

add_subdirectory(src)
add_subdirectory(tests)