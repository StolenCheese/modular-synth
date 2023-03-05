using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System.Diagnostics;

namespace modular_synth_frontend.UI;

internal class ModuleSpawnButton : Button
{
    private Texture2D _texture;
    private InputManager input = InputManager.GetInstance();
    private string secUiDefFileName;
  
    public ModuleSpawnButton(Texture2D sprite, Vector2 position,string secUiDefFileName) : base(sprite, position)
    {
        _texture = sprite;
        this.secUiDefFileName = secUiDefFileName;
    }

    public Module Spawn()
    {
        return new Module(input.MousePosVector(),secUiDefFileName); //TODO: spawn mouse on middle of sprite not on top - can use half of module width * gridsidelenth and all modules are same height so that's easy
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
