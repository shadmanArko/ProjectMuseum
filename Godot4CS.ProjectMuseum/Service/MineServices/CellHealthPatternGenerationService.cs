using System;
using System.Collections.Generic;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Service.MineServices;

public class CellHealthPatternGenerationService
{
    private static readonly Random _random = new();
    public static void GeneratePatternsForMineCells(Mine mine)
    {
        var mineWidth = mine.GridWidth;
        var mineHeight = mine.GridLength;

        var chunkWidth = (mineWidth - 1) / 2;
        var chunkHeight = (mineHeight - 1) / 3;

        var noOfHorizontalChunks = 2;
        var noOfVerticalChunks = 3;

        var chunks = new List<MineChunk>();

        for (var i = 0; i < noOfHorizontalChunks; i++)
        {
            for (var j = 0; j < noOfVerticalChunks; j++)
            {
                var chunk = new MineChunk
                {
                    ChunkStartX = i * chunkWidth,
                    ChunkEndX = (i + 1) * chunkWidth - 1,
                    ChunkStartY = j * chunkHeight,
                    ChunkEndY = (j + 1) * chunkHeight - 1,
                    MinSequenceLength = 2,
                    MaxSequenceLength = 3,
                    MinHorizontalGap = 1,
                    MaxHorizontalGap = 2,
                    MinVerticalGap = 1,
                    MaxVerticalGap = 2,
                    MineWidth = mineWidth,
                    MineHeight = mineHeight
                };

                chunks.Add(chunk);
            }
        }

        if (chunks.Count <= 0)
        {
            GD.PrintErr("Fatal Error: No chunks in list");
            return;
        }

        foreach (var chunk in chunks)
        {
            var isSmallPattern = _random.Next(7, 99999) % 2 == 0;
            if(isSmallPattern) 
                CreateSmallBlockPattern(chunk, mine.Cells);
            else
                CreateLargeBlockPattern(chunk, mine.Cells);
        }
    }

    private static void CreateSmallBlockPattern(MineChunk chunk, List<Cell> cells)
    {
        var isHorizontal = _random.Next(7, 10000) % 2 == 0;

        chunk.MinSequenceLength = 2;
        chunk.MaxSequenceLength = 3;
        
        if(isHorizontal)
            CreateHorizontalPattern(chunk, cells);
        else
            CreateVerticalPattern(chunk, cells);
    }

    private static void CreateLargeBlockPattern(MineChunk chunk, List<Cell> cells)
    {
        var isHorizontal = _random.Next(7, 10000) % 2 == 0;

        chunk.MinSequenceLength = 3;
        chunk.MaxSequenceLength = 5;
        
        if(isHorizontal)
            CreateHorizontalPattern(chunk, cells);
        else
            CreateVerticalPattern(chunk, cells);
    }

    #region Patterns

    #region Small Patterns

    private static void CreateHorizontalPattern(MineChunk chunk, List<Cell> cells)
    {
        
        var hardCellHitPoint = 64;

        for (var y = chunk.ChunkStartY; y <= chunk.ChunkEndY; y+=2)
        {
            var sequenceLength = _random.Next(chunk.MinSequenceLength, chunk.MaxSequenceLength + 1);
            var horizontalGap = _random.Next(chunk.MinHorizontalGap, chunk.MaxHorizontalGap + 1);

            for (var x = chunk.ChunkStartX; x <= chunk.ChunkEndX; x++)
            {
                var cellIndex = y * chunk.MineWidth + x;
                if (cellIndex >= cells.Count) continue;

                var cell = cells[cellIndex];
                if (cell == null) continue;

                if (sequenceLength > 0)
                {
                    cell.HitPoint = hardCellHitPoint;
                    cell.MaxHitPoint = hardCellHitPoint;
                    sequenceLength--;
                    GD.Print($"Hard Cell: {cell.PositionX}, {cell.PositionY}");
                }
                else if (horizontalGap > 0)
                {
                    horizontalGap--;
                }
                else
                {
                    sequenceLength = _random.Next(chunk.MinSequenceLength, chunk.MaxSequenceLength + 1);
                    horizontalGap = _random.Next(chunk.MinHorizontalGap, chunk.MaxHorizontalGap + 1);
                }
            }
        }
    }

    private static void CreateVerticalPattern(MineChunk chunk, List<Cell> cells)
    {
        var rand = new Random();
        var hardCellHitPoint = 64;

        for (var x = chunk.ChunkStartX; x <= chunk.ChunkEndX; x+=2)
        {
            var sequenceLength = rand.Next(chunk.MinSequenceLength, chunk.MaxSequenceLength + 1);
            var verticalGap = rand.Next(chunk.MinVerticalGap, chunk.MaxVerticalGap + 1);

            for (var y = chunk.ChunkStartY; y <= chunk.ChunkEndY; y++)
            {
                var cellIndex = y * chunk.MineWidth + x;
                if (cellIndex >= cells.Count) continue;

                var cell = cells[cellIndex];
                if (cell == null) continue;

                if (sequenceLength > 0)
                {
                    cell.HitPoint = hardCellHitPoint;
                    cell.MaxHitPoint = hardCellHitPoint;
                    sequenceLength--;
                    GD.Print($"Hard Cell: {cell.PositionX}, {cell.PositionY}");
                }
                else if (verticalGap > 0)
                {
                    verticalGap--;
                }
                else
                {
                    sequenceLength = rand.Next(chunk.MinSequenceLength, chunk.MaxSequenceLength + 1);
                    verticalGap = rand.Next(chunk.MinVerticalGap, chunk.MaxVerticalGap + 1);
                }
            }
        }
    }

    private static void CreateDiagonalPattern(MineChunk chunk, List<Cell> cells, bool isLeftToRight = true)
    {
        var rand = new Random();
        var hardCellHitPoint = 64;

        int diagonalSpacing = rand.Next(chunk.MinVerticalGap, chunk.MaxVerticalGap + 1);
        int currentSpacing = 0;

        for (int y = chunk.ChunkStartY; y <= chunk.ChunkEndY; y++)
        {
            if (currentSpacing == 0)
            {
                // Start a new diagonal line
                int sequenceLength = rand.Next(chunk.MinSequenceLength, chunk.MaxSequenceLength + 1);
                for (int i = 0; i < sequenceLength; i++)
                {
                    int x = isLeftToRight ? chunk.ChunkStartX + i : chunk.ChunkEndX - i;

                    int currentY = y + i;

                    if (x >= chunk.ChunkStartX && x <= chunk.ChunkEndX &&
                        currentY >= chunk.ChunkStartY && currentY <= chunk.ChunkEndY)
                    {
                        int cellIndex = currentY * chunk.MineWidth + x;
                        if (cellIndex >= 0 && cellIndex < cells.Count)
                        {
                            Cell cell = cells[cellIndex];
                            if (cell != null)
                            {
                                cell.HitPoint = hardCellHitPoint;
                                cell.MaxHitPoint = hardCellHitPoint;
                                GD.Print($"Hard Cell: {cell.PositionX}, {cell.PositionY}");
                            }
                        }
                    }
                }

                // Reset spacing for next diagonal
                currentSpacing = diagonalSpacing;
                diagonalSpacing = rand.Next(chunk.MinVerticalGap, chunk.MaxVerticalGap + 1);
            }
            else
            {
                currentSpacing--;
            }
        }
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