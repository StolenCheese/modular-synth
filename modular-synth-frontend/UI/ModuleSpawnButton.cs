using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System.Diagnostics;

namespace modular_synth_frontend.UI;

internal class ModuleSpawnButton : Button
{
    private Texture2D _texture;
    private InputManager input = InputManager.GetInstance();
    private Grid grid = Grid.GetInstance();

    private string uiDefFile;
    private string secDefFile;
  
    public ModuleSpawnButton(Texture2D sprite, Vector2 position,string uiDefFile, string secDefFile) : base(sprite, position)
    {
        _texture = sprite;
        this.uiDefFile = uiDefFile;
        this.secDefFile = secDefFile;
    }

    public Module Spawn()
    {
        int offset = Module.GetWidth(uiDefFile) * grid.GetGridSideLength() / 2; 
        Vector2 pos = new Vector2(input.MousePosVector().X - offset, input.MousePosVector().Y - (grid.GetGridSideLength() * Grid.ROWS/2));
        return new Module(pos, secDefFile, uiDefFile); 
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
