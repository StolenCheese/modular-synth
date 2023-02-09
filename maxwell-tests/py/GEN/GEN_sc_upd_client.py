# Importing BeautifulSoup class from the bs4 module
from dataclasses import dataclass, field
from bs4 import BeautifulSoup
import bs4

# Importing the HTTP library
import requests as req


@dataclass
class TypedParam:
    occurrence: str
    desc: str
    p_type: str
    name: str = None


@dataclass
class Command:
    group: str
    name: str
    await_command_success: str = None
    await_command_fail: str = None
    info: list[str] = field(default_factory=list)
    # occurrence, type, description
    type_info: list[TypedParam] = field(default_factory=list)
    sent_by_client: bool = True


def unpack_table(table: bs4.Tag, occurrence=[]):
    # recursively unpack type information from tables

    rows: list[bs4.tag] = table.contents[1].find_all(name="tr", recursive=False)

    table_contents: list[TypedParam] = []

    for r in rows:
        data = r.contents
        rhs = data[-1]

        lhs = "".join([d.text for d in data[:-1]]).split("*")

        if t := rhs.find(name="table"):
            # a deeper table on the RHS
            table_contents.extend(unpack_table(t, occurrence=occurrence + [lhs[0]]))
        else:
            if len(lhs) > 1 and len(lhs[1]) > 0:
                # we have something like N * int
                val_type = lhs[1]
                occurrence = occurrence + [lhs[0]]
            else:
                val_type = lhs[0]

            # format for python
            val_type = val_type.replace("or", "|").replace("string", "str").strip()

            table_contents.append(TypedParam(tuple(occurrence),  rhs.contents[0], val_type))

    return table_contents


def extract_address(text: str):
    i = text.find("/")
    if i == -1:
        return
    js = [x for x in [text.find(x, i) for x in [" ", "\n", ","]] if x != -1]
    if len(js) > 0:
        j = min(js)
    else:
        j = -1
        
    return text[i:j]


def decode_commands(tags: list[bs4.Tag]) -> list[Command]:
    current_group: str = None
    current_command: Command = None
    commands: list[Command] = []

    for t in tags:
        match t.name:
            case "h2":
                current_group = t.text

            case "h3":
                if current_command:
                    commands.append(current_command)

                print(current_group + t.text)

                if t.text.startswith("/"):
                    current_command = Command(current_group, t.text)
                    if current_group == "Replies to Commands":
                        current_command.sent_by_client = False
                else:
                    current_command = None

            case "p" | "dl" if current_command:
                current_command.info.append(t.text)
                a = extract_address(t.text)
                if a:
                    current_command.await_command_success = a

            case "table" if current_command:

                if not current_command.type_info:
                    current_command.type_info = unpack_table(t)
                    print("TYPE DATA----")
                    print("\n".join([f"{t.occurrence}: {t.p_type} - {t.desc}" for t in current_command.type_info]))
                else:
                    # this is documentation
                    current_command.info.append(ident.join([f"{t.occurrence}: {t.p_type} - {t.desc}" for t in unpack_table(t)]))

    return commands


def make_name(p: str, banned: list[str]):
    p = p.replace(' ', '_')
    p = p.replace(',', '')
    p = p.replace("'", '')
    p = p.replace('"', '')
    p = p[:p.find('(')] if p.find('(') != -1 else p
    p = p[:p.find('.')] if p.find('.') != -1 else p

    # just pick the biggest word
    return max([x for x in p.split('_') if x not in banned], key=len)


def listify(types: list[str | list]):
    """turn something like 
    - `["int", "int", ["int"]]` into `list[tuple[int,int,list[int]]]`
    - `"int"` into `"int"`
    - `["int"]` into `list[int]`

    For python typing
    """
    if isinstance(types, str):
        return types
    elif len(types) > 1:
        return f"list[tuple[{', '.join([listify(t) for t in  types])}]]"
    else:
        return f"list[{types[0]}]"


