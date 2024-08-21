using System;
using System.Collections.Generic;
using Godot4CS.ProjectMuseum.Scripts.Mine;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MineServices;

public class CellHealthPatternGenerationService
{
    public void GeneratePatternsForMineCells(MineGenerationVariables mineGenerationVariables)
    {
        var mine = mineGenerationVariables.Mine;
        var mineWidth = mine.GridWidth;
        var mineHeight = mine.GridLength;
        
        var chunkWidth = (mineWidth - 1) / 2;
        var chunkHeight = (mineHeight - 1) / 3;

        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                
            }
        }
        
        
    }

    private void CreateSmallBlockPattern()
    {
        
    }

    private void CreateLargeBlockPattern()
    {
        
    }

    #region Patterns

    #region Small Patterns

    private void CreateHorizontalPattern(MineChunk chunk, List<Cell> cells)
    {
        var rand = new Random();

        var sequenceLength = rand.Next(chunk.MinSequenceLength, chunk.MaxSequenceLength);
        var horizontalGap = rand.Next(chunk.MinHorizontalGap, chunk.MaxHorizontalGap);
        var verticalGap = rand.Next(chunk.MinVerticalGap, chunk.MaxVerticalGap);
        
        
        for (var j = chunk.ChunkStartY; j < chunk.ChunkEndY; j++)
        {
            if (verticalGap > 0)
            {
                verticalGap--;
                continue;
            }
            
            for (var i = chunk.ChunkStartX; i < chunk.ChunkEndX; i++)
            {
                var cell = cells[chunk.MineWidth * j + i];
                if(cell == null) continue;
                
                if (sequenceLength > 0)
                {
                    cell.HitPoint = 64;
                    sequenceLength--;
                }
                else
                {
                    if (horizontalGap > 0)
                        horizontalGap--;
                    else
                    {
                        sequenceLength = rand.Next(chunk.MinSequenceLength, chunk.MaxSequenceLength);
                        horizontalGap = rand.Next(chunk.MinHorizontalGap, chunk.MaxHorizontalGap);
                    }
                }
            }
            
            verticalGap = rand.Next(chunk.MinVerticalGap, chunk.MaxVerticalGap);
        }
    }

    #endregion

    #region Large Patterns

    private void CreateHorizontalLargePattern()
    {
        
    }

    #endregion

    #region SeparatePatterns

    private void CreateFourCornersPattern()
    {
        
    }

    #endregion

    #endregion
}

public struct MineChunk
{
    public int ChunkStartX { get; set; }
    public int ChunkEndX { get; set; }
    
    public int ChunkStartY { get; set; }
    public int ChunkEndY { get; set; }

    public int MinSequenceLength { get; set; }
    public int MaxSequenceLength { get; set; }
    
    public int MinHorizontalGap { get; set; }
    public int MaxHorizontalGap { get; set; }
    
    public int MinVerticalGap { get; set; }
    public int MaxVerticalGap { get; set; }
    
    public int MineWidth { get; set; }
    public int MineHeight { get; set; }
}