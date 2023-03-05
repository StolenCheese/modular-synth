#include <doctest/doctest.h>

#include "synth-api/section/Section.h""
#include "synth-api/exception/LinkException.hpp"
#include "sc-controller/SuperColliderController.hpp"

//#define SYNTH_API_ROOT_FOLDER @SYNTHDEFS_FILE_LOCATION@
#define SYNTH_API_ROOT_FOLDER "D:\\REPOS\\modular-synth\\modular-synth-backend\\synthdefs"

TEST_CASE("We can link I/P to I/P on the same section, but not I/P to O/P on the same section") {
	synth_api::Section section = synth_api::Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", "");

	SUBCASE("Linking I/P to I/P on same section is allowed") {
		CHECK_NOTHROW(section.getPortFor("mul")->linkTo(section.getPortFor("add")));
	}

	SUBCASE("Linking I/P to O/P on same section throws an exception") {
		CHECK_THROWS_AS(
			section.getPortFor("freq")->linkTo(section.getPortFor("out")), 
			synth_api::CyclicLinksException
		);
	}
}

int main(int argc, char** argv) {
	auto endpoint = IpEndpointName("127.0.0.1", 58000);
	SuperColliderController::Connect(endpoint);
	std::cout << "Ensure you have the SC Server started\n";
	doctest::Context context;
	context.applyCommandLine(argc, argv);
	int res = context.run();

	if (context.shouldExit()) {
		return res;
	}
}