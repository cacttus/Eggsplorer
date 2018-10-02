using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Core
{
    public class ButtonBase
    {
        public TouchState TouchState { get; private set; } = TouchState.Up;
        public void Update(bool touched)
        {
            if (touched)
            {
                if (TouchState == TouchState.Up)
                {
                    TouchState = TouchState.Press;
                }
                else if (TouchState == TouchState.Press)
                {
                    TouchState = TouchState.Down;
                }
                else if (TouchState == TouchState.Down)
                {
                    TouchState = TouchState.Down;
                }
                else if (TouchState == TouchState.Release)
                {
                    TouchState = TouchState.Press;
                }
            }
            else
            {
                if (TouchState == TouchState.Up)
                {
                    TouchState = TouchState.Up;
                }
                else if (TouchState == TouchState.Press)
                {
                    TouchState = TouchState.Release;
                }
                else if (TouchState == TouchState.Down)
                {
                    TouchState = TouchState.Release;
                }
                else if (TouchState == TouchState.Release)
                {
                    TouchState = TouchState.Up;
                }
            }
        }
    }

    public class Input
    {
        public vec2 LastTouch { get; private set; } = new vec2(0, 0);
        public bool UIClicked { get; set; } = false;
        public ButtonBase Global { get; private set; } = new ButtonBase();

        public Input()
        {
        }
        public void Update()
        {
            KeyboardState state = Keyboard.GetState();

            // If they hit esc, exit


            //vec2 touchPos = new vec2(0, 0);
            bool touched = false;
            MouseState ms = Mouse.GetState();
            TouchCollection tc = TouchPanel.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                LastTouch = new vec2(ms.Position.X, ms.Position.Y);
                touched = true;
            }
            else if (tc.Count > 0)
            {
                LastTouch = new vec2(tc[0].Position);
                touched = true;
            }

            //Your basic button state
            Global.Update(touched);


        }
    }
}
