x = "A:/Documents/synth/modular-synth/maxwell-tests/py/synthdefs/";

(
SynthDef("sin-kr", { arg freq= 440.0, phase= 0.0, mul= 1.0, add= 0.0, out=0;
    Out.kr(out, SinOsc.kr(freq, phase, mul, add));
}).writeDefFile(x);
)

(
SynthDef("sin-ar", { arg freq= 440.0, phase= 0.0, mul= 1.0, add= 0.0, out=0;
	Out.ar(out, SinOsc.ar([freq,freq], phase, mul, add));
}).writeDefFile(x);
)

(
SynthDef("impulse-ar", { arg freq= 440.0, phase= 0.0, mul= 1.0, add= 0.0, out=0;
	Out.ar(out, Impulse.ar([freq,freq], phase, mul, add));
}).writeDefFile(x);
)

(
SynthDef("impulse-kr", { arg freq= 440.0, phase= 0.0, mul= 1.0, add= 0.0, out=0;
	Out.kr(out, Impulse.kr(freq, phase, mul, add));
}).writeDefFile(x);
)

(
SynthDef("impulse-decay2-ar", { arg freq= 440.0, phase= 0.0, attackTime= 0.01, decayTime= 1.0, mul= 1.0, add= 0.0,out=0;
	Out.ar(out, Decay2.ar(Impulse.ar(freq, phase), attackTime, decayTime, mul, [add,add]));
}).writeDefFile(x);
)

(
SynthDef("impulse-decay2-kr", { arg freq= 440.0, phase= 0.0, attackTime= 0.01, decayTime= 1.0, mul= 1.0, add= 0.0,out=0;
	Out.kr(out, Decay2.kr(Impulse.kr(freq, phase), attackTime, decayTime, mul, add));
}).writeDefFile(x);
)


(
SynthDef("whitenoise-ar", { arg mul= 1.0, add= 0.0, out=0;
	Out.ar(out, WhiteNoise.ar([mul,mul], add));
}).writeDefFile(x);
)

(
SynthDef("whitenoise-kr", { arg mul= 1.0, add= 0.0, out=0;
	Out.kr(out, WhiteNoise.kr(mul, add));
}).writeDefFile(x);
)


(
SynthDef("brownnoise-ar", { arg mul= 1.0, add= 0.0, out=0;
	Out.ar(out, BrownNoise.ar([mul,mul], add));
}).writeDefFile(x);
)

(
SynthDef("brownnoise-kr", { arg mul= 1.0, add= 0.0, out=0;
	Out.kr(out, BrownNoise.kr(mul, add));
}).writeDefFile(x);
)



(
SynthDef("pinknoise-ar", { arg mul= 1.0, add= 0.0, out=0;
	Out.ar(out, PinkNoise.ar([mul,mul], add));
}).writeDefFile(x);
)

(
SynthDef("pinknoise-kr", { arg mul= 1.0, add= 0.0, out=0;
	Out.kr(out, PinkNoise.kr(mul, add));
}).writeDefFile(x);
)
(
SynthDef("graynoise-ar", { arg mul= 1.0, add= 0.0, out=0;
	Out.ar(out, GrayNoise .ar([mul,mul], add));
}).writeDefFile(x);
)

(
SynthDef("graynoise-kr", { arg mul= 1.0, add= 0.0, out=0;
	Out.kr(out, GrayNoise.kr(mul, add));
}).writeDefFile(x);
)

(
SynthDef("clipnoise-ar", { arg mul= 1.0, add= 0.0, out=0;
	Out.ar(out, ClipNoise .ar([mul,mul], add));
}).writeDefFile(x);
)

(
SynthDef("clipnoise-kr", { arg mul= 1.0, add= 0.0, out=0;
	Out.kr(out, ClipNoise .kr(mul, add));
}).writeDefFile(x);
)