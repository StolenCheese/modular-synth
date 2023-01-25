import collections
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
    groupHead = 0
    groupTail = 1
    beforeNode = 2
    afterNode = 3
    replaceNode = 4


class SuperColliderUPDClient(UDPClient):
    """Simple OSC client that automatically builds :class:`OscMessage` from arguments"""

    def __init__(self, address: str, port: int, allow_broadcast: bool = False, family: socket.AddressFamily = socket.AF_UNSPEC) -> None:
        super().__init__(address, port, allow_broadcast, family)

        self.incoming = queue.Queue()
        self._notify = False
        self.ids = set()

        threading.Thread(target=self.resv_loop).start()

    def resv_loop(self):
        while True:
            try:
                x = self._sock.recv(1024)

                packet = osc_packet.OscPacket(x)
                msg = packet.messages[0].message

                print(f"received {msg.address} {msg.params}")

                self.incoming.put(msg)

            except socket.error:
                time.sleep(0.1)
                continue

    def send_message(self, address: str, value: ArgValue) -> None:
        """Build :class:`OscMessage` from arguments and send to server

        Args:
            address: OSC address the message shall go to
            value: One or more arguments to be added to the message
        """
        builder = OscMessageBuilder(address=address)
        if value is None:
            values = []
        elif not isinstance(value, Iterable) or isinstance(value, (str, bytes)):
            values = [value]
        else:
            values = value
        for val in values:
            builder.add_arg(val)
        msg = builder.build()
        self.send(msg)

    def receive_message(self, fail_type: str = None):
        p = None
        while p == None or p.address == "/fail":
            while self.incoming.empty():
                time.sleep(0.1)
            p = self.incoming.get()
            if p.address == "/fail" and p.params[0] == fail_type:
                return p

        return p

    def status(self):
        self.send_message("/status", None)

        return self.receive_message()

    @property
    def notify(self) -> bool:
        return self._notify

    @notify.setter
    def notify(self, value: bool):
        if self._notify != value:
            self._notify = value
            self.send_message("/notify", 1 if value else 0)

            self.receive_message()

    def n_query(self, *id: int):
        self.send_message("/n_query", id)

        if self._notify:
            return self.receive_message("/n_query")

    def n_map(self, nodeID: int, *N: tuple[int | str, int]):
        """ Map a node's controls to read from a bus.
         Takes a list of pairs of control names or indices and bus indices and causes
         those controls to be read continuously from a global control bus.
         If the node is a group, then it maps the controls of every node in the group.
         If the control bus index is -1 then any current mapping is undone.
         Any `n_set`, `n_setn` and `n_fill` command will also unmap the control."""

        self.send_message("/n_map", [nodeID, *N])

    def n_set(self, nodeID: int, *N: tuple[int | str, float | int]):
        """Set a node's control value(s).

        ```
        int or string	a control index or name
        float or int	a control value
        ```

        Takes a list of pairs of control indices and values and sets the controls to those values. 
        If the node is a group, then it sets the controls of every node in the group.

        This message now supports array type tags ($[ and $]) in the control/value component of the OSC message. 
        Arrayed control values are applied in the manner of n_setn (i.e., sequentially starting at the indexed or named control).
        """

        self.send_message("/n_set", [nodeID, *[n for p in N for n in p]])

    def s_get(self, nodeID: int, *N: int | str):
        """
        Get control value(s).

        int	synth ID
        N * int or string	a control index or name

        Replies to sender with the corresponding /n_set command.
        """
        self.send_message("/s_get", [nodeID, *N])

        return self.receive_message(fail_type="/s_get")

    def s_new(self,  synth: str, id: int, add_action: AddAction, targetID: int, *control_values: tuple[int | str, float | int | str]):
        """
        Create a new synth.
        string	synth definition name
        int	synth ID
        int	add action (0,1,2, 3 or 4 see below)
        int	add target ID
        N *	
        int or string	a control index or name
        float or int or string	floating point and integer arguments are interpreted as control value. a symbol argument consisting of the letter 'c' or 'a' (for control or audio) followed by the bus's index.

        Create a new synth from a synth definition, give it an ID, and add it to the tree of nodes. There are four ways to add the node to the tree as determined by the add action argument which is defined as follows:

        Controls may be set when creating the synth. The control arguments are the same as for the n_set command.

        If you send `/s_new` with a synth ID of -1, then the server will generate an ID for you. 
        The server reserves all negative IDs. Since you don't know what the ID is, you cannot talk to this node directly later. 
        So this is useful for nodes that are of finite duration and that get the control information they need from arguments and buses or messages 
        directed to their group. In addition no notifications are sent when there are changes of state for this node, such as /go, /end, /on, /off.

        If you use a node ID of -1 for any other command, such as /n_map, then it refers to the most recently created node by 
        /s_new (auto generated ID or not). This is how you can map the controls of a node with an auto generated ID. 
        In a multi-client situation, the only way you can be sure what node -1 refers to is to put the messages in a bundle.

        This message now supports array type tags ($[ and $]) in the control/value component of the OSC message. 
        Arrayed control values are applied in the manner of `n_setn` (i.e., sequentially starting at the indexed or named control). 
        See the Node Messaging helpfile.
        """
        if id in self.ids:
            print("not adding duplicate ID")
            return
        self.ids.add(id)

        control = [n for p in control_values for n in p]

        self.send_message("/s_new", [synth,  id, add_action.value, targetID, *control])

    def n_free(self, *ids: int):

        if not self.ids.issuperset(ids):
            print("Warning; not all IDS registered")

        self.send_message("/n_free", ids)

    def n_run(self, id: int, on: int):

        self.send_message("/n_run", [id, on])

    def d_recv(self, buffer: bytes, onComplete: osc_message.OscMessage = None):
        """Receive a synth definition file.
        ```
        bytes	buffer of data.
        bytes	an OSC message to execute upon completion. (optional)
        ```

        Loads a file of synth definitions from a buffer in the message. Resident definitions with the same names are overwritten.

        Asynchronous.
            Replies to sender with /done when complete."""

        if onComplete == None:
            self.send_message("/d_recv", [buffer])
        else:
            self.send_message("/d_recv", [buffer, onComplete.dgram])

        return self.receive_message()

    def d_free(self, *name: str):
        """
        Delete synth definition.
        N * string	synth def name

        Removes a synth definition. The definition is removed immediately, and does not wait for synth nodes based on that definition to end.
        """
        self.send_message("/d_free", name)
