using System.Collections.Generic;

public class GameState
{

   //Number of columns and rows
   public const int Rows = 8;
   public const int Columns = 8;

   //Stores what is in each position on the board
   public Player[,] Board { get; private set; }

   // the Player is the key and value is the number of disks that player's color facing up
   public Dictionary<Player, int> Disk_Count {get; private set;}

   //Which player's turn is it
   public Player Current_Player {get; set;}
   

   //Which Moves the Current player can make
   public Dictionary<Position, List<Position>> Available_Moves {get; private set;}

   //Specifies whether the game is over or not
   public bool Game_Over {get; private set;}

   //Specifies which player is the winner
   public Player winner {get; private set;}


   public GameState()
   {
        Board = new Player[Rows, Columns];
        Board[3,3] = Player.White;
        Board[3,4] = Player.Black;
        Board[4,3] = Player.Black;
        Board[4,4] = Player.White;

        Disk_Count = new Dictionary<Player, int>(){

            {Player.Black, 2},
            {Player.White, 2},
            {Player.None, 0}

        };

        Current_Player = Player.Black;
        Available_Moves = Find_Available_Moves(Current_Player);
   }

   public bool Make_Move(Position pos, out MovementInfo moveinfo)
   {
        if(!Available_Moves.ContainsKey(pos))
        {
            moveinfo = null;
            return false;
        }

        Player Move_Player = Current_Player;
        List<Position> Out_Flanked = Available_Moves[pos];
        Board[pos.Row, pos.Col] = Move_Player;

        Flip_Disks(Out_Flanked);
        Update_Disk_Count(Move_Player, Out_Flanked.Count);
        Pass_Turn();

        moveinfo = new MovementInfo { Player = Move_Player, Position = pos, Out_Flanked = Out_Flanked};
        return true;
   }

   public IEnumerable<Position> Occupied_Positions(){

        for(int r = 0; r < Rows; r++) {

            for(int c = 0; c < Columns ; c++) {

                if(Board[r,c] != Player.None){

                    yield return new Position(r, c); 

                }
            }  
        }
   } 

   private void Flip_Disks (List<Position> positions){

        foreach (Position pos in positions)
        {
            Board[pos.Row, pos.Col] = Board[pos.Row, pos.Col].Opponent();
        }

   }

   private void Update_Disk_Count (Player Move_Player, int Out_FlankedCount) {

        Disk_Count[Move_Player] += Out_FlankedCount + 1;
        Disk_Count[Move_Player.Opponent()] -= Out_FlankedCount;
    
   }

   public void Change_Player () {

        Current_Player = Current_Player.Opponent();
        Available_Moves = Find_Available_Moves(Current_Player);

   }

   private Player Find_Winner () {

        if(Disk_Count[Player.Black] > Disk_Count[Player.White]){
            return Player.Black;
        }

        if(Disk_Count[Player.Black] < Disk_Count[Player.White]){
            return Player.White;
        }

        return Player.None;
    
   }

   private void Pass_Turn () {

        Change_Player();

        if(Available_Moves.Count > 0){
            return;
        }

        Change_Player();

        if(Available_Moves.Count == 0){
            Current_Player = Player.None;
            Game_Over = true;
            winner = Find_Winner();
        }
 
   }

    private bool Is_Inside_Board(int r, int c)
    {
        return r>= 0 && r < Rows && c>= 0 && c < Columns;
    }

   private List<Position> Out_Flanked_Directed(Position pos, Player plr, int rDelta, int cDelta) 
   {
        List<Position> Out_Flanked = new List<Position>();
        int r = pos.Row + rDelta;
        int c = pos.Col + cDelta;

        while (Is_Inside_Board(r,c) && Board[r, c] != Player.None){

            if (Board[r, c] == plr.Opponent())
            {
                Out_Flanked.Add(new Position(r, c));
                r += rDelta;
                c += cDelta;
            }
            else {return Out_Flanked;}
        }

        return new List<Position>();
   }

    private List<Position> Out_Flanked(Position pos, Player plr){

        List<Position> Out_Flanked = new List<Position>();

        for (int rDelta = -1; rDelta <= 1; rDelta++){

            for (int cDelta = -1; cDelta <= 1; cDelta++){

                if(rDelta == 0 && cDelta == 0)
                {
                    continue;
                }

                Out_Flanked.AddRange(Out_Flanked_Directed(pos, plr, rDelta, cDelta));

            }
        }
        return Out_Flanked;
   }

   private bool Is_Move_Legal(Player plr, Position pos, out List<Position> Out_Flanked)
   {

    if(Board[pos.Row, pos.Col] != Player.None){
        Out_Flanked = null;
        return false;
    }
    Out_Flanked = Out_Flanked(pos, plr);
    return Out_Flanked.Count > 0;
   }

   private Dictionary<Position, List<Position>> Find_Available_Moves(Player plr){

    Dictionary<Position, List<Position>> Available_Moves = new Dictionary<Position, List<Position>>();

    for(int r = 0; r < Rows; r++) {
        for(int c = 0; c < Columns; c++) {

            Position pos = new Position(r,c);

            if(Is_Move_Legal(plr, pos, out List<Position> Out_Flanked)) {
                Available_Moves[pos] = Out_Flanked;
            }    
        } 
    }

    return Available_Moves;
   }

    public GameState Clone()
    {
        GameState Copied_State = new GameState();

        // Copy the board
        Copied_State.Board = (Player[,])Board.Clone();

        // Copy the disk count
        Copied_State.Disk_Count = new Dictionary<Player, int>(Disk_Count);

        // Copy the current player
        Copied_State.Current_Player = Current_Player;

        // Copy the legal moves
        Copied_State.Available_Moves = new Dictionary<Position, List<Position>>();
        foreach (KeyValuePair<Position, List<Position>> kvp in Available_Moves)
        {
            Position key = kvp.Key;
            List<Position> value = kvp.Value;
            Copied_State.Available_Moves.Add(key, new List<Position>(value));
        }

        // Copy the game over and winner status
        Copied_State.Game_Over = Game_Over;
        Copied_State.winner = winner;

        return Copied_State;
    }

}
