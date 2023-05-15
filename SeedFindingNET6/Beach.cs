using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFindingNET6
{
    public struct Rect
    {
        public int minX;
        public int maxX;
        public int minY;
        public int maxY;
        public Rect(int minx, int maxx, int miny, int maxy)
        {
            minX = minx;
            maxX = maxx;
            minY = miny;
            maxY = maxy;
        }
        public bool Contains(Tile tile)
        {
            return minX <= tile.X && tile.X <= maxX && minY <= tile.Y && tile.Y <= maxY;
        }
    }
    public struct Tile
    {
        public int X;
        public int Y;
        public Tile(int x, int y) { X = x; Y = y; }
        public override string ToString()
        {
            return string.Format("({0:D2},{1:D2})", X, Y);
        }
    }
    public struct SpawnChance
    {
        public int Id;
        public double P;
        public SpawnChance(int id, double p) { Id = id; P = p; }
    }
    public class Beach : Location
    {
        public static Map map;
        public static HashSet<Tile> SpawnableTiles;
        public static HashSet<Tile> BadTiles;
        public static Dictionary<int, List<SpawnChance>> SeasonalSpawns;
        static Beach()
        {
            map = new Map("Beach");
            //Console.WriteLine("beach_layers: {0}\t{1}\t{2}", map.AlwaysFront.Tiles.Count, map.Front.Tiles.Count, map.Buildings.Tiles.Count);
            //throw new Exception("crud");
            SpawnableTiles = Location.LoadTiles(@"data/beach_spawn_tiles.json");
            BadTiles = new HashSet<Tile>();
            BadTiles.UnionWith(map.AlwaysFront.Tiles);
            BadTiles.UnionWith(map.Front.Tiles);
            BadTiles.UnionWith(map.Buildings.Tiles);
            SeasonalSpawns = new Dictionary<int, List<SpawnChance>>
            {
                [0] = new List<SpawnChance>() { new SpawnChance(372, 0.9), new SpawnChance(718, 0.1), new SpawnChance(719, 0.3), new SpawnChance(723, 0.3) },
                [1] = new List<SpawnChance>() { new SpawnChance(372, 0.9), new SpawnChance(394, 0.5), new SpawnChance(718, 0.1), new SpawnChance(719, 0.3), new SpawnChance(723, 0.3) },
                [2] = new List<SpawnChance>() { new SpawnChance(372, 0.9), new SpawnChance(718, 0.1), new SpawnChance(719, 0.3), new SpawnChance(723, 0.3) },
                [3] = new List<SpawnChance>() { new SpawnChance(372, 0.4), new SpawnChance(392, 0.8), new SpawnChance(718, 0.05), new SpawnChance(719, 0.2), new SpawnChance(723, 0.2) }
            };
        }

        public Beach(int seed) : base(seed, 104, 50)
        {
        }
        public override void Spawn()
        {

            Spawn(SpawnableTiles, BadTiles, SeasonalSpawns);
        }
    }
}