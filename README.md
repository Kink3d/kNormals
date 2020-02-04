# kNormals
### World space normals texture for Unity's Universal Render Pipeline.

![alt text](https://github.com/Kink3d/kNormals/wiki/Images/Home00.png?raw=true)
*An example of normals texture generated from kNormals using the [Boat Attack](https://github.com/Verasl/BoatAttack) demo.*

kNormals generates a world space normals texture for Unity's Universal Render Pipeline. It renders all opaque objects using a shader that outputs world space normals, writing them to a global texture shader variable. This texture can then be used for many screen space effects and on-GPU tools.

Refer to the [Wiki](https://github.com/Kink3d/kNormals/wiki/Home) for more information.

## Instructions
- Open your project manifest file (`MyProject/Packages/manifest.json`).
- Add `"com.kink3d.normals": "https://github.com/Kink3d/kNormals.git"` to the `dependencies` list.
- Open or focus on Unity Editor to resolve packages.

## Requirements
- Unity 2019.3.0f3 or higher.