#include <doctest/doctest.h>

#include "synth-api/section/Section.h""
#include "synth-api/exception/LinkException.hpp"
#include "sc-controller/SuperColliderController.hpp"

//#define SYNTH_API_ROOT_FOLDER @SYNTHDEFS_FILE_LOCATION@
#define SYNTH_API_ROOT_FOLDER "D:\\REPOS\\modular-synth\\modular-synth-backend\\synthdefs"

TEST_CASE("We can link I/P to I/P on the same section, but not I/P to O/P on the same section") {
	synth_api::Section section = synth_api::Section(SYNTH_API_ROOT_FOLDER "\\sin-kr.scsyndef", "");

	SUBCASE("Linking I/P to I/P on same section is allowed") {
		for (auto kv : section.inputPorts) {
			std::cout << kv.first << " ";
			std::cout << section.getPortFor(kv.first) << "\n";
		}
		section.getPortFor("freq")->linkTo(section.getPortFor("mul"));
		std::cout << "freq connections\n";
		for (auto kv : section.getPortFor("freq")->outgoingConnections) {
			std::cout << kv << "\n";
		}
		std::cout << "mul connections\n";
		for (auto kv : section.getPortFor("mul")->outgoingConnections) {
			std::cout << kv << "\n";
		}
	}

	SUBCASE("Linking I/P to O/P on same section throws an exception") {
		CHECK_THROWS_AS(
			section.getPortFor("freq")->linkTo(section.getPortFor("out")),
			synth_api::CyclicLinksException
		);
	}
	std::cout << "end of test\n";
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