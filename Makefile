all:
	g++ modular-synth-backend/src/synth-api/exception/LinkException.hpp \
		modular-synth-backend/src/synth-api/ports/Port.h \
		modular-synth-backend/src/synth-api/ports/InputPort.h \
		modular-synth-backend/src/synth-api/ports/OutputPort.h \
		modular-synth-backend/src/synth-api/ports/InputPort.cpp \
		-o a.out -Wall