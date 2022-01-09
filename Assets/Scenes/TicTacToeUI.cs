using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TicTacToeUI : MonoBehaviour
{
    public GameObject[] Squares;
    public int Player = 1;
    public int[] Board = new int[9];
    public GameObject CrossSprite, CircleSprite, Canvas;

    internal const int Cross = 1;
    internal const int Circle = 2;
    internal const int None = 0;

    internal void Awake()
    {
        //MakeMove();
    }

    internal void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //Get Mouse Pos
            Vector3 pos = Input.mousePosition;

            //Define DIstance and index
            float distance = 696969f;
            int index = 0;

            //Loop through and check distance
            for (int i = 0; i < Squares.Length; i++)
            {
                Vector3 pos_of_square = Squares[i].transform.position;
                float dist = Vector3.Distance(pos_of_square, pos);
                if (dist < distance)
                {
                    index = i;
                    distance = dist;
                }
            }

            MakeMoveOnBoard(Player, index);

            //Check if we've won
            bool won = CheckForWin(Player);

            if (won){
                Debug.Log("Human Wins!");
            }

            MakeAIMove();
        }
    }

    internal bool CheckForWin(int player)
    {
        int i = player;
        //First check for Horizontal
        if (Board[0] == i && Board[1] == i && Board[2] == i) return true;
        if (Board[3] == i && Board[4] == i && Board[5] == i) return true;
        if (Board[6] == i && Board[7] == i && Board[8] == i) return true;

        //Then check for Vertical
        if (Board[0] == i && Board[3] == i && Board[6] == i) return true;
        if (Board[1] == i && Board[4] == i && Board[7] == i) return true;
        if (Board[2] == i && Board[5] == i && Board[8] == i) return true;

        //Then check for Diagonal
        if (Board[0] == i && Board[4] == i && Board[8] == i) return true;
        if (Board[2] == i && Board[4] == i && Board[6] == i) return true;

        return false;
    }

    public void MakeAIMove()
    {
        //Define Best Score
        float best = float.NegativeInfinity;

        //Define Best Move
        int best_move = 0;

        //Loop through
        for (int i = 0; i < 9; i++)
        {
            if (Board[i] == 0)
            {
                Board[i] = (Player == 1) ? 2 : 1;
                float score = MiniMax(0, false);
                Board[i] = 0;
                best = Mathf.Max(score, best);
                best_move = i;
            }
        }

        //Override Manually using Alghorithims if we're off
        print($"MiniMax predicts {best_move}");

        if(!CheckForWin((Player == 1) ? 2 : 1)){
            best_move = OverrideIfItsBad(best_move);
            print($"Revision predicts {best_move}");
        }

        //Make Move
        MakeMoveOnBoard(Player == 1 ? 2 : 1, best_move);
        if (CheckForWin(Player == 1 ? 2 : 1)) Debug.Log("AI wins!");
    }

    public float MiniMax(int depth, bool isMaximizer)
    {
        //MiniMax algorithm
        float bestScore = -Mathf.Infinity;
        if (CheckForWin(Player)){
            return -10;
        }
        if (CheckForWin(Player == 1 ? 2 : 1)){
            return 1;
        }
        if (Isfull()){
            return 0;
        }

        if (isMaximizer)
        {
            for (int i = 0; i < Board.Length; i++)
            {
                if (Board[i] == 0)
                {
                    Board[i] = Player == 1 ? 2 : 1;
                    float score = MiniMax(depth + 1, false);
                    Board[i] = 0;
                    bestScore = Mathf.Max(score, bestScore);
                }
            }
            return bestScore;
        }

        else if (!isMaximizer) 
        {
            for (int i = 0; i < Board.Length; i++)
            {
                if (Board[i] == 0)
                {
                    Board[i] = Player;
                    float score = MiniMax(depth + 1, true);
                    Board[i] = 0;
                    bestScore = Mathf.Min(score, bestScore);
                }
            }

            return bestScore;
        }

        return 0f;
    }

    public int OverrideIfItsBad(int predicted)
    {
        int refined_prediction = predicted;
        for (int i = 0; i < Board.Length; i++){
            if (Board[i] == 0)
            {
                int x = i;
                Board[x] = Player;
                if (CheckForWin(Player) == true){
                    return x;
                }

                Board[x] = 0;
            }
        }

        return refined_prediction;
    }

    public bool Isfull()
    {
        foreach (int item in Board)
        {
            if (item == 0)
                return false;
        }

        return true;
    }

    public void MakeMoveOnBoard(int player, int index)
    {
        if (Isfull()){
            Debug.Log("Welp. Its a Tie");
            return;
        }

        GameObject noice = (player == 1) ? Instantiate(CrossSprite, Squares[index].transform.position, Quaternion.identity, Canvas.transform) :
                Instantiate(CircleSprite, Squares[index].transform.position, Quaternion.identity, Canvas.transform);

        Board[index] = player;
    }
}
