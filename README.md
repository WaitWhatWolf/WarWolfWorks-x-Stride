#Important Note
This library's developement has been abandoned until a more suitable version of stride is released.

# WarWolfWorks-x-Stride
 A Stride version of the WarWolfWorks library
 
 Note: It is currently under heavy developement, and not usable by any means (yet)

# Setting Up
To set-up the library, use the `WWWGame` class as the Game class;
It might look something like this:
```cs
using Stride.Engine;
using WarWolfWorks_x_Stride.Internal;

namespace TheGreatestJoJoGame
{
    class TheGreatestJoJoGameApp
    {
        static void Main(string[] args)
        {
            using (var game = new WWWGame())
            {
                game.Run();
            }
        }
    }
}
```

You can also use your own Game class, as long as it inherits from `WWWGame`.
