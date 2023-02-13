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

        return p
    def quit(self):
        """
        Quit program. Exits the synthesis server.
        
Asynchronous.Replies to sender with /done just before completion.
        """
        self.send_message("/quit", [])
        
        return self.receive_message(desired="/done")
    
    def notify(self, notifications: int, client: int):
        """
        Register to receive notifications from server
        If the first argument is 1, server will remember your return address and send you notifications; if 0, server will stop sending notifications.
        
Asynchronous.Replies to sender with /done /notify clientID [maxLogins] when complete. If this client has registered for notifications before, this may be the same ID. Otherwise it will be a new one. Clients can use this ID in multi-client situations to avoid conflicts when allocating resources such as node IDs, bus indices, and buffer numbers. maxLogins is only returned when the client ID argument is supplied in this command. maxLogins is not supported by supernova.
        :param:notifications: - 1 to receive notifications, 0 to stop receiving them.
        :param:client: - client ID (optional)
        """
        self.send_message("/notify", [notifications, client])
        
        return self.receive_message(desired="/done")
    
    def status(self):
        """
        Query the status. Replies to sender with the following message:
        
/status.reply
int1. unused.intnumber of unit generators.intnumber of synths.intnumber of groups.intnumber of loaded synth definitions.floataverage percent CPU usage for signal processingfloatpeak percent CPU usage for signal processingdoublenominal sample ratedoubleactual sample rate
NOTE: /status messages won't be posted, if the server is in /dumpOSC mode
        """
        self.send_message("/status", [])
        
        return self.receive_message(desired="/status.reply")
    
    def cmd(self, command: str, arguments: ...):
        """
        Plug-in defined command.
        Commands are defined by plug-ins.
        :param:command: - command name
        :param:arguments: - any arguments
        """
        self.send_message("/cmd", [command, arguments])
        
    def dumpOSC(self, code: int):
        """
        Display incoming OSC messages.
        Turns on and off printing of the contents of incoming Open Sound Control messages. This is useful when debugging your command stream.

        The values for the code are as follows:
        (): 0 - turn dumping OFF.
        (): 1 - print the parsed contents of the message.
        (): 2 - print the contents in hexadecimal.
        (): 3 - print both the parsed and hexadecimal representations of the contents.
        :param:code: - code
        """
        self.send_message("/dumpOSC", [code])
        
    def sync(self, identifying: int):
        """
        Notify when async commands have completed.
        Replies with a /synced message when all asynchronous commands received before this one have completed. The reply will contain the sent unique ID.
        
Asynchronous.Replies to sender with /synced, ID when complete.
        :param:identifying: - a unique number identifying this command.
        """
        self.send_message("/sync", [identifying])
        
        return self.receive_message(desired="/synced")
    
    def clearSched(self):
        """
        Clear all scheduled bundles. Removes all bundles from the scheduling queue.
        """
        self.send_message("/clearSched", [])
        
    def error(self, mode: int):
        """
        Enable/disable error message posting.
        Turn on or off error messages sent to the SuperCollider post window. Useful when sending a message, such as /n_free, whose failure does not necessarily indicate anything wrong.

        The values for mode are as follows:
        (): 0 - turn off error posting until the next ['/error', 1] message.
        (): 1 - turn on error posting.
        For convenience of client-side methods, you can also suppress errors temporarily, for the scope of a single bundle.
        (): -1 - turn off locally in the bundle -- error posting reverts to the "permanent" setting for the next message or bundle.
        (): -2 - turn on locally in the bundle.
        These "temporary" states accumulate within a single bundle -- so if you have nested calls to methods that use bundle-local error suppression, error posting remains off until all the layers have been unwrapped. If you use ['/error', -1] within a self-bundling method, you should always close it with ['/error', -2] so that subsequent bundled messages will take the correct error posting status. However, even if this is not done, the next bundle or message received will begin with the standard error posting status, as set by modes 0 or 1.

        Temporary error suppression may not affect asynchronous commands in every case.
        :param:mode: - mode
        """
        self.send_message("/error", [mode])
        
        return self.receive_message(desired="/error'")
    
    def version(self):
        """
        Query the SuperCollider version. Replies to sender with the following message:
        
/version.reply
stringProgram name. May be "scsynth" or "supernova".intMajor version number. Equivalent to sclang's Main.scVersionMajor.intMinor version number. Equivalent to sclang's Main.scVersionMinor.stringPatch version name. Equivalent to the sclang code "." ++ Main.scVersionPatch ++ Main.scVersionTweak.stringGit branch name.stringFirst seven hex digits of the commit hash.

        The standard human-readable version string can be constructed by concatenating major_version ++ "." ++ minor_version ++ patch_version. Since version information is easily accessible to sclang users via the methods described above, this command is mostly useful for alternate clients.

        The git branch name and commit hash could be anything if the user has forked SC, so they should only be used for display and user interface purposes.
        """
        self.send_message("/version", [])
        
        return self.receive_message(desired="/version.reply")
    
    def d_recv(self, buffer: bytes, completion: bytes):
        """
        Receive a synth definition file.
        Loads a file of synth definitions from a buffer in the message. Resident definitions with the same names are overwritten.
        
Asynchronous.Replies to sender with /done when complete.
        :param:buffer: - buffer of data.
        :param:completion: - an OSC message to execute upon completion. (optional)
        """
        self.send_message("/d_recv", [buffer, completion])
        
        return self.receive_message(desired="/done")
    
    def d_load(self, pathname: str, completion: bytes):
        """
        Load synth definition.
        Loads a file of synth definitions. Resident definitions with the same names are overwritten.
        
Asynchronous.Replies to sender with /done when complete.
        :param:pathname: - pathname of file. Can be a pattern like 
        :param:completion: - an OSC message to execute upon completion. (optional)
        """
        self.send_message("/d_load", [pathname, completion])
        
        return self.receive_message(desired="/done")
    
    def d_loadDir(self, directory: str, completion: bytes):
        """
        Load a directory of synth definitions.
        Loads a directory of synth definitions files. Resident definitions with the same names are overwritten.
        
Asynchronous.Replies to sender with /done when complete.
        :param:directory: - pathname of directory.
        :param:completion: - an OSC message to execute upon completion. (optional)
        """
        self.send_message("/d_loadDir", [directory, completion])
        
        return self.receive_message(desired="/done")
    
    def d_free(self, synth: list[str]):
        """
        Delete synth definition.
        Removes a synth definition. The definition is removed immediately, and does not wait for synth nodes based on that definition to end.
        :param:synth: - synth def name
        """
        self.send_message("/d_free", [synth])
        
    def n_free(self, node: list[int]):
        """
        Delete a node.
        Stops a node abruptly, removes it from its group, and frees its memory. A list of node IDs may be specified. Using this method can cause a click if the node is not silent at the time it is freed.
        :param:node: - node ID
        """
        self.send_message("/n_free", [node])
        
    def n_run(self, node: list[tuple[int, int]]):
        """
        Turn node on or off.
        
        Using this method to start and stop nodes can cause a click if the node is not silent at the time run flag is toggled.
        :param:node: - node ID
        :param:flag: - run flag
        """
        self.send_message("/n_run", [node])
        
    def n_set(self, node: int, control: list[tuple[int | str, float | int]]):
        """
        Set a node's control value(s).
        Takes a list of pairs of control indices and values and sets the controls to those values. If the node is a group, then it sets the controls of every node in the group.

        This message now supports array type tags ($[ and $]) in the control/value component of the OSC message. Arrayed control values are applied in the manner of n_setn (i.e., sequentially starting at the indexed or named control).
        :param:node: - node ID
        :param:control: - a control index or name
        :param:value: - a control value
        """
        self.send_message("/n_set", [node, control])
        
        return self.receive_message(desired="/value")
    
    def n_setn(self, node: int, control: list[tuple[int | str, int, list[float | int]]]):
        """
        Set ranges of a node's control value(s).
        Set contiguous ranges of control indices to sets of values. For each range, the starting control index is given followed by the number of controls to change, followed by the values. If the node is a group, then it sets the controls of every node in the group.
        :param:node: - node ID
        :param:control: - a control index or name
        :param:sequential: - number of sequential controls to change (M)
        :param:value: - control value(s)
        """
        self.send_message("/n_setn", [node, control])
        
    def n_fill(self, node: int, control: list[tuple[int | str, int, float | int]]):
        """
        Fill ranges of a node's control value(s).
        Set contiguous ranges of control indices to single values. For each range, the starting control index is given followed by the number of controls to change, followed by the value to fill. If the node is a group, then it sets the controls of every node in the group.
        :param:node: - node ID
        :param:control: - a control index or name
        :param:number: - number of values to fill (M)
        :param:value: - value
        """
        self.send_message("/n_fill", [node, control])
        
    def n_map(self, node: int, control: list[tuple[int | str, int]]):
        """
        Map a node's controls to read from a bus.
        Takes a list of pairs of control names or indices and bus indices and causes those controls to be read continuously from a global control bus. If the node is a group, then it maps the controls of every node in the group. If the control bus index is -1 then any current mapping is undone. Any n_set, n_setn and n_fill command will also unmap the control.
        :param:node: - node ID
        :param:control: - a control index or name
        :param:index: - control bus index
        """
        self.send_message("/n_map", [node, control])
        
    def n_mapn(self, node: int, control: list[tuple[int | str, int, int]]):
        """
        Map a node's controls to read from buses.
        Takes a list of triplets of control names or indices, bus indices, and number of controls to map and causes those controls to be mapped sequentially to buses. If the node is a group, then it maps the controls of every node in the group. If the control bus index is -1 then any current mapping is undone. Any n_set, n_setn and n_fill command will also unmap the control.
        :param:node: - node ID
        :param:control: - a control index or name
        :param:index: - control bus index
        :param:controls: - number of controls to map
        """
        self.send_message("/n_mapn", [node, control])
        
    def n_mapa(self, node: int, control: list[tuple[int | str, int]]):
        """
        Map a node's controls to read from an audio bus.
        Takes a list of pairs of control names or indices and audio bus indices and causes those controls to be read continuously from a global audio bus. If the node is a group, then it maps the controls of every node in the group. If the audio bus index is -1 then any current mapping is undone. Any n_set, n_setn and n_fill command will also unmap the control. For the full audio rate signal, the argument must have its rate set to \ar.
        :param:node: - node ID
        :param:control: - a control index or name
        :param:index: - control bus index
        """
        self.send_message("/n_mapa", [node, control])
        
    def n_mapan(self, node: int, control: list[tuple[int | str, int, int]]):
        """
        Map a node's controls to read from audio buses.
        Takes a list of triplets of control names or indices, audio bus indices, and number of controls to map and causes those controls to be mapped sequentially to buses. If the node is a group, then it maps the controls of every node in the group. If the audio bus index is -1 then any current mapping is undone. Any n_set, n_setn and n_fill command will also unmap the control. For the full audio rate signal, the argument must have its rate set to \ar.
        :param:node: - node ID
        :param:control: - a control index or name
        :param:index: - control bus index
        :param:controls: - number of controls to map
        """
        self.send_message("/n_mapan", [node, control])
        
    def n_before(self, place: list[tuple[int, int]]):
        """
        Place a node before another.
        Places node A in the same group as node B, to execute immediately before node B.
        :param:place: - the ID of the node to place (A)
        :param:before: - the ID of the node before which the above is placed (B)
        """
        self.send_message("/n_before", [place])
        
    def n_after(self, place: list[tuple[int, int]]):
        """
        Place a node after another.
        Places node A in the same group as node B, to execute immediately after node B.
        :param:place: - the ID of the node to place (A)
        :param:placed: - the ID of the node after which the above is placed (B)
        """
        self.send_message("/n_after", [place])
        
    def n_query(self, node: list[int]):
        """
        Get info about a node.
        The server sends an /n_info message for each node to registered clients. See Node Notifications below for the format of the /n_info message.
        :param:node: - node ID
        """
        self.send_message("/n_query", [node])
        
        return self.receive_message(desired="/n_info")
    
    def n_trace(self, node: list[int]):
        """
        Trace a node.
        Causes a synth to print out the values of the inputs and outputs of its unit generators for one control period. Causes a group to print the node IDs and names of each node in the group for one control period.
        :param:node: - node IDs
        """
        self.send_message("/n_trace", [node])
        
    def n_order(self, action: int, target: int, node: list[int]):
        """
        Move and order a list of nodes.
        Move the listed nodes to the location specified by the target and add action, and place them in the order specified. Nodes which have already been freed will be ignored.
        
add actions:
0construct the node order at the head of the group specified by the add target ID.1construct the node order at the tail of the group specified by the add target ID.2construct the node order just before the node specified by the add target ID.3construct the node order just after the node specified by the add target ID.

        :param:action: - add action (0,1,2 or 3 see below)
        :param:target: - add target ID
        :param:node: - node IDs
        """
        self.send_message("/n_order", [action, target, node])
        
    def s_new(self, definition: str, synth: int, action: int, target: int, control: list[tuple[int | str, float | int | str]]):
        """
        Create a new synth.
        Create a new synth from a synth definition, give it an ID, and add it to the tree of nodes. There are four ways to add the node to the tree as determined by the add action argument which is defined as follows:
        
add actions:
0add the new node to the head of the group specified by the add target ID.1add the new node to the tail of the group specified by the add target ID.2add the new node just before the node specified by the add target ID.3add the new node just after the node specified by the add target ID.4the new node replaces the node specified by the add target ID. The target node is freed.

        Controls may be set when creating the synth. The control arguments are the same as for the n_set command.

        If you send /s_new with a synth ID of -1, then the server will generate an ID for you. The server reserves all negative IDs. Since you don't know what the ID is, you cannot talk to this node directly later. So this is useful for nodes that are of finite duration and that get the control information they need from arguments and buses or messages directed to their group. In addition no notifications are sent when there are changes of state for this node, such as /go, /end, /on, /off.

        If you use a node ID of -1 for any other command, such as /n_map, then it refers to the most recently created node by /s_new (auto generated ID or not). This is how you can map the controls of a node with an auto generated ID. In a multi-client situation, the only way you can be sure what node -1 refers to is to put the messages in a bundle.

        This message now supports array type tags ($[ and $]) in the control/value component of the OSC message. Arrayed control values are applied in the manner of n_setn (i.e., sequentially starting at the indexed or named control). See the Node Messaging helpfile.
        :param:definition: - synth definition name
        :param:synth: - synth ID
        :param:action: - add action (0,1,2, 3 or 4 see below)
        :param:target: - add target ID
        :param:control: - a control index or name
        :param:interpreted: - floating point and integer arguments are interpreted as control value.  a symbol argument consisting of the letter 'c' or 'a' (for control or audio) followed by the bus's index.
        """
        self.send_message("/s_new", [definition, synth, action, target, control])
        
        return self.receive_message(desired="/value")
    
    def s_get(self, synth: int, control: list[int | str]):
        """
        Get control value(s).
        Replies to sender with the corresponding /n_set command.
        :param:synth: - synth ID
        :param:control: - a control index or name
        """
        self.send_message("/s_get", [synth, control])
        
        return self.receive_message(desired="/n_set")
    
    def s_getn(self, synth: int, control: list[tuple[int | str, int]]):
        """
        Get ranges of control value(s).
        Get contiguous ranges of controls. Replies to sender with the corresponding /n_setn command.
        :param:synth: - synth ID
        :param:control: - a control index or name
        :param:sequential: - number of sequential controls to get (M)
        """
        self.send_message("/s_getn", [synth, control])
        
        return self.receive_message(desired="/n_setn")
    
    def s_noid(self, synth: list[int]):
        """
        Auto-reassign synth's ID to a reserved value.
        This command is used when the client no longer needs to communicate with the synth and wants to have the freedom to reuse the ID. The server will reassign this synth to a reserved negative number. This command is purely for bookkeeping convenience of the client. No notification is sent when this occurs.
        :param:synth: - synth IDs
        """
        self.send_message("/s_noid", [synth])
        
    def g_new(self, group: list[tuple[int, int, int]]):
        """
        Create a new group.
        Create a new group and add it to the tree of nodes. There are four ways to add the group to the tree as determined by the add action argument which is defined as follows (the same as for /s_new):
        
add actions:
0add the new group to the head of the group specified by the add target ID.1add the new group to the tail of the group specified by the add target ID.2add the new group just before the node specified by the add target ID.3add the new group just after the node specified by the add target ID.4the new node replaces the node specified by the add target ID. The target node is freed.

        Multiple groups may be created in one command by adding arguments.
        :param:group: - new group ID
        :param:action: - add action (0,1,2, 3 or 4 see below)
        :param:target: - add target ID
        """
        self.send_message("/g_new", [group])
        
        return self.receive_message(desired="/s_new)")
    
    def p_new(self, group: list[tuple[int, int, int]]):
        """
        Create a new parallel group.
        Create a new parallel group and add it to the tree of nodes. Parallel groups are relaxed groups, their child nodes are evaluated in unspecified order. There are four ways to add the group to the tree as determined by the add action argument which is defined as follows (the same as for /s_new):
        
add actions:
0add the new group to the head of the group specified by the add target ID.1add the new group to the tail of the group specified by the add target ID.2add the new group just before the node specified by the add target ID.3add the new group just after the node specified by the add target ID.4the new node replaces the node specified by the add target ID. The target node is freed.

        Multiple groups may be created in one command by adding arguments.
        :param:group: - new group ID
        :param:action: - add action (0,1,2, 3 or 4 see below)
        :param:target: - add target ID
        """
        self.send_message("/p_new", [group])
        
        return self.receive_message(desired="/s_new)")
    
    def g_head(self, group: list[tuple[int, int]]):
        """
        Add node to head of group.
        Adds the node to the head (first to be executed) of the group.
        :param:group: - group ID
        :param:node: - node ID
        """
        self.send_message("/g_head", [group])
        
    def g_tail(self, group: list[tuple[int, int]]):
        """
        Add node to tail of group.
        Adds the node to the tail (last to be executed) of the group.
        :param:group: - group ID
        :param:node: - node ID
        """
        self.send_message("/g_tail", [group])
        
    def g_freeAll(self, group: list[int]):
        """
        Delete all nodes in a group.
        Frees all nodes in the group. A list of groups may be specified.
        :param:group: - group ID(s)
        """
        self.send_message("/g_freeAll", [group])
        
    def g_deepFree(self, group: list[int]):
        """
        Free all synths in this group and all its sub-groups.
        Traverses all groups below this group and frees all the synths. Sub-groups are not freed. A list of groups may be specified.
        :param:group: - group ID(s)
        """
        self.send_message("/g_deepFree", [group])
        
    def g_dumpTree(self, group: list[tuple[int, int]]):
        """
        Post a representation of this group's node subtree.
        Posts a representation of this group's node subtree, i.e. all the groups and synths contained within it, optionally including the current control values for synths.
        :param:group: - group ID
        :param:current: - flag; if not 0 the current control (arg) values for synths will be posted
        """
        self.send_message("/g_dumpTree", [group])
        
    def g_queryTree(self, group: list[tuple[int, int]]):
        """
        Get a representation of this group's node subtree.
        Request a representation of this group's node subtree, i.e. all the groups and synths contained within it. Replies to the sender with a /g_queryTree.reply message listing all of the nodes contained within the group in the following format:
        (): int - flag: if synth control values are included 1, else 0
        (): int - node ID of the requested group
        (): int - number of child nodes contained within the requested group
        ('then for each node in the subtree:',): int - node ID
        ('then for each node in the subtree:',): int - number of child nodes contained within this node. If -1 this is a synth, if >=0 it's a group
        ('then for each node in the subtree:',):  - then, if this node is a synth:
        ('then for each node in the subtree:',): symbol - the SynthDef name for this node.
        ('then for each node in the subtree:',):  - then, if flag (see above) is true:
        ('then for each node in the subtree:',): int - numControls for this synth (M)
        ('then for each node in the subtree:', 'M '): symbol | int - control name or index
        ('then for each node in the subtree:', 'M '): float | symbol - value or control bus mapping symbol (e.g. 'c1')
        N.B. The order of nodes corresponds to their execution order on the server. Thus child nodes (those contained within a group) are listed immediately following their parent. See the method Server:queryAllNodes for an example of how to process this reply.
        :param:group: - group ID
        :param:current: - flag: if not 0 the current control (arg) values for synths will be included
        """
        self.send_message("/g_queryTree", [group])
        
        return self.receive_message(desired="/g_queryTree.reply")
    
    def u_cmd(self, node: int, generator: int, command: str, arguments: ...):
        """
        Send a command to a unit generator.
        Sends all arguments following the command name to the unit generator to be performed. Commands are defined by unit generator plug ins.
        Buffers are stored in a global array, indexed by integers starting at zero.
        :param:node: - node ID
        :param:generator: - unit generator index
        :param:command: - command name
        :param:arguments: - any arguments
        """
        self.send_message("/u_cmd", [node, generator, command, arguments])
        
    def b_alloc(self, buffer: int, number: int, channels: int, completion: bytes):
        """
        Allocate buffer space.
        Allocates zero filled buffer to number of channels and samples.
        
Asynchronous.Replies to sender with /done /b_alloc bufNum when complete.
        :param:buffer: - buffer number
        :param:number: - number of frames
        :param:channels: - number of channels (optional. default = 1 channel)
        :param:completion: - an OSC message to execute upon completion. (optional)
        """
        self.send_message("/b_alloc", [buffer, number, channels, completion])
        
        return self.receive_message(desired="/done")
    
    def b_allocRead(self, buffer: int, sound: str, starting: int, number: int, completion: bytes):
        """
        Allocate buffer space and read a sound file.
        Allocates buffer to number of channels of file and number of samples requested, or fewer if sound file is smaller than requested. Reads sound file data from the given starting frame in the file. If the number of frames argument is less than or equal to zero, the entire file is read.
        
Asynchronous.Replies to sender with /done /b_allocRead bufNum when complete.
        :param:buffer: - buffer number
        :param:sound: - path name of a sound file.
        :param:starting: - starting frame in file (optional. default = 0)
        :param:number: - number of frames to read (optional. default = 0, see below)
        :param:completion: - an OSC message to execute upon completion. (optional)
        """
        self.send_message("/b_allocRead", [buffer, sound, starting, number, completion])
        
        return self.receive_message(desired="/done")
    
    def b_allocReadChannel(self, buffer: int, sound: str, starting: int, number: int, channel: list[int], completion: bytes):
        """
        Allocate buffer space and read channels from a sound file.
        As b_allocRead, but reads individual channels into the allocated buffer in the order specified. If the channels argument is absent or empty all channels are read in the order they appear in the file.
        
Asynchronous.Replies to sender with /done /b_allocReadChannel bufNum when complete.
        :param:buffer: - buffer number
        :param:sound: - path name of a sound file
        :param:starting: - starting frame in file
        :param:number: - number of frames to read
        :param:channel: - source file channel index
        :param:completion: - an OSC message to execute upon completion. (optional)
        """
        self.send_message("/b_allocReadChannel", [buffer, sound, starting, number, channel, completion])
        
        return self.receive_message(desired="/done")
    
    def b_read(self, buffer: int, sound: str, starting: int, number: int, frame: int, leave: int, completion: bytes):
        """
        Read sound file data into an existing buffer.
        Reads sound file data from the given starting frame in the file and writes it to the given starting frame in the buffer. If number of frames is less than zero, the entire file is read. If reading a file to be used by DiskIn ugen then you will want to set "leave file open" to one, otherwise set it to zero.
        
Asynchronous.Replies to sender with /done /b_read bufNum when complete.
        :param:buffer: - buffer number
        :param:sound: - path name of a sound file.
        :param:starting: - starting frame in file (optional. default = 0)
        :param:number: - number of frames to read (optional. default = -1, see below)
        :param:frame: - starting frame in buffer (optional. default = 0)
        :param:leave: - leave file open (optional. default = 0)
        :param:completion: - an OSC message to execute upon completion. (optional)
        """
        self.send_message("/b_read", [buffer, sound, starting, number, frame, leave, completion])
        
        return self.receive_message(desired="/done")
    
    def b_readChannel(self, buffer: int, sound: str, starting: int, number: int, frame: int, leave: int, channel: list[int], completion: bytes):
        """
        Read sound file channel data into an existing buffer.
        As b_read, but reads individual channels in the order specified. The number of channels requested must match the number of channels in the buffer.
        
Asynchronous.Replies to sender with /done /b_readChannel bufNum when complete.
        :param:buffer: - buffer number
        :param:sound: - path name of a sound file
        :param:starting: - starting frame in file
        :param:number: - number of frames to read
        :param:frame: - starting frame in buffer
        :param:leave: - leave file open
        :param:channel: - source file channel index
        :param:completion: - completion message
        """
        self.send_message("/b_readChannel", [buffer, sound, starting, number, frame, leave, channel, completion])
        
        return self.receive_message(desired="/done")
    
    def b_write(self, buffer: int, sound: str, header: str, sample: str, number: int, starting: int, leave: int, completion: bytes):
        """
        Write sound file data.
        Write a buffer as a sound file.
        
Header format is one of:"aiff", "next", "wav", "ircam"", "raw"Sample format is one of:"int8", "int16", "int24", "int32", "float", "double", "mulaw", "alaw"
        Not all combinations of header format and sample format are possible. If number of frames is less than zero, all samples from the starting frame to the end of the buffer are written. If opening a file to be used by DiskOut ugen then you will want to set "leave file open" to one, otherwise set it to zero. If "leave file open" is set to one then the file is created, but no frames are written until the DiskOut ugen does so.
        
Asynchronous.Replies to sender with /done /b_write bufNum when complete.
        :param:buffer: - buffer number
        :param:sound: - path name of a sound file.
        :param:header: - header format.
        :param:sample: - sample format.
        :param:number: - number of frames to write (optional. default = -1, see below)
        :param:starting: - starting frame in buffer (optional. default = 0)
        :param:leave: - leave file open (optional. default = 0)
        :param:completion: - an OSC message to execute upon completion. (optional)
        """
        self.send_message("/b_write", [buffer, sound, header, sample, number, starting, leave, completion])
        
        return self.receive_message(desired="/done")
    
    def b_free(self, buffer: int, completion: bytes):
        """
        Free buffer data.
        Frees buffer space allocated for this buffer.
        
Asynchronous.Replies to sender with /done /b_free bufNum when complete.
        :param:buffer: - buffer number
        :param:completion: - an OSC message to execute upon completion. (optional)
        """
        self.send_message("/b_free", [buffer, completion])
        
        return self.receive_message(desired="/done")
    
    def b_zero(self, buffer: int, completion: bytes):
        """
        Zero sample data.
        Sets all samples in the buffer to zero.
        
Asynchronous.Replies to sender with /done /b_zero bufNum when complete.
        :param:buffer: - buffer number
        :param:completion: - an OSC message to execute upon completion. (optional)
        """
        self.send_message("/b_zero", [buffer, completion])
        
        return self.receive_message(desired="/done")
    
    def b_set(self, buffer: int, sample: list[tuple[int, float]]):
        """
        Set sample value(s).
        Takes a list of pairs of sample indices and values and sets the samples to those values.
        :param:buffer: - buffer number
        :param:sample: - a sample index
        :param:value: - a sample value
        """
        self.send_message("/b_set", [buffer, sample])
        
    def b_setn(self, buffer: int, starting: list[tuple[int, int, list[float]]]):
        """
        Set ranges of sample value(s).
        Set contiguous ranges of sample indices to sets of values. For each range, the starting sample index is given followed by the number of samples to change, followed by the values.
        :param:buffer: - buffer number
        :param:starting: - sample starting index
        :param:sequential: - number of sequential samples to change (M)
        :param:sample: - a sample value
        """
        self.send_message("/b_setn", [buffer, starting])
        
    def b_fill(self, buffer: int, starting: list[tuple[int, int, float]]):
        """
        Fill ranges of sample value(s).
        Set contiguous ranges of sample indices to single values. For each range, the starting sample index is given followed by the number of samples to change, followed by the value to fill. This is only meant for setting a few samples, not whole buffers or large sections.
        :param:buffer: - buffer number
        :param:starting: - sample starting index
        :param:samples: - number of samples to fill (M)
        :param:value: - value
        """
        self.send_message("/b_fill", [buffer, starting])
        
    def b_gen(self, buffer: int, command: str, arguments: ...):
        """
        Call a command to fill a buffer.
        Plug-ins can define commands that operate on buffers. The arguments after the command name are defined by the command. The currently defined buffer fill commands are listed below in a separate section.

        /b_gen does not accept a completion message as the final argument.
        
Asynchronous.Replies to sender with /done /b_gen bufNum when complete.
        :param:buffer: - buffer number
        :param:command: - command name
        :param:arguments: - command arguments
        """
        self.send_message("/b_gen", [buffer, command, arguments])
        
        return self.receive_message(desired="/done")
    
    def b_close(self, buffer: int, completion: bytes):
        """
        Close soundfile.
        After using a buffer with DiskOut, close the soundfile and write header information.
        
Asynchronous.Replies to sender with /done /b_close bufNum when complete.
        :param:buffer: - buffer number
        :param:completion: - an OSC message to execute upon completion. (optional)
        """
        self.send_message("/b_close", [buffer, completion])
        
        return self.receive_message(desired="/done")
    
    def b_query(self, buffer: list[int]):
        """
        Get buffer info.
        Responds to the sender with a /b_info message. The arguments to /b_info are as follows:
        ('N ',): int - buffer number
        ('N ',): int - number of frames
        ('N ',): int - number of channels
        ('N ',): float - sample rate
        :param:buffer: - buffer number(s)
        """
        self.send_message("/b_query", [buffer])
        
        return self.receive_message(desired="/b_info")
    
    def b_get(self, buffer: int, sample: list[int]):
        """
        Get sample value(s).
        Replies to sender with the corresponding /b_set command.
        :param:buffer: - buffer number
        :param:sample: - a sample index
        """
        self.send_message("/b_get", [buffer, sample])
        
        return self.receive_message(desired="/b_set")
    
    def b_getn(self, buffer: int, starting: list[tuple[int, int]]):
        """
        Get ranges of sample value(s).
        Get contiguous ranges of samples. Replies to sender with the corresponding /b_setn command. This is only meant for getting a few samples, not whole buffers or large sections.
        :param:buffer: - buffer number
        :param:starting: - starting sample index
        :param:sequential: - number of sequential samples to get (M)
        """
        self.send_message("/b_getn", [buffer, starting])
        
        return self.receive_message(desired="/b_setn")
    
    def c_set(self, index: list[tuple[int, float | int]]):
        """
        Set bus value(s).
        Takes a list of pairs of bus indices and values and sets the buses to those values.
        :param:index: - a bus index
        :param:control: - a control value
        """
        self.send_message("/c_set", [index])
        
    def c_setn(self, starting: list[tuple[int, int, list[float | int]]]):
        """
        Set ranges of bus value(s).
        Set contiguous ranges of buses to sets of values. For each range, the starting bus index is given followed by the number of channels to change, followed by the values.
        :param:starting: - starting bus index
        :param:sequential: - number of sequential buses to change (M)
        :param:control: - a control value
        """
        self.send_message("/c_setn", [starting])
        
    def c_fill(self, starting: list[tuple[int, int, float | int]]):
        """
        Fill ranges of bus value(s).
        Set contiguous ranges of buses to single values. For each range, the starting sample index is given followed by the number of buses to change, followed by the value to fill.
        :param:starting: - starting bus index
        :param:number: - number of buses to fill (M)
        :param:value: - value
        """
        self.send_message("/c_fill", [starting])
        
    def c_get(self, index: list[int]):
        """
        Get bus value(s).
        Takes a list of buses and replies to sender with the corresponding /c_set command.
        :param:index: - a bus index
        """
        self.send_message("/c_get", [index])
        
        return self.receive_message(desired="/c_set")
    
    def c_getn(self, starting: list[tuple[int, int]]):
        """
        Get ranges of bus value(s).
        Get contiguous ranges of buses. Replies to sender with the corresponding /c_setn command.
        :param:starting: - starting bus index
        :param:sequential: - number of sequential buses to get (M)
        """
        self.send_message("/c_getn", [starting])
        
        return self.receive_message(desired="/c_setn")
    
    def nrt_end(self):
        """
        End real time mode, close file. Not yet implemented.

        This message should be sent in a bundle in non real time mode. The bundle timestamp will establish the ending time of the file. This command will end non real time mode and close the sound file. Replies to sender with /done when complete.
        These messages are sent by the server in response to some commands.
        """
        self.send_message("/nrt_end", [])
        
        return self.receive_message(desired="/done")
    
    def n_go(self):
        """
        A node was started. This command is sent to all registered clients when a node is created.
        """
        self.send_message("/n_go", [])
        
    def n_end(self):
        """
        A node ended. This command is sent to all registered clients when a node ends and is deallocated.
        """
        self.send_message("/n_end", [])
        
    def n_off(self):
        """
        A node was turned off. This command is sent to all registered clients when a node is turned off.
        """
        self.send_message("/n_off", [])
        
    def n_on(self):
        """
        A node was turned on. This command is sent to all registered clients when a node is turned on.
        """
        self.send_message("/n_on", [])
        
    def n_move(self):
        """
        A node was moved. This command is sent to all registered clients when a node is moved.
        """
        self.send_message("/n_move", [])
        
    def n_info(self):
        """
        Reply to /n_query. This command is sent to all registered clients in response to an /n_query command.
        These messages are sent as notification of some event to all clients who have registered via the /notify command.
        """
        self.send_message("/n_info", [])
        
        return self.receive_message(desired="/notify")
    
    def tr(self, node: int, trigger: int, value: float):
        """
        A trigger message.
        This command is the mechanism that synths can use to trigger events in clients. The node ID is the node that is sending the trigger. The trigger ID and value are determined by inputs to the SendTrig unit generator which is the originator of this message.
        These are the currently defined fill routines for use with the /b_gen command.
        :param:node: - node ID
        :param:trigger: - trigger ID
        :param:value: - trigger value
        """
        self.send_message("/tr", [node, trigger, value])
        
        return self.receive_message(desired="/b_gen")
    
