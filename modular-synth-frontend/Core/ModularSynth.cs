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

    Texture2D cardTexture;
    Texture2D spawnTexture;
    Texture2D gridTexture;
 
    ModuleSpawnButton button;

    public Texture2D slider;

    public ModularSynth()
    {
        instance = this;
        _graphics = new GraphicsDeviceManager(this);

        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;

        _graphics.ApplyChanges();

        input = InputManager.GetInstance();
        grid = new Grid();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        cardTexture = Content.Load<Texture2D>("Neutral Card Smol");
        spawnTexture = Content.Load<Texture2D>("uwu spawn");
        gridTexture = Content.Load<Texture2D>("gridtile");
        Slider.rail1 = Content.Load<Texture2D>("Rail1");
        Slider.slider1 = Content.Load<Texture2D>("Slider1");
        Slider.slider2 = Content.Load<Texture2D>("Slider2");

        button = new ModuleSpawnButton(spawnTexture, cardTexture, new Vector2(10,-10),_spriteBatch);

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
        GraphicsDevice.Clear(Color.DarkSeaGreen);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
   
        _spriteBatch.Draw(gridTexture, new Rectangle(0, 0, viewport.Width, menuBarHeight), Color.White);

        _spriteBatch.Draw(gridTexture, new Rectangle(0, ((viewport.Height - menuBarHeight)/3 + menuBarHeight - dividerHeight), viewport.Width, dividerHeight), Color.White);
        _spriteBatch.Draw(gridTexture, new Rectangle(0, (((viewport.Height - menuBarHeight) / 3)*2 + menuBarHeight - dividerHeight), viewport.Width, dividerHeight), Color.White);
        _spriteBatch.Draw(gridTexture, new Rectangle(0, viewport.Height - dividerHeight, viewport.Width, dividerHeight), Color.White);
        grid.Draw(_spriteBatch, gridTexture);
        button.Draw(_spriteBatch);
        EntityManager.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
