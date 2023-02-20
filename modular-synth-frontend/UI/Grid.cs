using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;

namespace modular_synth_frontend.UI;

internal class Grid
{
    const int ROWS = 6;

    int gridSquareSide = ((ModularSynth.viewport.Height - ModularSynth.menuBarHeight)/3 - ModularSynth.dividerHeight)/ROWS;
    int cols;

    public Grid()
    {
       cols = ModularSynth.viewport.Width / gridSquareSide;
    }

    public void Update()
    {
        
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D gridTexture)
    {
        //TODO: introudce a mask over this to only show it in the area around the section you're hovering a module over 

        for(int i = 0;i < ROWS; i++)
        {
            for(int j = 0; j < cols; j++)
            {
                Rectangle rect = new Rectangle(gridSquareSide*j,gridSquareSide*i + ModularSynth.menuBarHeight,gridSquareSide,gridSquareSide);
                spriteBatch.Draw(gridTexture, rect, Color.LightYellow);
            }
        }

        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Rectangle rect = new Rectangle(gridSquareSide * j, gridSquareSide * i + ModularSynth.menuBarHeight + ModularSynth.dividerHeight + ROWS*gridSquareSide, gridSquareSide, gridSquareSide);
                spriteBatch.Draw(gridTexture, rect, Color.LightYellow);
            }
        }

        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Rectangle rect = new Rectangle(gridSquareSide * j, gridSquareSide * i + +ModularSynth.menuBarHeight + 2*(ModularSynth.dividerHeight + ROWS * gridSquareSide), gridSquareSide, gridSquareSide);
                spriteBatch.Draw(gridTexture, rect, Color.LightYellow);
            }
        }
    }
}
