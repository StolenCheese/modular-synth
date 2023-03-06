using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.API;
using modular_synth_frontend.Core;
using Newtonsoft.Json;
using SynthAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace modular_synth_frontend.UI;
public class Module : Interactable
{
	private InputManager input = InputManager.GetInstance();
	private Grid grid = Grid.GetInstance();

	private int width; //in tiles - used for collision code and marking tiles as occupied

	private bool placed = false;
	private bool dragging;
	private bool invalidPos = false;
	private Vector2 originalPosition;
	private Vector2 clickOffset;

	//this is used for api calls and ports to check if they are on the same module
	private static int modules = 0;
	public int ModuleId { get; private set; }

	private readonly List<Component> components = new();

	public SCSection scSection;

	public string function;

	private bool canInteract;


	public Module(Texture2D sprite) : base(sprite)
	{
		width = sprite.Width / Grid.GetInstance().GetGridSideLength();
	}
	public Module(Vector2 pos, SectionDef def, bool canInteract = true, float modScale = 1) : base(LoadSprite(def), pos, Color.White, modScale)
	{
		this.canInteract = canInteract;
		this.ModuleId = modules++;

		//TODO: support control synths also
		function = def.audio_synth;
		if (function == null)
			Console.WriteLine("Error parsing function type");

		width = def.width;

		foreach ((string name, ComponentDef component) in def.components)
		{

			Vector2 moduleLocalPos = new(parsePositionX(component.xPos), parsePositionY(component.yPos));

			//Color col = (Color)new System.Drawing.ColorConverter().ConvertFromString(colString); //convert colour to colour type
			var col = new Color(int.Parse(component.col.Substring(0, 3)), int.Parse(component.col.Substring(3, 3)), int.Parse(component.col.Substring(6, 3)));


			switch (component)
			{
				case SliderDef sliderDef:


					Texture2D trackSprite = ModularSynth.content.Load<Texture2D>(sliderDef.trackSprite);
					Texture2D sliderSprite = ModularSynth.content.Load<Texture2D>(sliderDef.sliderSprite);

					moduleLocalPos = new Vector2(moduleLocalPos.X * modScale, moduleLocalPos.Y * modScale);

					//slider static part:
					components.Add(new Component(pos, ModuleId, moduleLocalPos, trackSprite, col, sliderDef.parameterID, sliderDef.trackScale * modScale, sliderDef.vertical, canInteract));
					components.Add(new Slider(pos, ModuleId, moduleLocalPos, trackSprite, sliderSprite, col, sliderDef.parameterID, sliderDef.trackScale * modScale, sliderDef.sliderScale * modScale, sliderDef.vertical, sliderDef.minValueForServer, sliderDef.maxValueForServer, canInteract));

					((Slider)components[components.Count - 1]).setvaluesBasedOnTrack(components[components.Count - 2]);

					break;

				case DialDef dialDef:


					//convert colour to colour type 
					Texture2D staticPartSprite = ModularSynth.content.Load<Texture2D>(dialDef.staticPartSprite);
					Texture2D dialSprite = ModularSynth.content.Load<Texture2D>(dialDef.dialSprite);

					moduleLocalPos = new Vector2(moduleLocalPos.X * modScale, moduleLocalPos.Y * modScale);

					components.Add(new Dial(pos, ModuleId, moduleLocalPos, staticPartSprite, dialSprite, col, dialDef.parameterID, dialDef.staticPartScale * modScale, dialDef.dialScale * modScale, dialDef.minValueForServer, dialDef.maxValueForServer));
					//dial static part:
					components.Add(new Component(pos, ModuleId, moduleLocalPos, staticPartSprite, col, dialDef.parameterID, dialDef.staticPartScale * modScale, canInteract));

					break;

				case PortDef portDef:


					Texture2D spritePort = ModularSynth.content.Load<Texture2D>((string.IsNullOrEmpty(portDef.sprite), portDef.isInput) switch
					{
						(true, true) => "portblue",
						(true, false) => "portred",
						(false, _) => portDef.sprite,
					});


					moduleLocalPos = new Vector2(moduleLocalPos.X * modScale, moduleLocalPos.Y * modScale);

					components.Add(new Port(pos, ModuleId, moduleLocalPos, spritePort, col, portDef.parameterID, portDef.isInput, portDef.scale * modScale));

					break;

				case ButtonDef buttonDef:


					Texture2D spriteButton = ModularSynth.content.Load<Texture2D>(buttonDef.sprite);

					moduleLocalPos = new Vector2(moduleLocalPos.X * modScale, moduleLocalPos.Y * modScale);

					components.Add(new ButtonComponent(pos, ModuleId, moduleLocalPos, spriteButton, col, buttonDef.parameterID, buttonDef.scale * modScale, buttonDef.minValueForServer, buttonDef.maxValueForServer));
					break;

				case SpriteDef spriteDef:

					Texture2D sprite = ModularSynth.content.Load<Texture2D>(spriteDef.sprite);

					moduleLocalPos = new Vector2(moduleLocalPos.X * modScale, moduleLocalPos.Y * modScale);

					components.Add(new Component(pos, ModuleId, moduleLocalPos, sprite, col, "", spriteDef.scale * modScale, false));
					break;

				default:
					throw new Exception("Unreachable, unknown component type");
			}


		}


		if (canInteract)
		{
			addToEtyMgr();
			API.API.CreateSection(this);
			sendInitialComponentValsToServer();
		}
	}

