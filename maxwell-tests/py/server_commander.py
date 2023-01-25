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


def nr(arg: str):
    if arg.isdigit():
        return int(arg)
    return arg


def parametrise(t):
    return ((param, note(value)) for (param, value) in zip(t[::2], t[1::2]))


if __name__ == "__main__":
    # https://doc.sccode.org/Reference/Server-Architecture.html
    # https://doc.sccode.org/Reference/Server-Command-Reference.html

    # run ./scsynth.exe -u 57117 in your scsynth install directory
    # C:\Program Files\SuperCollider-3.13.0-rc1 for me

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

    while True:
        time.sleep(0.1)
        print(">>> ", end="")
        c = [nr(a) for a in input().split()]

        match c:
            case ["add", f_name]:
                with open(f"synthdefs/{f_name}.scsyndef", "rb") as f:  # opening for [r]eading as [b]inary
                    client.d_recv(f.read())

            case ["del", f_name]:
                client.d_free(f_name)

            case ["play", synth, int(id), *t]:
                client.s_new(synth,  id, AddAction.groupTail, 0,  *parametrise(t))
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
                client.n_set(id, *parametrise(t))

            case ["get", int(id), *params]:
                client.s_get(id, params)

            case ["q", int(id)]:
                client.n_query(id)
            case c:
                print(f"unknown command {c}")
