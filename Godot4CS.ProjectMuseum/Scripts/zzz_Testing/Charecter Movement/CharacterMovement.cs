using Godot;
using System;

public partial class CharacterMovement : Node2D
{
    private const int NumCharacters = 30000;
    private const float MoveSpeed = 300.0f;
    private const float MoveAreaSize = 16000.0f;

    private RandomNumberGenerator randomGenerator = new RandomNumberGenerator();
    private Vector2[] characterPositions = new Vector2[NumCharacters];
    private Vector2[] characterDestinations = new Vector2[NumCharacters];
    private bool[] isMovingToDestination = new bool[NumCharacters];

    public override void _Ready()
    {
        randomGenerator.Randomize(); // Seed the random number generator

        // Initialize character positions
        for (int i = 0; i < NumCharacters; i++)
        {
            characterPositions[i] = new Vector2(
                randomGenerator.RandfRange(0, MoveAreaSize),
                randomGenerator.RandfRange(0, MoveAreaSize)
            );
            
            characterDestinations[i] = GetRandomDestination(characterPositions[i]);

            isMovingToDestination[i] = true;

            // Create and add a character sprite to the scene
            var characterSprite = new Sprite2D();
            characterSprite.Texture = GD.Load<Texture2D>("res://Assets/2D/Sprites/icon.svg");
            characterSprite.Position = characterPositions[i];
            AddChild(characterSprite);
        }
    }

    public override void _Process(double delta)
    {
        for (int i = 0; i < NumCharacters; i++)
        {   
            if (isMovingToDestination[i])
            {
                Vector2 moveDirection = (characterDestinations[i] - characterPositions[i]).Normalized();
                characterPositions[i] += moveDirection * MoveSpeed * (float)delta;

                if (characterPositions[i].DistanceTo(characterDestinations[i]) < 2.0f)
                {
                    isMovingToDestination[i] = false;
                }
            }
            else
            {
                characterDestinations[i] = GetRandomDestination(characterPositions[i]);
                isMovingToDestination[i] = true;
            }
            // Update the character's position in the scene
            GetChild<Sprite2D>(i).Position = characterPositions[i];
        }
    }
    
    private Vector2 GetRandomDestination(Vector2 currentPosition)
    {
        return new Vector2(
            randomGenerator.RandfRange(0, MoveAreaSize),
            randomGenerator.RandfRange(0, MoveAreaSize)
        );
    }
}