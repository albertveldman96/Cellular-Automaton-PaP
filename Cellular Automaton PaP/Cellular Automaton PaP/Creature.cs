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
            //95% of creatures are nothing.
            //4% of creatures are prey.
            //1% of creatures are predator.
            health = 50;
            var n = rnd.Next(0, 100);
            if (n > 5)
                creatureType = CreatureType.Nothing;
            else if (n > 1)
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
                    HealCreature(1);
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
            //More health equals brighter color
            var percentageHealth = (float)health / MAX_HEALTH;
            var healthBasedColor = 50 + (percentageHealth * 205);
            
            //Predators are red, prey are green and nothing or 0 health is black.
            switch (creatureType)
            {
                case CreatureType.Predator:
                    return new Color((byte)healthBasedColor, 0, 0);
                case CreatureType.Prey:
                    return new Color(0, (byte)healthBasedColor, 0);
                default:
                    return Color.Black;
            }
        }
    }
}
