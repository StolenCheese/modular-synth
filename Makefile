all:
	g++ \
		-I modular-synth-backend/include \
		modular-synth-backend/src/synth-api/test.cpp \
		modular-synth-backend/src/synth-api/ports/InputPort.cpp \
		modular-synth-backend/src/synth-api/ports/OutputPort.cpp \
		modular-synth-backend/src/synth-api/ports/Port.cpp \
		modular-synth-backend/src/synth-api/section/Section.cpp \
		modular-synth-backend/src/synth-api/section/_model/PortManager.cpp \
		modular-synth-backend/include/synth-api/ports/InputPort.h \
		modular-synth-backend/include/synth-api/ports/OutputPort.h \
		modular-synth-backend/include/synth-api/ports/Port.h \
		modular-synth-backend/include/synth-api/section/Section.h \
		modular-synth-backend/include/synth-api/section/_model/PortManager.h \
		-o a.out