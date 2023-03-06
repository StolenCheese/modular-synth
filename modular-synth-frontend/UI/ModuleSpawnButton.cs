using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Diagnostics;

namespace modular_synth_frontend.UI;

internal class ModuleSpawnButton : Button
{
    private Texture2D _texture;
    private Module buttonImage;

    private InputManager input = InputManager.GetInstance();
    private Grid grid = Grid.GetInstance();

    private string uiDefFile;
    private string secDefFile;

    public static event Action ModuleSpawned;

    public ModuleSpawnButton(Vector2 position,string uiDefFile, string secDefFile,double scale=0.5) : base(position)
    {
        //_texture = sprite;
        this.uiDefFile = uiDefFile;
        this.secDefFile = secDefFile;

        ModuleSpawned += Menu.GetInstance().ChangeState;

        buttonImage = new Module(position, secDefFile, uiDefFile,false,(float)scale);
        this.width = (int)(scale*buttonImage.sprite.Width);
        this.height = (int)(scale*buttonImage.sprite.Height);

        boundingBox = new Rectangle((int)position.X,(int)position.Y,width,height);
    }

    public Module Spawn()
    {
        ModuleSpawned.Invoke();

        int offset = Module.GetWidth(uiDefFile) * grid.GetGridSideLength() / 2; 
        Vector2 pos = new Vector2(input.MousePosVector().X - offset, input.MousePosVector().Y - (grid.GetGridSideLength() * Grid.ROWS/2));
        return new Module(pos, secDefFile, uiDefFile); 
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        buttonImage.Draw(spriteBatch);
    }

    public override void Update()
    {
        if (boundingBox.Contains(input.MousePosition()))
        {
            if (input.LeftMouseClickDown()&&!Menu.justOpen)
            {
                Module newModule = Spawn();
                newModule.Drag();

                //need to create a dragging event i think because we need to have "on drag stop if not moved out of region then delete newModule
            }
        }
    }
}
