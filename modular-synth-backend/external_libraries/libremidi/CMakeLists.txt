
ADD_LIBRARY(libremidi     
	libremidi.cpp
	reader.cpp
	writer.cpp
)
target_include_directories(libremidi PRIVATE "${CMAKE_SOURCE_DIR}/external_libraries")