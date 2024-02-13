shader_type canvas_item;

uniform sampler2D maskImage;
uniform sampler2D backgroundImage;										// Mask Image

void fragment() {
	vec4 maskImageColor = texture(maskImage, UV);
	vec4 bgColor = texture(backgroundImage, UV);
	// Caluclate the brightness of the pixel in the mask image
	float maskImageBrightness = (maskImageColor.r + maskImageColor.g + maskImageColor.b) / 3.0;
	bgColor.a = maskImageBrightness;
	// Show only the white area on the target texture
	//COLOR.a = maskImageBrightness;
}