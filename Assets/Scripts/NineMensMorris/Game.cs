using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NineMensMorris
{
    public class Game
    {
        public int[] Board { get; } = new int[24];
        public int[] StonesOnBoard { get; } = new int[2] { 0, 0 };
        public int[] AvailableStones { get; } = new int[2] { 9, 9 };
        
        public Move LastMove { get; private set; }
        public Move[] AvailableMoves { get; private set; }

        public int CurrentPlayer { get; private set; } = 1;
        public int OtherPlayer => CurrentPlayer == 1 ? 2 : 1;

        public bool IsGameOver => AvailableMoves.Length == 0 || (AvailableStones.Max() == 0 && StonesOnBoard.Min() < 3);

        public int Winner
        {
            get
            {
                if (!IsGameOver)
                    return 0;

                if (AvailableMoves.Length == 0)
                    return OtherPlayer;

                if (AvailableStones[0] + StonesOnBoard[0] > AvailableStones[1] + StonesOnBoard[1])
                    return 1;
                else
                    return 2;
            }
        }

        private static List<Tuple<int, int, int>> mills = new List<Tuple<int, int, int>>();
        private static List<int>[] adjacent = new List<int>[24];

        static Game()
        {
            for (int i = 0; i < 3; i++)
            {
                int x = i * 8;
                mills.Add(Tuple.Create(x + 0, x + 1, x + 2));
                mills.Add(Tuple.Create(x + 2, x + 3, x + 4));
                mills.Add(Tuple.Create(x + 4, x + 5, x + 6));
                mills.Add(Tuple.Create(x + 6, x + 7, x + 0));
            }

            mills.Add(Tuple.Create(1, 9, 17));
            mills.Add(Tuple.Create(3, 11, 19));
            mills.Add(Tuple.Create(5, 13, 21));
            mills.Add(Tuple.Create(7, 15, 23));

            for (int i = 0; i < 24; i++)
                adjacent[i] = new List<int>();

            for (int i = 0; i < 8; i++)
            for (int j = 0; j < 3; j++)
            {
                int index = i + j * 8 + 1;
                if (index >= (j + 1) * 8)
                    index -= 8;
                adjacent[i + j * 8].Add(index);

                index = i + j * 8 - 1;
                if (index < j * 8)
                    index += 8;
                adjacent[i + j * 8].Add(index);
            }

            adjacent[1].Add(9);
            adjacent[3].Add(11);
            adjacent[5].Add(13);
            adjacent[7].Add(15);

            adjacent[9].Add(1);
            adjacent[9].Add(17);
            adjacent[11].Add(3);
            adjacent[11].Add(19);
            adjacent[13].Add(5);
            adjacent[13].Add(21);
            adjacent[15].Add(7);
            adjacent[15].Add(23);

            adjacent[17].Add(9);
            adjacent[19].Add(11);
            adjacent[21].Add(13);
            adjacent[23].Add(15);
        }

        public bool CanRemove(int boardPos)
        {
            return !HasMill(boardPos);
        }

        public Game()
        {
            AvailableMoves = CalculateAvailableMoves().ToArray();
        }

        private IEnumerable<Move> CalculateAvailableMoves()
        {
            for (int to = 0; to < Board.Length; to++)
            {
                if (Board[to] != 0)
                    continue;

                for (int from = -1; from < Board.Length; from++)
                {
                    if (from == -1)
                    {
                        if (AvailableStones[CurrentPlayer - 1] == 0)
                            continue;
                    }
                    else
                    {
                        if (AvailableStones[CurrentPlayer - 1] != 0)
                            continue;

                        if (Board[from] != CurrentPlayer)
                            continue;

                        if ((StonesOnBoard[CurrentPlayer - 1] + AvailableStones[CurrentPlayer - 1]) > 3 && !adjacent[from].Contains(to))
                            continue;
                    }

                    if (WillHaveMill(from, to))
                    {
                        bool canRemoveStone = false;

                        for (int remove = 0; remove < Board.Length; remove++)
                        {
                            if (Board[remove] != OtherPlayer)
                                continue;

                            if (HasMill(remove))
                                continue;

                            canRemoveStone = true;
                            yield return new Move { To = to, From = from, Remove = remove };
                        }

                        if (!canRemoveStone)
                        {
                            yield return new Move { To = to, From = from, Remove = -1 };
                        }
                    }
                    else
                    {
                        yield return new Move { To = to, From = from, Remove = -1 };
                    }
                }
            }
        }

        public IEnumerable<Game> NextGameStates
        {
            get
            {
                foreach (var move in AvailableMoves.ToList())
                {
                    var newGame = Clone();
                    newGame.Move(move);
                    yield return newGame;
                }
            }
        }

        public bool WillHaveMill(int from, int to)
        {
            System.Diagnostics.Debug.Assert(from == -1 || Board[from] == CurrentPlayer);
            System.Diagnostics.Debug.Assert(Board[to] == 0);

            if (from != -1)
                Board[from] = 0;
            Board[to] = CurrentPlayer;

            var result = HasMill(to);

            if (from != -1)
                Board[from] = CurrentPlayer;
            Board[to] = 0;

            return result;
        }

        public bool IsValidFromTo(int from, int to)
        {
            var move = new Move { From = from, To = to };

            if (move.From == move.To)
                return false;
            if (move.To < 0 || move.To >= Board.Length)
                return false;
            if (move.From < -1 || move.From >= Board.Length)
                return false;
            if (Board[move.To] != 0)
                return false;
            if (move.From == -1 && AvailableStones[CurrentPlayer - 1] == 0)
                return false;
            if (move.From != -1 && AvailableStones[CurrentPlayer - 1] != 0)
                return false;
            if (move.From != -1 && Board[move.From] != CurrentPlayer)
                return false;
            if (move.From != -1 && StonesOnBoard[CurrentPlayer - 1] > 3 && !adjacent[move.From].Contains(move.To))
                return false;

            return true;
        }

        public bool IsValid(Move move)
        {
            if (!IsValidFromTo(move.From, move.To))
                return false;

            if (move.Remove < -1 || move.Remove > Board.Length)
                return false;

            if (move.Remove != -1 && (Board[move.Remove] != OtherPlayer))
                return false;

            //if (WillHaveMill(move.From, move.To) && move.Remove == -1)
            //    return false;

            return true;
        }

        public void Move(Move move)
        {
            if (!IsValid(move))
                throw new InvalidOperationException(nameof(move));

            Board[move.To] = CurrentPlayer;

            if (move.From != -1)
            {
                Board[move.From] = 0;
            }
            else
            {
                AvailableStones[CurrentPlayer - 1]--;
                StonesOnBoard[CurrentPlayer - 1]++;
            }

            if (move.Remove != -1)
            {
                StonesOnBoard[Board[move.Remove] - 1]--;
                Board[move.Remove] = 0;
            }

            LastMove = move;
            CurrentPlayer = OtherPlayer;
            AvailableMoves = CalculateAvailableMoves().ToArray();
        }

        public bool HasMill(int pos)
        {
            var p = Board[pos];

            foreach (var m in mills)
            {
                if (pos == m.Item1 || pos == m.Item2 || pos == m.Item3)
                {
                    if (Board[m.Item1] == p && Board[m.Item2] == p && Board[m.Item3] == p)
                        return true;
                }
            }

            return false;
        }

        public Game Clone()
        {
            var clone = new Game();

            clone.CurrentPlayer = CurrentPlayer;
            clone.LastMove = LastMove;
            Array.Copy(Board, clone.Board, Board.Length);
            Array.Copy(AvailableStones, clone.AvailableStones, AvailableStones.Length);
            Array.Copy(StonesOnBoard, clone.StonesOnBoard, StonesOnBoard.Length);

            clone.AvailableMoves = new Move[AvailableMoves.Length];
            Array.Copy(AvailableMoves, clone.AvailableMoves, AvailableMoves.Length);

            return clone;
        }
    }
}