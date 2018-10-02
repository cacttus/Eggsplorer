using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Core
{
    public abstract class Touchable
    {
        public TouchState TouchState { get; private set; } = TouchState.Up;
        public bool Visible { get; set; } = true;
        public Action<object> Click { get; set; } = null;
        public bool Update(Input inp)
        {
            if (Click != null)
            {
                if (this.Visible && inp.Global.TouchState != TouchState.Up && Globals.Pick(GetDest(), inp.LastTouch))
                {
                    //Visual state
                    if (inp.Global.TouchState == TouchState.Release && (TouchState == TouchState.Down || TouchState == TouchState.Press))
                    {
                        Click(this);
                        
                    }

                    TouchState = inp.Global.TouchState;
                }
                else
                {
                    TouchState = TouchState.Up;
                }

                return (TouchState == TouchState.Down || TouchState == TouchState.Press);
            }
            return false;
        }
        public abstract Rectangle GetDest();
    }


}
