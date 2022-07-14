using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimGame.source
{
    class Goat : Animal<Vegetation>
    {
        public Goat()
        {
            var animres = (SpriteFrames)ResourceLoader.Load("res://data/scenes/Goat.tres");
            character.Frames = animres;
            Calories = 80.0f;
            StarvationLevel = 70.0f;
        }
    }
}