	public void addToEtyMgr()
	{
		EntityManager.entities.Add(this);
		foreach (Component c in components)
		{
			c.addComponentToEtyMgr();
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		if (!canInteract)
		{
			foreach (Component c in components)
			{
				c.Draw(spriteBatch);
			}
		}
	}

	private void updateComponentPositions()
	{
		foreach (Component c in components)
		{
			c.UpdatePos(position);
		}
	}

	private bool isInteractingWithComponent()
	{
		foreach (Component c in components)
		{
			if (c.isInteracting)
			{
				//Console.WriteLine(c.GetType());
				return true;
			}
		}
		return false;
	}

	private void sendInitialComponentValsToServer()
	{
		foreach (Component c in components)
		{
			c.sendValToServer();
		}
	}
	private int parsePositionX(string secDefString)
	{
		Console.WriteLine("secDefString: " + secDefString);
		int offset = 0;
		try
		{
			return int.Parse(secDefString);
		}
		catch
		{
			switch (secDefString[0])
			{
				case 'm': offset = this.sprite.Width / 2; break;
				case 'r': offset = this.sprite.Width; break;
				case 'l': offset = 0; break;
			}
			if (secDefString.Substring(1) != "")
			{
				return offset + int.Parse(secDefString.Substring(1));
			}
			else
			{
				return offset;
			}
		}
	}
	private int parsePositionY(string secDefString)
	{
		//Console.WriteLine("secDefString: " + secDefString);
		int offset = 0;
		try
		{
			return int.Parse(secDefString);
		}
		catch
		{
			switch (secDefString[0])
			{
				case 'm': offset = this.sprite.Height / 2; break;
				case 'b': offset = this.sprite.Height; break;
				case 't': offset = 0; break;
			}
			if (secDefString.Substring(1) != "")
			{
				return offset + int.Parse(secDefString.Substring(1));
			}
			else
			{
				return offset;
			}
		}
	}

	public void AddComponent(Component c)
	{
		components.Add(c);
	}

	private static Texture2D LoadSprite(SectionDef sectionDef)
	{
		return ModularSynth.content.Load<Texture2D>(sectionDef.backgroundImage ?? "module");
	}


	public override void Update()
	{
		if (!isInteractingWithComponent())
		{

			if (boundingBox.Contains(input.MousePosition()))
			{
				if (input.LeftMouseClickDown())
				{
					dragging = true;
					grid.DeOccupyTiles(width, GetPosition());
					originalPosition = GetPosition();
					clickOffset = GetPosition() - input.MousePosVector();
				}

				if (input.RightMouseClickDown())
				{
					//EntityManager.entities.Remove(this);
					//God I hope that makes this garbage collect and we don't have a memory leak TODO: Check that lol
					//TODO: once merged update how entity manager actually works such that can alter list
				}
			}
		}

		if (dragging)
		{
			SetPosition(input.MousePosVector() + clickOffset);

			Vector2 TopLeftCorner = grid.GetNearestTileEdgeSnap(new Vector2(boundingBox.Left, boundingBox.Top));

			if (grid.AreTilesOccupied(TopLeftCorner, width))
			{
				invalidPos = true;
			}
			else
			{
				invalidPos = false;
			}



			if (invalidPos)
			{
				colour = Color.Red;
			}
			else
			{
				colour = Color.White;
			}

			if (input.LeftMouseClickUp())
			{
				dragging = false;
				if (invalidPos)
				{
					if (!placed)
					{
						//TODO: Actually delete don't just deactivate - we're gonna get a memory leak currently

						foreach (Component component in components)
						{
							component.enabled = false;
							component.visible = false;
						}

						visible = false;
						enabled = false;
					}

					else
					{
						SetPosition(originalPosition);
						grid.OccupyTiles(width, GetPosition());
						colour = Color.White;
						invalidPos = false;
					}
				}
				else
				{
					//Vector2 TopRightCorner = grid.GetNearestLeftEdgeTileSnap(new Vector2(boundingBox.Right, boundingBox.Top));
					//if (Math.Abs((position - TopLeftCorner).X) < Math.Abs((new Vector2(boundingBox.Right,position.Y) - TopRightCorner).X)) //TODO: either remove or fix this :(

					SetPosition(TopLeftCorner);
					grid.OccupyTiles(width, GetPosition());

					if (!placed)
					{
						placed = true;
						//Menu.GetInstance().ChangeState(); //TODO: Make this work and not look scuffed
					}
				}
			}
		}
		updateComponentPositions();
	}


	public int GetWidth()
	{
		return width;
	}

	public static int GetWidth(string uiDef)
	{
		var path = Path.GetFullPath("..\\..\\..\\..\\modular-synth-frontend\\SectionDef\\");
		string jsonCombinedFile = File.ReadAllText(path + uiDef + ".json");
		Dictionary<string, Dictionary<string, string>> UIDefDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonCombinedFile);

		if (UIDefDict.ContainsKey("moduleArgs"))
		{
			if (UIDefDict["moduleArgs"].ContainsKey("width"))
			{
				return (int.Parse(UIDefDict["moduleArgs"]["width"]));
			}
		}

		return 8;
	}

	//set on spawn
	public void Drag()
	{
		dragging = true;
		originalPosition = GetPosition();
		clickOffset = GetPosition() - input.MousePosVector();
	}
}
