using System;
using Zenject;

public class NumberFormatter {

    [Inject] private LocalizationData _localizationDataPC;  
    
    public string ValuteFormatter(double value) {
        if (value < 1000)
            return Math.Floor(value).ToString();

        int tier = (int)Math.Floor(Math.Log10(value) / 3);
        tier = Math.Min(tier, _localizationDataPC.Suffixies.Length - 1);

        double scaled = value / Math.Pow(1000, tier);

        // Оставляем максимум 2 знака после запятой
        string formatted = scaled.ToString("0.##");

        return formatted + _localizationDataPC.Suffixies[tier];
    }
    
    public string ValuteFormatterInteger(double value) {
        if (value < 1000)
            return Math.Floor(value).ToString();

        int tier = (int)Math.Floor(Math.Log10(value) / 3);
        tier = Math.Min(tier, _localizationDataPC.Suffixies.Length - 1);

        double scaled = value / Math.Pow(1000, tier);

        string formatted = scaled.ToString("0");

        return formatted + _localizationDataPC.Suffixies[tier];
    }
    
}
    
    

