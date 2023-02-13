using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace modular_synth_frontend.Core;

public class Game1 : Game
{

    Texture2D cardTexture;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private InputManager input;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        input = new InputManager();
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
        cardTexture = Content.Load<Texture2D>("Card Design");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        input.Update();

        // TODO: Add your update logic here
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkSeaGreen);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        _spriteBatch.Draw(cardTexture, new Rectangle(0,0,200,300), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
