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
SynthDef("sin-mix-ar", { arg freq1= 440,freq2=440,freq3=440,freq4=440,freq5=440, phase= 0.0, mul= 1.0, add= 0.0, out=0;
	y = [freq1,freq2,freq3,freq4,freq5];
	Out.ar(out,Mix.new(SinOsc.ar(y, phase, Clip.kr(y * mul), add)));
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
SynthDef("decay2-ar", { arg ain = 2, attackTime= 0.01, decayTime= 1.0, mul= 1.0, add= 0.0,out=0;
	Out.ar(out, Decay2.ar(AudioIn.ar(ain), attackTime, decayTime, mul, [add,add]));
}).writeDefFile(x);
)


{Out.ar(0, Decay2.ar(SinOsc.ar(440), 0.1, 0.1))}.play
{SinOsc.ar(440)}.play

(
SynthDef("mdapiano-ar", { arg freq= 440.0, gate= 1, vel= 100, decay= 0.8, release= 0.8, hard= 0.8, velhard= 0.8, muffle= 0.8, velmuff= 0.8, velcurve= 0.8, stereo= 0.2, tune= 0.5, random= 0.1, stretch= 0.1, sustain= 0, mul= 1, add= 0.0,out=0;
	Out.ar(out, MdaPiano.ar(freq, gate, vel, decay, release, hard, velhard, muffle, velmuff, velcurve, stereo, tune, random, stretch, sustain, mul, add));
}).writeDefFile(x);
)


(
SynthDef( "organ", { |freq = 440, gate = 1, amp = 0.1|
        var sig;
        sig = LFPar.ar( freq * [1,2,3,5], 0, amp/[2,4,5,7] );
	Out.ar( 0, EnvGen.kr(Env.adsr(), gate) * sig.dup )
}).writeDefFile(x);
);


(
SynthDef("impulse-decay2-kr", { arg freq= 440.0, phase= 0.0, attackTime= 0.01, decayTime= 1.0, mul= 1.0, add= 0.0,out=0;
	Out.kr(out, Decay2.kr(Impulse.kr(freq, phase), attackTime, decayTime, mul, add));
}).writeDefFile(x);
)

{Out.ar(0,OteyPianoStrings.ar())}.play
{ LPF.ar(Saw.ar(200), 600) * EnvGate.new(fadeTime:200) }.play;

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