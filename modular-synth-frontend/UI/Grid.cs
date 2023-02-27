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

    const int ROWS = 12;
    int cols;

    int gridSideLength = ((ModularSynth.viewport.Height - ModularSynth.menuBarHeight)/ModularSynth.RAILNUM - ModularSynth.dividerHeight)/ROWS;
    int railHeight;

    Dictionary<Vector2,GridTile> gridTiles = new Dictionary<Vector2, GridTile>(); //where the vector 2 is world position


    private Grid()
    {
       cols = ModularSynth.viewport.Width / gridSideLength;
       //railHeight = gridSideLength * ROWS;
       railHeight = (ModularSynth.viewport.Height - ModularSynth.menuBarHeight)/ ModularSynth.RAILNUM;

        for (int k = 0; k < ModularSynth.RAILNUM; k++)
        {
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Vector2 worldPosCoords = new Vector2(j,i + k*ROWS);
                    Debug.WriteLine(worldPosCoords);
                    gridTiles.Add(worldPosCoords, new GridTile());
                }
            }
        }
    
    }

    public static Grid GetInstance()
    {
        return instance;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D gridTexture)
    {
        //TODO: introudce a mask over this to only show it in the area around the section you're hovering a module over 
        //TODO: Just honestly anything about moving the screen

        for (int k = 0; k < ModularSynth.RAILNUM; k++)
        {
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Rectangle rect = new Rectangle(gridSideLength * j, (gridSideLength * i) + ModularSynth.menuBarHeight + (k * railHeight), gridSideLength, gridSideLength);

                    if (gridTiles[new Vector2(j, i + ROWS * k)].occupied)
                    {
                        spriteBatch.Draw(gridTexture, rect, Color.Red);
                    }
                    else
                    {
                        spriteBatch.Draw(gridTexture, rect, Color.LightYellow);
                    }
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
        return gridTiles[new Vector2(x,y)];
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
        int y = rail * (railHeight) + ModularSynth.menuBarHeight;
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
        return gridTiles[new Vector2(x, y)].occupied;
    } 
   
    public bool AreTilesOccupied(Vector2 corner, int width)
    {
        //Assumes that tiles are going from top of rail (correct if used correctly)
        corner = ScreenToWorldCoords(corner);

        for(int i = 0; i < width; i++) {
            //Assuming all modules are the same height (an invariant we established already - this should work)
            if (IsTileOccupied((int)corner.X + i, (int)corner.Y))
            {
                return true;
            }
        }
        return false;
    }

    public void OccupyTiles(int width, Vector2 topLeftCorner)
    {
        int x = (int)Math.Floor(topLeftCorner.X / gridSideLength);
        int rail = (int)Math.Round((topLeftCorner.Y - ModularSynth.menuBarHeight) / railHeight, 0);

        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (x + j < cols)
                {
                    //TODO: remove if and just have it add extra tiles
                    gridTiles[new Vector2(x + j, i + rail * ROWS)].occupied = true;
                }
            }
        }
    }

    public void DeOccupyTiles(int width, Vector2 topLeftCorner)
    {
        int x = (int)Math.Floor(topLeftCorner.X / gridSideLength);
        int rail = (int)Math.Round((topLeftCorner.Y - ModularSynth.menuBarHeight) / railHeight, 0);

        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (x + j < cols)
                {
                    //TODO: remove if and just have it add extra tiles
                    gridTiles[new Vector2(x + j, i + rail * ROWS)].occupied = false;
                }
            }
        }
    }

    public Vector2 ScreenToWorldCoords(Vector2 screenCoords)
    {
        //TODO: Add some knowledge of offset from base screen pos from Modular Synth script

        int x = (int)Math.Round(screenCoords.X / gridSideLength);
        if(x < 0)
        {
            x = 0;
        }
        else if(x > cols){
            x = cols - 8;
        }

        int rail = (int)Math.Round((screenCoords.Y - ModularSynth.menuBarHeight) / railHeight);

        if (rail < 0)
        {
            rail = 0;
        }
        else if (rail > ModularSynth.RAILNUM - 1)
        {
            rail = ModularSynth.RAILNUM - 1;
        }

        int railOffset = rail * (railHeight) + ModularSynth.menuBarHeight;
        int y = (int)Math.Round((screenCoords.Y - railOffset)/gridSideLength) + rail * ROWS;

        return new Vector2(x,y);
    }
}
