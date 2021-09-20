using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class Main : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    static Texture2D endOverlay;
    static SpriteFont menuFont;
    static SpriteFont subMenuFont;

    public delegate void MainEvents();
    public static MainEvents mainUpdate;
    public static State gameState;
    public static Rectangle screen = new Rectangle(0, 0, 768, 768);
    public float FPS;

    public static Point cursorPoint = new Point();
    public static MouseState mouseState;
    public static KeyboardState keyboardState;

    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = screen.Width;
        _graphics.PreferredBackBufferHeight = screen.Height;

        gameState = State.Playing;

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Tile.Initialize();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        endOverlay = Content.Load<Texture2D>("gameEndOverlay");
        menuFont = Content.Load<SpriteFont>("menuFont");
        subMenuFont = Content.Load<SpriteFont>("subMenuFont");

        Tile.LoadContent(new Dictionary<Tile.State, Texture2D>() {
            { Tile.State.Closed, Content.Load<Texture2D>("tileClosed") },
            { Tile.State.Opened, Content.Load<Texture2D>("tileOpened") },
            { Tile.State.Selected, Content.Load<Texture2D>("tileSelected") },
            { Tile.State.Flagged, Content.Load<Texture2D>("tileFlag") }
        },
        Content.Load<SpriteFont>("font"),
        Content.Load<Texture2D>("mine")
        );

        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        keyboardState = Keyboard.GetState();

        mouseState = Mouse.GetState();
        cursorPoint = mouseState.Position;

        Input.Update();

        if (gameState == State.Playing)
        {
            if (mainUpdate != null)
            {
                mainUpdate();
            }

            if(Tile.mineCount == Tile.closedLandCount)
            {
                gameState = State.Win;
            }
            if(Tile.revealedMines != 0)
            {
                gameState = State.Lose;
            }
        }
        else
        {
            if(Input.rKey.activated)
            {
                Tile.Initialize();
                gameState = State.Playing;
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        FPS = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        Rendering.Draw(_spriteBatch);

        if(gameState != State.Playing)
        {
            _spriteBatch.Draw(endOverlay, screen, Color.White);
            string endText = $"You {(gameState == State.Win ? "won" : "lost")}!";
            Vector2 dimension = menuFont.MeasureString(endText);
            _spriteBatch.DrawString(menuFont, endText, screen.Center.ToVector2() - (dimension / 2), Color.White);
            dimension = subMenuFont.MeasureString("Press 'R' to reset the field.");
            _spriteBatch.DrawString(subMenuFont, "Press 'R' to reset the field.", screen.Center.ToVector2() - (dimension / 2) + new Vector2(0, 100), Color.White);
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public enum State
    {
        Playing,
        Win,
        Lose
    }
}
