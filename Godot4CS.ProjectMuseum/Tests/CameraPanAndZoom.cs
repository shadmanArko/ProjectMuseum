// using Godot;
// using System;

// public partial class CameraPanAndZoom : Camera2D
// {
// 	// Zoom parameters
//     private const float MinZoom = 0.5f;
//     private const float MaxZoom = 2.0f;
//     private const float ZoomSpeed = 0.05f;

//     // Pan parameters
//     private const float PanSpeed = 200.0f;
//     private Vector2 panPosition;

//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
//         Input.SetMouseMode(Input.MouseMode.Captured); // Capture the mouse cursor
//     }

//     // Called every frame
//     public override void _Process(float delta)
//     {
//         // Zooming
//         float zoomLevel = Zoom;
//         zoomLevel += Input.GetMouseWheelDelta() * ZoomSpeed;
//         zoomLevel = Mathf.Clamp(zoomLevel, MinZoom, MaxZoom);
//         Zoom = zoomLevel;

//         // Panning
//         if (Input.IsMouseButtonPressed((int)ButtonList.Right))
//         {
//             var mouseDelta = -Input.GetMouseMotion() / Zoom;
//             panPosition += mouseDelta * PanSpeed * delta;
//         }

//         GlobalPosition = panPosition;
//     }
// }
