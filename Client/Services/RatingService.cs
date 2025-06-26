namespace Client.Services;

public static class RatingService
{
    public static string GetRatingMark(ushort rating) =>
        rating switch
        {
            >= 1500 and < 2000 => "status-success",
            >= 2000 and < 3000 => "status-info",
            >= 3000 => "status-danger",
            _ => "status-default"
        };

    public static string GetRatingName(ushort rating) =>
        rating switch
        {
            >= 1500 and < 2000 => "Semi-серия",
            >= 2000 and < 3000 => "Pro-серия",
            >= 3000 => "Star-серия",
            _ => "Amater-серия"
        };
}