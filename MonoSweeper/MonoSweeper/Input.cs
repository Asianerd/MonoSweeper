using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;


class Input
{
    public static Button rightMouse = new Button();
    public static Button leftMouse = new Button();
    public static Button middleMouse = new Button();

    public static Button rKey = new Button();

    public class Button
    {
        bool wasPressed;
        bool isPressed;
        public bool activated { get { return ((!wasPressed) && isPressed); } }

        public void Update(bool pressed)
        {
            wasPressed = isPressed;
            isPressed = pressed;
        }
    }

    public static void Update()
    {
        rightMouse.Update(Main.mouseState.RightButton == ButtonState.Pressed);
        leftMouse.Update(Main.mouseState.LeftButton == ButtonState.Pressed);
        middleMouse.Update(Main.mouseState.MiddleButton == ButtonState.Pressed);

        rKey.Update(Main.keyboardState.IsKeyDown(Keys.R));
    }
}
