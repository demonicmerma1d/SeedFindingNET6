using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFindingNET6
{
    public class CartPairs
    {
        public readonly List<int> Offsets;
        public readonly Dictionary<string, HashSet<int>> ItemIdByBundle;
        public readonly List<string> DataOrder;
        //public readonly Dictionary<string, bool> Data;
        public readonly Dictionary<string, HashSet<int>> GardenItemsByPair;
        public TravelingCart Cart = new();
        public CartPairs()
        {
            Offsets = new()
            {
                5,7,9,12,14,16,19,21,23,26,28,30,33,35,37,40,42,44,47,49,51,54,56,58,61,63,65,68,70,72,75,77,79
            };
            ItemIdByBundle = new() //remove redundant ones after do code
            {
                //craft room
                ["SpForage"] = { 16, 18, 20, 22, 399 }, //logic
                ["SuForage"] = { 398, 402 },
                ["FaForage"] = { 404, 408 }, //logic, handle independantly
                ["WiForage"] = { 416, 418, 283 }, //logic
                //pantry
                ["SpCrops"] = { 190, 192, 24 }, //here for reference by logic 
                ["SuCrops"] = { 254, 256, 258, 260 },
                ["FaCrops"] = { 272, 276 },

                ["Animal"] = { 442, 174, 182 }, //here for reference, logic(s), milk removed to handle seperately
                ["Garden"] = { 591, 593, 595, 597, 421 },
                //
                ["Brewer"] = { 459, 303, 350, 614 }, //here for reference, logic(s)
                //fish
                ["TunaSturgeon"] = { 130, 698 },
                ["WalleyeTiger"] = { 140, 699 },
                //
                ["MasterFish"] = { 800, 165 }, //here for reference, logic(s)
                
                //bulliten board
                ["Chef"] = { 430, 376, 259 },
                ["Dye"] = { 284, 300, 268, 444, 90, 266 }, //here for reference
                ["HomeCook"] = { 184, 186, 436, 438 }, //here for reference for fuzzy logic
                ["DemGiftItems"] = { 128, 254 }
            };
            DataOrder = new()
            {
                "SpForage16","SpForage18","SpForage20","SpForage22","SpForage399",
                "SuForage","FaForage404","FaForage408",
                "WiForage416","WiForage418","WiForage283",
                "WildMed",
                "SpCrops","SuCrops","FaCrops","Corn",
                "Animal","Garden591","Garden593","Garden595","Garden597","Garden421",
                "Brewer",
                "SpSp","SpSu","SpFa","SuSu","FaFa",
                "SandFish","PufferSand",
                "Master","MasterSu",
                "Engineer",
                "Chef",
                "Dye284","Dye300","Dye268","Dye444","Dye90","Dye258","Dye266",
                "Research","Enchanter","HomeCook",
                "ChefWildMed","ExtraSunflower","AnimalHomeCook","EnchanterBrewer",
                "ResearchCrops","ResearchFish","Snapper",
                "SpFaCrops","GiftPumpkin"
            };
            GardenItemsByPair = new()
            {
                ["SpSp"] = { 593, 595, 421 },
                ["SpSu"] = { 591, 593, 595, 597, 421 },
                ["SpFa"] = { 591, 593, 595, 597, 421 },
                ["SuSu"] = {591,595,597},
                ["SuFa"] = {591,593,595,597},
                ["FaFa"] = {591,593,597}
            };
        }
        public Dictionary<int,int> GetCartStock(int seed)
        {
            var CartStock = Cart.GetStockCache(seed, 0);
            var dictCart = CartStock.ToDictionary(item => item.Key,item => item.Value.Quantity);
            return dictCart;
        }
        public static int OffsetCase(int offset)
        {
            return (offset < 28) ? 0 : (offset == 28) ? 1 : (offset < 56) ? 2 : 3;
        }
        public void ForageLogic(string Bundle, HashSet<int> CartItems,HashSet<string> PossibleDaysOld,BitEncoder LogicData,out BitEncoder LogicData2, out HashSet<string> PossibleDays)
        {
            LogicData2 = LogicData;
            PossibleDays = PossibleDaysOld;
            var Items = ItemIdByBundle[Bundle];
            Items.RemoveWhere(x => CartItems.Contains(x));
            int ItemCount = Items.Count;
            bool FullExit = Bundle == "WiForage";
            if (ItemCount > 1) //fail to have bundle
            {
                if (FullExit) PossibleDays.Clear();
                else PossibleDays.RemoveWhere(x => !x.Contains("Sp"));
                return;
            }
            if (ItemCount == 0)
            {
                foreach (var item in ItemIdByBundle[Bundle]) LogicData2.TrySetValue(string.Concat(Bundle, item.ToString()), true);
                return;
            }
            foreach (var item in Items) LogicData2.TrySetValue(string.Concat(Bundle, item.ToString()), true);
            return;
        }
        public static void CropLogic(HashSet<string> Crops, int offset,HashSet<string> PossibleDaysOld, out HashSet<string> PossibleDays )
        {
            //counts normal crop bundles possible
            PossibleDays = PossibleDaysOld;
            var BaseCrops = new HashSet<string> { "SpCrops", "SuCrops", "FaCrops" };
            BaseCrops.IntersectWith(Crops);
            var count = BaseCrops.Count;
            bool SpFaException = Crops.Contains("SpCrops") && Crops.Contains("FaCrops") && !Crops.Contains("SuCrops") && !Crops.Contains("SpFaCrops");
            if (count < 2 || SpFaException)
            {
                PossibleDays.Clear();
                return;
            }

            switch (OffsetCase(offset))
            {
                case 0:
                    PossibleDays.IntersectWith(new HashSet<string> { "SpSp", "SpSu", "SuSu", "SuFa", "FaFa" });
                    break;
                case 1:
                    PossibleDays.IntersectWith(new HashSet<string>{ "SpSu", "SuFa" });
                    break;
                case 2:
                    PossibleDays.IntersectWith(new HashSet<string>{ "SpSu", "SpFa", "SuFa" });
                    break;
                case 3:
                    PossibleDays.IntersectWith(new HashSet<string> { "SpFa" });
                    break;
                default:
                    PossibleDays.Clear();
                    break;
            }
            //cases:
            if (!Crops.Contains("Corn"))
            {
                PossibleDays.RemoveWhere(str => str.Contains("Sp"));
                if (PossibleDays.Count == 0) return;
            }
            if (count > 2 && Crops.Contains("SpFaCrops"));
            else if (count > 2) PossibleDays.IntersectWith(new HashSet<string> { "SpSp", "FaFa" });
            else if (!Crops.Contains("SpCrops")) PossibleDays.IntersectWith(new HashSet<string> { "SpSp" });    
            else if (!Crops.Contains("SuCrops")) PossibleDays.IntersectWith(new HashSet<string> { "SuSu" });
            else PossibleDays.IntersectWith(new HashSet<string> { "FaFa" });
            return;
        }
        public void GardenLogic(BitEncoder LogicData, HashSet<int> Garden,int AnimalItems, HashSet<string> PossibleDaysOld, out HashSet<string> PossibleDays)
        {
            PossibleDays = PossibleDaysOld;
            if (AnimalItems > 3) return;
            bool skip = true;
            //iterate through possible day pairs, check to see if garden possible, for each if not remove it as possible
            foreach (var DayPair in PossibleDaysOld)
            {
                int GardenItems = Utility.BundleIntersect(GardenItemsByPair[DayPair], Garden).Count;
                if (GardenItems > GardenItemsByPair[DayPair].Count - 2) skip = false;
                else PossibleDays.Remove(DayPair);
            }
            if (skip) PossibleDays.Clear();
            return;
        }
        public long EvaluatePair(int seed, int offset, Dictionary<int, int> BaseCartStock, bool FodderBundle, bool HasCarolineGift, HashSet<int> DemGiftItems)
        {
            var LogicData = new BitEncoder(DataOrder, 0);
            var CartStock = Utility.DictSum(BaseCartStock, GetCartStock(seed + offset));
            var CartItems = new HashSet<int>(CartStock.Keys);
            HashSet<string> ForcedSeasons = new();
            //WiForage with early exit check
            HashSet<string> PossibleDays = new()
            {
                "SpSp","SpSu","SpFa","SuFa","FaFa"
            }; //lists all possible days

            ForageLogic("WiForage", CartItems,PossibleDays, LogicData, out LogicData, out PossibleDays); //zero logicData if fail, check and exit
            if (PossibleDays.Count == 0) return 0;

            //crops with early exit logic
            bool GiftPumk = !(Utility.BundleIntersect(ItemIdByBundle["SpCrops"], CartItems).Count == 3);
            LogicData.TrySetValue("GiftPumpkin",GiftPumk);
            HashSet<string> Crops = new(); 

            bool SpCrops = CartItems.Contains(188) && (HasCarolineGift || !GiftPumk);
            if (SpCrops)
            {
                LogicData.TrySetValue("SpCrops", true);
                Crops.Add("SpCrops");
            }

            if (Utility.BundleIntersect(ItemIdByBundle["SuCrops"], CartItems).Count == 4)
            {
                LogicData.TrySetValue("SuCrops", true);
                Crops.Add("SuCrops");
            }

            bool FaCrops = Utility.BundleIntersect(ItemIdByBundle["FaCrops"], CartItems).Count == 2;
            if (FaCrops)
            {
                LogicData.TrySetValue("FaCrops", true);
                Crops.Add("FaCrops");
            }

            if (CartItems.Contains(270))
            {
                LogicData.TrySetValue("Corn", true);
                Crops.Add("Corn");
            }
            
            if (SpCrops && FaCrops && (HasCarolineGift ? CartStock[276] > 1 : true))
            {
                LogicData.TrySetValue("SpFaCrops", true);
                Crops.Add("SpFaCrops");
            }

            //gives us list of possible day combos
            CropLogic(Crops, offset,PossibleDays,out PossibleDays);
            int numDays = PossibleDays.Count;
            if (numDays == 0) return 0;

            //garenteed fish bundles
            bool WT = Utility.BundleIntersect(ItemIdByBundle["WalleyeTiger"], CartItems).Count == 2;
            bool Snapper = CartItems.Contains(150);
            LogicData.TrySetValue("Snapper",Snapper);
            HashSet<string> FishDays = new()
            {
                { "SuFa" }
            };
            if (Utility.BundleIntersect(ItemIdByBundle["TunaSturgeon"], CartItems).Count == 2)
            {
                FishDays.Add("SpFa");
                FishDays.Add("FaFa");
                if (WT && Snapper && CartItems.Contains(701)) FishDays.Add("SpSp"); //snapper and tilapia
            }
            if (WT && CartItems.Contains(148))
            {
                FishDays.Add("SpSu");
                if (CartItems.Contains(131)) FishDays.Add("SuSu");
            }
            //compare fish bundles to possibleDays, exit if nothing possible
            PossibleDays.IntersectWith(FishDays);
            if (PossibleDays.Count == 0) return 0;
            foreach (var DayPair in PossibleDays) LogicData.TrySetValue(DayPair, true);

            //forage bundles
            //SuForage
            if (Utility.BundleIntersect(ItemIdByBundle["SuForage"], CartItems).Count == 2)
            {
                LogicData.TrySetValue("SuForage", true);
            }
            else
            {
                PossibleDays.RemoveWhere(str => !str.Contains("Su"));
                if (PossibleDays.Count == 0) return 0;
            }

            //FaForage
            if (CartItems.Contains(408)) LogicData.TrySetValue("FaForage408", true);
            else
            {
                PossibleDays.RemoveWhere(str => !str.Contains("Fa"));
                if (PossibleDays.Count == 0) return 0;
            }
            if (CartItems.Contains(404)) LogicData.TrySetValue("FaForage404", true);
            else
            {
                PossibleDays.RemoveWhere(str => str == "SpSp");
                if (PossibleDays.Count == 0) return 0;
            }

            //SpForage
            ForageLogic("SpForage", CartItems,PossibleDays, LogicData, out LogicData,out PossibleDays);
            if (PossibleDays.Count == 0) return 0;

            //variable bundles
            //garden
            HashSet<int> Garden = new();
            foreach (int item in ItemIdByBundle["Garden"]) if (CartItems.Contains(item))
                {
                    Garden.Add(item);
                    LogicData.TrySetValue(string.Concat("Garden", item.ToString()), true);
                }
            CartStock.TryGetValue(421, out int sunflower);
            LogicData.TrySetValue("ExtraSunflower", sunflower > 1);

            //animal,home cook
            int AnimalItems = Utility.BundleIntersect(ItemIdByBundle["Animal"], CartItems).Count;
            int Milk = CartStock.TryGetValue(184, out int _Milk) ? _Milk : 0;
            int LMilk = CartStock.TryGetValue(186, out int _LMilk) ? _LMilk : 0;
            int GMilk = CartStock.TryGetValue(436, out int _GMilk) ? _GMilk : 0;
            int LGMilk = CartStock.TryGetValue(438, out int _LGMilk) ? _LGMilk : 0;

            foreach (int count in new List<int> { LMilk, LGMilk }) AnimalItems += count > 0 ? 1 : 0;
            LogicData.TrySetValue("Animal", AnimalItems > 3);

            //animal/garden early exit
            GardenLogic(LogicData, Garden, AnimalItems, PossibleDays, out PossibleDays);
            if (PossibleDays.Count == 0) return 0;

            int TotalMilk = Milk + LMilk + GMilk + LGMilk;
            LogicData.TrySetValue("HomeCook", TotalMilk > 9);
            LogicData.TrySetValue("AnimalHomeCook", TotalMilk - AnimalItems + 3 > 9);

            //specialty fish
            if (CartItems.Contains(164))
            {
                LogicData.TrySetValue("Sandfish", true);
                LogicData.TrySetValue("PufferSand", CartItems.Contains(128));
            }
            //master fish
            int MasterFish = Utility.BundleIntersect(ItemIdByBundle["MasterFish"], CartItems).Count;
            if (MasterFish > 0)
            {
                LogicData.TrySetValue("MasterSu", true);
                LogicData.TrySetValue("Master", MasterFish == 2 || CartItems.Contains(149));
            }
            LogicData.TrySetValue("Engineer", CartItems.Contains(787));
            //various bulliten board bundles and clashing bundles

            //brewer,enchanter
            int BrewerItems = Utility.BundleIntersect(ItemIdByBundle["Brewer"], CartItems).Count;
            bool HasWine = CartStock.TryGetValue(348, out int Wine);

            bool Enchanter = CartItems.Contains(446) && HasWine;
            LogicData.TrySetValue("Enchanter", Enchanter);

            bool Brewer = BrewerItems > 3 || BrewerItems == 3 && HasWine;
            LogicData.TrySetValue("Brewer", Brewer);

            LogicData.TrySetValue("EnchanterBrewer", Enchanter && Brewer && Wine > 1);

            //chef, WildMed
            bool Chef = Utility.BundleIntersect(ItemIdByBundle["Chef"], CartItems).Count == 3;
            //case if 5 hops
            if (CartStock.TryGetValue(304, out int value) && value > 4)
            {
                LogicData.TrySetValue("WildMed", true);
                LogicData.TrySetValue("Chef", Chef); //need 1 fern for chef
                LogicData.TrySetValue("ChefWildMed", Chef);
            }
            //case have at least 5 ferns
            else if (CartStock.TryGetValue(259, out value) && value > 4)
            {
                LogicData.TrySetValue("Chef", Chef);
                LogicData.TrySetValue("WildMed", true);
                LogicData.TrySetValue("ChefWildMed", value > 5);
            }
            //otherwise fail
            else LogicData.TrySetValue("Chef", Chef);

            //dye 
            LogicData.TrySetValue("Dye258", CartStock.TryGetValue(258, out value) && value > 1); //blueberry
            foreach (var Item in ItemIdByBundle["Dye"]) LogicData.TrySetValue(string.Concat("Dye", Item.ToString()), CartItems.Contains(Item));
            bool Dye = LogicData.ContainsSubset(new HashSet<string> { "Dye90", "Dye444" });

            //Fodder
            FodderBundle = FodderBundle && CartStock.TryGetValue(262, out value) && value == 10;
            if (FodderBundle) LogicData.TrySetValue("Fodder", true);

            //research REDO, rename summer bits to relevent interaction bits
            if (CartItems.Contains(392))
            {
                LogicData.TrySetValue("Research", true); //nautilus in cart
                LogicData.TrySetValue("ResearchCrops", true);
                LogicData.TrySetValue("ResearchFish", true);
            }
            else if (DemGiftItems.Contains(128))
            {
                LogicData.TrySetValue("Research", true);
                LogicData.TrySetValue("ResearchFish", CartStock.TryGetValue(128, out value) && value > 1);
                LogicData.TrySetValue("ResearchCrops", true);
            }
            else if (DemGiftItems.Contains(254))
            {
                 LogicData.TrySetValue("Research", true);
                 LogicData.TrySetValue("ResearchCrops", CartStock.TryGetValue(254, out value) && value > 1);
                  LogicData.TrySetValue("ResearchFish", true);
            }
            bool Bulletin = TotalMilk > 9 || Chef || Enchanter || Dye;
            if (!Bulletin) return 0;
            return LogicData.EncodeData;
        }
        public HashSet<string> EvaluatePairs(int seed)
        {
            var BaseCartStock = GetCartStock(seed);
            var FodderBundle = BaseCartStock.TryGetValue(262, out int value) && value == 5;
            var HasCarolineGift = BaseCartStock.ContainsKey(276);
            var DemGiftItems = Utility.BundleIntersect(ItemIdByBundle["DemGiftItems"],new HashSet<int>(BaseCartStock.Keys));
            HashSet<string> Solns = new();
            for (int index = 0; index < Offsets.Count; index++)
            {
                long PairValue = EvaluatePair(seed, Offsets[index],BaseCartStock,FodderBundle,HasCarolineGift,DemGiftItems);
                if (PairValue > 0)
                {
                    Solns.Add(string.Concat(seed.ToString(), "-", index.ToString(), ":", PairValue.ToString()));
                }
            }
            return Solns;
        }
        public HashSet<string> SeedRange(int MinSeed = 0, int MaxSeed = int.MaxValue)
        {
            HashSet<string> Solns = new();
            for (var seed = MinSeed; seed < MaxSeed; seed++)
            {
                Solns.UnionWith(EvaluatePairs(seed));
            }
            return Solns;
        }
    }
}