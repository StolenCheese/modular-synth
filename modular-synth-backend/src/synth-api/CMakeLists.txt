
file(GLOB synthapi_SRC
     "**/*.cpp"
)

ADD_LIBRARY(synthapi ${synthapi_SRC})

target_include_directories(synthapi PUBLIC
                            "${CMAKE_SOURCE_DIR}"
                            "${CMAKE_SOURCE_DIR}/src/sc-controller"
                          "${CMAKE_SOURCE_DIR}/external_libraries/oscpack_1_1_0" 
                            "${CMAKE_SOURCE_DIR}/include"
                          )