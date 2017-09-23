using System;
using SFML.Graphics;

namespace Cellular_Automaton_PaP
{
    class Creature
    {
        static public int MAX_HEALTH = 100;
        static Random rnd = new Random();

        public CreatureType creatureType { get; set; }
        public int health { get; set; }

        public enum CreatureType
        {
            Predator = 0,
            Prey = 1,
            Nothing = 2
        };

        public Creature()
        {
            //90% of creatures are nothing.
            //7% of creatures are prey.
            //3% of creatures are predator.
            health = 100;
            var n = rnd.Next(0, 100);
            if (n > 10)
                creatureType = CreatureType.Nothing;
            else if (n > 3)
                creatureType = CreatureType.Prey;
            else
                creatureType = CreatureType.Predator;
        }

        public void UpdateCreatureHealth()
        {
            switch (creatureType)
            {
                //Every step, predators lose health.
                case CreatureType.Predator:
                    HealCreature(-1);
                    break;
                //Every step, prey gain health.
                case CreatureType.Prey:
                    HealCreature(2);
                    break;
                default:
                    break;
            }
        }

        public void MoveCreature(Creature location)
        {
            //Set old values to new location
            location.health = health;
            location.creatureType = creatureType;
            creatureType = CreatureType.Nothing;
        }

        public void HealCreature(int amount)
        {
            health += amount;
            if (health > MAX_HEALTH) health = MAX_HEALTH;
        }

        public void Reproduce(Creature location)
        {
            location.health = 10;
            location.creatureType = CreatureType.Prey;
        }

        public Color GetColor()
        {
            if (creatureType == CreatureType.Nothing || health == 0)
                return Color.Black;
            //var percentageHealth = (float)health / MAX_HEALTH;
            //var healthBasedColor = (byte)(percentageHealth * 255);
            
            //Predators are red, prey are green and nothing or 0 health is black.
            switch (creatureType)
            {
                case CreatureType.Predator:
                    return new Color(255, 0, 0);
                case CreatureType.Prey:
                    return new Color(0, 255, 0);
                default:
                    return Color.Black;
            }
        }
    }
}
