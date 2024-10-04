shader_type canvas_item;

uniform sampler2D light_texture : hint_screen_texture; // PointLight2D light texture
uniform vec4 light_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
    // Sample the light texture (gradient texture of the PointLight2D)
    vec4 light_sample = texture(light_texture, UV);
    
    // Calculate the light intensity based on the red channel (assuming light color intensity)
    float light_intensity = light_sample.r * light_color.a;
    
    // Set the color of the rect
    vec4 rect_color = vec4(1.0, 1.0, 1.0, 1.0); // Change to your desired color

    // Inverse mask the rect color based on light intensity
    rect_color.a *= (1.0 - light_intensity); // Make the color transparent based on light intensity

    // Output the final color
    COLOR = rect_color;
}