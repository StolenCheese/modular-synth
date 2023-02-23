using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System.Diagnostics;

namespace modular_synth_frontend.UI;

internal class ModuleSpawnButton : Interactable
{
    private Texture2D _texture; 
    private Texture2D moduleSprite; //at some point this will store an actual module but atm i just need it to spawn an image so
    private InputManager input = InputManager.GetInstance();
    private SpriteBatch _spriteBatch;
    public ModuleSpawnButton(Texture2D texture, Texture2D sprite, Vector2 position,SpriteBatch spriteBatch) : base(texture, position)
    {
        _texture = texture;
        moduleSprite = sprite;

        _spriteBatch = spriteBatch;
    }

    public Module Spawn()
    {
        return new Module(moduleSprite, input.MousePosVector(),_spriteBatch); //TODO: spawn mouse on middle of sprite not on top (so set some kind of centre position on the module class defintion or something)
    }

    public override void Update()
    {
        if (boundingBox.Contains(input.MousePosition()))
        {
            if (input.LeftMouseClickDown())
            {
                Module newModule = Spawn();
                newModule.Drag();

                //need to create a dragging event i think because we need to have "on drag stop if not moved out of region then delete newModule
            }
        }
    }
}
