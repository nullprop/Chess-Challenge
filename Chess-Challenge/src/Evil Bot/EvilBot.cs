/*
 * Stockfish UCI implementation
 * for evaluating changes to MyBot.
 */

namespace ChessChallenge.Example
{
    using System;
    using System.Diagnostics;
    using ChessChallenge.API;

    public class EvilBot : IChessBot
    {
        Process stockfish;

        public EvilBot()
        {
            stockfish = new();
            stockfish.StartInfo.RedirectStandardInput = true;
            stockfish.StartInfo.RedirectStandardOutput = true;
            stockfish.StartInfo.FileName = "/bin/stockfish";
            stockfish.Start();

            WriteLine("uci");
            if (!IsOk())
            {
                throw new Exception("Stockfish is not ok");
            }

            // WriteLine($"setoption name Skill Level value {3}");
            WriteLine("setoption name UCI_LimitStrength value true");
            WriteLine("setoption name UCI_ELO value 1650"); // min 1320 max 3190
            WriteLine("setoption name Threads value 16");
            WriteLine("setoption name Ponder value false");
            WriteLine("ucinewgame");
        }

        public Move Think(Board board, Timer timer)
        {
            WriteLine($"position fen {board.GetFenString()}");

            var ourTeam = board.IsWhiteToMove ? "w" : "b";
            var enemyTeam = board.IsWhiteToMove ? "b" : "w";
            WriteLine($"go {ourTeam}time {timer.MillisecondsRemaining} {enemyTeam}time {timer.OpponentMillisecondsRemaining}");

            Move? move = GetBestMove(board);
            if (move == null)
            {
                throw new Exception("Stockfish returned no move");
            }
            return (Move)move;
        }

        string? ReadLine()
        {
            var line = stockfish.StandardOutput.ReadLine();
            // if (line != null)
            // {
            //     Console.WriteLine($"stockfish: {line}");
            // }
            return line;
        }

        void WriteLine(string line)
        {
            stockfish.StandardInput.WriteLine(line);
        }

        private bool IsOk()
        {
            string? line;
            var ok = false;
            while ((line = ReadLine()) != null)
            {
                if (line == "uciok")
                {
                    ok = true;
                    break;
                }
            }
            return ok;
        }

        private Move? GetBestMove(Board board)
        {
            string? line;
            Move? move = null;
            while ((line = ReadLine()) != null)
            {
                if (line.StartsWith("bestmove"))
                {
                    move = new Move(line.Split()[1], board);
                    break;
                }
            }
            return move;
        }
    }
}