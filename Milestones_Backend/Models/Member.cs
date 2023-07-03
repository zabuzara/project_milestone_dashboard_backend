using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace Milestones.Models
{
    /// <summary>
    /// Member entity class.
    /// </summary>
    public class Member
    {
        [BsonId]
        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        [BsonRequired]
        [MinLength(1), MaxLength(255)]
        [BsonElement("Firstname")]
        public string Firstname { get; set; }


        [BsonRequired]
        [MinLength(1), MaxLength(255)]
        [BsonElement("Lastname")]
        public string Lastname { get; set; }


        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Member member = (Member) obj;
            return  member != null &&
                    this.Firstname != null &&
                    this.Lastname != null &&
                    this.Firstname.Equals(member.Firstname) &&
                    this.Lastname.Equals(member.Lastname);
        }


        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Firstname.Length, this.Lastname.Length);
        }


        /// <summary>
        /// Checks the equility of two given members
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns><c>true</c> if they are equals, <c>false</c> if they are not</returns>
        public static bool operator==(Member m1, Member m2)
        {
            return m1.Equals(m2) && (m1.GetHashCode() == m2.GetHashCode());
        }


        /// <summary>
        /// Checks the not equility of two given members
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns><c>true</c> if they are not equals, <c>false</c> if they are</returns>
        public static bool operator!=(Member m1, Member m2)
        {
            return !(m1.Equals(m2) && (m1.GetHashCode() == m2.GetHashCode()));
        }


        /// <summary>
        /// Checks if all of properties given from JSON-Object
        /// </summary>
        /// <param name="m"></param>
        /// <returns>True if all properties given</returns>
        public static bool IsMissingProperties (Member m)
        {
            return  m == null || 
                    m.Firstname == null || 
                    m.Lastname == null;
        }
    }
}