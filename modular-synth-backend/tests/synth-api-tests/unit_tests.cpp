#include <doctest/doctest.h>

#include "synth-api/section/Section.h"
#include "synth-api/exception/LinkException.hpp"

//#define SYNTH_API_ROOT_FOLDER @SYNTHDEFS_FILE_LOCATION@
#define SYNTH_API_ROOT_FOLDER "D:REPOS\\modular-synth\\modular-synth-backend\\synthdefs"

TEST_CASE("We can link I/P to I/P on the same section, but not I/P to O/P on the same section") {
	synth_api::Section section = synth_api::Section(SYNTH_API_ROOT_FOLDER "\\sin-ar.scsyndef", "");

	SUBCASE("Linking I/P to I/P on same section is allowed") {
		CHECK_NOTHROW(section.getPortFor("freq")->linkTo(section.getPortFor("add")));
	}

	SUBCASE("Linking I/P to O/P on same section throws an exception") {
		CHECK_THROWS_AS(
			section.getPortFor("freq")->linkTo(section.getPortFor("out")), 
			synth_api::CyclicLinksException
		);
	}
}