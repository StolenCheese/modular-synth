using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System.Collections.Generic;

namespace modular_synth_frontend.UI;

internal class Menu : Interactable
{
    InputManager input = InputManager.GetInstance();
    private Texture2D boxSprite;

    private List<Button> ActiveButtons;
    public bool open;

    private Texture2D moduleTexture;
    private Texture2D spawnTexture;

    public Menu(Texture2D boxSprite, Texture2D handleSprite, Vector2 position) : base(handleSprite, position)
    {
        this.boxSprite = boxSprite;
        open = false;
        //TODO: Make button spawning proper but for now this will do:
        ActiveButtons = new List<Button>();
    }

    public void LoadContent()
    {
        spawnTexture = ModularSynth.instance.Content.Load<Texture2D>("module Spawner");
        moduleTexture = ModularSynth.instance.Content.Load<Texture2D>("module");
        ActiveButtons.Add(new ModuleSpawnButton(spawnTexture, moduleTexture, new Vector2(100, 50)));
    }

    public override void Update()
    {
        if ((boundingBox.Contains(input.MousePosition()) && input.LeftMouseClickDown()) || input.KeyDown(Microsoft.Xna.Framework.Input.Keys.E))
        {
            ChangeState();
        }

        if (open)
        {
            foreach (Button button in ActiveButtons)
            {
                button.Update();
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(sprite, GetPosition(), colour);

        if (open)
        {
            spriteBatch.Draw(boxSprite, new Vector2(ModularSynth.viewport.Width / 2 - boxSprite.Width / 2, 0), Color.White);

            foreach (Button button in ActiveButtons)
            {
                //Gonna have to have the menu handle how the positioning and everything is done for these (equal spacing and that)
                button.Draw(spriteBatch);
            }
        }
    }

    public void ChangeState()
    {
        if (open)
        {
            ShiftPosition(0, -ModularSynth.viewport.Height/2);
            open = false;
        }
        else
        {
            ShiftPosition(0, ModularSynth.viewport.Height / 2);
            open = true;
        }
    }

}
