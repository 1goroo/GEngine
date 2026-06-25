<img width="2800" height="1545" alt="Снимок экрана 2026-04-25 232617" src="https://github.com/user-attachments/assets/b5606c52-51f6-4a79-8561-a330acb58577" />
GEngine is game framevork for ASCII graphic text game.

>It is distributed under the MIT open source license.


> **Note:** This is an educational project created for fun purposes only. Do not expect any special performance or quality of execution from it.
## Features

* **Graphics**: Simple text rendering powered by `FontStashSharp`, special classes for drawing ASCII image.
* **Physics**: None. (The framework focuses purely on architecture/graphics).
* **Audio**: Basic, without MGCB (supported formats: `mp3`, `wav`, `ogg`)

## For what?
* This framevork intendet for:
* `->`Text game (like visual novells if you like ASCII)
* `->`Text RPG (object system allows made logic)
* `->`Other strange projects of yours!

## Used frameworks
* MonoGame
* FontStachSharp
* NLayer
* NVorbis

## How to start with GEngine?
Very simple!
create your first scene: 
```csharp
using GEngine.Framework;
using System;
public class MainMenuScene : Scene
{
  public override void OnEnter()
  {
    Console.WriteLine("Im Alive!");
  }
}
```
In Program.cs (From MonoGame Tamplate):
```csharp
using GEngine.Core;

using var game = new GEngineGame(new MainMenuScene(), "My Cool Game Name");
game.Run();
```

## Important classes and Namespaces
* GAPI (Namespace - GAPI) - Adds the ability to easily use the base template and core - display text and images, use resources, etc.
* Scene (Namespace - GEngine.Framework) - Having hook methods: OnUpdate, OnDraw, OnEnter, OnExit, OnLoad and methods 
for manipulating objects: Destroy, Find, Spawn and SpawnToGlobal.
* BaseObject (Namespace - GEngine.Framework) - Having hook methods: Update, Draw, Awake, Start, OnDestroy, mehods for manipulating objects and method Invoke for action in time.
* Input (Namespace - GEngine.Input) - Add methods for get keyboard and mouse actions by methods.
* ASCIIImage (Namespace - GEngine.Graphic) - Struct for drawing art by text.
* AudioSource (Namespace - GEngine.Audio) - Class for working with audio. 
* AudioManager (Namespace - GEngine.Audio) - Adds a method for working with audio: PlayOneShot
