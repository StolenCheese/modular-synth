using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.UI;
using System;

namespace modular_synth_frontend.Core;

public abstract class Interactable : Entity
{
	public Texture2D sprite;
	protected Color colour;
	protected Vector2 worldSpacePosition; //relative to world space not screenspace is the idea here (0,0) for this program will be centre of main screen when completely static
	protected Vector2 screenSpacePosition =>
		fixedOnScreen ?
		worldSpacePosition :
		worldSpacePosition + new Vector2(Grid.SideScroll, 0);

	public bool fixedOnScreen = false;
	protected Rectangle worldSpaceBoundingBox;
	protected Rectangle screenSpaceBoundingBox =>
		fixedOnScreen ?
		worldSpaceBoundingBox :
		new Rectangle(new Point(worldSpaceBoundingBox.Left + Grid.SideScroll, worldSpaceBoundingBox.Top), worldSpaceBoundingBox.Size);

	private int xOffset = 0;
	private int yOffset = 0;
	public int height;
	public int width;
	private double scale = 1;

	public Interactable(Texture2D sprite)
	{
		this.sprite = sprite;
		colour = Color.White;
		worldSpacePosition = new Vector2(0, 0);
		worldSpaceBoundingBox = sprite.Bounds;
		this.height = sprite.Height;
		this.width = sprite.Width;
	}

	public Interactable(Texture2D sprite, Vector2 position)
	{
		this.sprite = sprite;
		colour = Color.White;
		worldSpacePosition = position;
		this.height = sprite.Height;
		this.width = sprite.Width;
		worldSpaceBoundingBox = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
	}
	public Interactable(Vector2 position)
	{
		this.worldSpacePosition = position;
	}

	public Interactable(Texture2D sprite, Vector2 position, Color colour)
	{
		this.sprite = sprite;
		this.colour = colour;
		this.worldSpacePosition = position;
		this.height = sprite.Height;
		this.width = sprite.Width;
		worldSpaceBoundingBox = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
	}

	//offset is how much larger the bounding box is than the sprite
	public Interactable(Texture2D sprite, Vector2 position, Color colour, int offset)
	{
		this.sprite = sprite;
		this.colour = colour;
		this.worldSpacePosition = position;
		this.xOffset = offset;
		this.yOffset = offset;
		this.height = sprite.Height;
		this.width = sprite.Width;
		worldSpaceBoundingBox = new Rectangle((int)position.X, (int)position.Y, sprite.Width + offset, sprite.Height + offset);
	}

	public Interactable(Texture2D sprite, Vector2 position, Color colour, double scale)
	{
		this.sprite = sprite;
		this.colour = colour;
		this.worldSpacePosition = position;
		this.height = (int)(sprite.Height * scale);
		this.width = (int)(sprite.Width * scale);
		worldSpaceBoundingBox = new Rectangle((int)position.X, (int)position.Y, width, height);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(sprite, screenSpaceBoundingBox, colour);
	}

	/*
    public override void Draw(SpriteBatch spriteBatch, int width, int height)
    {
        spriteBatch.Draw(sprite, new Rectangle((int)position.X, (int)position.Y, width, height), colour);
    }
    */
	public Vector2 GetPosition()
	{
		return worldSpacePosition;
	}

	public void SetWorldCenter(Vector2 pos)
	{
		worldSpacePosition = pos;
		worldSpaceBoundingBox = new Rectangle(
			(int)worldSpacePosition.X - worldSpaceBoundingBox.Width / 2,
			(int)worldSpacePosition.Y - worldSpaceBoundingBox.Height / 2,
			worldSpaceBoundingBox.Width,
			worldSpaceBoundingBox.Height
			);
	}

	public void SetScreenTopLeft(Vector2 pos) => SetWorldTopLeft(pos - new Vector2(Grid.SideScroll, 0));


	public void SetWorldTopLeft(Vector2 pos)
	{
		worldSpacePosition = pos;
		worldSpaceBoundingBox = new Rectangle(
			(int)worldSpacePosition.X,
			(int)worldSpacePosition.Y,
			worldSpaceBoundingBox.Width,
			worldSpaceBoundingBox.Height
			);
	}

	public void ShiftPosition(int x, int y)
	{
		worldSpacePosition.X += x;
		worldSpacePosition.Y += y;
		SetWorldCenter(worldSpacePosition);
	}
}
