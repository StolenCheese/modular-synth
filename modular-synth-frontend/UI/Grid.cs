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

		//Updates which grid tiles to render based on current scroll distance
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
		//Renders visible grid tiles (plus a few extra so scrolling pop-in is less aggressive)

		for (int k = 0; k < ModularSynth.RAILNUM; k++)
		{
			for (int i = 0; i < ROWS; i++)
			{
				for (int j = minVisibleCol - 2; j < maxVisibleCol + 2; j++)
				{
					var rect = new Rectangle(gridSideLength * j + sideScroll, (gridSideLength * i) + ModularSynth.menuBarHeight + (k * railHeight), gridSideLength, gridSideLength);
					Vector2 vec = new Vector2(j, i + ROWS * k);

					if (gridTiles.ContainsKey(vec))
					{
						if (!gridTiles[vec].occupied)
						{
							spriteBatch.Draw(gridTexture, rect, Color.WhiteSmoke);
						}
					}
					else
					{
                        spriteBatch.Draw(gridTexture, rect, Color.WhiteSmoke);
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

	//Gets snap position on grid (goes to left-most corner snapping position)
	public Vector2 GetNearestTileEdgeSnap(Vector2 pos)
	{
		int x = (int)Math.Round(pos.X / gridSideLength, 0); //converts x to a grid number

		x *= gridSideLength; //then multiplies that by pixels needed on screen

		int rail = (int)Math.Round((pos.Y - ModularSynth.menuBarHeight) / railHeight, 0);
        rail = Math.Clamp(rail, 0, ModularSynth.RAILNUM - 1);

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

	//Function that checks if any of the tiles a module is covering are occupied
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

	//Converts from world coordinates to grid-square index for the gridlist
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
