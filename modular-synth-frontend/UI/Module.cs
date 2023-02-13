using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;

namespace UI;
public class Module : Entity
{
    Transform t = new Transform();
    SpriteRenderer sprite; 

    public Module(Texture2D image)
    {
        AddComponent(t);
        sprite = new SpriteRenderer(image);
        AddComponent(sprite);
    }
}