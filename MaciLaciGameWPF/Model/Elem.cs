using System;

namespace MaciLaciGame
{
    public class Elem
    {
        private int x;
        private int y;
        private GameModel.Direction direction;

        public GameModel.Direction Direction { get { return this.direction; } }
        public Elem(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void SetDirection(GameModel.Direction direction)
        {
            this.direction = direction;
        }

        public int X { get { return x; } }
        public int Y { get { return y; } }
    }
}
