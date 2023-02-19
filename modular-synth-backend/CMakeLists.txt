cmake_minimum_required(VERSION 3.10)
project(modular_synth_backend)
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
 

add_subdirectory(external_libraries)


add_subdirectory(src)