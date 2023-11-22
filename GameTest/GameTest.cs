using MaciLaciGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;

namespace GameTest
{
    [TestClass]
    public class GameTest
    {
        private GameTable? table;
        private GameModel? model;
        

        [TestMethod]
        public void TestGameTableSetUp1()
        {
            int[,] matrix = { { 1, 0, 0 }, { 2, 3, 3 }, { 4, 4, 4 } };
            table = new GameTable(matrix);
            int expectedValue = 1;
            int value = table.GetField(0, 0);
            Assert.AreEqual(expectedValue, value);

            table.SetField(0, 0, 0);
            int expectedValue2 = 0;
            Assert.AreEqual(expectedValue2, table.GetField(0, 0));
        }

        [TestMethod]                                                         // Visual
        public void HeroRightMove()                                         //1 0 0 4 4 4 0      
        {                                                                   //0 4 0 0 0 0 0
            model = new GameModel();                                        //0 4 0 0 0 0 0
            model.NewGame(GameModel.Levels.Easy);                           //0 4 0 0 3 4 4
            model.HeroMove(0, 0, 1, Key.D);           //0 0 0 0 0 0 0
            Assert.AreEqual(model.Table.GetField(0,1), 1);                  //0 0 2 0 4 0 0
            Assert.AreEqual(model.Table.GetField(0, 0), 0);                 //0 4 4 4 0 0 3
        }

        [TestMethod]
        public void HeroLeftMove()                                              
        {                                                                  
            model = new GameModel();                                        
            model.NewGame(GameModel.Levels.Easy);
            // y x  size
            model.HeroMove(1, 1, 1, Key.A);// balra lepesnel (1,1) pozicioban helyezkedunk el        
            Assert.AreEqual(model.Table.GetField(1, 0), 1);      // megtortenik az egyel balra lepes          
            Assert.AreEqual(model.Table.GetField(1, 1), 0);      // a regi hely felszabadul          
        }

        [TestMethod]
        public void HeroMoveUp()
        {
            model = new GameModel();
            model.NewGame(GameModel.Levels.Easy);
            model.HeroMove(2, 3, 1, Key.W); // a harmadik sorba es a masodik oszlopba helyezkedunk el, 
            Assert.AreEqual(model.Table.GetField(2, 2), 1);           // egyel fel lepesnel a masodik sorba es a masodik oszlopba leszzunk
            Assert.AreEqual(model.Table.GetField(3, 2), 0);
        }

        [TestMethod]
        public void HeroMoveDown()
        {
            model = new GameModel();
            model.NewGame(GameModel.Levels.Easy);
            model.HeroMove(2, 2, 1, Key.S); // az masodik sorba es masodik oszlopba helyezkedunk el, 
            Assert.AreEqual(model.Table.GetField(3,2), 1);           // egyel le lepesnel a harmadik sorba es a masodik oszlopba leszzunk
            Assert.AreEqual(model.Table.GetField(2, 2), 0);
        }

        [TestMethod]
        public void HeroMeetWall()
        {
            model = new GameModel();
            model.NewGame(GameModel.Levels.Easy);
            model.HeroMove(0, 0, 1, Key.S);
            Assert.AreEqual(model.Table.GetField(1, 0), 1);
            model.HeroMove(0, 1, 1, Key.D);
            Assert.AreEqual(model.Table.GetField(1, 0), 1);
            Assert.AreEqual(model.Table.GetField(1,1),4);
        }


        [TestMethod]
        public void CollectBaskett()
        {
            model = new GameModel();
            model.NewGame(GameModel.Levels.Easy);
            Assert.AreEqual(model.Table.GetField(3,4), 3); // a (3,4) mezobe kosar van
            Assert.AreEqual(model.Collected,0); // kezdetben nincs begyujtott kosarunk
            model.HeroMove(3, 3, 1, Key.D); // a hos a kosar jobb oldalan helyezkedik el es ha egyet jobra lep akkor begyujti
            Assert.AreEqual(model.Collected, 1); 
        }

        [TestMethod]
        public void EnemyMoveTest()
        {
            model = new GameModel();
            model.NewGame(GameModel.Levels.Easy);// ez az enemy fel le mozog
            for (int i = 5; i >= 0; --i) // elmegy az enemy a palya tetejebe
            {
                Assert.AreEqual(model.Table.GetField(i, 2), 2);
                model.EnemyMove();
            }

            for (int i = 0; i <= 5; ++i) // majd megfordul es jon vissza
            {
                Assert.AreEqual(model.Table.GetField(i, 2), 2);
                model.EnemyMove();
            }
        }


        [TestMethod]
        public void TestFileReading() // filebeolvasas tesztelese
        {
            string path = @"Textfiles/level1.txt";
            DataAccess data = new DataAccess(path);
            int[,] matrix = data.ReadFile();

            Assert.AreEqual(7, matrix.GetLength(0));
            Assert.AreEqual(4, matrix[0, 3]);
            Assert.AreEqual(2, matrix[5, 2]);
            Assert.AreEqual(3, matrix[3, 4]);
            
        }
    } }