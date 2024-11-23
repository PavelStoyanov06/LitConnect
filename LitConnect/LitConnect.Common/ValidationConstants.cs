namespace LitConnect.Common;

public static class ValidationConstants
{
    public static class User
    {
        public const int FirstNameMinLength = 2;
        public const int FirstNameMaxLength = 50;
        public const int LastNameMinLength = 2;
        public const int LastNameMaxLength = 50;
    }

    public static class Book
    {
        public const int TitleMinLength = 1;
        public const int TitleMaxLength = 100;
        public const int AuthorMinLength = 2;
        public const int AuthorMaxLength = 50;
        public const int DescriptionMaxLength = 1000;
        public const int GenreMaxLength = 50;
        public const int GenreMinLength = 2;
    }

    public static class BookClub
    {
        public const int NameMinLength = 3;
        public const int NameMaxLength = 50;
        public const int DescriptionMaxLength = 500;
    }

    public static class Discussion
    {
        public const int TitleMinLength = 5;
        public const int TitleMaxLength = 100;
        public const int ContentMaxLength = 2000;
    }

    public static class Meeting
    {
        public const int TitleMinLength = 5;
        public const int TitleMaxLength = 100;
        public const int DescriptionMaxLength = 500;
    }

    public static class Review
    {
        public const int ContentMaxLength = 1000;
        public const int MinRating = 1;
        public const int MaxRating = 5;
    }

    public static class Comment
    {
        public const int ContentMaxLength = 500;
    }
}