set(project_name synth-api-managed)

add_library(${project_name} SHARED
	"SCController.cpp" 
	"SCSection.cpp" 
	"SCPort.cpp")

target_link_libraries(${project_name} PUBLIC  oscpack)
target_link_libraries(${project_name} PUBLIC  servercommander)
target_link_libraries(${project_name} PUBLIC  synthapi)

target_include_directories(${project_name} PUBLIC
                            "${CMAKE_SOURCE_DIR}"
                            "${CMAKE_SOURCE_DIR}/src/sc-controller"
                            "${CMAKE_SOURCE_DIR}/include/synthapi"
                          "${CMAKE_SOURCE_DIR}/external_libraries/oscpack_1_1_0" 
                          )


target_compile_options(${project_name} PRIVATE /clr)
#target_compile_options(${project_name} PRIVATE /fp:precise)


#set_property(TARGET ${project_name} PROPERTY VS_GLOBAL_ROOTNAMESPACE ${project_name})

#set_property(TARGET ${project_name} PROPERTY VS_GLOBAL_KEYWORD "ManagedCProj")

#set_property(TARGET ${project_name} PROPERTY VS_GLOBAL_CLRSupport "true")

set_target_properties(${project_name} 
	PROPERTIES  COMMON_LANGUAGE_RUNTIME ""
				VS_DOTNET_REFERENCES   "System")

set_property(TARGET ${project_name} PROPERTY VS_DOTNET_REFERENCES "System" "System.Data")


# Note: Modification of compiler flags is required for CLR compatibility now that we are using .resx files.
string(REPLACE "/EHsc" "" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
string(REPLACE "/RTC1" "" CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG}")