class SuperColliderController;

#pragma once


# include "SuperColliderCommander.hpp"
# include<set>
# include "Synth.hpp"
# include "Group.hpp"

class SuperColliderController : public SuperColliderCommander {
	std::set<std::string> loaded_synthdefs{};

	Group root{this,0};

	int next_node_id{ 100 };

public:
	SuperColliderController(IpEndpointName endpoint);

	std::future<Synth> InstantiateSynth(const std::string& synthdef);

	void SyncTrees();
};
 