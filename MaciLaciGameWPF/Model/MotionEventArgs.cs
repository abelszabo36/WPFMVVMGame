using System;

namespace MaciLaciGame
{
    public class MotionEventArgs : EventArgs
    {
        #region Properties
        public int X { get; private set; }
        public int Y { get; private set; }
        #endregion

        public MotionEventArgs(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }


}
