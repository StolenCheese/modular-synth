using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace modular_synth_frontend.UI;

internal class Grid
{
    private static Grid instance = new Grid();

    const int ROWS = 6;
    int cols;

    int gridSideLength = ((ModularSynth.viewport.Height - ModularSynth.menuBarHeight)/ModularSynth.RAILNUM - ModularSynth.dividerHeight)/ROWS;
    int railHeight;

    List<List<GridTile>> gridTiles = new List<List<GridTile>>();


    private Grid()
    {
       cols = ModularSynth.viewport.Width / gridSideLength;
       railHeight = gridSideLength * ROWS;
    }

    public static Grid GetInstance()
    {
        return instance;
    }

    public void Update()
    {
        
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D gridTexture)
    {
        //TODO: introudce a mask over this to only show it in the area around the section you're hovering a module over 

        for (int k = 0; k < ModularSynth.RAILNUM; k++)
        {
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Rectangle rect = new Rectangle(gridSideLength * j, gridSideLength * i + +ModularSynth.menuBarHeight + k * (ModularSynth.dividerHeight + ROWS * gridSideLength), gridSideLength, gridSideLength);
                    spriteBatch.Draw(gridTexture, rect, Color.LightYellow);
                }
            }
        }
    }

    public int GetGridSideLength()
    {
        return gridSideLength;
    }

    //<summary>
    //use this to get the tile the mouse is currently in not worrying about closeness
    //</summary>
    public GridTile GetCurrentTile(Vector2 pos)
    {
        int x = (int)Math.Floor(pos.X / gridSideLength);
        //Change this:
        int y = (int)pos.Y; //TODO: make this right lol
        return gridTiles[x][y];
    }

    //The two following functions are useful for dragging as whether you want to assume left or right will depend on where the majority of the shape is currently covering.
    public Vector2 GetNearestRightEdgeTileSnap(Vector2 pos)
    {
        int x = (int)Math.Round(pos.X / gridSideLength,0);
        x *= gridSideLength;

        int rail = (int)Math.Round((pos.Y - ModularSynth.menuBarHeight)/railHeight,0);

        if(rail < 0)
        {
            rail= 0;
        }
        else if(rail > ModularSynth.RAILNUM -1)
        {
            rail = ModularSynth.RAILNUM - 1;
        }

        Debug.WriteLine("Rail Number: " + rail);
        int y = rail * (railHeight + ModularSynth.dividerHeight) + ModularSynth.menuBarHeight;
        Debug.WriteLine("Placing at: " + y);
        return new Vector2(x,y);
    }

    public Vector2 GetNearestLeftEdgeTileSnap(Vector2 pos)
    {
        //TODO: This is never running rip

        int x = (int)Math.Round(pos.X/gridSideLength,0) - 1;
        x *= gridSideLength;
        float y = pos.Y;
        return new Vector2(x, y);
    }

    public bool IsTileOccupied(int x, int y)
    {
        return gridTiles[x][y].occupied;
    }  

    public void OccupyTiles(Rectangle rect)
    {
        throw new NotImplementedException();
    }
}
