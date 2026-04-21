using EventHarbor.Common;

namespace EventHarbor.Tests;

public class InputValidationTests
{
    [Theory]
    [InlineData("hello", "hello")]
    [InlineData("  trimmed  ", "trimmed")]
    public void ValidateText_accepts_non_empty(string input, string expected)
    {
        Assert.Equal(expected, InputValidation.ValidateText(input, "pole"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateText_rejects_empty(string? input)
    {
        Assert.Throws<ArgumentException>(() => InputValidation.ValidateText(input, "pole"));
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("42", 42)]
    [InlineData("-5", -5)]
    public void ValidateInt_parses_valid_integers(string input, int expected)
    {
        Assert.Equal(expected, InputValidation.ValidateInt(input, "pole"));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("1.5")]
    public void ValidateInt_rejects_invalid(string input)
    {
        Assert.Throws<ArgumentException>(() => InputValidation.ValidateInt(input, "pole"));
    }

    [Fact]
    public void ValidateInt_enforces_range()
    {
        Assert.Throws<ArgumentException>(() => InputValidation.ValidateInt("150", "pole", 0, 100));
        Assert.Throws<ArgumentException>(() => InputValidation.ValidateInt("-1", "pole", 0, 100));
        Assert.Equal(50, InputValidation.ValidateInt("50", "pole", 0, 100));
    }

    [Theory]
    [InlineData("19.50", 19.50)]
    [InlineData("1000", 1000)]
    [InlineData("0", 0)]
    public void ValidateDecimal_parses_valid(string input, double expected)
    {
        var result = InputValidation.ValidateDecimal(input, "cena");
        Assert.Equal((decimal)expected, result);
    }

    [Fact]
    public void ValidateDateRange_rejects_end_before_start()
    {
        var start = new DateTime(2026, 5, 20);
        var end = new DateTime(2026, 5, 10);
        Assert.Throws<ArgumentException>(() => InputValidation.ValidateDateRange(start, end));
    }

    [Fact]
    public void ValidateDateRange_accepts_equal_dates()
    {
        var d = new DateTime(2026, 5, 20);
        InputValidation.ValidateDateRange(d, d);
    }

    [Fact]
    public void ValidateDateRange_rejects_null()
    {
        Assert.Throws<ArgumentException>(() => InputValidation.ValidateDateRange(null, DateTime.Today));
        Assert.Throws<ArgumentException>(() => InputValidation.ValidateDateRange(DateTime.Today, null));
    }
}
