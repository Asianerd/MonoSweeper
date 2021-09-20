using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

class Rendering
{
    public delegate void RenderingEvents(SpriteBatch spriteBatch);
    public static RenderingEvents drawEvent;

    public static void Draw(SpriteBatch _spriteBatch)
    {
        if(drawEvent != null)
        {
            drawEvent(_spriteBatch);
        }
    }
}
