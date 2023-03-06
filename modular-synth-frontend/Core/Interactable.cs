using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace modular_synth_frontend.Core;

public abstract class Interactable : Entity
{
    public Texture2D sprite;
    protected Color colour;
    protected Vector2 position; //relative to world space not screenspace is the idea here (0,0) for this program will be centre of main screen when completely static
    protected Rectangle boundingBox;
    private int xOffset=0;
    private int yOffset=0;
    public int height;
    public int width;
    private double scale = 1;

    public Interactable(Texture2D sprite)
    {
        this.sprite = sprite;
        colour = Color.White;
        position = new Vector2(0, 0);
        boundingBox = sprite.Bounds;
        this.height = sprite.Height;
        this.width = sprite.Width;
    }

    public Interactable(Texture2D sprite, Vector2 position)
    {
        this.sprite = sprite;
        colour = Color.White;
        this.position = position;
        this.height = sprite.Height;
        this.width = sprite.Width;
        boundingBox = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
    }
    public Interactable( Vector2 position)
    {
        this.position = position;
    }

    public Interactable(Texture2D sprite, Vector2 position, Color colour)
    {
        this.sprite = sprite;
        this.colour = colour;
        this.position = position;
        this.height = sprite.Height;
        this.width = sprite.Width;
        boundingBox = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
    }

    //offset is how much larger the bounding box is than the sprite
    public Interactable(Texture2D sprite, Vector2 position, Color colour, int offset)
    {
        this.sprite = sprite;
        this.colour = colour;
        this.position = position;
        this.xOffset = offset;
        this.yOffset = offset;
        this.height = sprite.Height;
        this.width = sprite.Width;
        boundingBox = new Rectangle((int)position.X,(int)position.Y, sprite.Width + offset, sprite.Height + offset);
    }

    public Interactable(Texture2D sprite, Vector2 position, Color colour, double scale)
    {
        this.sprite = sprite;
        this.colour = colour;
        this.position = position;
        this.height = (int)(sprite.Height*scale);
        this.width = (int)(sprite.Width*scale);
        boundingBox = new Rectangle((int)position.X,(int)position.Y, width, height);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        boundingBox = new Rectangle((int)position.X,(int)position.Y, width, height);
        spriteBatch.Draw(sprite, boundingBox, colour);
    }

    /*
    public override void Draw(SpriteBatch spriteBatch, int width, int height)
    {
        spriteBatch.Draw(sprite, new Rectangle((int)position.X, (int)position.Y, width, height), colour);
    }
    */
    public Vector2 GetPosition()
    {
        return position;
    }

    public void SetPosition(Vector2 pos)
    {
        position = pos;
        boundingBox = new Rectangle((int)position.X, (int)position.Y, boundingBox.Width, boundingBox.Height);
    }

    public void ShiftPosition(int x, int y)
    {
        position.X += x;
        position.Y += y;
        boundingBox = new Rectangle((int)position.X, (int)position.Y, boundingBox.Width, boundingBox.Height);
    }
}
