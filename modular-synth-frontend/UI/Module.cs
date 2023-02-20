using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Diagnostics;

namespace modular_synth_frontend.UI;
internal class Module : Interactable
{
    private InputManager input = InputManager.GetInstance();
    private Grid grid = Grid.GetInstance();

    private int width; //in tiles - used for collision code and marking tiles as occupied

    private bool dragging;
    private bool invalidPos = false;
    private Vector2 originalPosition;
    private Vector2 clickOffset;

    public Module(Texture2D sprite) : base(sprite)
    { }

    public Module(Texture2D sprite, Vector2 pos) : base(sprite, pos)
    { }

    public override void Update()
    {
        if (boundingBox.Contains(input.MousePosition()))
        {
            if (input.LeftMouseClickDown())
            {
                dragging = true;
                originalPosition = position;
                clickOffset = position - input.MousePosVector();
            }
        }

        if (dragging)
        {
            position = input.MousePosVector() + clickOffset;
            //TODO: turn red if invalid placement
            //TODO: also check for invalid placement
            if (input.LeftMouseClickUp())
            {
                dragging = false;
                if (invalidPos)
                {
                    //original position can not be null therefore we do need drag event - sad :(
                    position = originalPosition;
                }
                else
                {
                    boundingBox = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height); //TODO: edit this to include collision box offset size instead of just sprite width + height

                    Vector2 TopRightCorner = grid.GetNearestLeftEdgeTileSnap(new Vector2(boundingBox.Right, boundingBox.Top));
                    Vector2 TopLeftCorner = grid.GetNearestRightEdgeTileSnap(new Vector2(boundingBox.Left, boundingBox.Top));

                    if (Math.Abs((position - TopLeftCorner).X) < Math.Abs((position - TopRightCorner).X))
                    {
                        position = TopLeftCorner;
                    }
                    else
                    {
                        position = TopRightCorner;
                    }
                }
            }
        }
    }

    //set on spawn
    public void Drag()
    {
        dragging = true;
        originalPosition = position;
        clickOffset = position - input.MousePosVector();
    }
}