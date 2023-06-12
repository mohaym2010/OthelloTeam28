using System.Collections.Generic;
using UnityEngine;

public class AIPlayer
{
    public Player Player_Type;
    private System.Random random;

    public AIPlayer(Player player)
    {
        this.Player_Type = player;
        random = new System.Random();
    }

    public Position Make_Move(GameState gameState)
    {
        List<Position> Available_Moves = new List<Position>(gameState.Available_Moves.Keys);
        int randomIndex = random.Next(Available_Moves.Count);
        Position selectedMove = Available_Moves[randomIndex];
        return selectedMove;
    }
}






