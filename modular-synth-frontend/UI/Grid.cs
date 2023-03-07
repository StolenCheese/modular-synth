using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace modular_synth_frontend.UI;

internal class Grid
{
	private static Grid instance = new();

	public static int SideScroll => instance.sideScroll;
	public const int ROWS = 12;

	int minGeneratedCol;
	int maxGeneratedCol;
	int minVisibleCol = 0;
	int maxVisibleCol;

	int gridSideLength = ((ModularSynth.viewport.Height - ModularSynth.menuBarHeight) / ModularSynth.RAILNUM - ModularSynth.dividerHeight) / ROWS;
	int railHeight;

	Dictionary<Vector2, GridTile> gridTiles = new(); //vector 2 is grid index

	int sideScroll;
	Vector2? mouseStart;

	private Grid()
	{
		maxVisibleCol = ModularSynth.viewport.Width / gridSideLength + 1;
		//railHeight = gridSideLength * ROWS;
		railHeight = (ModularSynth.viewport.Height - ModularSynth.menuBarHeight) / ModularSynth.RAILNUM;

		for (int k = 0; k < ModularSynth.RAILNUM; k++)
		{
			for (int i = 0; i < ROWS; i++)
			{
				for (int j = 0; j < maxVisibleCol; j++)
				{
					Vector2 worldPosCoords = new(j, i + k * ROWS);
					AddNewTile(worldPosCoords);
				}
			}
		}

	}

	public static Grid GetInstance()
	{
		return instance;
	}
	public void Update()
	{ 
		var p = InputManager.GetInstance().MousePosVector();
        //Drag to scroll side to side

        if (-sideScroll + ModularSynth.viewport.Width > maxVisibleCol * gridSideLength)
        {
            maxVisibleCol++;
            minVisibleCol++;
        }
        else if (-sideScroll < minVisibleCol * gridSideLength)
        {
            minVisibleCol--;
            maxVisibleCol--;
        }

        if (!EntityManager.isMouseOverEntity)
		{
			//Drag side to side
			if (InputManager.GetInstance().LeftMouseClickDown())
			{
				mouseStart = p - new Vector2(sideScroll, 0);
			}
			else if (InputManager.GetInstance().LeftMousePressed() && mouseStart is Vector2 ms)
			{
				sideScroll = (int)(p.X - ms.X);
				sideScroll = Math.Clamp(sideScroll, -((maxGeneratedCol + 5) * gridSideLength - ModularSynth.viewport.Width) , -((minGeneratedCol - 5) * gridSideLength));
			}
			else
			{
				mouseStart = null;
			}
		}
		else
		{
			if (InputManager.GetInstance().LeftMousePressed())
			{
				//Most likely dragging while trying to move something
				// if on the edge of the screen, we wanna move the view to allow this
				if (p.X < 20 || p.X > 1260)
				{
					sideScroll += Math.Sign(500 - p.X) * 20;

				}
            }
		}
	}

	public void Draw(SpriteBatch spriteBatch, Texture2D gridTexture)
	{
		//TODO: introudce a mask over this to only show it in the area around the section you're hovering a module over
		//TODO: Just honestly anything about moving the screen

		for (int k = 0; k < ModularSynth.RAILNUM; k++)
		{
			for (int i = 0; i < ROWS; i++)
			{
				for (int j = minVisibleCol - 2; j < maxVisibleCol + 2; j++)
				{
					var rect = new Rectangle(gridSideLength * j + sideScroll, (gridSideLength * i) + ModularSynth.menuBarHeight + (k * railHeight), gridSideLength, gridSideLength);
					Vector2 vec = new Vector2(j, i + ROWS + k);

					if (gridTiles.ContainsKey(vec))
					{
						if (!gridTiles[new Vector2(j, i + ROWS * k)].occupied)
						{
							spriteBatch.Draw(gridTexture, rect, Color.LightYellow);
						}
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

	public void AddNewTile(Vector2 gridCoords)
	{
        gridTiles.Add(gridCoords, new GridTile());
		if(gridCoords.X > maxGeneratedCol)
		{
			maxGeneratedCol = (int)gridCoords.X;
		}
		else if(gridCoords.X < minGeneratedCol)
		{
			minGeneratedCol = (int)gridCoords.X;
		}
    }

	//<summary>
	//use this to get the tile the mouse is currently in not worrying about closeness
	//</summary>
	public GridTile GetCurrentTile(Vector2 pos)
	{
		int x = (int)Math.Floor(pos.X / gridSideLength);
		//Change this:
		int y = (int)pos.Y; //TODO: make this right lol
		return gridTiles[new Vector2(x, y)];
	}

	//The two following functions are useful for dragging as whether you want to assume left or right will depend on where the majority of the shape is currently covering.
	public Vector2 GetNearestTileEdgeSnap(Vector2 pos)
	{
		int x = (int)Math.Round(pos.X / gridSideLength, 0);

		x *= gridSideLength;

		int rail = (int)Math.Round((pos.Y - ModularSynth.menuBarHeight) / railHeight, 0);

		if (rail < 0)
		{
			rail = 0;
		}
		else if (rail > ModularSynth.RAILNUM - 1)
		{
			rail = ModularSynth.RAILNUM - 1;
		}

		int y = rail * (railHeight) + ModularSynth.menuBarHeight;
		return new Vector2(x, y);
	}

	public bool IsTileOccupied(int x, int y)
	{
		if (!gridTiles.ContainsKey(new Vector2(x, y)))
		{
			for (int i = 0; i < ROWS; i++)
			{
				AddNewTile(new Vector2(x, y + i));
				Debug.WriteLine("Adding new tile at: ", x + "," + (y + i));
			}
		}
		return gridTiles[new Vector2(x, y)].occupied;
	}

	public bool AreTilesOccupied(Vector2 corner, int width)
	{
		//Assumes that tiles are going from top of rail (correct if used correctly)
		corner = WorldtoGridCoords(corner);

		for (int i = 0; i < width; i++)
		{
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
        rail = Math.Clamp(rail, 0, ModularSynth.RAILNUM - 1);

        for (int i = 0; i < ROWS; i++)
		{
			for (int j = 0; j < width; j++)
			{
				gridTiles[new Vector2(x + j, i + rail * ROWS)].occupied = true;
			}
		}
	}

	public void DeOccupyTiles(int width, Vector2 topLeftCorner)
	{
		int x = (int)Math.Floor(topLeftCorner.X / gridSideLength);
		int rail = (int)Math.Round((topLeftCorner.Y - ModularSynth.menuBarHeight) / railHeight, 0);
		rail = Math.Clamp(rail, 0, ModularSynth.RAILNUM - 1);

		for (int i = 0; i < ROWS; i++)
		{
			for (int j = 0; j < width; j++)
			{
				gridTiles[new Vector2(x + j, i + rail * ROWS)].occupied = false;
			}
		}
	}

	public Vector2 WorldtoGridCoords(Vector2 worldCoords)
	{
		int x = (int)Math.Round((worldCoords.X) / gridSideLength );

		int rail = (int)Math.Round((worldCoords.Y - ModularSynth.menuBarHeight) / railHeight);
		rail = Math.Clamp(rail, 0, ModularSynth.RAILNUM - 1);

		int railOffset = rail * (railHeight) + ModularSynth.menuBarHeight;
		int y = (int)Math.Round((worldCoords.Y - railOffset) / gridSideLength) + rail * ROWS;

		return new Vector2(x, y);
	}
}
