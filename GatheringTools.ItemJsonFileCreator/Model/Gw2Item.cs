namespace GatheringTools.ItemJsonFileCreator.Model
{
    public class Gw2Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string Rarity { get; set; }
        public Details Details { get; set; } = new Details();
    }
}