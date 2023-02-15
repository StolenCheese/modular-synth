using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using modular_synth_frontend.UI;
using System.Diagnostics;

namespace modular_synth_frontend.Core;

public class Game1 : Game
{

    Texture2D cardTexture;
    Module cardModule;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private InputManager input;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        input = InputManager.GetInstance();
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
        cardModule = new Module(cardTexture);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        input.Update();
        cardModule.Update();
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkSeaGreen);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        cardModule.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
