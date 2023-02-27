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
    {
        width = 8; //TODO: this is temp
    }

    public Module(Texture2D sprite, Vector2 pos) : base(sprite, pos)
    {
        width = 8;
    }

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

            if(invalidPos)
            {
                colour = Color.Red;
            }
            else
            {
                colour = Color.White;
            }

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

                    if (Math.Abs((position - TopLeftCorner).X) < Math.Abs((new Vector2(boundingBox.Right,position.Y) - TopRightCorner).X)) //TODO: either remove or fix this :(
                    {
                        Debug.WriteLine("Placing in left corner");
                        position = TopLeftCorner;
                        grid.OccupyTiles(width,position);
                    }
                    else
                    {
                        Debug.WriteLine("Placing in right corner");
                        position = TopRightCorner;
                        grid.OccupyTiles(width, position);
                    }
                }
            }
        }
    }

    public int GetWidth()
    {
        return width;
    }

    //set on spawn
    public void Drag()
    {
        dragging = true;
        originalPosition = position;
        clickOffset = position - input.MousePosVector();
    }
}