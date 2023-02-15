using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace modular_synth_frontend.Core;

internal abstract class Interactable
{
    // <summary>
    // Checks if we run Update events on entity
    // </summary>
    public bool enabled = true;

    // <summary>
    // Checks if we run Draw events on entity
    // </summary>
    public bool visible = true;

    protected Texture2D sprite;
    protected Color colour;
    protected Vector2 position; //relative to world space not screenspace is the idea here (0,0) for this program will be centre of main screen when completely static
    protected Rectangle boundingBox;

    public Interactable(Texture2D sprite)
    {
        this.sprite = sprite;
        colour = Color.White;
        position = new Vector2(0, 0);
        boundingBox = sprite.Bounds;
    }

    public Interactable(Texture2D sprite, Vector2 position)
    {
        this.sprite = sprite;
        colour = Color.White;
        this.position = position;
        boundingBox = sprite.Bounds;
    }

    public Interactable(Texture2D sprite, Vector2 position, Color colour)
    {
        this.sprite = sprite;
        this.colour = colour;
        this.position = position;
        boundingBox = sprite.Bounds;
    }

    //offset is how much larger the bounding box is than the sprite
    public Interactable(Texture2D sprite, Vector2 position, Color colour, int offset)
    {
        this.sprite = sprite;
        this.colour = colour;
        this.position = position;
        boundingBox = new Rectangle((int)position.X,(int)position.Y, sprite.Width + offset, sprite.Height + offset);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(sprite, position, colour);
    }
}
