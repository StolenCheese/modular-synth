
(
SynthDef( "organ", { |freq = 440, gate = 1, amp = 0.1|
        var sig;
        sig = LFPar.ar( freq * [1,2,3,5], 0, amp/[2,4,5,7] );
	Out.ar( 0, EnvGen.kr(Env.adsr(), gate) * sig.dup )
}).add;
);

(
{
    var env = Env([0, 1, 0.5, 1, 0], [0.01, 0.5, 0.02, 0.5]);
    SinOsc.ar(470) * EnvGen.kr(env, doneAction: Done.freeSelf)
}.play
)


x = Synth(\organ)

x.set(\gate, -1.1)

x.set(\gate, 1)


x.free

s.queryAllNodes;