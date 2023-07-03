using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MongoDB.Bson;
using System;

namespace Milestones.Models
{
    /// <summary>
    /// Project entity class.
    /// </summary>
    public class Project
    {
        [BsonId]
        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        [BsonRequired]
        [BsonElement("Name")]
        [MinLength(1), MaxLength(255)]
        public string Name { get; set; }


        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonDefaultValue("03.11.1971 00:00:00")]
        public DateTime Start { get; set; } = DateTime.Parse("03.11.1971 00:00:00");


        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonDefaultValue("03.11.1971 23:59:59")]
        public DateTime End { get; set; } = DateTime.Parse("03.11.1971 23:59:59");


        [BsonRequired]
        [BsonElement("Milestones")]
        public List<Milestone> Milestones { get; set; } = new List<Milestone>();


        /// <inheritdoc/>
        public override bool Equals(object obj) 
        { 
            if (obj == null) return false;
            Project project = (Project) obj;
            bool found = project != null &&
                        this.Name.Equals(project.Name) &&
                        this.Start.Millisecond == project.Start.Millisecond &&
                        this.End.Millisecond == project.End.Millisecond &&
                        this.Milestones.Count == project.Milestones.Count;
            if (!found) return false;
            for (int milestoneIndex = 0; milestoneIndex < project.Milestones.Count; milestoneIndex++)
                found &= this.Milestones[milestoneIndex] == project.Milestones[milestoneIndex];
            return found;
        }


        /// <inheritdoc/>
        public override int GetHashCode() 
        { 
            return HashCode.Combine(this.Name.Length, this.Start.Millisecond, this.End.Millisecond);
        }


        /// <summary>
        /// Checks the equility of two given projects
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns><c>true</c> if they are equals, <c>false</c> if they are not</returns>
        public static bool operator ==(Project p1, Project p2)
        {
            return p1.Equals(p2);
        }


        /// <summary>
        /// Checks the not equility of two given projects
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns><c>true</c> if they are not equals, <c>false</c> if they are</returns>
        public static bool operator !=(Project p1, Project p2)
        {
            return !(p1.Equals(p2));
        }


        /// <summary>
        /// Checks if all of properties given from JSON-Object
        /// </summary>
        /// <param name="p"></param>
        /// <returns>True if all properties given</returns>
        public static bool IsMissingProperties(Project p)
        {
            return p == null ||
                    p.Name == null ||
                    p.Milestones == null;
        }
    }
}