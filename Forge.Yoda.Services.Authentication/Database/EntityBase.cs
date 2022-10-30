using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Forge.Yoda.Services.Authentication.Database
{

    [Serializable]
    public abstract class EntityBase : IEquatable<EntityBase>
    {

        private bool mHashcodeCreated = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mHashcode = 0;

        protected EntityBase()
        {
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        [Display(Name = "DbId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(EntityBase? other)
        {
            return Equals((object?)other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            EntityBase other = (EntityBase)obj;
            if (!Id.Equals(other.Id))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override int GetHashCode()
        {
            if (!mHashcodeCreated)
            {
                if (Id == 0)
                {
                    mHashcode = base.GetHashCode();
                }
                else
                {
                    mHashcode = 9 * Id.GetHashCode();
                    //mHashcode = 7 ^ mHashcode + this.EntityCreationTime.GetHashCode();
                }
                mHashcodeCreated = true;
            }
            return mHashcode;
        }

    }

}
