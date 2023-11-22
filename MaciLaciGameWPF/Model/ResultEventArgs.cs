using System;

namespace MaciLaciGame
{
    public class ResultEventArgs : EventArgs
    {
       private readonly bool win;
       private readonly  bool lose;

        public bool Win { get { return win; } } 
        public bool Lose { get { return lose; } }

        public ResultEventArgs(bool win, bool lose)
        {
            this.win = win;
            this.lose = lose;
        }

    }
}