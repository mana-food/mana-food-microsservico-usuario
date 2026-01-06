namespace ManaFood.Application.Utils;

public static class BirthdayUtils
{
    public static bool IsValidBirthday(DateOnly birthday)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthday.Year;
        if (birthday > today.AddYears(-age)) age--;
        return age >= 18;
    }
}