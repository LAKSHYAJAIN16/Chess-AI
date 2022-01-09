using UnityEngine;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class ConnectFourAI : MonoBehaviour
{
    public int[][] Board = new int[8][];
    public GameObject RedOBJ, YellowOBJ;
    public Transform index;
    public int Depth = 3, MaxDepth = 10;

    public bool BabyMode = false;

    internal const int Red = 1;
    internal const int Yellow = 2;
    internal const int None = 0;
    internal const float offset = 37f;

    private void Awake()
    {
        for (int i = 0; i < Board.Length; i++) {
            Board[i] = new int[8];
        }
    }

    public int InduceGravity(int row)
    {
        int max = 7;
        for (int i = 7; i >= 0; i--)
        {
            int board_occupation = Board[row][i];
            if (board_occupation == 0){
                max = i;
            }
        }

        return max;
    }

    public void PlayRed(int row)
    {
        int spot = InduceGravity(row);
        Board[row][spot] = Red;
        PlayMoveOnBoard(Red, row, spot);
        PlayAI();
    }

    public async void PlayAI()
    {
        if (BabyMode){
            //Select Random One(Bcuz we're babies)
            int index = Random.Range(0, 7);
            index = OverrideIfBad(index);
            int after_grav = InduceGravity(index);
            Board[index][after_grav] = Yellow;
            PlayMoveOnBoard(Yellow, index, after_grav);
        }

        else if (!BabyMode)
        {
            Stopwatch watch = Stopwatch.StartNew();

            //Define Best Score
            float max_value = -69696969696969696969696969696969f;
            int max_index = 1;

            for (int row = 0; row < 7; row++){
                //Fill Spot
                int spot = InduceGravity(row);
                Board[row][spot] = Yellow;

                //MINIMAX
                float score = await MiniMax(Depth, MaxDepth, false, float.NegativeInfinity, float.PositiveInfinity);
                print($"Row {row} score = {score}");

                //Reset and apply
                Board[row][spot] = 0;
                if (score > max_value){
                    max_value = score;
                    max_index = row;
                }
            }

            //Check if we're wrong completely using this check
            max_index = OverrideIfBad(max_index);

            //Induce Gravity as Always
            int after_grav = InduceGravity(max_index);

            //Play
            Board[max_index][after_grav] = Yellow;
            PlayMoveOnBoard(Yellow, max_index, after_grav);

            //Stop Stopwatch
            watch.Stop();
            print($"Elapsed time = {watch.ElapsedTicks}");
        }
    }

    public void PlayMoveOnBoard(int player, int row, int spot)
    {
        GameObject piece = (player == Red) ? RedOBJ : YellowOBJ;
        Vector3 pos = new Vector3(row * offset, spot * offset, 0f) + index.position;
        GameObject n = Instantiate(piece, pos, Quaternion.identity, index);
    }

    public bool HasWon(int player)
    {
        //Check for horizontal
        for (int row = 0; row < 7; row++)
        {
            int ones_in_row = 0;
            if (Board[0][row] == player) ones_in_row++;
            if (Board[1][row] == player) ones_in_row++;
            if (Board[2][row] == player) ones_in_row++;
            if (Board[3][row] == player) ones_in_row++;
            if (Board[4][row] == player) ones_in_row++;
            if (Board[5][row] == player) ones_in_row++;
            if (Board[6][row] == player) ones_in_row++;
            if (Board[7][row] == player) ones_in_row++;

            if (ones_in_row >= 4) return true;
        }

        //Check for Vertical
        for (int row2 = 0; row2 < 7; row2++)
        {
            int ones_in_a_column = 0;
            int[] target = Board[row2];
            for (int i = 0; i < target.Length; i++){
                int item = target[i];
                if (item == player) ones_in_a_column++;
            }

            if (ones_in_a_column >= 4) return true;
        }

        return false;

    }

    public async Task<int> Evaluate()
    {
        //First Check for AI
        await Task.Delay(1);
        int max_AI = 0;
        int player = Yellow;
        for (int row = 0; row < 7; row++)
        {
            int ones_in_row = 0;
            if (Board[0][row] == player) ones_in_row++;
            if (Board[1][row] == player) ones_in_row++;
            if (Board[2][row] == player) ones_in_row++;
            if (Board[3][row] == player) ones_in_row++;
            if (Board[4][row] == player) ones_in_row++;
            if (Board[5][row] == player) ones_in_row++;
            if (Board[6][row] == player) ones_in_row++;
            if (Board[7][row] == player) ones_in_row++;

            max_AI = Mathf.Max(ones_in_row, max_AI * 10);
        }
        for (int row2 = 0; row2 < 7; row2++)
        {
            int ones_in_a_column = 0;
            int[] target = Board[row2];
            for (int i = 0; i < target.Length; i++)
            {
                int item = target[i];
                if (item == player) ones_in_a_column++;
            }

            max_AI = Mathf.Max(ones_in_a_column, max_AI * 10);
        }

        //Then For Player
        int max_player = 0;
        player = Red;
        for (int row = 0; row < 7; row++)
        {
            int ones_in_row = 0;
            if (Board[0][row] == player) ones_in_row++;
            if (Board[1][row] == player) ones_in_row++;
            if (Board[2][row] == player) ones_in_row++;
            if (Board[3][row] == player) ones_in_row++;
            if (Board[4][row] == player) ones_in_row++;
            if (Board[5][row] == player) ones_in_row++;
            if (Board[6][row] == player) ones_in_row++;
            if (Board[7][row] == player) ones_in_row++;

            max_player = Mathf.Max(ones_in_row, max_player * 10);
        }
        for (int row2 = 0; row2 < 7; row2++)
        {
            int ones_in_a_column = 0;
            int[] target = Board[row2];
            for (int i = 0; i < target.Length; i++)
            {
                int item = target[i];
                if (item == player) ones_in_a_column++;
            }

            max_player = Mathf.Max(ones_in_a_column, max_player * 10);
        }

        //print($"Evaluation {max_AI} : {max_player}");
        return max_AI - max_player;
    }

    public async Task<float> MiniMax(int depth, int max_depth, bool player, float alpha, float beta, int iters = 0, int iters_max = 4)
    {
        //MiniMax algorithm
        await Task.Delay(1);
        iters++;

        //Evaluation
        int ev = await Evaluate();
        if (Depth <= 0 || Depth >= max_depth || iters >= iters_max) {
            return alpha;
        }

        if (HasWon(Red)){
            return -10;
        }
        if (HasWon(Yellow)){
            return 10;
        }

        if (player)
        {
            float bestScore = ev;
            for (int row = 0; row < 7; row++)
            {
                int spot = InduceGravity(row);
                Board[row][spot] = Yellow;
                float score = await MiniMax(depth + 1, max_depth, false, alpha, beta, iters);
                Board[row][spot] = 0;
                bestScore = Mathf.Max(bestScore, score);
                alpha = Mathf.Max(alpha, score);

                if (beta <= alpha){
                    break;
                }
            }

            return bestScore;
        }

        else if (!player)
        {
            float bestScore = ev;
            for (int row = 0; row < 7; row++)
            {
                int spot = InduceGravity(row);
                Board[row][spot] = Red;
                float score = await MiniMax(depth - 1, max_depth, true, alpha, beta, iters);
                Board[row][spot] = 0;
                bestScore = Mathf.Min(score, bestScore);
                beta = Mathf.Min(beta, score);

                if (beta <= alpha){
                    break;
                }
            }

            return bestScore;
        }

        return 0f;
    }

    public int OverrideIfBad(int predicted)
    {
        int refined_prediction = predicted;
        for (int i = 0; i < Board.Length; i++)
        {
            int slot = InduceGravity(i);
            Board[i][slot] = Red;
            if (HasWon(Red)){
                refined_prediction = i;
            }

            Board[i][slot] = None;
        }

        return refined_prediction;
    }
}
