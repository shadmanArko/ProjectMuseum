shader_type canvas_item;

uniform vec4 blend_color : source_color;
uniform bool blend = true;

vec3 rgb_to_hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv_to_rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);

    // Convert the base texture color to HSV
    vec3 base_hsv = rgb_to_hsv(tex_color.rgb);
    // Convert the blend color to HSV
    vec3 blend_hsv = rgb_to_hsv(blend_color.rgb);

    // Preserve the luminance from the base color, and use hue and saturation from the blend color
    vec3 final_hsv = vec3(blend_hsv.x, base_hsv.y*1.035, base_hsv.z*1.2);
    vec3 final_rgb = hsv_to_rgb(final_hsv);

    // Blend the result with the original texture color
    vec4 result_color = mix(tex_color, vec4(final_rgb, tex_color.a), float(blend));

    COLOR = result_color;
}
