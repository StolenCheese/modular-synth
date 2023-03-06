﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modular_synth_frontend.UI;
public class Wire : Component
{
	//This is required by ports to get the wire that is about to be connected to them

	public static Texture2D orangewire;
	public static List<Texture2D> wireCols = new();
	public static List<Wire> wires = new();
	public Vector2 endPosition;
	public Vector2 Position { private get { return worldSpacePosition; } set { this.worldSpacePosition = value; } }
	Vector2 wireLine;
	int spriteNum = 10;
	Rectangle node;
	public bool animate;
	public int nodesToDraw = 0;


	public Wire(Vector2 modulePos, int parentModuleId, Vector2 moduleLocalPos, Texture2D sprite, Color col, String ParameterID, double scale = 1, bool animate = false) : base(modulePos, parentModuleId, moduleLocalPos, sprite, col, ParameterID, scale, true)
	{
		//set to true as wire is made when someone clicks on a port
		this.visible = true;

		//TODO: use an alternative? as this seems quite inefficient
		Wire.wires.Add(this);

		this.animate = animate;
		if (animate)
		{
			Random rand = new();
			this.sprite = wireCols[rand.Next(0, wireCols.Count)];
		}

	}

	//order is important!
	public override void addComponentToEtyMgr()
	{
		EntityManager.entities.Add(this);
	}

	//unique Draw method required to draw wire sprite from start to end positions
	public override void Draw(SpriteBatch spriteBatch)
	{
		wireLine = endPosition - this.worldSpacePosition;

		//quick and easy way to get a number of sprites to render that is suitably proportional to line length
		spriteNum = MathHelper.Max(MathHelper.Max((int)Math.Abs(wireLine.X), (int)Math.Abs(wireLine.Y)) / 2, 40);

		static float deviation(float step, float maxStep)
		{
			int n = 1000;
			float x = step;
			float b = maxStep;
			float pow = 2.3f;
			float dev = n * (-1 * (float)Math.Pow(x, 2) + (b * x)) / (float)Math.Pow(b, pow);
			float maxVal = n * (-1 * (float)Math.Pow(maxStep / 2, 2) + (b * maxStep / 2)) / (float)Math.Pow(b, pow);
			if (maxVal > 50)
			{
				return dev * 50 / maxVal;
			}
			else return dev;
		}
		int count = spriteNum;
		if (animate)
		{
			count = 1 + nodesToDraw;
			nodesToDraw += spriteNum / 8;
			if (nodesToDraw > spriteNum)
			{
				nodesToDraw = 0;
				animate = false;
			}
		}

		for (int i = 0; i < count + 1; i++)
		{
			float xDeviation = wireLine.X < 0 ? deviation(i, spriteNum) / 2 : -deviation(i, spriteNum) / 2;
			node = new Rectangle(
				(int)(screenSpacePosition.X + i * (wireLine.X / spriteNum) + xDeviation),
				(int)(screenSpacePosition.Y + i * (wireLine.Y / spriteNum) + deviation(i, spriteNum)),
				width, height);

			spriteBatch.Draw(sprite, node, null, colour, 0, new Vector2(this.sprite.Width / 2, this.sprite.Height / 2), SpriteEffects.None, 0);

		}
		//For reding hitbox
		//spriteBatch.Draw(Slider.slider1, boundingBox,colour);

		//Using position instead of rect due to strange behaviour

	}


	public override void Update()
	{

	}
}
