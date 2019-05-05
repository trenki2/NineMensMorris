using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public enum MillState
{
    PlacingStones,
    MovingStones,
    TakingStones,
    GameOver
}

[Serializable]
public class MillAction
{
    public int Pos0 { get; }
    public int Pos1 { get; }

    public bool IsMoveAction => Pos1 != -1;

    public MillAction(int pos0, int pos1 = -1)
    {
        Pos0 = pos0;
        Pos1 = pos1;
    }
}

[Serializable]
public class MillGame
{
    public bool IsGameOver => State == MillState.GameOver;
    public int Winner { get; private set; }

    public int[] Board { get; } = new int[24];
    public int[] AvailableStones { get; } = new int[2] { 9, 9 };

    public int Player { get; set; } = 1;
    public int OtherPlayer => Player == 1 ? 2 : 1;

    public MillAction LastAction { get; private set; }
    public MillState State { get; set; } = MillState.PlacingStones;
    private MillState lastState;

    private static List<Tuple<int, int, int>> mills = new List<Tuple<int, int, int>>();
    private static List<int>[] adjacent = new List<int>[24];

    static MillGame()
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

    public IEnumerable<MillAction> PossibleActions
    {
        get
        {
            switch (State)
            {
                case MillState.PlacingStones:
                    for (int i = 0; i < 24; i++)
                    {
                        if (Board[i] == 0)
                            yield return new MillAction(i);
                    }
                    break;

                case MillState.MovingStones:
                    var canJump = Board.Where(x => x == Player).Count() == 3;

                    for (int i = 0; i < 24; i++)
                    {
                        if (Board[i] == Player)
                        {
                            if (canJump)
                            {
                                for (int j = 0; j < 24; j++)
                                {
                                    if (Board[j] == 0)
                                        yield return new MillAction(i, j);
                                }
                            }
                            else
                            {
                                foreach (var j in adjacent[i])
                                {
                                    if (Board[j] == 0)
                                        yield return new MillAction(i, j);
                                }
                            }
                        }
                    }
                    break;

                case MillState.TakingStones:
                    for (int i = 0; i < 24; i++)
                    {
                        if (Board[i] == OtherPlayer && !CheckForMill(i))
                            yield return new MillAction(i);
                    }
                    break;
            }
        }
    }

    public IEnumerable<MillGame> Children
    {
        get
        {
            foreach (var action in PossibleActions)
            {
                var newGame = Clone();
                newGame.Execute(action);
                yield return newGame;
            }
        }
    }

    private bool CheckForMill(int pos)
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

    public bool CanExecute(MillAction action)
    {
        if (action.IsMoveAction)
        {
            var count = Board.Where(x => x == Player).Count();
            if (count > 3 && !adjacent[action.Pos0].Contains(action.Pos1))
                return false;
            else if (State != MillState.MovingStones)
                return false;
            else if (Board[action.Pos0] != Player)
                return false;
            else if (Board[action.Pos1] != 0)
                return false;
        }
        else if (State == MillState.PlacingStones)
        {
            if (Board[action.Pos0] != 0)
                return false;
        }
        else if (State == MillState.TakingStones)
        {
            if (Board[action.Pos0] != OtherPlayer || CheckForMill(action.Pos0))
                return false;
        }

        return true;
    }

    public void Execute(MillAction action)
    {
        if (!CanExecute(action))
            throw new ArgumentException("Invalid action");

        LastAction = action;

        if (action.IsMoveAction)
        {
            Board[action.Pos1] = Board[action.Pos0];
            Board[action.Pos0] = 0;

            if (CheckForMill(action.Pos1))
            {
                lastState = State;
                State = MillState.TakingStones;
            }
            else
            {
                Player = OtherPlayer;
            }
        }
        else if (State == MillState.PlacingStones)
        {
            Board[action.Pos0] = Player;
            AvailableStones[Player - 1]--;
            if (AvailableStones[0] == 0 && AvailableStones[1] == 0)
                State = MillState.MovingStones;

            if (CheckForMill(action.Pos0))
            {
                lastState = State;
                State = MillState.TakingStones;
            }
            else
            {
                Player = OtherPlayer;
            }
        }
        else if (State == MillState.TakingStones)
        {
            Board[action.Pos0] = 0;
            Player = OtherPlayer;
            State = lastState;

            if (AvailableStones[0] + Board.Where(x => x == 1).Count() < 3)
            {
                State = MillState.GameOver;
                Winner = 2;
            }
            else if (AvailableStones[1] + Board.Where(x => x == 2).Count() < 3)
            {
                State = MillState.GameOver;
                Winner = 1;
            }
        }

        if (PossibleActions.Count() == 0)
        {
            State = MillState.GameOver;
            Winner = OtherPlayer;
        }
    }

    public MillGame Clone()
    {
        var newNode = new MillGame();

        Array.Copy(Board, newNode.Board, Board.Length);
        Array.Copy(AvailableStones, newNode.AvailableStones, AvailableStones.Length);
        newNode.Winner = Winner;
        newNode.Player = Player;
        newNode.LastAction = LastAction;
        newNode.State = State;
        newNode.lastState = lastState;

        return newNode;
    }

    public float CalculateRating(int player)
    {
        var scores = new int[2];

        for (int i = 0; i < 2; i++)
        {
            scores[i] += AvailableStones[i] + Board.Where(x => x == i + 1).Count();
            scores[i] += IsGameOver && Winner == player ? 100 : 0;
        }

        return player == 2 ? scores[1] - scores[0] : scores[0] - scores[1];
    }

    public int CountMills(int player)
    {
        var count = 0;
        foreach (var m in mills)
        {
            if (Board[m.Item1] == player && Board[m.Item2] == player && Board[m.Item3] == player)
                count++;
        }
        return count;
    }
}