using EightPuzzle.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EightPuzzle.Pages
{
    public partial class Puzzle : ComponentBase
    {
        public bool VerifyPuzzleTable()
        {
            int inversions = 0;
            for(int i=0; i<InitialState.Count-1;i++)
            {
                for(int j=0; i<InitialState.ElementAt(i).Count; j++)
                {
                    if (InitialState.ElementAt(i).ElementAt(j).Value > 0 && InitialState.ElementAt(i).ElementAt(j).Value > InitialState.ElementAt(j).ElementAt(i).Value)
                        inversions++;
                }
            }

            return inversions % 2 == 0;
        }

        public List<List<Tile>> InitialState { get; set; } = new List<List<Tile>>
        {
            new List<Tile>
            {
                new Tile() { Value = 1, Color = "bisque" },
                new Tile() { Value = 2, Color = "aquamarine" },
                new Tile() { Value = 3, Color = "darksalmon" }
            },
            new List<Tile>
            {
                new Tile() { Value = 4, Color = "cadetblue" },
                new Tile() { Value = 5, Color = "gainsboro" },
                new Tile() { Value = 6, Color = "greenyellow" }
            },
            new List<Tile>
            {
                new Tile() { Value = 7, Color = "goldenrod" },
                new Tile() { Value = 8, Color = "palegreen" },
                new Tile() { Value = 0, Color = "deepskyblue" }
            }
        };

        public List<List<Tile>> FinalState { get; set; } = new List<List<Tile>>
        {
            new List<Tile>
            {
                new Tile() { Value = 1, Color = "bisque" },
                new Tile() { Value = 2, Color = "aquamarine" },
                new Tile() { Value = 3, Color = "darksalmon" }
            },
            new List<Tile>
            {
                new Tile() { Value = 4, Color = "cadetblue" },
                new Tile() { Value = 5, Color = "gainsboro" },
                new Tile() { Value = 6, Color = "greenyellow" }
            },
            new List<Tile>
            {
                new Tile() { Value = 7, Color = "goldenrod" },
                new Tile() { Value = 8, Color = "palegreen" },
                new Tile() { Value = 0, Color = "deepskyblue" }
            }
        };
    }
}
