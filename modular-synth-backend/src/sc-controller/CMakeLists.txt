

ADD_LIBRARY(servercommander 
"ServerSocket.cpp"
"SuperColliderCommander.cpp"
"SuperColliderController.cpp"
 "Synth.cpp" "Bus.cpp" "Node.cpp" "Group.cpp" "SuperColliderPacketBuilder.cpp")

target_include_directories(servercommander PUBLIC
                            "${CMAKE_SOURCE_DIR}"
                            "${CMAKE_SOURCE_DIR}/src/sc-controller"
                          "${CMAKE_SOURCE_DIR}/external_libraries/oscpack_1_1_0" 
                          )

