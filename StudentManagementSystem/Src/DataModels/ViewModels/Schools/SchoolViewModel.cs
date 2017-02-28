namespace StudentManagementSystem.Data.ViewModels.Schools
{
    using System;
    using System.Collections.Generic;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    using TypeLite;

    [TsClass]
    public class SchoolViewModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime StartDate { get; set; }

        public string Name { get; set; }

        public SchoolAddress Address { get; set; }

        public List<CourseSchedule> Courses { get; set; }
    }

    [TsClass]
    public class SchoolAddress
    {
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string[] Links { get; set; }
    }

    [TsClass]
    public class CourseSchedule
    {
        public string course_id { get; set; }

        public string Day { get; set; }
        public string Time { get; set; }
    }
}
