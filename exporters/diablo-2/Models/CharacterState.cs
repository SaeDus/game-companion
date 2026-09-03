public class CharacterState
{
    public string Name { get; set; } = "";
    public int Level { get; set; }
    public string Class { get; set; } = "";

    public AttributeState BaseAttributes { get; set; } = new();
    public int UnspentStatPoints { get; set; }

    public List<SkillState> BaseSkills { get; set; } = [];
    public int UnspentSkillPoints { get; set; }

    public List<ItemState> Equipment { get; set; } = [];
    public List<ItemState> Inventory { get; set; } = [];
    public List<ItemState> PersonalStash { get; set; } = [];
}
