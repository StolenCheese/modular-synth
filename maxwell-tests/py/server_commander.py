
import mido
from notes import note, midi_node
import random
import time

from pythonosc import udp_client
from pythonosc import osc_packet
from pythonosc import osc_message
import socket

from supercollider_udp_client import *
from supercollider_server import *


def isfloat(num):
    try:
        float(num)
        return True
    except ValueError:
        return False


def augment(arg: str):
    # convert command parameter to format wanted

    if arg.isdigit():
        return int(arg)
    # turn . and = into lists or tuples

    s = arg.split('=', 2)
    if len(s) == 2:
        if s[1].isdigit():
            s[1] = int(s[1])
        elif isfloat(s[1]):
            s[1] = float(s[1])
        else:
            s[1] = note(s[1])
        return {s[0]: s[1]}

    s = arg.split('.')
    if len(s) > 1:
        return [augment(a) for a in s]

    return arg


def merge_dicts(*dict_args):
    """
    Given any number of dictionaries, shallow copy and merge into a new dict,
    precedence goes to key-value pairs in latter dictionaries.
    """
    result = {}
    for dictionary in dict_args:
        result.update(dictionary)
    return result


def run_command(server: SuperColliderServer, command: str):
    c = [augment(a) for a in command.split()]
    print(c)

    # ignore these variable names please
    client = server._server

    match c:
        case ["add", f_name]:
            with open(f"synthdefs/{f_name}.scsyndef", "rb") as f:  # opening for [r]eading as [b]inary
                client._server.d_recv(f.read())

        case ["del", f_name]:
            client.d_free(f_name)

        case ["play", synth, int(id), *t]:
            client.s_new(synth,  id, AddAction.groupTail, 0,  *t)

        case ["play", int(id)]:
            client[id].play()

        case ["pause", int(id)]:
            client[id].pause()

        case ["run", int(id), int(on)]:
            client[id].run(not not on)

        case ["stop", *ids]:
            client.n_free(*ids)

        case ["list"]:
            print(client.ids)

        case ["set", int(id), *t]:
            print(f"setting {id} values: {t}")

            client.n_set(id, *merge_dicts(*t).items())

        case ["bus", "set",  *t]:  # set bus at index to value
            client.c_set(t)

        case ["bus", "get",  *t]:  # set bus at index to value
            print(t)
            print(client.c_get(*t))

        # map 1000 freq=bus1 ...
        case ["map", int(id),  *t]:  # (maps to bus)
            client.n_map(id, *t)

        # connect 100 to 1000.freq
        case ["connect", int(source), "to", [int(dest), param]]:
            server[source]["out"].connect(server[dest], param)

        case ["get", int(id), *params]:
            client.s_get(id, params)

        case ["q", int(id)]:
            client.n_query(id)
        case ["q"]:
            print("\n".join([f"{id}: {data}" for (id, data) in client.g_queryTree((0, 1)).items()]))

        case ["ls"]:
            server.sync()
            print("\n".join([f"{id}: {s}" for (id, s) in server.synths.items()]))
        case c:
            print(f"unknown command {c}")


if __name__ == "__main__":
    # https://doc.sccode.org/Reference/Server-Architecture.html
    # https://doc.sccode.org/Reference/Server-Command-Reference.html

    # run ./scsynth.exe -u 57117 in your scsynth install directory
    # C:\Program Files\SuperCollider-3.13.0-rc1 for me

    # cd "C:\Program Files\SuperCollider-3.13.0-rc1"; ./scsynth.exe -u 58000

    # m = osc_packet.OscPacket(b"\x23\x62\x75\x6e\x64\x6c\x65\x00\x00\x00\x00\x00\x00\x00\x00\x01\x00\x00\x00\x14\x2f\x63\x5f\x73\x65\x74\x00\x00\x2c\x69\x69\x00\x00\x00\x00\x00\x00\x00\x03\x70")
    # m = osc_packet.OscPacket(b"\x23\x62\x75\x6e\x64\x6c\x65\x00\x00\x00\x00\x00\x00\x00\x00\x01\x00\x00\x00\x38\x2f\x6e\x5f\x6d\x61\x70\x6e\x00\x2c\x69\x73\x69\x69\x73\x69\x69\x00\x00\x00\x00\x00\x00\x03\xe8\x66\x72\x65\x71\x31\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x01\x66\x72\x65\x71\x32\x00\x00\x00\x00\x00\x00\x01\x00\x00\x00\x01")

    ip = "127.0.0.1"
    port = 59000

    client = SuperColliderServer(ip, port)

    client.notify = True

    # example series of commands:
    # add sine_with_freq
    # play sine_with_freq 500 freq c#3
    # pause 500
    # play 500
    # set 500 freq c#4
    # add drum
    # play drum 600
    # stop 500

    client.add_scsyndef("impulse-decay2-kr")
    client.add_scsyndef("sin-ar")

    client.add_scsyndef("pinknoise-kr")
    client.add_scsyndef("sin-kr")

    # Play the terraria underground theme
    mid = mido.MidiFile('Underground.mid')
    synths: dict[int, Synth] = {}
    for msg in mid.play():
        if msg.type == "note_on":
            if msg.channel in synths:
                synths[msg.channel].free()

            synths[msg.channel] = client.create_synth(
                "sin-ar", AddAction.groupHead, client[0], freq=midi_node(msg.note))
        elif msg.type == "note_off":
            synths[msg.channel].free()

    for i in range(1, 4):
        bus = client.create_bus()

        sinar = client.create_synth("sin-ar", AddAction.groupHead, client[0], freq=bus)

        impulsekr = client.create_synth("impulse-decay2-kr", AddAction.groupHead, client[0], out=bus, freq=2 ** i, mul=300/i)

    #bus2.connect(sinar, "freq")

    while True:
        time.sleep(0.1)
        print(">>> ", end="")
        run_command(client, input())
