# Image Viewer and Tonemapper

An image viewer for anyone related to computer graphics.

## Download

x64 Windows: [Download](https://github.com/kopaka1822/ImageViewer/blob/Release/Build/Texture%20Viewer.zip?raw=true)

## File Formats

Currently the following image formats can be imported:
* PNG, JPG, BMP
* HDR, PFM
* KTX, DDS

Exporting is supported for:
* PNG, JPG
* HDR

## View Modes
### Simple Images
View simple images with and without alpha channel. The status bar displays the current texture coordinates (cursor) along with the corresponding RGBA color values.

![alt text](https://github.com/kopaka1822/ImageViewer/blob/master/examples/transparent.png)

### Images with multiple mipmaps and faces

Select preferred mipmap level and layer (face) of DDS and KTX textures:

![alt text](https://github.com/kopaka1822/ImageViewer/blob/master/examples/layer_level_view.png)

View cubemaps in crossview:


![alt text](https://github.com/kopaka1822/ImageViewer/blob/master/examples/cross_view.png)

View and navigate through cubemaps in a projection view:

![alt text](https://github.com/kopaka1822/ImageViewer/blob/master/examples/cube_view.png)

### Polar Images

View the raw polar image:

![alt text](https://github.com/kopaka1822/ImageViewer/blob/master/examples/balcony_raw.png)

View and navigate through the polar image in a projection view:

![alt text](https://github.com/kopaka1822/ImageViewer/blob/master/examples/balcony_polar.png)

## Image Manipulation

Add a custom or predefined tonemapper to your image:

![alt text](https://github.com/kopaka1822/ImageViewer/blob/master/examples/balcony_tonemapper.png)

Or define a custom tonemapper like this. Tonemapper are GLSL compute shader. The work group size will be set by the application and only the main method needs to be implemented (this method will be called once for every pixel).

```glsl
// general information about the shader
#setting title, Gamma Correction
#setting description, Nonlinear operation used to encode and decode luminance or tristimulus values in video or still image systems. Formula: (Factor * V) ^ Gamma.

// define variables for the user to interact with
layout(location = 3) uniform float gamma;
layout(location = 2) uniform float factor;

// define displayed name, location (see above), variable type, default value and optional minimum, maximum
#param Gamma, 3, float, 0.45454545, 0
#param Factor, 2, float, 1.0, 0

void main()
{
  // retrieve the pixel coordiante
  ivec2 pixelCoord = ivec2(gl_GlobalInvocationID.xy) + pixelOffset;
  
  // get the current pixel color
  vec4 color = texelFetch(src_image, pixelCoord, 0);
  // calculate the new color
  vec3 newColor = pow(factor * color.rgb, vec3(gamma));
  // save the new color in the destination image
  imageStore(dst_image, pixelCoord, vec4(newColor, color.a));
}
```

Compare pictures side by side. In this example the image before (right) and after (left) tonemapping:

![alt text](https://github.com/kopaka1822/ImageViewer/blob/master/examples/leanna_compare.png)

You can even import more than one image and combine them into one with a user defined function:

![alt text](https://github.com/kopaka1822/ImageViewer/blob/master/examples/image_formula.png)

I0 and I1 are the pixels from the first and the second image. RGB values are in range [0,1] and you can combine them with following operators: * + - / ^. Numerical constants can be used as well.
The following function from GLSL can be used as well:
* abs(value)
* sin(value)
* cos(value)
* tan(value)
* asin(value)
* acos(value)
* atan(value)
* exp(value)
* log(value)
* exp2(value)
* log2(value)
* sqrt(value)
* sign(value)
* floor(value)
* ceil(value)
* fract(value)
* normalize(value)
* length(value)

Additionaly you can use:
* red(value)
* green(value)
* blue(value)
* alpha(value)

to extract a single color channel as value.

`rgb(value, value, value)` may be used to construct an rgb value width alpha component 1.0