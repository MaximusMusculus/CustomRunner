using System.Collections.Generic;
using System.Linq;

public class BaseAttribute
{
    private int _baseValue;
    private double _baseMultiplier;

    public BaseAttribute(int value, double multiplier = 0)
    {
        _baseValue = value;
        _baseMultiplier = multiplier;
    }

    public int BaseValue
    {
        get { return _baseValue; }
    }

    public double BaseMultiplier
    {
        get { return _baseMultiplier; }
    }
}


public class RawBonus : BaseAttribute
{
    public RawBonus(int value = 0, double multiplier = 0)
        : base(value, multiplier)
    {
    }
}



public class FinalBonus : BaseAttribute
{
    public FinalBonus(int value = 0, double multiplier = 0)
        : base(value, multiplier)
    {
    }
}

/*public class FinalBonus : BaseAttribute
{
    private Timer _timer;
    private Attribute _parent;

    public FinalBonus(int time, int value = 0, double multiplier = 0)
        : base(value, multiplier)
    {
        _timer = new Timer(time);
        _timer.Elapsed += OnTimerEnd;
        _timer.AutoReset = false; // Ensure the timer runs only once
    }

    public void StartTimer(Attribute parent)
    {
        _parent = parent;
        _timer.Start();
    }

    private void OnTimerEnd(object sender, ElapsedEventArgs e)
    {
        _timer.Stop();
        _parent?.RemoveFinalBonus(this);
    }
}*/


public class Attribute : BaseAttribute
{
    private List<RawBonus> _rawBonuses;
    private List<FinalBonus> _finalBonuses;

    private int _finalValue;

    public Attribute(int startingValue) : base(startingValue)
    {
        _rawBonuses = new List<RawBonus>();
        _finalBonuses = new List<FinalBonus>();

        _finalValue = BaseValue; // Note: Property names in C# are PascalCase
    }

    public void AddRawBonus(RawBonus bonus)
    {
        _rawBonuses.Add(bonus);
    }

    public void AddFinalBonus(FinalBonus bonus)
    {
        _finalBonuses.Add(bonus);
    }

    public void RemoveRawBonus(RawBonus bonus)
    {
        _rawBonuses.Remove(bonus); // List<T>.Remove() automatically checks for the item's existence
    }

    public void RemoveFinalBonus(FinalBonus bonus)
    {
        _finalBonuses.Remove(bonus); // Same as above
    }

    public int CalculateValue()
    {
        _finalValue = BaseValue;

        // Adding value from raw bonuses
        int rawBonusValue = _rawBonuses.Sum(bonus => bonus.BaseValue);
        double rawBonusMultiplier = _rawBonuses.Sum(bonus => bonus.BaseMultiplier);

        _finalValue += rawBonusValue;
        _finalValue = (int)(_finalValue * (1 + rawBonusMultiplier));

        // Adding value from final bonuses
        int finalBonusValue = _finalBonuses.Sum(bonus => bonus.BaseValue);
        double finalBonusMultiplier = _finalBonuses.Sum(bonus => bonus.BaseMultiplier);

        _finalValue += finalBonusValue;
        _finalValue = (int)(_finalValue * (1 + finalBonusMultiplier));

        return _finalValue;
    }

    public int FinalValue
    {
        get { return CalculateValue(); }
    }
}
