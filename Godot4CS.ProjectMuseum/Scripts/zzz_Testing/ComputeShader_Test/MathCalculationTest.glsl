#[compute]
#version 450

layout(set = 0, binding = 0, std430) restrict buffer AIAgentInputData {
	mat4 CameraToWorld;
	float CameraFOV;
	float CameraFarPlane;
	float CameraNearPlane;
}
ai_agent_input_data;


layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;		
void Main()
{
    
}