all:
	g++ \
		modular-synth-backend/src/synth-api/exception/LinkException.hpp \
		modular-synth-backend/src/synth-api/ports/Port.h \
		modular-synth-backend/src/synth-api/ports/Port.cpp \
		modular-synth-backend/src/synth-api/ports/InputPort.h \
		modular-synth-backend/src/synth-api/ports/InputPort.cpp \
		modular-synth-backend/src/synth-api/ports/OutputPort.h \
		modular-synth-backend/src/synth-api/ports/OutputPort.cpp \
		modular-synth-backend/src/synth-api/section/Section.h \
		modular-synth-backend/src/synth-api/section/Section.cpp \
		modular-synth-backend/src/synth-api/test.cpp \
		-o a.out -Wall