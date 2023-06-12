using System.Collections.Generic;
using UnityEngine;

public class MinMaxAIPlayer
{
    public Player Player_Type;
    private int Hard_Difficulty_Depth = 3;

    public MinMaxAIPlayer(Player player)
    {
        this.Player_Type = player;
    }

    public Position Make_Move(GameState gameState)
    {
        List<Position> Possible_Moves = new List<Position>(gameState.Available_Moves.Keys);
        int Highest_Score = int.MinValue;
        Position Highest_Scoring_Move = null;

        foreach (Position move in Possible_Moves)
        {

            GameState Game_State = gameState.Clone();
            Game_State.Make_Move(move, out MovementInfo moveInfo);

            int Current_Value = Minimax(Game_State, 0, int.MinValue, int.MaxValue, false);

            if (Current_Value > Highest_Score)
            {
                Highest_Score = Current_Value;
                Highest_Scoring_Move = move;
            }
        }

        return Highest_Scoring_Move;
    }

    private int Minimax(GameState gameState, int depth, int alpha, int beta, bool is_Maximizing_Player)
    {
        // Base case: evaluate the game state if maximum depth is reached or the game is over
        if (depth == Hard_Difficulty_Depth || gameState.Game_Over)
        {
            return Evaluate_Game_State(gameState);
        }

        // Recursive case
        List<Position> Possible_Moves = new List<Position>(gameState.Available_Moves.Keys);
        int Highest_Score;

        if (is_Maximizing_Player)
        {
            Highest_Score = int.MinValue;

            foreach (Position move in Possible_Moves)
            {
                GameState Game_State = gameState.Clone();
                Game_State.Make_Move(move, out MovementInfo moveInfo);

                int Current_Value = Minimax(Game_State, depth + 1, alpha, beta, false);
                Highest_Score = Mathf.Max(Highest_Score, Current_Value);
                alpha = Mathf.Max(alpha, Highest_Score);

            if (beta <= alpha){

                break; // Beta Cutoff
                
            }
            }
        }
        else
        {
            Highest_Score = int.MaxValue;

            foreach (Position move in Possible_Moves)
            {
                GameState Game_State = gameState.Clone();
                Game_State.Make_Move(move, out MovementInfo moveInfo);

                int Current_Value = Minimax(Game_State, depth + 1, alpha, beta, true);
                Highest_Score = Mathf.Min(Highest_Score, Current_Value);
                beta = Mathf.Min(beta, Highest_Score);

                if (beta <= alpha)
                    break; // Alpha cutoff
            }
        }

        return Highest_Score;
    }

    private int Evaluate_Game_State(GameState Game_State)
    {
        int Mobility_Value = Evaluate_Mobility(Game_State);
        int Coin_Parity_Value = Evaluate_Coin_Parity(Game_State);
        int Corners_Captured_Value = Evaluate_Corners_Captured(Game_State);
        int Stability_Value = Evaluate_Stability(Game_State);
        //int Pattern_Current_Value = Evaluate_Pattern(Game_State);

        // Weighted sum heuristic 
        int Total_Current_Value = 3*Mobility_Value + 2*Coin_Parity_Value + 10*Corners_Captured_Value + 10*Stability_Value; //+ 5*Pattern_Current_Value;
        return Total_Current_Value;
    }

    private int Evaluate_Mobility(GameState Game_State)
    {
        List<Position> Possible_Moves = new List<Position>(Game_State.Available_Moves.Keys);
        int Current_Player_Moves = Possible_Moves.Count;

        // Count the opponent's legal moves
        Player Opponent = GetOpponent(Game_State.Current_Player);
        Game_State.Change_Player();
        List<Position> Opponent_Available_Moves = new List<Position>(Game_State.Available_Moves.Keys);
        int Opponent_Moves = Opponent_Available_Moves.Count;
        Game_State.Change_Player();

        // Calculate the mobility Current_Value as the difference in the number of moves
        int Mobility_Value = Current_Player_Moves - Opponent_Moves;

        return Mobility_Value;
    }

    private int Evaluate_Coin_Parity(GameState Game_State)
    {
        int Current_Player_Disks = Game_State.Disk_Count[Game_State.Current_Player];
        int Opponent_Disks = Game_State.Disk_Count[GetOpponent(Game_State.Current_Player)];

        // Calculate the coin parity Current_Value as the difference in the number of disks
        int Coin_Parity_Value = Current_Player_Disks - Opponent_Disks;

        return Coin_Parity_Value;
    }

    private int Evaluate_Corners_Captured(GameState Game_State)
    {
        int Current_Player_Corners_Captured = Count_Corners_Captured(Game_State, Game_State.Current_Player);
        int Opponent_Corners_Captured = Count_Corners_Captured(Game_State, GetOpponent(Game_State.Current_Player));

        // Calculate the corners captured Current_Value as the difference in the number of corners captured
        int cornersCurrent_Value = Current_Player_Corners_Captured - Opponent_Corners_Captured;

        return cornersCurrent_Value;
    }

