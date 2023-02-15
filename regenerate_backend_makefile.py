import pathlib
import os

"""
It's easy to forget to add in random headers & cpp files to the makefile.

This handles all of that for you.

Run this from the ROOT of the project directory
"""

def regenerate_makefile(include_dir, output_file_name = "a.out"):
    if not os.path.exists(include_dir):
        print("Invalid include directory!")
        exit(1)
    command = ["all:\n", "\t" + "g++ \\\n", "\t" * 2 + "-I ", include_dir, " \\\n"]
    for file in pathlib.Path("modular-synth-backend").rglob("*.cpp"):
        command.append("\t" * 2 + str(file) + " \\\n")
    for file in pathlib.Path(include_dir).rglob("*.h"):
        command.append("\t" * 2 + str(file) + " \\\n")
    command.append("\t" * 2 + "-o " + output_file_name)
    return "".join(command)

if __name__ == "__main__":
    sanity_check = input("are you running me from the ROOT of the project (y/N)? ")
    if sanity_check != "y":
        print("exiting...")
        exit(0)
    with open("Makefile", "w+") as F:
        F.write(regenerate_makefile("modular-synth-backend/include"))
