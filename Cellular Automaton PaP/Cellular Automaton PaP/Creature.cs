using System;
using SFML.Graphics;

namespace Cellular_Automaton_PaP
{
    class Creature
    {
        static public int MAX_HEALTH = 100;
        static private Random rnd = new Random();

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
            health = 100;
            var n = rnd.Next(0, 100);
            if (n > 10)
                creatureType = CreatureType.Nothing;
            else if (n > 3)
                creatureType = CreatureType.Prey;
            else
                creatureType = CreatureType.Predator;
        }

        public void Update()
        {
            switch (creatureType)
            {
                case CreatureType.Predator:
                    HealCreature(-1);
                    break;
                case CreatureType.Prey:
                    HealCreature(2);
                    break;
                default:
                    break;
            }
        }

        public void MoveCreature(Creature location)
        {
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
            var percentageHealth = (float)health / MAX_HEALTH;
            var healthBasedColor = (byte)(percentageHealth * 255);
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
