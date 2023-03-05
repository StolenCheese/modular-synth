add_executable(synthapitests "unit_tests.cpp")
#configure_file("unit_tests.cpp" "unit_tests.cpp")

target_link_libraries(synthapitests PUBLIC  servercommander)
target_link_libraries(synthapitests PUBLIC  synthapi)
target_link_libraries(synthapitests PUBLIC  oscpack)
target_link_libraries(synthapitests PUBLIC  doctest)

target_include_directories(synthapitests PUBLIC
                            "${CMAKE_SOURCE_DIR}" 
                            "${CMAKE_SOURCE_DIR}/include"
                          )
