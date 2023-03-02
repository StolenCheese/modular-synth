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
    private Menu menu;

    public const int menuBarHeight = 42;
    public const int dividerHeight = 9;
    public const int RAILNUM = 2;

    Texture2D gridTexture;
    Texture2D handleTexture;
    Texture2D boxTexture;

    public Texture2D slider;

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
        gridTexture = Content.Load<Texture2D>("gridtile");
        handleTexture = Content.Load<Texture2D>("handletemp");
        boxTexture = Content.Load<Texture2D>("menubox");

        Slider.rail1 = Content.Load<Texture2D>("Rail1");
        Slider.slider1 = Content.Load<Texture2D>("Slider1");
        Slider.slider2 = Content.Load<Texture2D>("Slider2");
        Dial.dial1 = Content.Load<Texture2D>("dial1");
        Dial.indicator1 = Content.Load<Texture2D>("indicator1");
        Port.port1 = Content.Load<Texture2D>("port1");

        menu = new Menu(boxTexture,handleTexture, new Vector2(viewport.Width / 2 - handleTexture.Width/2, 0));
        menu.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        input.Update();
        EntityManager.Update();
        menu.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gray);

        _spriteBatch.Begin();

        //Drawing Static UI
        /*
        for (int i = 1; i <= RAILNUM; i++)
        {
            _spriteBatch.Draw(gridTexture, new Rectangle(0, (((viewport.Height - menuBarHeight) / RAILNUM) * i + menuBarHeight - dividerHeight), viewport.Width, dividerHeight), Color.White);
        }
        */

        //Drawing Grid (Furthest Back Dynamic UI)
        grid.Draw(_spriteBatch, gridTexture);

        //Drawing all Entities: Modules -> Components -> Wires
        EntityManager.Draw(_spriteBatch);

        //Drawing the Menu (needs to cover Entities so it is drawn last)
        menu.Draw(_spriteBatch);

        //TODO: Maybe in Entity Manager? Add concept of active entity, i.e: one currently being dragged that is drawn over everything else

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
