using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace SimGame.source
{
    abstract class Animal<FoodType> : AnimalBase where FoodType : Creature
    {
        private Vector2 walkToNeighbour = new Vector2();
        private Vector2 walkToLocation = new Vector2();
        private Vector2 lastCellLocation = new Vector2();
        public float Satiety { get; set; } = 100.0f;
        public float Speed { get; set; } = 0.95f;
        public float StarvationLevel = Consts.GetSingleton().StarvationLevel;

        public enum EGender
        {
            Male,
            Female
        }

        public EGender Gender { get; set; } = EGender.Male;

        public bool IsInLoveMood()
        {
            return Satiety > Consts.GetSingleton().LoveLevel && ((int)Game.GetSingleton().CurrentTick - LastSexTime) > Consts.GetSingleton().SexBreak;
        }
        public bool IsHugry()
        {
            return Satiety < StarvationLevel;
        }

        public int LastSexTime { get; set; } = -999;

        public Animal()
        {
            Satiety = random.RandfRange(10.0f, 100.0f);
            satietyNode = character.GetNode<Polygon2D>("Satiety");
            DebugTools.Assert(satietyNode != null, "No satiety visual");
            satietyNode.Visible = true;
            Gender = (EGender)random.RandiRange(0, 1);
            var genderNode = character.GetNode<Sprite>("Gender");
            DebugTools.Assert(genderNode != null, "No gender visual");
            genderNode.Visible = true;
            genderNode.Texture = ResourceLoader.Load<Texture>(Gender == EGender.Male ? "res://data/sprites/characters/male.png" : "res://data/sprites/characters/female.png");
        }

        private void WalkToward(Vector2 dir)
        {
            DebugTools.Assert(dir.Length() <= 1.0f, "Too far");
            walkToNeighbour = ChessLocation + new Vector2(0.5f, 0.5f) + dir;
        }

        public void WalkTo(Vector2 position)
        {
            walkToLocation = position.Floor() + new Vector2(0.5f, 0.5f);
        }

        public override void Tick()
        {
            const float Hunger = 0.5f;
            Satiety -= Hunger;

            var game = Game.GetSingleton();

            if (Satiety <= 0.0f)
            {
                Destroy();
                return;
            }

            if (IsHugry())
            {
                Creature closestFood = null;
                foreach (var creature in Game.GetSingleton().Creatures)
                {
                    if (creature != this && creature.GetType() != GetType()) // TOO SLOW
                    {
                        var food = creature as FoodType;
                        if (food != null)
                        {
                            float dist = food.ChessLocation.DistanceSquaredTo(ChessLocation);
                            if (dist == 0.0f)
                            {
                                Satiety += food.Calories;
                                closestFood = null;
                                food.Destroy();
                                break;
                            }

                            if (closestFood == null)
                                closestFood = food;
                            else if (dist < closestFood.ChessLocation.DistanceSquaredTo(ChessLocation))
                                closestFood = food;
                        }
                    }
                }
                if (closestFood != null)
                    WalkTo(closestFood.ChessLocation);
            }
            else if (IsInLoveMood())
            {
                Animal<FoodType> closestSexPartner = null;
                foreach (var creature in Game.GetSingleton().Creatures)
                {
                    var animal = creature as Animal<FoodType>;
                    if (animal != null && animal != this)
                    {
                        if (animal.Gender != Gender && GetType() == animal.GetType() && animal.IsInLoveMood())
                        {
                            float dist = animal.ChessLocation.DistanceSquaredTo(ChessLocation);
                            if (dist == 0.0f)
                            {
                                var child = (Animal<FoodType>)Activator.CreateInstance(GetType());
                                child.ChessLocation = ChessLocation;
                                closestSexPartner = null;
                                LastSexTime = game.CurrentTick;
                                animal.LastSexTime = game.CurrentTick;
                                child.LastSexTime = game.CurrentTick;
                                break;
                            }

                            if (closestSexPartner == null)
                                closestSexPartner = animal;
                            else if (dist < closestSexPartner.ChessLocation.DistanceSquaredTo(ChessLocation))
                                closestSexPartner = animal;
                        }
                    }
                }
                if (closestSexPartner != null)
                    WalkTo((closestSexPartner.ChessLocation + ChessLocation) / 2);
            }
            else
            {
                Vector2[] dirs = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
                var randLocation = ChessLocation + dirs[random.RandiRange(0, 3)];
                randLocation = Game.GetSingleton().WorldClamp(randLocation);
                WalkTo(randLocation);
            }

            TickNavigation();
        }

        public override void Update(float tickProgress)
        {
            Travel(tickProgress);
            UpdateVisuals();
            base.Update(tickProgress);
        }

        private void UpdateVisuals()
        {
            satietyNode.Scale = new Vector2(Satiety / 100.0f, 1.0f);
            satietyNode.Color = Color.Color8(0, 255, 0);
            if (IsHugry())
            {
                satietyNode.Color = Color.Color8(255, 0, 0);
            }
            else if (IsInLoveMood())
            {
                satietyNode.Color = Color.Color8(255, 0, 255);
            }
        }

        private void TickNavigation()
        {
            lastCellLocation = ChessLocation + new Vector2(0.5f, 0.5f);

            if (Location.Floor() != walkToLocation.Floor())
            {
                var travelVec = walkToLocation - Location;
                if (travelVec.x != 0.0f)
                    WalkToward(new Vector2(Mathf.Sign(travelVec.x), 0));
                else
                    WalkToward(new Vector2(0, Mathf.Sign(travelVec.y)));
            }
            else
                walkToLocation = Location;
        }
        private void Travel(float tickProgress)
        {
            Location = (1.0f - tickProgress) * lastCellLocation + tickProgress * walkToNeighbour;
        }

        protected override void OnChessLocationSet()
        {
            walkToNeighbour = Location;
            walkToLocation = Location;
            lastCellLocation = Location;
        }

        private Polygon2D satietyNode = null;
    }
}
