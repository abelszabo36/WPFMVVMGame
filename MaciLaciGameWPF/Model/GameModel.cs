using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Windows;
using System.Windows.Input;
namespace MaciLaciGame
{
    public class GameModel
    {
        #region Properties
        private int collected;
        private int needCollect;
        private int timer;
        private GameTable? table;
        private List<Elem>? enemy;
        private DataAccess? datacess;
        private const string levelUrl1 = @"TextFiles\level1.txt";
        private const string levelUrl2 = @"TextFiles\level2.txt";
        private const string levelUrl3 = @"TextFiles\level3.txt";

        #endregion

        #region Enums
        public enum Direction { Left, Right, Up, Down }
        public enum Levels { Easy, Medium, Hard };
        #endregion

        #region Events
        public event EventHandler<MotionEventArgs>? Motion; // a hos mozgasanak eventje
        public event EventHandler<CollectedEventArgs>? Collect; // a begyujtes eventje
        public event EventHandler<ResultEventArgs>? Result; // a nyeres/vesztes eventje
        #endregion

        #region Getters
        public int Timer { get { return timer; } }

        public int Collected { get { return collected; } }
        public GameTable Table
        {
            get
            {
                try
                {
                    if (table != null)
                    {
                        return table;
                    }
                    else
                    {
                        throw new ArgumentNullException();
                    }
                }
                catch (ArgumentNullException)
                {

                    return new GameTable(new int[,] { { 0 } });
                }
            }
        }
        public void TimeTick() { ++this.timer; }

        #endregion


