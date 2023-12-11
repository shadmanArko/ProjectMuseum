namespace Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;

public static class TimeHelpers
{
    public static string ToCorrespondedDay(this int day)
    {
        string correspondedDay = "";
        switch (day)
        {
            case 1: correspondedDay = "Monday";
                break;
            case 2: correspondedDay = "Tuesday";
                break;
            case 3: correspondedDay = "Wednesday";
                break;
            case 4: correspondedDay = "Thursday";
                break;
            case 5: correspondedDay = "Friday";
                break;
            case 6: correspondedDay = "Saturday";
                break;
            case 7: correspondedDay = "Sunday";
                break;
            default: correspondedDay = $"day {day}";
                break;
        }

        return correspondedDay;
    }
}