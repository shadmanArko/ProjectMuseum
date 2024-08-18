shader_type canvas_item;

uniform float width : hint_range(0.0, 1.0, 0.1);
uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);



uniform vec4 shine_color: source_color = vec4(1.0, 1.0, 1.0, 0.25);

uniform float shine_line_width: hint_range(0.0, 2.0, 0.01) = 0.1;
uniform float shine_angle: hint_range(0.0, 6.28318530718, 0.1308996939) = 0.785398163397;

uniform float shine_speed: hint_range(0.0, 10.0, 0.1) = 1.0;
uniform float shine_wait_cycles: hint_range(0.0, 10.0, 0.1) = 1.0;


vec2 rotate_precalculated(vec2 _pos, float _sine, float _cosine) {
	return vec2(_pos.x * _cosine + _pos.y * -_sine, _pos.x * _sine + _pos.y * _cosine);
}


void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment()
{
	vec2 size = vec2(width) / vec2(textureSize(TEXTURE, 0));

    vec4 sprite_color = texture(TEXTURE, UV);

    float alpha = sprite_color.a;
    alpha += texture(TEXTURE, UV + vec2(0.0, -size.y)).a;
    alpha += texture(TEXTURE, UV + vec2(size.x, -size.y)).a;
    alpha += texture(TEXTURE, UV + vec2(size.x, 0.0)).a;
    alpha += texture(TEXTURE, UV + vec2(size.x, size.y)).a;
    alpha += texture(TEXTURE, UV + vec2(0.0, size.y)).a;
    alpha += texture(TEXTURE, UV + vec2(-size.x, size.y)).a;
    alpha += texture(TEXTURE, UV + vec2(-size.x, 0.0)).a;
    alpha += texture(TEXTURE, UV + vec2(-size.x, -size.y)).a;

    vec3 final_color = mix(outline_color.rgb, sprite_color.rgb, sprite_color.a);
    COLOR = vec4(final_color, clamp(alpha, 0.0, 1.0));

	float sine = sin(shine_angle);
	float cosine = cos(shine_angle);
	float len = 1.5 - max(abs(sine), abs(cosine)) + shine_line_width;
	float line = smoothstep(-shine_line_width, shine_line_width,
			rotate_precalculated((UV - vec2(0.5)), sine, cosine).y - mod(TIME * shine_speed, (len * 2.0) * shine_wait_cycles) + len);
	COLOR.rgb += shine_color.rgb * shine_color.a * vec3(line * (1.0 - line) * 4.0);

}


	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
