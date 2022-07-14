using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimGame.source
{
    class Vegetation : Creature
    {
        public Vegetation()
        {
            var animres = (SpriteFrames)ResourceLoader.Load("res://data/scenes/Bush.tres");
            character.Frames = animres;
        }

        public override void Destroy()
        {
            base.Destroy();

            Game.GetSingleton().SetVegeAt(ChessLocation, false);
        }

        public override void Tick()
        {
            if (random.Randi() % Consts.GetSingleton().VegeGrowthProb == 0)
            {
                var game = Game.GetSingleton();
                Vector2[] dirs = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
                int i = random.RandiRange(0, 3);
                var pos = ChessLocation + dirs[i];
                if (game.IsValidPosition(pos) && !game.IsVegeAt(pos))
                {
                    var creature = new Vegetation();
                    creature.ChessLocation = pos;
                    game.SetVegeAt(pos, true);
                    game.Creatures.Add(creature);
                }
            }
        }
    }
}
