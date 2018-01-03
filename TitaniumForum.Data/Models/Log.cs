namespace TitaniumForum.Data.Models
{
    using System;

    public class Log
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string TableName { get; set; }

        public LogType LogType { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}