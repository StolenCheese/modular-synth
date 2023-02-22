using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace modular_synth_frontend.Core;

internal abstract class Interactable : Entity
{
    protected Texture2D sprite;
    protected Color colour;
    protected Vector2 position; //relative to world space not screenspace is the idea here (0,0) for this program will be centre of main screen when completely static
    protected Rectangle boundingBox;
    private int xOffset=0; 
    private int yOffset=0; 
    private int height; 
    private int width;

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
        boundingBox = sprite.Bounds;
        this.height = sprite.Height;
        this.width = sprite.Width;
    }

    public Interactable(Texture2D sprite, Vector2 position, Color colour)
    {
        this.sprite = sprite;
        this.colour = colour;
        this.position = position;
        this.height = sprite.Height;
        this.width = sprite.Width;
        boundingBox = sprite.Bounds;
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
}