    private int Count_Corners_Captured(GameState Game_State, Player player)
    {
        int Corner_Captured = 0;

        if (Game_State.Board[0, 0] == player)
            Corner_Captured++;
        if (Game_State.Board[0, GameState.Columns - 1] == player)
            Corner_Captured++;
        if (Game_State.Board[GameState.Rows - 1, 0] == player)
            Corner_Captured++;
        if (Game_State.Board[GameState.Rows - 1, GameState.Columns - 1] == player)
            Corner_Captured++;

        return Corner_Captured;
    }

    private Player GetOpponent(Player player)
    {
        return player == Player.Black ? Player.White : Player.Black;
    }

    private int Evaluate_Stability(GameState gameState)
    {
        int Stability_Value = 0;

        // Define stable disk positions (corners and edges)
        List<Position> Stable_Positions = new List<Position>
        {
            new Position(0, 0), new Position(0, GameState.Columns - 1),
            new Position(GameState.Rows - 1, 0), new Position(GameState.Rows - 1, GameState.Columns - 1)
        };

        // Evaluate stability for each disk
        foreach (Position position in gameState.Occupied_Positions())
        {
            Player player = Game_State.Board[position.Row, position.Col];

            if (player == Game_State.Current_Player)
            {
                int stability = 1;

                // Check if the disk is on the edge
                if (position.Row == 0 || position.Row == GameState.Rows - 1 ||
                    position.Col == 0 || position.Col == GameState.Columns - 1)
                {
                    stability++;
                }

                // Check if the disk is in a stable position
                if (Stable_Positions.Contains(position))
                {
                    stability += 3;
                }

                Stability_Value += stability;
            }
        }

        return Stability_Value;
    }

    private int Evaluate_Pattern(GameState gameState)
    {
        int Pattern_Current_Value = 0;

        // Define patterns and their corresponding Current_Values
        Dictionary<string, int> patterns = new Dictionary<string, int>
        {
            { "X.X", 5 },   // Pattern with two AI player's disks and an empty space in between
            { "XX.", 10 },  // Pattern` with two AI player's disks and an empty space at the end
            { ".XX", 10 },  // Pattern with two AI player's disks and an empty space at the beginning
            { "XXX", 20 },  // Pattern with three AI player's disks in a row
            { "XXXX", 30 }, // Pattern with four AI player's disks in a row
        };

        // Adjust pattern Current_Values for capturing corners
        if (Game_State.Board[0, 0] == Game_State.Current_Player)
            Pattern_Current_Value += 100;  // Capture top-left corner
        if (Game_State.Board[0, GameState.Columns - 1] == Game_State.Current_Player)
            Pattern_Current_Value += 100;  // Capture top-right corner
        if (Game_State.Board[GameState.Rows - 1, 0] == Game_State.Current_Player)
            Pattern_Current_Value += 100;  // Capture bottom-left corner
        if (Game_State.Board[GameState.Rows - 1, GameState.Columns - 1] == Game_State.Current_Player)
            Pattern_Current_Value += 100;  // Capture bottom-right corner

        // Check horizontal patterns
        for (int row = 0; row < GameState.Rows; row++)
        {
            string rowString = "";
            for (int Column = 0; Column < GameState.Columns; Column++)
            {
                rowString += Game_State.Board[row, Column] == Game_State.Current_Player ? "X" : ".";
            }
            Pattern_Current_Value += GetPattern_Current_Value(rowString, patterns);
        }

        // Check vertical patterns
        for (int Column = 0; Column < GameState.Columns; Column++)
        {
            string Columnstring = "";
            for (int Row = 0; Row < GameState.Rows; Row++)
            {
                Columnstring += Game_State.Board[Row, Column] == Game_State.Current_Player ? "X" : ".";
            }
            Pattern_Current_Value += GetPattern_Current_Value(Columnstring, patterns);
        }

        // Check diagonal patterns (top-left to bottom-right)
        for (int Start_Row = 0; Start_Row < GameState.Rows; Start_Row++)
        {
            string diagonalString = "";
            int Row = Start_Row;
            int Column = 0;
            while (Row < GameState.Rows && Column < GameState.Columns)
            {
                diagonalString += Game_State.Board[Row, col] == Game_State.Current_Player ? "X" : ".";
                Row++;
                Column++;
            }
            Pattern_Current_Value += GetPattern_Current_Value(diagonalString, patterns);
        }

        // Check diagonal patterns (top-right to bottom-left)
        for (int Start_Row = 0; Start_Row < GameState.Rows; Start_Row++)
        {
            string diagonalString = "";
            int Row = Start_Row;
            int Column = GameState.Columns - 1;
            while (Row < GameState.Rows && Column >= 0)
            {
                diagonalString += Game_State.Board[Row, col] == Game_State.Current_Player ? "X" : ".";
                Row++;
                Column--;
            }
            Pattern_Current_Value += GetPattern_Current_Value(diagonalString, patterns);
        }

        return Pattern_Current_Value;
    }

    private int GetPattern_Current_Value(string pattern, Dictionary<string, int> patterns)
    {
        int Current_Value = 0;
        foreach (KeyValuePair<string, int> kvp in patterns)
        {
            if (pattern.Contains(kvp.Key))
            {
                Current_Value += kvp.Value;
            }
        }
        return Current_Value;
    }

}
