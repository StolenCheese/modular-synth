#include <doctest/doctest.h>

#include "synth-api/section/Section.h""
#include "synth-api/exception/LinkException.hpp"
#include "sc-controller/SuperColliderController.hpp"

//#define SYNTH_API_ROOT_FOLDER @SYNTHDEFS_FILE_LOCATION@
#define SYNTH_API_ROOT_FOLDER "D:\\REPOS\\modular-synth\\modular-synth-backend\\synthdefs"

TEST_CASE("We can link I/P to I/P on the same section, but not I/P to O/P on the same section") {
	synth_api::Section section = synth_api::Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", "");

	SUBCASE("Linking I/P to I/P on same section is allowed") {
		section.getPortFor("freq")->linkTo(section.getPortFor("mul"));
	}

	SUBCASE("Linking I/P to O/P on same section throws an exception") {
		CHECK_THROWS_AS(
			section.getPortFor("freq")->linkTo(section.getPortFor("out")),
			synth_api::CyclicLinksException
		);
	}
}

TEST_CASE("Cycle detection - throw exception") {
	synth_api::Section section1 = synth_api::Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", SYNTH_API_ROOT_FOLDER "\\sin-kr.scsyndef");
	synth_api::Section section2 = synth_api::Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", SYNTH_API_ROOT_FOLDER "\\sin-kr.scsyndef");

	SUBCASE("Trivial cycle detection; connecting same port to itself") {
		CHECK_THROWS_AS(
			section1.getPortFor("freq")->linkTo(section1.getPortFor("freq")),
			synth_api::CyclicLinksException
		);
	}
}

int main(int argc, char** argv) {
	auto endpoint = IpEndpointName("127.0.0.1", 58000);
	SuperColliderController::Connect(endpoint);
	SuperColliderController::get().dumpOSC(3);
	std::cout << "Ensure you have the SC Server started\n";
	doctest::Context context;
	context.applyCommandLine(argc, argv);
	int res = context.run();

	if (context.shouldExit()) {
		return res;
	}
}