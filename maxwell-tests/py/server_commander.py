"""Small example OSC client

This program sends 10 random values between 0.0 and 1.0 to the /filter address,
waiting for 1 seconds between each value.
"""

from notes import note
import random
import time

from pythonosc import udp_client
from pythonosc import osc_packet
from pythonosc import osc_message
import socket

from SuperColliderUDPClient import *


def decode_response(packet: osc_message.OscMessage):

    match packet.address:
        case "/status.reply":
            return {x: packet.params[i] for i, x in enumerate(
                ("1. unused.",
                 "number of unit generators.",
                 "number of synths.",
                 "number of groups.",
                 "number of loaded synth definitions.",
                 "average percent CPU usage for signal processing",
                 "peak percent CPU usage for signal processing",
                 "nominal sample rate",
                 "actual sample rate",
                 )
            )}


def augment(arg: str):
    # convert command parameter to format wanted

    if arg.isdigit():
        return int(arg)
    # turn . and = into lists or tuples
    s = arg.split('.')
    if len(s) > 1:
        return [augment(a) for a in s]

    s = arg.split('=', 2)
    if len(s) == 2:
        s[1] = note(s[1])
        return (s[0], s[1])

    return arg


def run_command(client: SuperColliderUPDClient, command: str):
    c = [augment(a) for a in command.split()]

    match c:
        case ["add", f_name]:
            with open(f"synthdefs/{f_name}.scsyndef", "rb") as f:  # opening for [r]eading as [b]inary
                client.d_recv(f.read())

        case ["del", f_name]:
            client.d_free(f_name)

        case ["play", synth, int(id), *t]:
            client.s_new(synth,  id, AddAction.groupTail, 0,  *t)
        case ["play", int(id)]:
            client.n_run(id, 1)

        case ["pause", int(id)]:
            client.n_run(id, 0)

        case ["run", int(id), int(on)]:
            client.n_run(id, on)

        case ["stop", *ids]:
            client.n_free(*ids)

        case ["list"]:
            print(client.ids)

        case ["set", int(id), *t]:
            print(f"setting {id} values: {t}")
            client.n_set(id, *t)

        case ["bus", "set",  *t]:  # set bus at index to value
            client.c_set(t)
        case ["bus", "get",  *t]:  # set bus at index to value
            print(t)
            print(client.c_get(*t))

        case ["map", int(id),  *t]:  # (maps to bus)
            client.n_map(id, *t)

        case ["connect", int(source), "to", [int(dest), param]]:
            s_data = client.s_get(source, "bus")
            bus = s_data['bus']
            client.n_map(dest, (param, bus))

        case ["get", int(id), *params]:
            client.s_get(id, params)

        case ["q", int(id)]:
            client.n_query(id)
        case ["q"]:
            print("\n".join([f"{id}: {data}" for (id, data) in client.g_queryTree((0, 1)).items()]))
        case c:
            print(f"unknown command {c}")


if __name__ == "__main__":
    # https://doc.sccode.org/Reference/Server-Architecture.html
    # https://doc.sccode.org/Reference/Server-Command-Reference.html

    # run ./scsynth.exe -u 57117 in your scsynth install directory
    # C:\Program Files\SuperCollider-3.13.0-rc1 for me

    # m = osc_packet.OscPacket(b"\x23\x62\x75\x6e\x64\x6c\x65\x00\x00\x00\x00\x00\x00\x00\x00\x01\x00\x00\x00\x14\x2f\x63\x5f\x73\x65\x74\x00\x00\x2c\x69\x69\x00\x00\x00\x00\x00\x00\x00\x03\x70")
    # m = osc_packet.OscPacket(b"\x23\x62\x75\x6e\x64\x6c\x65\x00\x00\x00\x00\x00\x00\x00\x00\x01\x00\x00\x00\x38\x2f\x6e\x5f\x6d\x61\x70\x6e\x00\x2c\x69\x73\x69\x69\x73\x69\x69\x00\x00\x00\x00\x00\x00\x03\xe8\x66\x72\x65\x71\x31\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x01\x66\x72\x65\x71\x32\x00\x00\x00\x00\x00\x00\x01\x00\x00\x00\x01")

    ip = "127.0.0.1"
    port = 57117

    client = SuperColliderUPDClient(ip, port)

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
    run_command(client, "add sine_with_freq")
    run_command(client, "add oscillator")

    while True:

        time.sleep(0.1)
        print(">>> ", end="")
        run_command(client, input())
