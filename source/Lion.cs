using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimGame.source
{
    class Lion : Animal<AnimalBase>
    {
        public Lion()
        {
            var animres = (SpriteFrames)ResourceLoader.Load("res://data/scenes/Lion.tres");
            character.Frames = animres;
        }
    }
}