def function_from_command(c: Command) -> str:
    # step 1. Name params
    all_names = []
    for t in c.type_info:
        t.name = make_name(t.desc, all_names)
        all_names.append(t.name)
    del all_names

    # Generate the function command
    doc = ident.join(c.info)
    if c.type_info:
        doc += ident
        doc += ident.join([f":param:{t.name}: - {t.desc}" for t in c.type_info])

    async_await = ""
    if c.await_command_success:
        async_await = f"""
        return self.receive_message(desired="{c.await_command_success}")
    """

    # First pass on types - group and label

    params = []  # (Occurrences, Param, Types)
    for t in c.type_info:
        n = "".join(t.occurrence)
        if n == '':
            params.append(("S", t.name, t.p_type))
        elif n == "N ":
            if len(params) > 0 and params[-1][0] == "N":
                # append
                params[-1][2].append(t.p_type)
            else:
                params.append(("N", t.name, [t.p_type]))
        elif n == "N M ":
            # N must have occurred before, at the min to define an int M
            # I would say this is probably one of the worst pieces of code I've ever written
            # If I didn't have to actively not do it in one line
            if isinstance(params[-1][2][-1], list):
                params[-1][2][-1].append(t.p_type)
            else:
                params[-1][2].append([t.p_type])
        else:
            print("Unknown occurrence format: '"+n+"'")

    # Second pass on types: format for python
    typed_params = []

    for i, p in enumerate(params):
        typed_params.append((p[1], listify(p[2])))

    return f'''    def {c.name[1:]}({", ".join(["self"] + [f"{p}: {t}" for p, t in typed_params])}):
        """
        {doc}
        """
        self.send_message("{c.name}", [{", ".join([p for o, p,t in params])}])
        {async_await}'''


with open("GEN/supercollider_udp_client.py", "w") as f:

    f.write('''import collections
import queue
import threading
import time
from typing import Iterable
from pythonosc.udp_client import UDPClient
from pythonosc.osc_message_builder import OscMessageBuilder, ArgValue
from pythonosc.osc_message import OscMessage
from pythonosc.osc_bundle import OscBundle

from enum import IntEnum

from pythonosc import udp_client
from pythonosc import osc_packet
from pythonosc import osc_message
import socket


class AddAction(IntEnum):
    """
    ```
    add actions:
        0	add the new node to the head of the group specified by the add target ID.
        1	add the new node to the tail of the group specified by the add target ID.
        2	add the new node just before the node specified by the add target ID.
        3	add the new node just after the node specified by the add target ID.
        4	the new node replaces the node specified by the add target ID. The target node is freed.
    ```
    """
    groupHead=0
    groupTail=1
    beforeNode=2
    afterNode=3
    replaceNode=4


class SuperColliderUPDClient(UDPClient):
    """Simple OSC client that automatically builds :class:`OscMessage` from arguments"""

    def __init__(self, address: str, port: int, allow_broadcast: bool=False, family: socket.AddressFamily=socket.AF_UNSPEC) -> None:
        super().__init__(address, port, allow_broadcast, family)

        self.incoming: queue.Queue[osc_message.OscMessage]=queue.Queue()
        self._notify=False

        threading.Thread(target=self.resv_loop).start()

    def resv_loop(self):
        while True:
            try:
                x=self._sock.recv(1024)

                packet=osc_packet.OscPacket(x)
                msg=packet.messages[0].message

                #
                # print(f"received \{msg.address\}")

                self.incoming.put(msg)

            except socket.error:
                time.sleep(0.1)
                continue

    def send_message(self, address: str, value: list) -> None:
        """Build :class:`OscMessage` from arguments and send to server

        Args:
            address: OSC address the message shall go to
            value: One or more arguments to be added to the message
        """
        builder = OscMessageBuilder(address=address)

        def fill_params(vs):
            if isinstance(value, Iterable) and not isinstance(value, (str, bytes)):
                for v in vs:
                    fill_params(v)
            elif vs:
                builder.add_arg(vs)

        fill_params(value)

        msg = builder.build()
        self.send(msg)

    def receive_message(self, desired: str=None, fail_type: str=None):
        p=None
        while p == None or p.address == "/fail" or (desired and p.address != desired):
            while self.incoming.empty():
                time.sleep(0.1)
            p=self.incoming.get()

            if p.address == "/fail" and p.params[0] == fail_type:
                return p

        return p''')
    f.write("\n")

    # Requesting for the website
    Web = req.get('https://doc.sccode.org/Reference/Server-Command-Reference.html')

    ident = '\n        '
    # Creating a BeautifulSoup object and specifying the parser
    scr = BeautifulSoup(Web.text, 'html5lib')

    contents = scr.findChild(attrs={"class": "contents"})

    tags: list[bs4.Tag] = contents.find_all(recursive=False)

    commands = decode_commands(tags)

    for c in commands:
        if c.sent_by_client:
            f.write(function_from_command(c))
            f.write("\n")
