namespace ProjectMuseum.Models;

public class MuseumTicketCounter
{
    public string Id { get; set; }
    public float TicketPrice { get; set; }
    public float MuseumRating { get; set; }
    public int NumberOfPeopleInQueue { get; set; }
    public int MuseumOpeningHour { get; set; }
    public int MuseumClosingHour { get; set; }
}