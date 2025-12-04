using Bookify.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Registrations
{

    public static class GradeErrors
    {
        public static Error EmptyLetter => new("Grade.EmptyLetter", "Grade Letter cannot be empty.");
        public static Error InvalidLetter(string letter) => new("Grade.InvalidLetter", $"Invalid grade letter {letter}.");
        public static Error NegativeScore => new("Grade.NegativeScore", "Scores cannot be negative.");
        public static Error ExceededScore => new("Grade.ExceededScore", "Scores cannot exceed 100.");
    }
}
