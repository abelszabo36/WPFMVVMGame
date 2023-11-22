using System;

namespace MaciLaciGame
{
    public class CollectedEventArgs : EventArgs
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public CollectedEventArgs(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}