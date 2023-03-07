using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modular_synth_frontend.UI;
internal class Slider : Component
{
	public static Texture2D rail1;
	public static Texture2D slider1;
	public static Texture2D slider2;
	private bool dragging;
	private float clickOffset = 0;
	private float sliderOffset = 0;
	//TODO: make slider offsets based on StationaryComponent
	private int maxSliderOffset = 130;
	private int minSliderOffset = 0;
	private InputManager input = InputManager.GetInstance();

	public Slider(Vector2 modulePos, int parentModuleId, Vector2 moduleLocalPos, Texture2D trackSprite, Texture2D sliderSprite, Color col, string ParameterID, double trackScale = 1, double sliderScale = 1, bool vertical = false, double minValueForServer = 0, double maxValueForServer = 1, bool canInteract = true) : base(modulePos, parentModuleId, moduleLocalPos, sliderSprite, col, ParameterID, sliderScale, vertical, canInteract)
	{

		this.minValueForServer = minValueForServer;
		this.maxValueForServer = maxValueForServer;
		this.vertical = vertical;

	}

	//We want the module that this component belongs to to give the component its coordinates
	public override void UpdatePos(Vector2 modulePos)
	{
		this.modulePos = modulePos;
	}

	public override void sendValToServer()
	{
		if (SliderOffset > maxSliderOffset || SliderOffset < minSliderOffset)
		{
			Console.WriteLine($"Slider offset of {SliderOffset} is greater than max allowed!! skipping send");
		}
		else
		{
			double svRange = maxValueForServer - minValueForServer;
			double thisRange = maxSliderOffset - minSliderOffset;
			//translate value relative to this range to value relative to server range
			//Console.WriteLine($"SliderOffset:{SliderOffset},svRange:{svRange},thisRange:{thisRange},minValueForServer:{minValueForServer},minSliderOffset:{minSliderOffset}");
			float val = (float)((SliderOffset - minSliderOffset) * svRange / thisRange + minValueForServer);
			if (val != lastSentVal)
			{
				API.API.SetValue(this.parentModuleId, this.parameterID, val);
				lastSentVal = val;
			}
		}
	}

	public void setvaluesBasedOnTrack(Component track)
	{
		if (vertical)
		{
			maxSliderOffset = track.height / 2;
			minSliderOffset = -track.height / 2;
		}
		else
		{
			maxSliderOffset = track.width / 2;
			minSliderOffset = -track.width / 2;
		}
	}

	//order is important!
	public override void addComponentToEtyMgr()
	{
		EntityManager.entities.Add(this);
	}

	private float SliderOffset
	{
		get { return sliderOffset; }
		set { sliderOffset = MathHelper.Clamp(value, minSliderOffset, maxSliderOffset); }
	}

	public override void Update()
	{
		SetWorldCenter(worldSpacePosition);
		//Console.WriteLine($"SliderOffset: {SliderOffset}");  
		if (canInteract)
		{
			if (screenSpaceBoundingBox.Contains(input.MousePosition()))
			{
				EntityManager.isMouseOverEntity = true;
				isInteracting = true;
				if (input.LeftMouseClickDown())
				{
					dragging = true;
					clickOffset = worldSpacePosition.X - input.MousePosVector().X;
				}
			}
			else
			{
				isInteracting = false;
			}
			if (dragging)
			{
				isInteracting = true;
				if (vertical)
				{
					//minus used here to make sliding up increase the value and sliding down decrease
					SliderOffset = -(input.MousePosVector().Y + clickOffset - modulePos.Y - moduleLocalPos.Y);
					worldSpacePosition.Y = -SliderOffset + modulePos.Y + moduleLocalPos.Y;
				}
				else
				{
					SliderOffset = input.MousePosVector().X + clickOffset - modulePos.X - moduleLocalPos.X;
					worldSpacePosition.X = SliderOffset + modulePos.X + moduleLocalPos.X;
				}

				if (input.LeftMouseClickUp())
				{
					dragging = false;
				}
				sendValToServer();
			}
			else
			{
				worldSpacePosition = modulePos + moduleLocalPos;
				if (vertical)
				{
					worldSpacePosition.Y -= SliderOffset;
				}
				else
				{
					worldSpacePosition.X += SliderOffset;
				}
			}
		}
		else
		{
			worldSpacePosition = modulePos + moduleLocalPos;
		}
	}
}

