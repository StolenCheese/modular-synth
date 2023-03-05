#include <doctest/doctest.h>

#include "synth-api/section/Section.h"
#include "synth-api/exception/LinkException.hpp"
#include "sc-controller/SuperColliderController.hpp"

//#define SYNTH_API_ROOT_FOLDER @SYNTHDEFS_FILE_LOCATION@
#define SYNTH_API_ROOT_FOLDER "D:\\REPOS\\modular-synth\\modular-synth-backend\\synthdefs"
#define LINK(sxn1, param1, sxn2, param2) (sxn1.getPortFor(param1)->linkTo(sxn2.getPortFor(param2)))
#define UNLINK(sxn1, param1, sxn2, param2) (sxn1.getPortFor(param1)->removeLink(sxn2.getPortFor(param2)))
#define GET_CONNECTIONS(sxn, port) (TestingProdder::getConnections(sxn.getPortFor(port)))
#define ARE_CONNECTED(sxn1, param1, sxn2, param2) ((GET_CONNECTIONS(sxn1, param1)->find(sxn2.getPortFor(param2)) != GET_CONNECTIONS(sxn1, param1)->cend()) && ((GET_CONNECTIONS(sxn2, param2)->find(sxn1.getPortFor(param1)) != GET_CONNECTIONS(sxn2, param2)->cend())))

using namespace synth_api;

namespace synth_api {
	class TestingProdder {
	public:
		static Port* getControllerFrom(InputPort* port) {
			return port->controller;
		}
		static std::set<Port*>* getConnections(Port* port) {
			return &port->outgoingConnections;
		}
	};
}

TEST_CASE("Wire linking logic") {
	Section section1 = Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", "");
	Section section2 = Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", SYNTH_API_ROOT_FOLDER "\\sin-kr.scsyndef");
	Section section3 = Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", SYNTH_API_ROOT_FOLDER "\\sin-kr.scsyndef");

	SUBCASE("Same section") {
		SUBCASE("Linking I/P to I/P on same section is allowed") {
			REQUIRE_NOTHROW(LINK(section1, "freq", section1, "mul"));

			// Check that the ports have each other added as connections
			CHECK(ARE_CONNECTED(section1, "mul", section1, "freq"));

			// Check that the controller relation is set up correct
			CHECK(TestingProdder::getControllerFrom(dynamic_cast<InputPort *>(section1.getPortFor("mul"))) == nullptr);
			CHECK(TestingProdder::getControllerFrom(dynamic_cast<InputPort*>(section1.getPortFor("freq"))) == section1.getPortFor("mul"));

			// Clean up
			REQUIRE_NOTHROW(UNLINK(section1, "freq", section1, "mul"));
		}

		SUBCASE("Linking I/P to O/P on same section throws an exception") {
			CHECK_THROWS_AS(
				LINK(section1, "freq", section1, "out"),
				synth_api::CyclicLinksException
			);
		}
	}

	SUBCASE("Different sections") {
		SUBCASE("Linking I/P to I/P across multiple sections") {
			// Set up initial links
			// (S1).add <---> (S1).freq <---> (S2).freq <---> (S3).freq
			REQUIRE_NOTHROW(LINK(section1, "freq", section1, "add"));
			REQUIRE_NOTHROW(LINK(section1, "freq", section2, "freq"));
			REQUIRE_NOTHROW(LINK(section2, "freq", section3, "freq"));

			// Check that ports have each other added as connections
			CHECK(ARE_CONNECTED(section1, "freq", section1, "add"));
			CHECK(ARE_CONNECTED(section1, "freq", section2, "freq"));
			CHECK(ARE_CONNECTED(section2, "freq", section3, "freq"));

			// Clean up
			REQUIRE_NOTHROW(UNLINK(section1, "freq", section1, "add"));
			REQUIRE_NOTHROW(UNLINK(section1, "freq", section2, "freq"));
			REQUIRE_NOTHROW(UNLINK(section2, "freq", section3, "freq"));
		}

		SUBCASE("Linking I/P and O/P across multiple sections") {
			// Set up initial links
			// (S1).out <---> (S2).freq <---> (S2).add
			// (S2).out <---> (S3).freq
			REQUIRE_NOTHROW(LINK(section1, "out", section2, "freq"));
			REQUIRE_NOTHROW(LINK(section2, "freq", section2, "add"));
			REQUIRE_NOTHROW(LINK(section2, "out", section3, "freq"));

			// Check that ports have each other added as connections
			CHECK(ARE_CONNECTED(section1, "out", section2, "freq"));
			CHECK(ARE_CONNECTED(section2, "freq", section2, "add"));
			CHECK(ARE_CONNECTED(section2, "out", section3, "freq"));

			// Clean up
			REQUIRE_NOTHROW(UNLINK(section1, "out", section2, "freq"));
			REQUIRE_NOTHROW(UNLINK(section2, "freq", section2, "add"));
			REQUIRE_NOTHROW(UNLINK(section2, "out", section3, "freq"));
		}
	}
}

