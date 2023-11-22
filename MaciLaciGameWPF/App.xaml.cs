using MaciLaciGame;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MaciLaciGameWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Adattagok
        private MainWindow _mainwindow = null!;
        private GameViewModel gameViewModel = null!;
        private GameModel gameModel = null!;
        private DispatcherTimer _gameTimer = new DispatcherTimer();
        private bool PauseOn = false;
        #endregion
        #region StartMetods
        public App()
        {
            Startup += new StartupEventHandler(AppStart);
        }

        private void AppStart(object? sender, StartupEventArgs e)
        {
            _mainwindow = new MainWindow();
            InitializeGame(GameModel.Levels.Easy);
            _mainwindow.Show();
            _gameTimer.Start();
        }
        #endregion

        #region Timer
        private void Time_Tick(object? sender, EventArgs e)
        {
            ++gameViewModel.Timer; // minden ticknel no egyet a timer
            gameViewModel.ClearEnemy(); // letorli a regi ellensegeket
            gameModel.EnemyMove(); // lepteti egyet a modelbe
            gameViewModel.StepEnemy(); // visszarakja az uj pozicioba oket
        }

        #endregion
        #region InicializeGame
        private void SetUpGame(GameModel.Levels level) // Palya inicializalasa regi lap eltuntetese ej az uj elohozasa
        {
            MainWindow oldWindow = _mainwindow;
            _mainwindow = new MainWindow();
            InitializeGame(level);
            oldWindow.Close();
            _mainwindow.Show();
            _gameTimer.Start();
        }

        private void InitializeGame(GameModel.Levels level) // elemek inicializalasa eventhandlerek inicializalasa
        {
            gameModel = new GameModel();
            gameModel.NewGame(level);
            gameViewModel = new GameViewModel(gameModel, level);
            _mainwindow.DataContext = gameViewModel;
            _gameTimer.Stop();
            _gameTimer.Tick -= Time_Tick;
            PauseOn = false;
            _gameTimer.Tick += Time_Tick;
            _gameTimer.Interval = TimeSpan.FromSeconds(1);

            _mainwindow.KeyDown += KeyDown;

            gameModel.Motion += new EventHandler<MotionEventArgs>(HeroMotion);
            gameModel.Result += new EventHandler<ResultEventArgs>(GameOver);
            gameModel.Collect += new EventHandler<CollectedEventArgs>(Collect);

            gameViewModel.ExitEvent += new EventHandler(ExitGame);
            gameViewModel.EasyLevelEvent += new EventHandler(EasyLevel);
            gameViewModel.MediumLevelEvent += new EventHandler(MediumLevel);
            gameViewModel.HardLevelEvent += new EventHandler(HardLevel);
            gameViewModel.PauseEvent += new EventHandler(Pause);
        }

        #endregion
        #region ExitPauseGameOverMethod
        private void ExitGame(object? sender, EventArgs e)
        {
            PauseOn = true;
            _gameTimer.Stop();
            MessageBoxResult dialogResult = MessageBox.Show("Biztos ki szeretnél lépni?", "Figyelem", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                _mainwindow.Close();
            }
            else
            {
                PauseOn = false;
                _gameTimer.Start();
            }

        }

        private void Pause(object? sender, EventArgs e)
        {
            if (PauseOn == false)
            {
                _gameTimer.Stop();
                PauseOn = true;
            }
            else
            {
                _gameTimer.Start();
                PauseOn = false;
            }
        }


        private void GameOver(object? sender, ResultEventArgs e)
        {
            PauseOn = true;
            _gameTimer.Stop();
            if (e.Win)
            {
                MessageBox.Show("Gratulálok nyertél. " + gameViewModel.Timer.ToString() + " másodperc alatt teljesítetted a pályát.");
            }
            else if (e.Lose)
            {
                MessageBox.Show("Sajnálom vesztettél");
            }
        }

        #endregion
        #region HeroMoveCollectEvents
        private void HeroMotion(object? sender, MotionEventArgs e)
        {
            gameViewModel.ClearHero(); // hos regi poziciojanak torlese
            gameViewModel.HeroStep(e.Y, e.X); // hero leptetese
        }

        private void Collect(object? sender, CollectedEventArgs e)
        {
            gameViewModel.Baskett = gameModel.Collected;
        }
        #endregion

        #region LevelSelector
        private void EasyLevel(object? sender, EventArgs e)
        {
            SetUpGame(GameModel.Levels.Easy);
        }

        private void MediumLevel(object? sender, EventArgs e)
        {
            SetUpGame(GameModel.Levels.Medium);
        }

        private void HardLevel(object? sender, EventArgs e)
        {
            SetUpGame(GameModel.Levels.Hard);
        }
        #endregion

        #region KeyDownEvent
        private void KeyDown(object? sender, KeyEventArgs e)
        {


            if (e.Key == Key.W && PauseOn == false) // ha talal az adott lenyomott gomb es nincs pause
            {
                (int CurrentX, int CurrentY, bool Possible) = gameViewModel.getPlayerPosition(GameModel.Direction.Up); // lekerjuk a hos poziciojat
                if (Possible) // ha lehetseges (nem vagyunk palya szelen) akkor lepunk
                {
                    gameModel.HeroMove(CurrentY, CurrentX, 1, Key.W);
                }
            }
            else if (e.Key == Key.S && PauseOn == false)
            {
                (int CurrentX, int CurrentY, bool Possible) = gameViewModel.getPlayerPosition(GameModel.Direction.Down);
                if (Possible)
                {
                    gameModel.HeroMove(CurrentY, CurrentX, 1, Key.S);
                }
            }
            else if (e.Key == Key.A && PauseOn == false)
            {
                (int CurrentX, int CurrentY, bool Possible) = gameViewModel.getPlayerPosition(GameModel.Direction.Left);
                if (Possible)
                {
                    gameModel.HeroMove(CurrentY, CurrentX, 1, Key.A);
                }
            }
            else if (e.Key == Key.D && PauseOn == false)
            {
                (int CurrentX, int CurrentY, bool Possible) = gameViewModel.getPlayerPosition(GameModel.Direction.Right);
                if (Possible)
                {
                    gameModel.HeroMove(CurrentY, CurrentX, 1, Key.D);
                }
            }

        }

    }
    #endregion
}
