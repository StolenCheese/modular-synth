"""Small example OSC client

This program sends 10 random values between 0.0 and 1.0 to the /filter address,
waiting for 1 seconds between each value.
"""

import random
import time

from pythonosc import udp_client
from pythonosc import osc_packet
from pythonosc import osc_message
import socket


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


if __name__ == "__main__":
    # https://doc.sccode.org/Reference/Server-Architecture.html
    # https://doc.sccode.org/Reference/Server-Command-Reference.html

    # run ./scsynth.exe -u 57117 in your scsynth install directory
    # C:\Program Files\SuperCollider-3.13.0-rc1 for me

    ip = "127.0.0.1"
    port = 57117

    client = udp_client.SimpleUDPClient(ip, port)

    client.send_message("/s_new", ["sine",  1000, 1, 0])
    time.sleep(1)

    client.send_message("/status", [])
    try:
        while True:
            try:
                x = client._sock.recv(1024)
                break
            except socket.error:
                continue

        packet = osc_packet.OscPacket(x)
        print([decode_response(x.message) for x in packet.messages])
        time.sleep(1)
    finally:
        client.send_message("/n_free", [1000])
