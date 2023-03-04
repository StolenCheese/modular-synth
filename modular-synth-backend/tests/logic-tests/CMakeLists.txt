

add_executable(coreTests "coreTests.cpp")

target_link_libraries(coreTests PUBLIC  oscpack)
target_link_libraries(coreTests PUBLIC  servercommander)
target_link_libraries(coreTests PUBLIC  synthapi)

target_include_directories(coreTests PUBLIC
                            "${CMAKE_SOURCE_DIR}" 
                            "${CMAKE_SOURCE_DIR}/include"
                          )
