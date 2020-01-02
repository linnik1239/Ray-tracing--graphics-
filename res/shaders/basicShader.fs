#version 330

in vec2 texCoord0;
in vec3 normal0;
in vec3 color0;
in vec3 position0;

uniform vec4 lightColor;
uniform sampler2D sampler;
uniform sampler1D sampler1;









void main()
{
    vec3 newColor = color0;

	newColor = vec3(1,0,0);
	//m = texture1D(sampler, texCoord0)* vec4(newColor,1.0);
	gl_FragColor = texture2D(sampler, texCoord0)* vec4(newColor,1.0); //you must have gl_FragColor

}
