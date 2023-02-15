using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace modular_synth_frontend.Core;

public class SpriteRenderer : Component{
    private Texture2D image;

    public Color colour = Color.White;

    public SpriteRenderer(Texture2D sprite)
    {
        this.image = sprite;
        colour = Color.White;
    }

    public SpriteRenderer(Texture2D sprite, Color colour)
    {
        this.image = sprite;
        this.colour = colour;
    }

    public void Draw()
    {

    }
}