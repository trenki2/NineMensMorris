using NUnit.Framework;
using System.Linq;

namespace Tests
{
    public class TestMillGame
    {
        [Test]
        public void TestMillGameSimple1()
        {
            var game = new Game();
            game.Board[3] = 2;
            game.Board[4] = 1;
            game.Board[5] = 1;
            game.Board[6] = 2;
            //game.Board[7] = 2;
            game.Board[13] = 2;
            game.Board[14] = 2;
            game.Board[15] = 1;
            game.Board[16] = 2;
            game.Board[17] = 1;
            game.Board[18] = 2;
            game.Board[19] = 1;
            game.Board[20] = 1;
            game.Board[21] = 1;
            game.Board[22] = 2;
            game.Board[23] = 1;
            game.AvailableStones[0] = 9 - game.Board.Where(x => x == 1).Count();
            game.AvailableStones[1] = 9 - game.Board.Where(x => x == 2).Count();
            game.State = MillState.PlacingStones;
            game.Player = 2;

            var ai = new MillAI();
            var action = ai.CalculateAction(game);
            //Assert.IsFalse(action.IsMoveAction);
            //Assert.AreEqual(12, action.Pos0);
        }

        [Test]
        public void TestMillGameSimple2()
        {
            var game = new Game();
            game.Board[4] = 1;
            game.Board[5] = 1;
            game.Board[3] = 2;
            game.AvailableStones[0] = 9 - game.Board.Where(x => x == 1).Count();
            game.AvailableStones[1] = 9 - game.Board.Where(x => x == 2).Count();
            game.State = MillState.PlacingStones;
            game.Player = 2;

            var ai = new MillAI();
            var action = ai.CalculateAction(game);

            Assert.IsFalse(action.IsMoveAction);
            Assert.AreEqual(6, action.Pos0);
        }

        [Test]
        public void TestMillGameSimple3()
        {
            var game = new Game();
            game.Board[2] = 2;
            game.Board[3] = 1;
            game.Board[4] = 2;
            game.Board[9] = 2;
            game.Board[20] = 1;
            game.Board[21] = 1;
            game.AvailableStones[0] = 0;
            game.AvailableStones[1] = 0;
            game.State = MillState.MovingStones;
            game.Player = 2;

            var ai = new MillAI();
            var action = ai.CalculateAction(game);

            Assert.IsTrue(action.IsMoveAction);
            Assert.AreEqual(22, action.Pos1);
        }
    }
}