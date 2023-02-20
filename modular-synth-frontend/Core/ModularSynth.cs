using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using modular_synth_frontend.UI;
using System.Diagnostics;

namespace modular_synth_frontend.Core;

public class ModularSynth : Game
{
    public static ModularSynth instance { get; private set; }
    public static Viewport viewport { get { return instance.GraphicsDevice.Viewport; } }
    public static Vector2 screenSize { get { return new Vector2(viewport.Height, viewport.Height); } }
    public static GameTime gameTime { get; private set; }

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private InputManager input;
    private Grid grid;

    public const int menuBarHeight = 42;
    public const int dividerHeight = 9;
    public const int RAILNUM = 3;

    Texture2D moduleTexture;
    Texture2D spawnTexture;
    Texture2D gridTexture;
    ModuleSpawnButton button;

    public ModularSynth()
    {
        instance = this;
        _graphics = new GraphicsDeviceManager(this);

        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;

        _graphics.ApplyChanges();

        input = InputManager.GetInstance();
        grid = Grid.GetInstance();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        Debug.WriteLine(grid.GetGridSideLength());
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        moduleTexture = Content.Load<Texture2D>("module");
        spawnTexture = Content.Load<Texture2D>("module Spawner");
        gridTexture = Content.Load<Texture2D>("gridtile");
        button = new ModuleSpawnButton(spawnTexture, moduleTexture, new Vector2(0,0));
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        input.Update();
        EntityManager.Update();
        button.Update();
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gray);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
   
        _spriteBatch.Draw(gridTexture, new Rectangle(0, 0, viewport.Width, menuBarHeight), Color.White);

        for (int i = 1; i <= RAILNUM; i++)
        {
            _spriteBatch.Draw(gridTexture, new Rectangle(0, (((viewport.Height - menuBarHeight) / RAILNUM) * i + menuBarHeight - dividerHeight), viewport.Width, dividerHeight), Color.White);
        }

        grid.Draw(_spriteBatch, gridTexture);

        button.Draw(_spriteBatch);
        EntityManager.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
