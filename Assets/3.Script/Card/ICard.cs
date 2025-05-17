public interface ICard
{
    string name { get; set; }
    string symbol { get; set; }
    int number { get; set; }

    enum cardType
    {
        Active,
        Passive
    }

    public void UseCard();
}