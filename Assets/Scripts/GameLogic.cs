﻿using UnityEngine;
using System;

public enum State
{
    Empty, X, O
}

public class GameLogic
{
    static GameLogic instance = new GameLogic ();

    private State[,] gridStates = new State[3,3];
    private int round = 1;
    private bool isGameOver = false;
    private State winner = State.Empty;

    private GameLogic ()
    {
    }

    public static GameLogic GetInstance ()
    {
        return instance;
    }

    public void PlaceZeroOrCross(Vector2Int cell, bool isCross)
    {
        gridStates[cell.x, cell.y] = isCross ? State.X : State.O;
        round++;
        CheckIfGameOver ();
    }

    public State GetCellState(Vector2Int cell)
    {
        return gridStates[cell.x, cell.y];
    }


    public bool IsGameOver()
    {
        return isGameOver;
    }

    public State WhoWon()
    {
        Debug.Assert (isGameOver);
        return winner;
    }

    public void RestartGame()
    {
        isGameOver = false;
        round = 1;
        winner = State.Empty;
        gridStates = new State[3, 3];
    }

    private void CheckIfGameOver()
    {
        if (round > 9)
        {
            isGameOver = true;
            winner = State.Empty;
            return;
        }

        // horizontally and vertically
        for (int i = 0; i < 3 && winner == State.Empty; i++)
        {
            if (gridStates[i,0] != State.Empty && gridStates[i,0] == gridStates[i,1] && gridStates[i,1] == gridStates[i,2])
            {
                isGameOver = true;
                winner = gridStates[i, 0];
            }

            if (gridStates[0,i] != State.Empty && gridStates[0,i] == gridStates[1,i] && gridStates[1,i] == gridStates[2,i])
            {
                isGameOver = true;
                winner = gridStates[0,i];
            }
        }

        //diagonally
        if (gridStates[0, 0] != State.Empty && gridStates[0, 0] == gridStates[1, 1] && gridStates[1, 1] == gridStates[2, 2])
        {
            isGameOver = true;
            winner = gridStates[0, 0];
        }

        if (gridStates[0, 2] != State.Empty && gridStates[0, 2] == gridStates[1, 1] && gridStates[1, 1] == gridStates[2, 0])
        {
            isGameOver = true;
            winner = gridStates[0, 2];
        }
    }


    // methods implementing AI
    public Vector2Int findOptimalMove ()
    {
        State[,] tempGrid = gridStates;
        int bestValue = -1000;
        int rowValue = 0;
        int colValue = 0;

        // check the gridStates and find the best next move
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (tempGrid[i, j] == State.Empty)
                {
                    // we make a guess and AI decided to choose this 
                    tempGrid[i, j] = State.X; // make a move AI is only X???

                     // calculate the cost of this step using the minimax
                    int costMove = minimax(tempGrid, 0, false);

                    //remove this step
                    tempGrid[i, j] = State.Empty;

                    // if this is best, then update this new score
                    if (costMove > bestValue)
                    {
                        rowValue = i;
                        colValue = j;
                        bestValue = costMove;
                    }
                }
            }
        }

        return new Vector2Int (rowValue, colValue);
    }

    //minimax algorithm
    private int minimax (State[,] tempGrid,   //I am not sure what type tempGrid is
                   int depth, bool isMax)
    {
        int score = totalscorecheck(tempGrid);

        // maximizer won the game  
        if (score == 10)
            return score;

        // minimizer won the game   
        if (score == -10)
            return score;

        // no moves left and no winner 
        // ? how to check
        if(HasEmptyCell(tempGrid)) // maybe like this
            return 0;

        // maximizer's move 
        if (isMax)
        {
            int best = -1000;

            // check all cells
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (tempGrid[i, j] == State.Empty)
                    {
                        // guess the step
                        tempGrid[i, j] = State.X;

                        // minimax recursively to compute maximum value 
                        best = Math.Max (best, minimax (tempGrid, depth + 1, !isMax));

                        // back this step
                        tempGrid[i, j] = State.Empty;
                    }
                }
            }
            return best;
        }

        // minimizer's move 
        else
        {
            int best = 1000;

            // check the tempGrid
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (tempGrid[i, j] == State.Empty)
                    {
                        // Make a guess
                        tempGrid[i, j] = State.O;

                        // minimax recursively to compute minimum value 
                        best = Math.Min (best, minimax (tempGrid, depth + 1, !isMax));

                        // back this step
                        tempGrid[i, j] = State.Empty;
                    }
                }
            }
            return best;
        }
    }

    private int totalscorecheck (State[,] tempGrid)
    {
        // Rows - X or O win
        for (int row = 0; row < 3; row++)
        {
            if (tempGrid[row, 0] == tempGrid[row, 1] && tempGrid[row, 1] == tempGrid[row, 2])
            {
                if (tempGrid[row, 0] == State.X)
                    return +10;
                else if (tempGrid[row, 0] == State.O)
                    return -10;
            }
        }

        // Columns - X or O win
        for (int col = 0; col < 3; col++)
        {
            if (tempGrid[0, col] == tempGrid[1, col] && tempGrid[1, col] == tempGrid[2, col])
            {
                if (tempGrid[0, col] == State.X)
                    return +10;

                else if (tempGrid[0, col] == State.O)
                    return -10;
            }
        }

        // Diagonals - X or O win
        if (tempGrid[0, 0] == tempGrid[1, 1] && tempGrid[1, 1] == tempGrid[2, 2])
        {
            if (tempGrid[0, 0] == State.X)
                return +10;
            else if (tempGrid[0, 0] == State.O)
                return -10;
        }

        if (tempGrid[0, 2] == tempGrid[1, 1] && tempGrid[1, 1] == tempGrid[2, 0])
        {
            if (tempGrid[0, 2] == State.X)
                return +10;
            else if (tempGrid[0, 2] == State.O)
                return -10;
        }

        // Elif 
        return 0;
    }

    private bool HasEmptyCell (State[,] gridStates)
    {
        foreach (State cellState in gridStates)
        {
            if (cellState == State.Empty)
                return true;
        }

        return false;
    }
}



