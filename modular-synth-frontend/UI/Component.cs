using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modular_synth_frontend.UI;
public class Component : Interactable
{
	protected Vector2 moduleLocalPos;
	protected Vector2 modulePos;
	public int parentModuleId;
	public string parameterID;

	//TODO: make slider offsets based on StationaryComponent
	public bool isInteracting = false;
	protected double scale;
	public double rotation = 0;


	//TODO: remove these 2
	public int height;
	public int width;
	public bool vertical = false;
	protected double maxValueForServer = 1;
	protected double minValueForServer = 0;
	protected double lastSentVal = -1111; //set to value that will never be used to make sure all components sync at start
	protected bool canInteract = true;

	//origin is at the center of the component. Use this to add offset

	public Component(Vector2 modulePos, int parentModuleId, Vector2 moduleLocalPos, Texture2D baseSprite, Color col, string parameterID, double scale = 1, bool canInteract = true) : base(baseSprite, modulePos + moduleLocalPos, col, scale)
	{
		this.scale = scale;
		this.height = (int)(this.sprite.Height * this.scale);
		this.width = (int)(this.sprite.Width * this.scale);
		this.modulePos = modulePos;
		this.moduleLocalPos = moduleLocalPos;
		this.parentModuleId = parentModuleId;
		this.parameterID = parameterID;
		this.canInteract = canInteract;

	}

	public Component(Vector2 modulePos, int parentModuleId, Vector2 moduleLocalPos, Texture2D baseSprite, Color col, string paramID, double scale = 1, bool vertical = false, bool canInteract = true) : base(baseSprite, modulePos + moduleLocalPos, col, scale)
	{
		this.scale = scale;
		this.height = (int)(this.sprite.Height * this.scale);
		this.width = (int)(this.sprite.Width * this.scale);
		this.modulePos = modulePos;
		this.moduleLocalPos = moduleLocalPos;
		this.vertical = vertical;
		this.parentModuleId = parentModuleId;
		this.parameterID = paramID;
		this.canInteract = canInteract;


		setSCLimitsBasedOnParamID();

		if (vertical)
		{
			this.rotation = Math.PI / 2;
			int temp = this.height;
			this.height = this.width;
			this.width = temp;
		}
	}

	void setSCLimitsBasedOnParamID()
	{
		switch (this.parameterID)
		{
			case "freq":
				//suitable Hz range
				this.maxValueForServer = 10000;
				this.minValueForServer = 200;
				break;
		}
	}

	//We want the module that this component belongs to to give the component its coordinates
	public virtual void UpdatePos(Vector2 modulePos)
	{
		this.modulePos = modulePos;
		worldSpacePosition = modulePos + moduleLocalPos;
		SetWorldCenter(worldSpacePosition);
	}

	//override this for subclasses with itneraction with the sc server who need to sync values (dials, sliders)
	public virtual void sendValToServer() { }

	public virtual void addComponentToEtyMgr()
	{
		EntityManager.entities.Add(this);
	}
	public override void Draw(SpriteBatch spriteBatch)
	{
		//boundingBox = new Rectangle((int)position.X - width / 2, (int)position.Y - height / 2, width, height);

		//For reding hitbox
		//spriteBatch.Draw(Slider.slider1, boundingBox,colour);

		//Using position instead of rect due to strange behaviour when rotating sliders to be vertical
		spriteBatch.Draw(sprite, screenSpacePosition, null, colour, (float)rotation, new Vector2(this.sprite.Width / 2, this.sprite.Height / 2), (float)this.scale, SpriteEffects.None, 1.0f);
	}
	public override void Update()
	{
		this.worldSpacePosition = modulePos + moduleLocalPos;
	}

}

