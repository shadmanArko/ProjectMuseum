shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform vec4 light_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
    // Sample the light texture (gradient texture of the PointLight2D)
    vec4 light_texture = texture(SCREEN_TEXTURE, UV);
    
    // Calculate the light intensity
    float light_intensity = light_texture.r;
    
    // Output the inverse of the light intensity
    // This will be used to cut out the ColorRect
    COLOR = vec4(light_color.rgb, 1.0 - light_intensity);
}