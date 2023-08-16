using EightPuzzle.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EightPuzzle.Components
{
    public partial class Solution : ComponentBase
    {
        [Parameter]
        public List<List<Tile>> InitialState { get; set; }
        [Parameter]
        public EventCallback<List<List<Tile>>> InitialStateChanged { get; set; }
        [Parameter]
        public List<List<Tile>> FinalState { get; set; }
        [Parameter]
        public EventCallback<List<List<Tile>>> FinalStateChanged { get; set; }
        private Func<List<List<Tile>>, List<List<Tile>>, int> Heuristic;
        private List<List<List<Tile>>> Trail;
        private string selectedAlgorithm;
        private int AcumulatedCost;
        private int solveStatus = 0;
        private Stopwatch elapsedTime = new Stopwatch();

        private async Task Solve()
        {
            if (!string.IsNullOrEmpty(selectedAlgorithm))
            {
                elapsedTime.Reset();
                solveStatus = -1;
                StateHasChanged();

                bool solved = false;

                switch (selectedAlgorithm)
                {
                    case "AStar":
                        elapsedTime.Start();
                        solved = await AStar();
                        break;
                    case "BranchAndBound":
                        elapsedTime.Start();
                        solved = await BranchAndBound();
                        break;
                }

                if (solved)
                {
                    elapsedTime.Stop();
                    solveStatus = 1;
                    StateHasChanged();
                }
            }
        }

        private async Task<bool> AStar()
        {
            await Task.Run(() =>
            {
                List<List<Tile>> state = CopyPuzzleTable(InitialState);
                List<List<Tile>> goal = FinalState;
                int currentRow, currentIndex, tempPos;
                int totalCost;
                PriorityQueue<List<List<Tile>>, int> possibilities = new PriorityQueue<List<List<Tile>>, int>();

                List<List<Tile>> proposedState = null;
                Trail = new List<List<List<Tile>>>();
                AcumulatedCost = 0;

                #region Setting up the Heuristic Function
                Heuristic = ((state, goal) =>
                {
                    int tilesOutOfPosition = 0;

                    for (int i = 0; i < state.Count; i++)
                    {
                        for (int j = 0; j < state.ElementAt(i).Count; j++)
                        {
                            if (state.ElementAt(i).ElementAt(j).Value != goal.ElementAt(i).ElementAt(j).Value)
                                tilesOutOfPosition++;
                        }
                    }

                    return tilesOutOfPosition;
                });
                #endregion

                while (!PuzzleTablesIsEquals(state, goal))
                {
                    currentRow = 0;
                    currentIndex = -1;

                    for (; currentIndex < 0 && currentRow < state.Count; currentRow++)
                    {
                        var row = state.ElementAt(currentRow);
                        currentIndex = row.FindIndex(t => t.Value == 0);
                    }
                    currentRow -= 1;

                    #region Generating Possibilites
                    if (currentIndex + 1 < state.ElementAt(currentRow).Count)
                    {
                        proposedState = CopyPuzzleTable(state);
                        tempPos = currentIndex + 1;

                        var tileZero = proposedState.ElementAt(currentRow).ElementAt(currentIndex);
                        var tileSwap = proposedState.ElementAt(currentRow).ElementAt(tempPos);

                        SwapTiles(tileZero, tileSwap);
                        if (!IsInTrail(proposedState))
                        {
                            totalCost = Heuristic(proposedState, goal) + AcumulatedCost;
                            possibilities.Enqueue(proposedState, totalCost);
                        }
                    }

                    if (currentIndex - 1 >= 0)
                    {
                        proposedState = CopyPuzzleTable(state);
                        tempPos = currentIndex - 1;

                        var tileZero = proposedState.ElementAt(currentRow).ElementAt(currentIndex);
                        var tileSwap = proposedState.ElementAt(currentRow).ElementAt(tempPos);

                        SwapTiles(tileZero, tileSwap);
                        if (!IsInTrail(proposedState))
                        {
                            totalCost = Heuristic(proposedState, goal) + AcumulatedCost;
                            possibilities.Enqueue(proposedState, totalCost);
                        }
                    }

                    if (currentRow + 1 < state.Count)
                    {
                        proposedState = CopyPuzzleTable(state);
                        tempPos = currentRow + 1;

                        var tileZero = proposedState.ElementAt(currentRow).ElementAt(currentIndex);
                        var tileSwap = proposedState.ElementAt(tempPos).ElementAt(currentIndex);

                        SwapTiles(tileZero, tileSwap);
                        if (!IsInTrail(proposedState))
                        {
                            totalCost = Heuristic(proposedState, goal) + AcumulatedCost;
                            possibilities.Enqueue(proposedState, totalCost);
                        }
                    }

                    if (currentRow - 1 >= 0)
                    {
                        proposedState = CopyPuzzleTable(state);
                        tempPos = currentRow - 1;

                        var tileZero = proposedState.ElementAt(currentRow).ElementAt(currentIndex);
                        var tileSwap = proposedState.ElementAt(tempPos).ElementAt(currentIndex);

                        SwapTiles(tileZero, tileSwap);
                        if (!IsInTrail(proposedState))
                        {
                            totalCost = Heuristic(proposedState, goal) + AcumulatedCost;
                            possibilities.Enqueue(proposedState, totalCost);
                        }
                    }
                    #endregion

                    #region Updating State
                    Trail.Add(state);
                    state = possibilities.Dequeue();
                    #endregion
                }

                Trail.Add(state);
            }).ConfigureAwait(false);

            return true;
        }

        private async Task<bool> BranchAndBound()
        {
            await Task.Run(() =>
            {
                List<List<Tile>> state = CopyPuzzleTable(InitialState);
                List<List<Tile>> goal = FinalState;
                int currentRow, currentIndex, tempPos;
                PriorityQueue<List<List<Tile>>, int> possibilities = new PriorityQueue<List<List<Tile>>, int>();

                List<List<Tile>> proposedState = null;
                Trail = new List<List<List<Tile>>>();
                AcumulatedCost = 0;

                while (!PuzzleTablesIsEquals(state, goal))
                {
                    currentRow = 0;
                    currentIndex = -1;

                    for (; currentIndex < 0 && currentRow < state.Count; currentRow++)
                    {
                        var row = state.ElementAt(currentRow);
                        currentIndex = row.FindIndex(t => t.Value == 0);
                    }
                    currentRow -= 1;

                    #region Generating Possibilites
                    if (currentIndex + 1 < state.ElementAt(currentRow).Count)
                    {
                        proposedState = CopyPuzzleTable(state);
                        tempPos = currentIndex + 1;

                        var tileZero = proposedState.ElementAt(currentRow).ElementAt(currentIndex);
                        var tileSwap = proposedState.ElementAt(currentRow).ElementAt(tempPos);

                        SwapTiles(tileZero, tileSwap);
                        if (!IsInTrail(proposedState))
                        {
                            possibilities.Enqueue(proposedState, AcumulatedCost+1);
                        }
                    }

                    if (currentIndex - 1 >= 0)
                    {
                        proposedState = CopyPuzzleTable(state);
                        tempPos = currentIndex - 1;

                        var tileZero = proposedState.ElementAt(currentRow).ElementAt(currentIndex);
                        var tileSwap = proposedState.ElementAt(currentRow).ElementAt(tempPos);

                        SwapTiles(tileZero, tileSwap);
                        if (!IsInTrail(proposedState))
                        {
                            possibilities.Enqueue(proposedState, AcumulatedCost);
                        }
                    }

                    if (currentRow + 1 < state.Count)
                    {
                        proposedState = CopyPuzzleTable(state);
                        tempPos = currentRow + 1;

                        var tileZero = proposedState.ElementAt(currentRow).ElementAt(currentIndex);
                        var tileSwap = proposedState.ElementAt(tempPos).ElementAt(currentIndex);

                        SwapTiles(tileZero, tileSwap);
                        if (!IsInTrail(proposedState))
                        {
                            possibilities.Enqueue(proposedState, AcumulatedCost);
                        }
                    }

                    if (currentRow - 1 >= 0)
                    {
                        proposedState = CopyPuzzleTable(state);
                        tempPos = currentRow - 1;

                        var tileZero = proposedState.ElementAt(currentRow).ElementAt(currentIndex);
                        var tileSwap = proposedState.ElementAt(tempPos).ElementAt(currentIndex);

                        SwapTiles(tileZero, tileSwap);
                        if (!IsInTrail(proposedState))
                        {
                            possibilities.Enqueue(proposedState, AcumulatedCost);
                        }
                    }
                    #endregion

                    #region Updating State
                    Trail.Add(state);
                    state = possibilities.Dequeue();
                    #endregion
                }

                Trail.Add(state);
            }).ConfigureAwait(false);

            return true;
        }

        private (int, int) GetPosition(int value, List<List<Tile>> state)
        {
            int x = 0;
            int y = 0;
            
            for (int i = 0; i < state.Count; i++)
            {
                for (int j = 0; j < state.ElementAt(i).Count; j++)
                {
                    if (state.ElementAt(i).ElementAt(j).Value == value)
                    {
                        x = i;
                        y = j;
                    }
                }
            }

            return (x, y);
        }

        private bool PuzzleTablesIsEquals(List<List<Tile>> state, List<List<Tile>> goal)
        {
            bool isEqual = true;
            for (int i = 0; i < state.Count && isEqual; i++)
            {
                for (int j = 0; j < state.ElementAt(i).Count; j++)
                {
                    if (state.ElementAt(i).ElementAt(j).Value != goal.ElementAt(i).ElementAt(j).Value)
                        isEqual = false;
                }
            }

            return isEqual;
        }

        private bool IsInTrail(List<List<Tile>> state)
        {
            foreach(var oldState in Trail)
            {
                if (PuzzleTablesIsEquals(state, oldState))
                    return true;
            }

            return false;
        }

        private List<List<Tile>> CopyPuzzleTable(List<List<Tile>> state)
        {
            List<List<Tile>> copy = new List<List<Tile>>();
            foreach (var row in state)
            {
                var newRow = new List<Tile>();
                foreach (var tile in row)
                {
                    newRow.Add(new Tile(tile.Value, tile.Color));
                }

                copy.Add(newRow);
            }

            return copy;
        }

        public void SwapTiles(Tile t1, Tile t2)
        {
            string t1Color;
            int t1Value;

            t1Color = t1.Color;
            t1Value = t1.Value;

            t1.Color = t2.Color;
            t1.Value = t2.Value;
            t2.Color = t1Color;
            t2.Value = t1Value;
        }
    }
}
