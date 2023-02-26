cmake_minimum_required(VERSION 3.16)
project(modular_synth_backend)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
 
add_subdirectory(external_libraries)


add_subdirectory(src)