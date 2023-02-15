cmake_minimum_required(VERSION 3.10)
project(modular_synth_backend)
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
 
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++20")

add_subdirectory(external_libraries)


add_subdirectory(src)