TEST_CASE("Cycle detection") {
	Section section1 = Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", SYNTH_API_ROOT_FOLDER "\\sin-kr.scsyndef");
	Section section2 = Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", SYNTH_API_ROOT_FOLDER "\\sin-kr.scsyndef");

	SUBCASE("Trivial cycle detection; connecting same port to itself") {
		CHECK_THROWS_AS(
			LINK(section1, "freq", section1, "freq"),
			synth_api::CyclicLinksException
		);
	}

	SUBCASE("Trivial cycle detection; Connecting I/P to O/P on same section") {
		CHECK_THROWS_AS(
			LINK(section1, "out", section1, "freq"),
			synth_api::CyclicLinksException
		);
	}

	SUBCASE("Cross-section loop through symbolic links") {
		// Set up the initial link
		// (S1).freq - - -> (S1).out <-----> (S2).freq
		REQUIRE_NOTHROW(LINK(section1, "out", section2, "freq"));

		// Set up the cyclic link
		CHECK_THROWS_AS(
			LINK(section1, "freq", section2, "freq"),
			synth_api::CyclicLinksException
		);

		// Clean up
		REQUIRE_NOTHROW(UNLINK(section1, "out", section2, "freq"));
	}

	SUBCASE("Cross-section loop via input daisy chain") {
		// Set up the initial links
		// (S1).add <---> (S1).freq <---> (S2).add
		REQUIRE_NOTHROW(LINK(section1, "freq", section2, "add"));
		REQUIRE_NOTHROW(LINK(section1, "freq", section1, "add"));

		// Set up cyclic link
		CHECK_THROWS_AS(
			LINK(section1, "add", section2, "add"),
			synth_api::CyclicLinksException
		);

		// Clean up
		REQUIRE_NOTHROW(UNLINK(section1, "freq", section2, "add"));
		REQUIRE_NOTHROW(UNLINK(section1, "freq", section1, "add"));
	}
}

TEST_CASE("Output to Output logic") {
	Section section1 = Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", SYNTH_API_ROOT_FOLDER "\\sin-kr.scsyndef");
	Section section2 = Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", SYNTH_API_ROOT_FOLDER "\\sin-kr.scsyndef");
	Section section3 = Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", SYNTH_API_ROOT_FOLDER "\\sin-kr.scsyndef");
	Section section4 = Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", SYNTH_API_ROOT_FOLDER "\\sin-kr.scsyndef");

	SUBCASE("Linking O/P to each other DIRECTLY throw OutputToOutputException") {
		CHECK_THROWS_AS(
			LINK(section1, "out", section2, "out"),
			synth_api::OutputToOutputException
		);
	}

	SUBCASE("Linking two bound I/P to each other throws OutputToOutputException") {
		// Set up initial links
		// (S1).out <---> (S2).freq         (S3).freq <---> (S4).out
		REQUIRE_NOTHROW(LINK(section1, "out", section2, "freq"));
		REQUIRE_NOTHROW(LINK(section3, "freq", section4, "out"));

		// Set up output binding link
		CHECK_THROWS_AS(
			LINK(section2, "freq", section3, "freq"),
			synth_api::OutputToOutputException
		);

		// Clean up
		REQUIRE_NOTHROW(UNLINK(section1, "out", section2, "freq"));
		REQUIRE_NOTHROW(UNLINK(section3, "freq", section4, "out"));
	}

	SUBCASE("Linking a bound I/P to an O/P throws AlreadyBoundInputException") {
		// Set up initial 
		// (S1).out <---> (S2).freq <---> (S3).freq         (S4).out
		REQUIRE_NOTHROW(LINK(section1, "out", section2, "freq"));
		REQUIRE_NOTHROW(LINK(section2, "freq", section3, "freq"));

		// Set up output binding link
		CHECK_THROWS_AS(
			LINK(section3, "freq", section4, "out"),
			synth_api::AlreadyBoundInputException
		);

		// Clean up
		REQUIRE_NOTHROW(UNLINK(section1, "out", section2, "freq"));
		REQUIRE_NOTHROW(UNLINK(section2, "freq", section3, "freq"));

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