using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MongoDB.Bson;
using System;

namespace Milestones.Models
{
    /// <summary>
    /// Milestone entity class.
    /// </summary>
    public class Milestone
    {
        [BsonId]
        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProjectReference { get; set; }


        [BsonRequired]
        [MinLength(1), MaxLength(255)]
        [BsonElement("Name")]
        public string Name { get; set; }


        [BsonRequired]
        [MinLength(1), MaxLength(1000)]
        [BsonElement("Description")]
        public string Description { get; set; }


        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonDefaultValue("01.01.0001 00:00:00")]
        public DateTime Start { get; set; }


        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonDefaultValue("01.01.0001 00:00:00")]
        public DateTime End { get; set; }


        [BsonRequired]
        [BsonRepresentation(BsonType.Boolean)]
        public bool? IsCompleted { get; set; } = null;


        [BsonRequired]
        [BsonElement("Members")]
        public List<Member> Members { get; set; }


        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Milestone milestone = (Milestone) obj;
            bool found = milestone != null &&
                         this.Name.Equals(milestone.Name) &&
                         this.ProjectReference.Equals(milestone.ProjectReference) &&
                         this.Start.Millisecond == milestone.Start.Millisecond &&
                         this.End.Millisecond == milestone.End.Millisecond &&
                         this.Description.Equals(milestone.Description) &&
                         this.IsCompleted.Equals(milestone.IsCompleted) &&
                         this.Members.Count == milestone.Members.Count;
            if (!found) return false;
            for (int memberIndex = 0; memberIndex < this.Members.Count; memberIndex++)
                found &= this.Members[memberIndex] == milestone.Members[memberIndex];
            return found;
        }


        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Name.Length, this.ProjectReference.Length, this.Start.Millisecond, this.End.Millisecond, this.Description.Length, this.Members.Count);
        }


        /// <summary>
        /// Checks the equility of two given milestones
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns><c>true</c> if they are equals, <c>false</c> if they are not</returns>
        public static bool operator==(Milestone m1, Milestone m2)
        {
            return m1.Equals(m2) && (m1.GetHashCode() == m2.GetHashCode());
        }


        /// <summary>
        /// Checks the not equility of two given milestones
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns><c>true</c> if they are not equals, <c>false</c> if they are</returns>
        public static bool operator!=(Milestone m1, Milestone m2)
        {
            return !(m1.Equals(m2) && (m1.GetHashCode() == m2.GetHashCode()));
        }


        /// <summary>
        /// Checks if all of properties given from JSON-Object
        /// </summary>
        /// <param name="m"></param>
        /// <returns>True if all properties given</returns>
        public static bool IsMissingProperties (Milestone m)
        {
            return  m == null || 
                    m.ProjectReference == null || 
                    m.Name == null || 
                    m.Description == null || 
                    m.Members == null || 
                    m.IsCompleted == null ||
                    m.Start.Equals(DateTime.Parse("01.01.0001 00:00:00")) || 
                    m.End.Equals(DateTime.Parse("01.01.0001 00:00:00"));
        }
    }
}