using CardTypeEnum;

namespace CardTypeEnum
{
    public enum CardType
    {
        Active,
        Passive
    }
    
    public enum CardSymbol
    {
        Space,
        Diamond,
        Heart,
        Club
    }
}

public interface ICard
{
    string Name { get; }
    int Number { get; }
    CardType CardType { get; }
    CardSymbol CardSymbol { get; }

    public void UseCard();
}