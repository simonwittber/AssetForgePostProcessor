# AssetForgePostProcessor
A Unity post processor for AssetForge models.

![Image of AssetForgeModels](https://github.com/simonwittber/AssetForgePostProcessor/blob/master/Screen%20Shot.jpg)

AssetForgePostProcess.cs will take an FBX file from AssetForge and optimise it for use in Unity.

For each child transform in the imported model, it will:
 - Change all negative and non-uniform scales to (1,1,1).
 - Change the rotation to Quaternion.identity.

It then collects all submeshes inside the model and builds a single mesh with a single material.
The vertex colors of the new mesh are assign a value for each unique material. This allows the
shader on the mesh to lookup color and orther material properties from a texture, thus preserving
different colors on on the model, and allowing different smooth/metal properties across the mesh.

The Palette ScriptableObject allows an albedo and property mesh to be easily baked into the 
required textures. See Scenes/TestScene for an example. There are some caveats with this approach,
notably that the texture mapping from AssetForge is not preserved, and must be replicated using 
the included StandardVertexColors shader.


```git submodule add https://github.com/simonwittber/AssetForgePostProcessor AssetForgePostProcessor```
