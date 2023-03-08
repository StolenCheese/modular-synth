# Modular Synth

Cambridge CS Part 1B Project for group Lima - ModularSynth.io

## Dependencies

Make sure to have SuperCollider from <https://supercollider.github.io/downloads.html> installed, or the program will not run! The program is built off of their excellent platform.

## Description

A visual "old-school synth" style graphical interface for the SuperCollider audio server. Create sound by connecting wires between, and turning dials on, modules with a range of different functionalities, from an oscilator to a midi controller.

## Project Brief

Sophisticated digital music composition tools like the Sonic Pi language rely on an internal architecture of samples, waveforms and filters. In the popular SuperCollider system, a new synthesiser is defined by software-wiring together these "UGens”. Your task is to create a SuperCollider client that looks like a retro-style modular synthesiser or guitar pedal board, where connecting literal wires between pictures of hardware modules on the screen will construct an exact digital equivalent within the SuperCollider server. A live audio input would give you a universal guitar pedal, sample mixing makes you a DJ/producer, or if bleeps and whooshes are your thing, you can impress your Grandpa by channelling Brian Eno in the glory days of Roxy Music.

## Building

- All code in `modular-synth-backend` can be built using CMake version 3.22 or above. MSVC with the C++/CLR extension is required to build the library dll. Once built, to use the library in the frontend, copy and paste the `.dll` file from the build location to `modular-synth-frontend/`
- Code in `modular-synth-frontend` can be built using Visual Studio 2019 or 2022.
