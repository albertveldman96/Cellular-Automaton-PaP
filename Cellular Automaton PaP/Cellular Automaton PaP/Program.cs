using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.IO;

namespace Cellular_Automaton_PaP
{
    class Program
    {
        static uint widthUint = 600;
        static uint heightUint = 400;
        static Random rnd = new Random();

        VideoMode videoMode = new VideoMode(widthUint, heightUint);
        RenderWindow window;
        View view;
        VertexArray pixels;
        Creature[] creatures;
        Text frameCountText;
        Text preyCountText;
        Text predatorCountText;
        Font textFont;

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
            window = new RenderWindow(videoMode, "Cellular Automoton");
            view = new View(new Vector2f(widthUint / 2, heightUint / 2), new Vector2f(widthUint, heightUint));
            pixels = new VertexArray(PrimitiveType.Points, widthUint * heightUint);
            creatures = new Creature[widthUint * heightUint];
            frameCount = 0;
            preyCount = 0;
            predatorCount = 0;

            textFont = new Font(Path.Combine(Environment.CurrentDirectory, "arial.ttf"));

            frameCountText = new Text();
            frameCountText.CharacterSize = 15;
            frameCountText.Color = Color.White;
            frameCountText.Position = new Vector2f(0, 30);
            frameCountText.Font = textFont;

            preyCountText = new Text();
            preyCountText.CharacterSize = 15;
            preyCountText.Color = Color.White;
            preyCountText.Position = new Vector2f(0, 0);
            preyCountText.Font = textFont;

            predatorCountText = new Text();
            predatorCountText.CharacterSize = 15;
            predatorCountText.Color = Color.White;
            predatorCountText.Position = new Vector2f(0, 15);
            predatorCountText.Font = textFont;



            //outline = new RectangleShape(new Vector2f(widthInt, heightInt));
            //outline.OutlineColor = Color.White;
            //outline.OutlineThickness = 5;
            //outline.FillColor = new Color(0, 0, 0, 0);

            window.SetFramerateLimit(60);
            CreatePixelsCreatures();

            //window.Closed += (s, a) => window.Close();

            Run();
        }

        private void CreatePixelsCreatures()
        {
            for (int x = 0; x < widthUint; x++)
            {
                for (int y = 0; y < heightUint; y++)
                {
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
            //var clock = new Clock();
            while (window.IsOpen)
            {
                //var deltaTime = clock.Restart().AsSeconds();
                window.Clear();
                Update();

                //window.SetView(view);
                window.Draw(pixels);
                //window.Draw(outline);
                window.Draw(frameCountText);
                window.Draw(preyCountText);
                window.Draw(predatorCountText);

                window.Display();
                frameCount++;
            }
        }

        private void Update()
        {
            for (int x = 0; x < widthUint; x++)
            {
                for (int y = 0; y < heightUint; y++)
                {
                    var index = getIndex(x, y);
                    var thisCreature = creatures[index];
                    var creatureType = thisCreature.creatureType;

                    if (creatureType == Creature.CreatureType.Nothing)
                        continue;

                    //Randomly move max one pixel in x and y
                    int xMove = rnd.Next(-1, 2);
                    int yMove = rnd.Next(-1, 2);
                    var newX = x + xMove;
                    var newY = y + yMove;

                    //Abort if new position is outside of area.
                    if (newX < 0 || newX >= widthUint) continue;
                    if (newY < 0 || newY >= heightUint) continue;

                    var newIndex = getIndex(newX, newY);
                    var otherCreature = creatures[newIndex];

                    thisCreature.Update();
                    switch (creatureType)
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

                    var vertex = pixels[index];
                    vertex.Color = thisCreature.GetColor();
                    pixels[index] = vertex;
                }
            }
            frameCountText.DisplayedString = "Steps: " + frameCount;
            preyCountText.DisplayedString = "Prey: " + preyCount;
            predatorCountText.DisplayedString = "Predator: " + predatorCount;
        }

        

        private void UpdatePredator(Creature thisCreature, Creature otherCreature)
        {
            if (thisCreature.health <= 0)
            {
                thisCreature.creatureType = Creature.CreatureType.Nothing;
                predatorCount--;
                return;
            }

            var typeTwo = otherCreature.creatureType;
            switch (typeTwo)
            {
                case Creature.CreatureType.Prey:
                    otherCreature.creatureType = Creature.CreatureType.Predator;
                    thisCreature.HealCreature(otherCreature.health);
                    predatorCount++;
                    preyCount--;
                    break;

                case Creature.CreatureType.Nothing:
                    thisCreature.MoveCreature(otherCreature);
                    break;

                default:
                    break;
            }
        }

        private void UpdatePrey(Creature thisCreature, Creature otherCreature)
        {
            var typeTwo = otherCreature.creatureType;
            bool reproduce = false;
            if (thisCreature.health >= Creature.MAX_HEALTH)
            {
                thisCreature.health = 10;
                reproduce = true;
            }

            switch (typeTwo)
            {
                case Creature.CreatureType.Prey:
                    break;
                case Creature.CreatureType.Predator:
                    break;
                case Creature.CreatureType.Nothing:
                    if (reproduce)
                    {
                        thisCreature.Reproduce(otherCreature);
                        preyCount++;
                    }
                    else
                        thisCreature.MoveCreature(otherCreature);
                    break;
            }
        }



        private uint getIndex(int x, int y)
        {
            return (uint)(y * widthUint + x);
        }
    }
}