        #region NewGame
        public void NewGame(Levels level)
        {
            switch (level) // szint beallitasa
            {
                case Levels.Easy:
                    datacess = new DataAccess(levelUrl1);
                    break;
                case Levels.Medium:
                    datacess = new DataAccess(levelUrl2);
                    break;
                case Levels.Hard:
                    datacess = new DataAccess(levelUrl3);
                    break;
                default:
                    break;
            }
            if (datacess == null)
            {
                throw new NullReferenceException();
            }
            int[,] matrix = datacess.ReadFile(); // palya beolvasasa
            timer = 0;
            collected = 0;
            needCollect = 0;
            int counter = 0;
            table = new(matrix);
            enemy = new List<Elem>();
            for (int i = 0; i < matrix.GetLength(0); i++) // ellensegek iranyanak megadasa es begyujtheto elemek megszamlalasa
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    switch (matrix[i, j])
                    {
                        case 2:
                            Elem enemyy = new(i, j);
                            ++counter;
                            if (counter % 2 == 0)
                            {
                                enemyy.SetDirection(Direction.Right);
                            }
                            else
                            {
                                enemyy.SetDirection(Direction.Up);
                            }
                            enemy.Add(enemyy);
                            break;
                        case 3:
                            ++needCollect;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion


        #region Movements
        public void HeroMove(int currentx, int currenty, int size, Key key) // a hos mozgasa
        {
            int y = currentx / size; // palyameret alapjan az aktualis pozicio kiszamitasa (ellentetesen mivel x y kordinatarendszer ellentetes a matrix beallitasaval)
            int x = currenty / size;
            if (table != null)
            {
                if (key == Key.A) // balra lepes
                {
                    if (ObstacleCheck(x, y - 1)) // megnezi hogy bal oldalt van e akadaly
                    {
                        BaskettChecker(x, y - 1); // megnezi hogy bal oldalt van e kosar
                        table?.SetField(1, x, y - 1); // arrebb lepteti a host
                        table?.SetField(0, x, y); // Az aktuális pozíciót töröljük
                        OnMoving(currentx - size, currenty); // uj poziciot allitunk be a mozgas esemeny argumentumaba
                    }
                }

                if (key == Key.D) // jobra lepes
                {
                    
                    if (ObstacleCheck(x, y + 1))
                    {
                        BaskettChecker(x, y + 1);
                        table?.SetField(1, x, y + 1);
                        table?.SetField(0, x, y);
                        OnMoving(currentx + size, currenty);
                    }
                }

                if (key == Key.S) // lefele lepes
                {
                    if (ObstacleCheck(x + 1, y))
                    {
                        BaskettChecker(x + 1, y);
                        table?.SetField(1, x + 1, y);
                        table?.SetField(0, x, y);
                        OnMoving(currentx, currenty + size);
                    }
                }

                if (key == Key.W) // fel lepes
                {
                    if (ObstacleCheck(x - 1, y))
                    {
                        BaskettChecker(x - 1, y);
                        table?.SetField(1, x - 1, y);
                        table?.SetField(0, x, y); // Az aktuális pozíciót töröljük
                        OnMoving(currentx, currenty - size);
                    }
                }
            }
        }

        public void EnemyMove() // az ellenseg mozgasa
        {

            if (enemy != null)
            {
                foreach (Elem elem in enemy) // vegigmegyunk az osszes ellensegen
                {
                    if (elem.Direction == Direction.Right)
                    {
                        if (elem.Y + 1 < table?.TableSize && ObstacleCheck(elem.X, elem.Y + 1)) // megnezi van e akadaly a lepesnel vagy a elertuk e a palya szelet
                        {
                            table?.SetField(0, elem.X, elem.Y); // toroljuk a regi poziciot a magtrixbol
                            elem.Set(elem.X, elem.Y + 1); // beallitsuk az ellensegnek az uj pozicojat
                            EnemyCheck(elem.X, elem.Y); // megnezzuk hogy kozelben van e a hos
                            table?.SetField(2, elem.X, elem.Y); // beallitjuk az uj poziciojat az ellensegnek a matrixba
                        }
                        else
                        {
                            // Ha akadály van az utban vagy elérte a pálya szélét,megfordul
                            elem.SetDirection(Direction.Left);
                        }
                    }
                    else if (elem.Direction == Direction.Left)
                    {
                        if (elem.Y - 1 >= 0 && ObstacleCheck(elem.X, elem.Y - 1))
                        {
                            table?.SetField(0, elem.X, elem.Y);
                            elem.Set(elem.X, elem.Y - 1);
                            EnemyCheck(elem.X, elem.Y);
                            table?.SetField(2, elem.X, elem.Y);
                        }
                        else
                        {
                            elem.SetDirection(Direction.Right);
                        }
                    }
                    else if (elem.Direction == Direction.Up)
                    {
                        if (elem.X - 1 >= 0 && ObstacleCheck(elem.X - 1, elem.Y))
                        {
                            table?.SetField(0, elem.X, elem.Y);
                            elem.Set(elem.X - 1, elem.Y);
                            EnemyCheck(elem.X, elem.Y);
                            table?.SetField(2, elem.X, elem.Y);
                        }
                        else
                        {
                            elem.SetDirection(Direction.Down);
                        }
                    }
                    else if (elem.Direction == Direction.Down)
                    {
                        if (elem.X + 1 < table?.TableSize && ObstacleCheck(elem.X + 1, elem.Y))
                        {
                            table?.SetField(0, elem.X, elem.Y);
                            elem.Set(elem.X + 1, elem.Y);
                            EnemyCheck(elem.X, elem.Y);
                            table?.SetField(2, elem.X, elem.Y);
                        }
                        else
                        {
                            elem.SetDirection(Direction.Up);
                        }
                    }
                }
            }


        }
        #endregion


        #region Triggers
        private void OnMoving(int x, int y)
        {
            Motion?.Invoke(this, new MotionEventArgs(x, y));
        }

        private void OnCollecting(int x, int y)
        {
            Collect?.Invoke(this, new CollectedEventArgs(x, y));
        }

        private void GetResult(bool win, bool lose)
        {
            Result?.Invoke(this, new ResultEventArgs(win, lose));
        }
        #endregion

        #region Checkers
        private void BaskettChecker(int x, int y) // kosar ellenorzes
        {
            if (table?.GetField(x, y) == 3) // ha a kapott pozicon kosar van
            {
                ++collected; // noveljuk a begyujtottek szamat
                OnCollecting(x, y); // uj erteket allitunk az gyujtes esemeny argumentumaba
                table?.SetField(0, x, y);
                if (collected == needCollect) // ha begyujtottuk az osszes kosarat akkor uj erteket allitunk be a jatek vegert felelos esemenyargumentumba
                {
                    GetResult(true, false);
                }
            }
        }

        private bool ObstacleCheck(int x, int y) // akadalyellenorzes
        {
            if (table?.GetField(x, y) == 4) // ha akadaly van a megadott pozicioba akkor letiltjuk a mozgast
            {
                return false;
            }

            return true;
        }

        private void EnemyCheck(int x, int y) // ellenseg ellenorzes
        {
            if (table != null)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int newX = x + i;
                        int newY = y + j;

                       // a megadott kordinatak helyessegenek ellenorzese
                        if (newX >= 0 && newX < table.TableSize && newY >= 0 && newY < table.TableSize)
                        {
                            if (table.GetField(newX, newY) == 1)  // a szomszedos mezoben van a hos akkor
                            {
                                GetResult(false, true); // veszett a jatekos
                                return;
                            }
                        }
                    }
                }
            }
        }




        #endregion


        #region Debug
        public void PrintMatrix()
        {
            for (int i = 0; i < table?.TableSize; i++)
            {
                for (int j = 0; j < table?.TableSize; j++)
                {
                    Debug.Write(table.GetField(i, j) + " ");
                }
                Debug.WriteLine("");
            }
        }

        #endregion


    }
}
