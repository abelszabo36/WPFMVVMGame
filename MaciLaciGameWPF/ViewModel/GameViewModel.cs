using MaciLaciGame;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MaciLaciGameWPF
{
    public class GameViewModel : ViewModelBase
    {
        #region Adattagok
        private GameModel _gameModel;
        private int _mapSize;
        private int _timer = 0;
        private int _baskett = 0;
        private const int _easyMapSize = 7;
        private const int _mediumMapSize = 10;
        private const int _hardMapSize = 11;
        #endregion
        #region DelegateCommandok
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand EasyLevelCommand { get; private set; }
        public DelegateCommand MediumLevelCommand { get; private set; }
        public DelegateCommand HardLevelCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        #endregion

        #region Events
        public event EventHandler? ExitEvent;
        public event EventHandler? EasyLevelEvent;
        public event EventHandler? MediumLevelEvent;
        public event EventHandler? HardLevelEvent;
        public event EventHandler? PauseEvent;
        #endregion

        #region Properties
        public int Size { get { return _mapSize; } set { _mapSize = value; } }
        public int Timer { get { return _timer; } set { _timer = value; OnpropertyChange(); } }

        public int Baskett { get { return _baskett; } set { _baskett = value; OnpropertyChange(); } }

        public ObservableCollection<FieldModel>? Fields { get; set; }
        #endregion

        #region Constructor
        public GameViewModel(GameModel gameModel, GameModel.Levels level)
        {
            switch (level) // megnezzuk melyik palyan szeretne jatszani es ahhoz allitjuk be a Size propertit ami az XAML be adja a grid meretet
            {
                case GameModel.Levels.Easy:
                    _mapSize = _easyMapSize;
                    break;
                case GameModel.Levels.Medium:
                    _mapSize = _mediumMapSize;
                    break;
                case GameModel.Levels.Hard:
                    _mapSize = _hardMapSize;
                    break;
                default:
                    break;
            }
            _gameModel = gameModel;
            ExitCommand = new DelegateCommand(param => OnExit()); // inicializaljuk a DelegateCommandokat
            EasyLevelCommand = new DelegateCommand(param => OnEasy());
            MediumLevelCommand = new DelegateCommand(param => OnMedium());
            HardLevelCommand = new DelegateCommand(param => OnHard());
            PauseCommand = new DelegateCommand(param => OnPause());

            Fields = new ObservableCollection<FieldModel>(); // inicializaljuk az observablecollectiont
            GenerateMap(); // legeneraljuk a mapot (akadalyokkal kosarakkal ellensegekkel)

        }
        #endregion

        #region Generator
        private void GenerateMap()
        {
            Fields?.Clear(); // kiuritjuk a Fieldset
            for (int i = 0; i < _mapSize; i++) // vegig megyunk a mapon es beallitjuk a matrix alapjan a megfelelo elemeket
            {
                for (int j = 0; j < _mapSize; j++)
                {


                    if (_gameModel.Table.GetField(i, j) == 1)
                    {
                        Fields?.Add(new FieldModel
                        {
                            X = i,
                            Y = j,
                            Color = Colors.Black,
                        });
                    }
                    else if (_gameModel.Table.GetField(i, j) == 2)
                    {
                        Fields?.Add(new FieldModel
                        {
                            X = i,
                            Y = j,
                            Color = Colors.Red,
                        });
                    }
                    else if (_gameModel.Table.GetField(i, j) == 3)
                    {
                        Fields?.Add(new FieldModel
                        {
                            X = i,
                            Y = j,
                            Color = Colors.Brown,
                        });
                    }
                    else if (_gameModel.Table.GetField(i, j) == 4)
                    {
                        Fields?.Add(new FieldModel
                        {
                            X = i,
                            Y = j,
                            Color = Colors.Gray,
                        });
                    }
                    else
                    {
                        Fields?.Add(new FieldModel
                        {
                            X = i,
                            Y = j,
                            Color = Colors.Green,
                        });
                    }

                }
            }
        }
        #endregion
        #region EnemyMethods
        public void ClearEnemy() // letoroljuk a regi enemyiket
        {
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    int i = field.X;
                    int j = field.Y;

                    if (_gameModel.Table.GetField(i, j) == 2)
                    {
                        field.Color = Colors.Green;
                    }
                }
            }
        }

        public void StepEnemy() // beallitjuk az uj enemyket
        {
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    int i = field.X;
                    int j = field.Y;

                    if (_gameModel.Table.GetField(i, j) == 2)
                    {
                        field.Color = Colors.Red;
                    }
                }
            }
        }
        #endregion
        #region HeroMetods
        public (int, int, bool) getPlayerPosition(GameModel.Direction direction)
        {

            for (int i = 0; i < _mapSize; i++)
            {
                for (int j = 0; j < _mapSize; j++)
                {
                    if (direction == GameModel.Direction.Left) // ha tud meg lepni az adott iranyba akkor megengedjuk neki
                    {
                        if (j > 0 && _gameModel.Table.GetField(i, j) == 1)
                        {
                            return (i, j, true);
                        }
                    }

                    if (direction == GameModel.Direction.Right)
                    {
                        if (j < _mapSize - 1 && _gameModel.Table.GetField(i, j) == 1)
                        {
                            return (i, j, true);
                        }
                    }

                    if (direction == GameModel.Direction.Up)
                    {
                        if (i > 0 && _gameModel.Table.GetField(i, j) == 1)
                        {
                            return (i, j, true);
                        }
                    }

                    if (direction == GameModel.Direction.Down)
                    {
                        if (i < _mapSize - 1 && _gameModel.Table.GetField(i, j) == 1)
                        {
                            return (i, j, true);
                        }
                    }

                }
            }

            return (0, 0, false);
        }

        public void ClearHero() // letroljuk a hero regi poziciojat
        {
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    int i = field.X;
                    int j = field.Y;

                    if (_gameModel.Table.GetField(i, j) == 0)
                    {
                        field.Color = Colors.Green;
                    }
                }
            }
        }

        public void HeroStep(int x, int y) // beallitjuk az ujat
        {
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    if (field.X == x && field.Y == y)
                    {
                        field.Color = Colors.Black;
                    }
                }
            }
        }
        #endregion

        #region EventTriggers
        private void OnExit()
        {
            ExitEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnEasy()
        {
            EasyLevelEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnMedium()
        {
            MediumLevelEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnHard()
        {
            HardLevelEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnPause()
        {
            PauseEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion

    }

}
