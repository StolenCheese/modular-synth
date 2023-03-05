ADD_LIBRARY(doctest 
	doctest.h
	"parts/doctest.cpp")
	
target_include_directories(doctest PRIVATE "${CMAKE_SOURCE_DIR}/external_libraries")