using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace modular_synth_frontend.Core;

public enum SectionType
{
	Synth = 0,
	Midi = 1,
}

[Serializable]
public record ComponentDef(
	string type, string parameterID, string xPos, string yPos,
	double minValueForServer, double maxValueForServer,
	string col)
{

	public virtual ComponentDef ApplyDefaults()
	{
		return this with
		{
			col = col ?? "255255255",
			maxValueForServer = maxValueForServer == 0 ? 1 : maxValueForServer,
		};
	}

};


//These are a little ugly definitions, but they work well

//"volume dial": {
//		"type": "dial",
//     "xPos": "r-50",
//     "yPos": "60",
//     "staticPartScale": "0.5",
//     "dialScale": "0.5",
//     "staticPartSprite": "indicator1",
//     "dialSprite": "dial1"

//},

[Serializable]
public record DialDef(string type, string parameterID, string xPos, string yPos,
	double minValueForServer, double maxValueForServer, string col, double staticPartScale,
	double dialScale, string staticPartSprite, string dialSprite)
	: ComponentDef(type, parameterID, xPos, yPos, minValueForServer, maxValueForServer, col)
{
	public override ComponentDef ApplyDefaults()
	{
		return (DialDef)base.ApplyDefaults() with { dialScale = dialScale == 0 ? 1 : dialScale };
	}

}


[Serializable]
public record SliderDef(string type, string parameterID, string xPos, string yPos,
	double minValueForServer, double maxValueForServer, string col, bool vertical, double trackScale,
	double sliderScale, string trackSprite, string sliderSprite)
	: ComponentDef(type, parameterID, xPos, yPos, minValueForServer, maxValueForServer, col)
{
	public override ComponentDef ApplyDefaults()
	{
		return (SliderDef)base.ApplyDefaults() with
		{
			trackScale = trackScale == 0 ? 1 : trackScale,
			sliderScale = sliderScale == 0 ? 1 : sliderScale
		};
	}
}


//"i/p ear": {
//  "type": "port",
//  "xPos": "m",
//  "yPos": "m",
//},
[Serializable]
public record PortDef(string type, string parameterID, string xPos, string yPos, double minValueForServer,
	double maxValueForServer, string col, bool isInput, string sprite, double scale)
	: ComponentDef(type, parameterID, xPos, yPos, minValueForServer, maxValueForServer, col)
{
	public override ComponentDef ApplyDefaults()
	{
		return (PortDef)base.ApplyDefaults() with { scale = scale == 0 ? 1 : scale };
	}
}

[Serializable]
public record SpriteDef(string type, string parameterID, string xPos, string yPos, double minValueForServer,
	double maxValueForServer, string col, string sprite, double scale)
	: ComponentDef(type, parameterID, xPos, yPos, minValueForServer, maxValueForServer, col)
{
	public override ComponentDef ApplyDefaults()
	{
		return (SpriteDef)base.ApplyDefaults() with { scale = scale == 0 ? 1 : scale };
	}
}


[Serializable]
public record ButtonDef(string type, string parameterID, string xPos, string yPos, double minValueForServer,
	double maxValueForServer, string col, string sprite, double scale)
	: ComponentDef(type, parameterID, xPos, yPos, minValueForServer, maxValueForServer, col)
{
	public override ComponentDef ApplyDefaults()
	{
		return (ButtonDef)base.ApplyDefaults() with { scale = scale == 0 ? 1 : scale };
	}
}


[Serializable]
public record SectionDef(string audio_synth, string control_synth, SectionType type, string backgroundImage, int width)
{
	[NonSerialized]
	public readonly Dictionary<string, ComponentDef> components = new();

	public SectionDef ApplyDefaults()
	{
		return this with { width = Math.Min(1, width) };
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new();
		stringBuilder.Append("SectionDef"); // type name

		stringBuilder.Append("audio_synth = ");
		stringBuilder.Append(audio_synth);

		stringBuilder.Append("control_synth = ");
		stringBuilder.Append(control_synth);

		stringBuilder.Append("backgroundImage = ");
		stringBuilder.Append(backgroundImage);

		stringBuilder.Append("width = ");
		stringBuilder.Append(width);

		stringBuilder.Append(" { ");
		if (PrintMembers(stringBuilder))
		{
			foreach (var item in components)
			{
				stringBuilder.Append(item);
				stringBuilder.AppendLine();
			}
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

}



static class SectionDefManager
{
	static void JsonInherit(JObject parent, JObject child)
	{
		if (parent == null) return;

		//Debug.WriteLine("Merging:");
		//Debug.WriteLine(parent);
		//Debug.WriteLine("AND:");
		//Debug.WriteLine(child);



		foreach ((string key, JToken token) in parent)
		{
			// insert into child if not exist
			if (child[key] == null)
			{
				child[key] = token;
			}
		}

		foreach ((string key, JToken token) in child)
		{
			// update objects with values from parent
			if (token is JObject obj)
				JsonInherit(parent[key]?.ToObject<JObject>(), obj);
		}
	}

	static JObject LoadSectionDefJson(string sectionName)
	{
		var path = Path.GetFullPath("..\\..\\..\\..\\modular-synth-frontend\\SectionDef\\");

		string text = File.ReadAllText(Path.Combine(path, sectionName + ".json"));

		var secDef = JObject.Parse(text);


		// Inheritance tree!
		// Careful not to loop lol

		var parentDef = secDef["inherits"] switch
		{
			JToken i => LoadSectionDefJson(i.ToObject<string>()),
			_ => null
		};

		// Inherit properties from the parent


		JsonInherit(parentDef, secDef);

		return secDef;

	}
	public static SectionDef LoadSectionDef(string sectionName)
	{


		JObject secDef = LoadSectionDefJson(sectionName);


		//Debug.WriteLine(secDef);


		var def = secDef.ToObject<SectionDef>();

		foreach ((string key, JToken comp) in secDef["components"].ToObject<JObject>())
		{
			ComponentDef compDef = comp["type"]?.ToObject<string>() switch
			{
				"dial" => comp.ToObject<DialDef>(),
				"port" => comp.ToObject<PortDef>(),
				"slider" => comp.ToObject<SliderDef>(),
				"sprite" => comp?.ToObject<SpriteDef>(),
				"button" => comp?.ToObject<ButtonDef>(),
				_ => throw new InvalidOperationException("No such component type=" + comp["type"]),
			};

			def.components[key] = compDef.ApplyDefaults();


		}


		Debug.WriteLine(def);

		return def;
	}

}
