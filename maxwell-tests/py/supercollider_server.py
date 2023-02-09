
from supercollider_udp_client import *

from notes import note

OUT = "out"


class Bus:
    ...


class ConstBus(Bus):
    def __init__(self, value) -> None:
        self.value = value


class DynBus(Bus):

    def __init__(self, index: int,  inputs: "set[Synth]", outputs: "set[Synth]") -> None:
        self.index = index
        self.inputs = set(inputs) if inputs != None else None
        self.outputs = set(outputs) if outputs != None else None

    def connect(self, synth: "Synth", param):
        synth.server._server.n_map(synth.index, (param, self.index))
        synth.params[param] = self
        self.outputs.add(synth)

    def __repr__(self) -> str:
        return f"Bus {self.index}"


class Node:
    def __init__(self, server: "SuperColliderServer", id: int,  group: "Group") -> None:
        self.index = id
        self.group = group
        self.server = server

    def __del__(self):
        self.server._server.n_free(self.index)


class Group(Node):
    ...


class Synth(Node):
    def __init__(self, server: "SuperColliderServer", id: int, synth: str, group: Group, params: dict) -> None:
        super().__init__(server, id, group)

        self.params: dict[str, int | float | Bus] = {k: v for k, v in params.items()}
        self.synth = synth

        self.control = self.synth.endswith("-kr")

        self.valid = True  # Synth could have been freed

        # Get param options
        # if OUT in self.params:
        #     self.params[OUT] = clie

    def __getitem__(self, item):
        return self.get(item)

    def __setitem__(self, param, value):
        self.set(**{param: value})

    def get(self, param: int | str):
        if param in self.params:
            self.server._server.s_get(self.id, param)

    def set(self, ** vals: Bus | float | int):
        st = vals.copy()
        mp = {}

        for param, value in vals.items():
            if param in self.params:

                if isinstance(value, DynBus):
                    # this is a bus, and must be mapped
                    value.inputs.append(self)
                    mp[param] = value.index
                    del st[param]
            else:
                raise AttributeError(f"{param} not a parameter of {self.synth}")

        if len(st) > 0:
            self.server._server.n_set(self.index, *st.items())
        if len(mp) > 0:
            self.server._server.n_map(self.index, *mp.items())

        self.params.update(vals)

    def play(self):
        self.run(True)

    def pause(self):
        self.run(False)

    def run(self, enable: bool):
        self.server._server.n_run(self.index, 1 if enable else 0)

    def free(self):
        if self.valid:
            self.server._server.n_free(self.index)
            self.valid = False

    def __repr__(self) -> str:
        return f"{self.synth}: {self.params}"


class SuperColliderServer:
    def __init__(self,  address, port) -> None:
        self._server = SuperColliderUPDClient(address, port)

        self.synths: dict[int, Synth] = {}
        self.groups: dict[int, Group] = {0: Group(self, 0, None)}  # group is populated with the root group

        self.buses: dict[int, DynBus] = {0: DynBus(0,  [], None)}
        self.nextBusID = 10  # auto-bus-IDS start at 10
        self.nextNodeID = 100
        self._server.g_deepFree(0)
        self.sync()

    def create_synth(self, synth: str, add_action: AddAction, add_target: Node, **args: float | int | Bus) -> Synth:
        while self.nextNodeID in self.synths:
            self.nextNodeID += 1

        for k, v in args.items():
            if isinstance(v, DynBus):
                if k.startswith(OUT):  # only out is currently a valid bus output param name
                    args[k] = v.index
                else:
                    args[k] = f"c{v.index}"
            elif isinstance(v, str):
                args[k] = note(v)

        self._server.s_new(synth, self.nextNodeID, add_action, add_target.index, *args.items())

        # self.sync(add_target)  # this will load the synth we just created, and all it's params, into `synths`

        self.synths[self.nextNodeID] = Synth(self, self.nextNodeID, synth, add_target, args)

        # self.synths[self.nextNodeID].create_output_bus()

        s = self.synths[self.nextNodeID]
        self.nextNodeID += 1
        return s

    def ensure_id(self, id):
        if id not in self.synths:
            raise UserWarning(f"ID:{id} not in map")

    def create_bus(self) -> DynBus:
        x = self.nextBusID
        self.nextBusID += 1
        self.buses[x] = DynBus(x,  [], [])
        return self.buses[x]

    def add_scsyndef(self, scsyndef: str):
        with open(f"synthdefs/{scsyndef}.scsyndef", "rb") as f:  # opening for [r]eading as [b]inary
            self._server.d_recv(f.read())

    def del_scsyndef(self, scsyndef: str):
        self._server.d_free(scsyndef)

    def __getitem__(self, i: int) -> Synth:
        return self.synths.get(i, self.groups.get(i))

    def sync(self, group: Group = None):
        nodes = self._server.g_queryTree(((group or self.groups[0]).index, 1))

        for id, data in nodes.items():

            if isinstance(data, tuple):
                params = data[1]
                if id not in self.synths:
                    #
                    # Sprint(f"discovered new synth {id}: ")
                    self.synths[id] = Synth(self, id, data[0], group, params)

                for k, v in params:
                    if isinstance(v, str) and v.startswith('c'):
                        # this is a control bus
                        busID = int(v[1:])

                        self.synths[id].params[k] = self.buses[busID]
                        self.buses[busID].outputs.add(self.synths[id])

                        self.nextBusID = max(self.nextBusID, busID + 1)

                        #print(f"    {k} connected to {busID}")

                    elif k.startswith(OUT):
                        busID = int(v)

                        self.synths[id].params[k] = self.buses[busID]
                        self.buses[busID].inputs.add(self.synths[id])

                        self.nextBusID = max(self.nextBusID, busID + 1)

                        #print(f"    {k} outputting to {busID}")

    def queryTree(self, *t: tuple[int, int]) -> dict[int, int | tuple[str, list[tuple]]]:
        """Get a representation of this group's node subtree.
        N *	
        int	group ID
        int	flag: if not 0 the current control (arg) values for synths will be included

        Request a representation of this group's node subtree, i.e. all the groups and synths contained within it. 
        Replies to the sender with a /g_queryTree.reply message listing all of the nodes 
        contained within the group in the following format:
        """
        msg = self._server.g_queryTree(*t)

        data = msg.params
        print(data)
        flag = not not data[0]
        nodeID = data[1]
        childCount = data[2]

        #print("----", nodeID, childCount)

        tree: dict[int, int | tuple[str, list[tuple]]] = {}

        i = 3
        while i < len(data):
            n_id: int = data[i]
            children: int = data[i+1]
            if children == -1:
                s_type: str = data[i+2]
                if flag:
                    M = data[i+3]
                    params = [(data[i+4+m], data[i+5+m]) for m in range(0, M*2, 2)]
                    i += 4+M*2
                    tree[n_id] = (s_type, params)
                else:
                    i += 3
                    tree[n_id] = (s_type, None)
            else:
                i += 2
                tree[n_id] = children
        return tree
