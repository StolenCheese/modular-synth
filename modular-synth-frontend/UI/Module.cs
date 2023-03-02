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
                grid.DeOccupyTiles(width, GetPosition());
                originalPosition = GetPosition();
                clickOffset = GetPosition() - input.MousePosVector();
            }

            if(input.RightMouseClickDown())
            {
                //EntityManager.entities.Remove(this);
                //God I hope that makes this garbage collect and we don't have a memory leak TODO: Check that lol
                //TODO: once merged update how entity manager actually works such that can alter list
            }
        }

        if (dragging)
        {
            SetPosition(input.MousePosVector() + clickOffset);

            Vector2 TopLeftCorner = grid.GetNearestRightEdgeTileSnap(new Vector2(boundingBox.Left, boundingBox.Top));

            if(grid.AreTilesOccupied(TopLeftCorner, width))
            {
                invalidPos= true;
            }
            else
            {
                invalidPos= false;
            }
            


            if (invalidPos)
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
                    //TODO: if menu still open then delete module
                    SetPosition(originalPosition);
                    grid.OccupyTiles(width, GetPosition());
                    colour = Color.White;
                    invalidPos = false;
                }
                else
                {
                    //Vector2 TopRightCorner = grid.GetNearestLeftEdgeTileSnap(new Vector2(boundingBox.Right, boundingBox.Top));
                    //if (Math.Abs((position - TopLeftCorner).X) < Math.Abs((new Vector2(boundingBox.Right,position.Y) - TopRightCorner).X)) //TODO: either remove or fix this :(
                    
                    SetPosition(TopLeftCorner);
                    grid.OccupyTiles(width,GetPosition());

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
        originalPosition = GetPosition();
        clickOffset = GetPosition() - input.MousePosVector();
    }
}