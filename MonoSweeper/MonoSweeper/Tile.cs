using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

class Tile
{
    public static List<Tile> tiles = new List<Tile>();

    public static Dictionary<State, Texture2D> sprites;
    public static Texture2D mineSprite;
    public static SpriteFont font;
    public static List<Color> numberColors = new List<Color>() { Color.White, Color.Blue, Color.Green, Color.Red, Color.DarkBlue, Color.DarkRed, Color.DarkCyan, Color.Black, Color.Gray };
    public static int scale = 48;
    public static Vector2 size = new Vector2(scale, scale);

    public static int mineCount { get { return tiles.Where(n => n.hasMine).Count(); } }
    public static int flagCount { get { return tiles.Where(n => n.state == State.Flagged).Count(); } }
    public static int closedLandCount { get { return tiles.Where(n => n.state != State.Opened).Count(); } }
    public static int revealedMines { get { return tiles.Where(n => n.hasMine && (n.state == State.Opened)).Count(); } }

    public Vector2 position;
    public Rectangle rect;
    public State state;
    public bool hasMine = false;
    public int surroundingMines = 0;

    public static void LoadContent(Dictionary<State, Texture2D> _sprites, SpriteFont _font, Texture2D _mineSprite)
    {
        sprites = _sprites;
        font = _font;
        mineSprite = _mineSprite;
    }

    public static void Initialize()
    {
        tiles = new List<Tile>();

        int amount = 16;
        Random _rand = new Random();
        for (int x = 0; x < amount; x++)
        {
            for (int y = 0; y < amount; y++)
            {
                tiles.Add(new Tile(new Vector2(x, y), _rand.Next(0, 8) == 0));
            }
        }
        float _minimumDistance = (float)Math.Sqrt(Math.Pow(1, 2) + Math.Pow(1, 2));
        foreach(Tile x in tiles)
        {
            x.surroundingMines = tiles.Where(n => n != x).Where(n => n.hasMine).Where(n => Vector2.Distance(x.position, n.position) <= _minimumDistance).Count();
        }
    }

    public Tile(Vector2 _position, bool _hasMine)
    {
        Rendering.drawEvent += Draw;
        Main.mainUpdate += Update;

        state = State.Closed;
        position = _position;
        hasMine = _hasMine;
        rect = new Rectangle((_position * scale).ToPoint(), size.ToPoint());
    }

    public void Update()
    {
        if (rect.Contains(Main.cursorPoint))
        {
            switch (state)
            {
                default:
                    break;
                case State.Closed: case State.Selected:
                    if (Input.leftMouse.activated)
                    {
                        if (hasMine)
                        {
                            state = State.Opened;
                        }
                        else
                        {
                            Reveal();
                        }
                    }
                    else if (Input.rightMouse.activated)
                    {
                        state = State.Flagged;
                    }
                    break;
                case State.Flagged:
                    if (Input.rightMouse.activated)
                    {
                        state = State.Closed;
                    }
                    break;
                case State.Opened:
                    if (Input.middleMouse.activated)
                    {
                        AutoReveal();
                    }
                    break;
            }
        }
    }

    public void Draw(SpriteBatch _spriteBatch)
    {
        Vector2 _renderedPosition = position * scale;
        _spriteBatch.Draw(sprites[state], _renderedPosition, null, Color.White, 0f, Vector2.Zero, scale / 8, SpriteEffects.None, 0f);
        if((state == State.Opened) && hasMine)
        {
            _spriteBatch.Draw(mineSprite, _renderedPosition, null, Color.White, 0f, Vector2.Zero, scale / 8, SpriteEffects.None, 0f);
        }
        if ((surroundingMines > 0) && (state == State.Opened) && !hasMine)
        {
            _spriteBatch.DrawString(font, surroundingMines.ToString(), _renderedPosition+(Vector2.One * 8), numberColors[surroundingMines]);
        }
    }

    public void Reveal()
    {
        // The most troublesome part of this project
        float _distance = (float)Math.Sqrt(2);
        if(surroundingMines != 0)
        {
            state = State.Opened;
            return;
        }
        foreach (Tile x in tiles.Where(n => Vector2.Distance(position, n.position) <= _distance).Where(n => !n.hasMine && (n.state == State.Closed)))
        {
            if(x.state == State.Closed)
            {
                x.state = State.Opened;
                if (x.surroundingMines == 0)
                {
                    x.Reveal();
                }
            }
        }
    }

    public void AutoReveal()
    {
        float _distance = (float)Math.Sqrt(2);
        int numberMarked = tiles.Where(n => Vector2.Distance(position, n.position) <= _distance).Where(n => n.state == State.Flagged).Count();
        if(numberMarked == surroundingMines)
        {
            foreach(Tile x in tiles.Where(n => Vector2.Distance(position, n.position) <= _distance))
            {
                if(x.state == State.Closed)
                {
                    x.state = State.Opened;
                    if(x.surroundingMines == 0)
                    {
                        x.Reveal();
                    }
                }
            }
        }
    }

    public enum State
    {
        Opened,
        Closed,
        Selected,
        Flagged
    }
}
