using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace SimGame.source
{
    class Bear : Animal<Creature>
    {
        public Bear()
        {
            var animres = (SpriteFrames)ResourceLoader.Load("res://data/scenes/Bear.tres");
            character.Frames = animres;
            Calories = 100.0f;
            StarvationLevel = 50.0f;
        }
    }
}
