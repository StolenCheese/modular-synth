﻿using Microsoft.Xna.Framework;
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
	int cols;

	int gridSideLength = ((ModularSynth.viewport.Height - ModularSynth.menuBarHeight) / ModularSynth.RAILNUM - ModularSynth.dividerHeight) / ROWS;
	int railHeight;

	Dictionary<Vector2, GridTile> gridTiles = new(); //where the vector 2 is world position

	int sideScroll;
	Vector2? mouseStart;

	private Grid()
	{
		cols = 30 * ModularSynth.viewport.Width / gridSideLength;
		//railHeight = gridSideLength * ROWS;
		railHeight = (ModularSynth.viewport.Height - ModularSynth.menuBarHeight) / ModularSynth.RAILNUM;

		for (int k = 0; k < ModularSynth.RAILNUM; k++)
		{
			for (int i = 0; i < ROWS; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					Vector2 worldPosCoords = new(j, i + k * ROWS);
					gridTiles.Add(worldPosCoords, new GridTile());
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
				for (int j = 0; j < cols; j++)
				{

					var rect = new Rectangle(gridSideLength * j + sideScroll, (gridSideLength * i) + ModularSynth.menuBarHeight + (k * railHeight), gridSideLength, gridSideLength);

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
		return gridTiles[new Vector2(x, y)];
	}

	//The two following functions are useful for dragging as whether you want to assume left or right will depend on where the majority of the shape is currently covering.
	public Vector2 GetNearestTileEdgeSnap(Vector2 pos)
	{
		int x = (int)Math.Round(pos.X / gridSideLength, 0);

		x = Math.Clamp(x, 0, cols);

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
				gridTiles.Add(new Vector2(x, y + i), new GridTile());
				Debug.WriteLine("Adding new tile at: ", x + "," + (y + i));
			}
		}
		return gridTiles[new Vector2(x, y)].occupied;
	}

	public bool AreTilesOccupied(Vector2 corner, int width)
	{
		//Assumes that tiles are going from top of rail (correct if used correctly)
		corner = ScreenToWorldCoords(corner);

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
		x = Math.Clamp(x, 0, cols);
		int rail = (int)Math.Round((topLeftCorner.Y - ModularSynth.menuBarHeight) / railHeight, 0);
		rail = Math.Clamp(rail, 0, 1);

		for (int i = 0; i < ROWS; i++)
		{
			for (int j = 0; j < width; j++)
			{
				gridTiles[new Vector2(x + j, i + rail * ROWS)].occupied = false;
			}
		}
	}

	public Vector2 ScreenToWorldCoords(Vector2 screenCoords)
	{
		//TODO: Add some knowledge of offset from base screen pos from Modular Synth script

		int x = (int)Math.Round(screenCoords.X / gridSideLength - sideScroll);

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
		int y = (int)Math.Round((screenCoords.Y - railOffset) / gridSideLength) + rail * ROWS;

		return new Vector2(x, y);
	}
}
