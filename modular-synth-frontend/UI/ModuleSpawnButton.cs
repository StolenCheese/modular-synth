using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System.Diagnostics;

namespace modular_synth_frontend.UI;

internal class ModuleSpawnButton : Button
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

    public ModuleSpawnButton(Texture2D sprite, Vector2 position) : base(sprite, position)
    {
        _texture = sprite;
        moduleSprite = sprite;
    }

    public Module Spawn()
    {
        return new Module(moduleSprite, input.MousePosVector()); //TODO: spawn mouse on middle of sprite not on top - can use half of module width * gridsidelenth and all modules are same height so that's easy
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
