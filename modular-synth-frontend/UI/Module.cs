using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System.Diagnostics;

namespace modular_synth_frontend.UI;
internal class Module : Interactable
{
    private InputManager input = InputManager.GetInstance();
    private bool dragging;
    private Vector2 clickOffset;

    public Module(Texture2D sprite) : base(sprite)
    { }

    public void Update()
    {
        if (boundingBox.Contains(input.MousePosition()))
        {
            if (input.LeftMouseClickDown())
            {
                dragging = true;
                clickOffset = position - MousePosVector();
            }
        }

        if (dragging)
        {
            position = MousePosVector() + clickOffset;
            if (input.LeftMouseClickUp())
            {
                dragging = false;
                boundingBox = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height); //TODO: edit this to include collision box offset size instead of just sprite width + height
            }
        }
    }

    public Vector2 MousePosVector()
    {
        return new Vector2(input.MousePosition().X, input.MousePosition().Y);
    }
}