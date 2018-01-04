namespace TitaniumForum.Data
{
    public static class DataConstants
    {
        public static class CategoryConstants
        {
            public const int MinNameLength = 3;
            public const int MaxNameLength = 20;
        }

        public static class SubCategotyConstants
        {
            public const int MinNameLength = 3;
            public const int MaxNameLength = 20;
        }

        public static class QuestionConstants
        {
            public const int MinTitleLength = 3;
            public const int MaxTitleLength = 150;
            public const int MinContentLength = 3;
        }

        public static class AnswerConstants
        {
            public const int MinContentLength = 3;
        }

        public static class CommentConstants
        {
            public const int MinContentLength = 3;
        }

        public static class TagConstants
        {
            public const int MinNameLength = 3;
            public const int MaxNameLength = 10;
        }

        public static class UserConstants
        {
            public const int MaxProfileImageSize = 100 * 1024;
        }
    }
}