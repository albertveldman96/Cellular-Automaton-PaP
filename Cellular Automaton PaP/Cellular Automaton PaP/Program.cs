using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.IO;

namespace Cellular_Automaton_PaP
{
    class Program
    {
        static uint widthInt = 600;
        static uint heightInt = 400;
        static Random rnd = new Random();

        VideoMode videoMode = new VideoMode(widthInt, heightInt);
        RenderWindow window;

        VertexArray pixels;
        Creature[] creatures;

        Text frameCountText;
        Text preyCountText;
        Text predatorCountText;
        Font textFont;
        int characterSize = 15;

        RectangleShape outline;
        int frameCount;
        int preyCount;
        int predatorCount;
        

        static void Main(string[] args)
        {
            var program = new Program();
            program.Setup();
        }

        public void Setup()
        {
            SetBaseVariables();
            window.SetFramerateLimit(60);
            SetTextProperties();
            SetPixelsAndCreatures();

            //window.Closed += (s, a) => window.Close();
            Run();
        }

        private void SetBaseVariables()
        {
            var pixelAmount = widthInt * heightInt;
            window = new RenderWindow(videoMode, "Cellular Automoton");
            pixels = new VertexArray(PrimitiveType.Points, pixelAmount);
            creatures = new Creature[pixelAmount];
            frameCount = 0;
            preyCount = 0;
            predatorCount = 0;

            frameCountText = new Text();
            preyCountText = new Text();
            predatorCountText = new Text();
            textFont = new Font(Path.Combine(Environment.CurrentDirectory, "arial.ttf"));
        }

        private void SetTextProperties()
        {
            frameCountText.CharacterSize = 15;
            frameCountText.Color = Color.White;
            frameCountText.Position = new Vector2f(0, 30);
            frameCountText.Font = textFont;

            predatorCountText.CharacterSize = 15;
            predatorCountText.Color = Color.White;
            predatorCountText.Position = new Vector2f(0, 15);
            predatorCountText.Font = textFont;

            preyCountText.CharacterSize = 15;
            preyCountText.Color = Color.White;
            preyCountText.Position = new Vector2f(0, 0);
            preyCountText.Font = textFont;
        }

        private void SetPixelsAndCreatures()
        {
            for (int x = 0; x < widthInt; x++)
            {
                for (int y = 0; y < heightInt; y++)
                {
                    //Creature type is randomly selected when created
                    var creature = new Creature();
                    var vertex = new Vertex();
                    vertex.Position = new Vector2f(x, y);
                    vertex.Color = creature.GetColor();

                    var index = getIndex(x, y);
                    pixels[index] = vertex;
                    creatures[index] = creature;

                    switch (creature.creatureType)
                    {
                        case Creature.CreatureType.Prey:
                            preyCount++;
                            break;
                        case Creature.CreatureType.Predator:
                            predatorCount++;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void Run()
        {
            while (window.IsOpen)
            {
                window.Clear();
                Update();

                window.Draw(pixels);
                window.Draw(frameCountText);
                window.Draw(preyCountText);
                window.Draw(predatorCountText);
                window.Display();
            }
        }

        private void Update()
        {
            for (int x = 0; x < widthInt; x++)
            {
                for (int y = 0; y < heightInt; y++)
                {
                    var index = getIndex(x, y);
                    var thisCreature = creatures[index];
                    var thisCreatureType = thisCreature.creatureType;

                    //Go to next position if this one is nothing
                    if (thisCreatureType == Creature.CreatureType.Nothing)
                        continue;

                    thisCreature.UpdateCreatureHealth();

                    //Randomly move max one pixel in x and y
                    int xMove = rnd.Next(-1, 2);
                    int yMove = rnd.Next(-1, 2);
                    var newX = x + xMove;
                    var newY = y + yMove;

                    //Abort if new position is outside of area.
                    if (newX < 0 || newX >= widthInt) continue;
                    if (newY < 0 || newY >= heightInt) continue;

                    var newIndex = getIndex(newX, newY);
                    var otherCreature = creatures[newIndex];

                    //Action depends on creaturetype and status of new position.
                    switch (thisCreatureType)
                    {
                        case Creature.CreatureType.Predator:
                            UpdatePredator(thisCreature, otherCreature);
                            break;
                        case Creature.CreatureType.Prey:
                            UpdatePrey(thisCreature, otherCreature);
                            break;
                        default:
                            break;
                    }

                    //Update pixel color
                    var vertex = pixels[index];
                    vertex.Color = thisCreature.GetColor();
                    pixels[index] = vertex;
                }
            }

            //Update creature and frame count text
            frameCount++;
            frameCountText.DisplayedString = "Steps: " + frameCount;
            preyCountText.DisplayedString = "Prey: " + preyCount;
            predatorCountText.DisplayedString = "Predator: " + predatorCount;
        }

        private void UpdatePredator(Creature thisCreature, Creature otherCreature)
        {
            //If health is 0 or lower, predator dies.
            if (thisCreature.health <= 0)
            {
                thisCreature.creatureType = Creature.CreatureType.Nothing;
                predatorCount--;
                //Create new predator in center of screen if last predator dies.
                if (predatorCount == 0)
                {
                    newPredator();
                    predatorCount++;
                }
                return;
            }
            
            switch (otherCreature.creatureType)
            {
                //If other creature is prey. prey dies, predator is healed, new predator is created on prey position.
                case Creature.CreatureType.Prey:
                    otherCreature.creatureType = Creature.CreatureType.Predator;
                    otherCreature.HealCreature(100);
                    thisCreature.HealCreature(otherCreature.health);
                    predatorCount++;
                    preyCount--;
                    break;

                //If other creature is nothing. predator moves to new position.
                case Creature.CreatureType.Nothing:
                    thisCreature.MoveCreature(otherCreature);
                    break;

                //If other creature is predator. nothing happens.
                default:
                    break;
            }
        }

        //Create new predator in center of screen.
        private void newPredator()
        {
            var center = getIndex((int)widthInt / 2, (int)heightInt / 2);
            var vertex = pixels[center];
            var creature = creatures[center];

            creature.creatureType = Creature.CreatureType.Predator;
            creature.health = 100;
            vertex.Color = creature.GetColor();

            //Set new variables in array
            pixels[center] = vertex;
            creatures[center] = creature;
        }

        private void UpdatePrey(Creature thisCreature, Creature otherCreature)
        {
            //If health is max. Create new prey.
            bool reproduce = false;
            if (thisCreature.health >= Creature.MAX_HEALTH)
            {
                thisCreature.health = 10;
                reproduce = true;
            }

            switch (otherCreature.creatureType)
            {
                //If able to reproduce and other creature is nothing. Create new prey. Else move to new position.
                case Creature.CreatureType.Nothing:
                    if (reproduce)
                    {
                        thisCreature.Reproduce(otherCreature);
                        preyCount++;
                    }
                    else
                        thisCreature.MoveCreature(otherCreature);
                    break;

                //If other creature is prey or predator. Nothing happens.
                default:
                    break;
            }
        }


        //Returns 1d array index from 2d location
        private uint getIndex(int x, int y)
        {
            return (uint)(y * widthInt + x);
        }
    }
}
