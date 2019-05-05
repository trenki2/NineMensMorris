using NUnit.Framework;
using System.Linq;

namespace Tests
{
    public class TestMillGame
    {
        [Test]
        public void TestMillGameSimple()
        {
            var game = new MillGame();
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
    }
}