

add_executable(controller_debug "SCCommandLine.cpp")

target_link_libraries(controller_debug PUBLIC  oscpack)
target_link_libraries(controller_debug PUBLIC  servercommander)
target_link_libraries(controller_debug PUBLIC  synthapi)

target_include_directories(controller_debug PUBLIC
                            "${CMAKE_SOURCE_DIR}" 
                            "${CMAKE_SOURCE_DIR}/include"
                          "${CMAKE_SOURCE_DIR}/external_libraries/oscpack_1_1_0" 
                          )
