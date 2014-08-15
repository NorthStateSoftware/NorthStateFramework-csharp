// Copyright 2004-2014, North State Software, LLC.  All rights reserved.

// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.ComponentModel;

using NSFId = System.Int64;
using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
#if Doxygen

    // Workaround for doxygen documentation generation limitation.
    // Doxygen does not understand aliases created with "using" keyword.
    /// <summary>
    /// An alias for System.Int64.
    /// </summary>
    /// <remarks>
    /// The alias is created by placing the line "using NSFId = System.Int64;" at the top of the referencing file.
    /// </remarks>
    class NSFId
    {
    }

    /// <summary>
    /// An alias for System.String.
    /// </summary>
    /// <remarks>
    /// The alias is created by placing the line "using NSFString = System.String;" at the top of the referencing file.
    /// </remarks>
    class NSFString
    {
    }

#endif

    /// <summary>
    /// Represents an interface for objects containing a name.
    /// </summary>
    public interface INSFNamedObject
    {
        #region Properties

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        NSFString Name { get; }

        #endregion Properties
    }

    /// <summary>
    /// Represents an interface for objects containing an id number.
    /// </summary>
    public interface INSFIdObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id number of the object.
        /// </summary>
        NSFId Id { get; }

        #endregion Properties
    }

    /// <summary>
    /// Represents an object containing a uniquely numbered id.
    /// </summary>
    /// <remarks>
    /// There are 2^64 unique ids available.
    /// </remarks>
    public class NSFUniquelyNumberedObject
    {
        #region Public Constructors

        /// <summary>
        /// Creates a uniquely numbered object.
        /// </summary>
        public NSFUniquelyNumberedObject()
        {
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets or sets the unique id of the object.
        /// </summary>
        public NSFId UniqueId
        {
            get { return uniqueId; }
        }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Gets the next unique id.
        /// </summary>
        /// <remarks>
        /// There are 2^64 unique ids available.
        /// </remarks>
        public static NSFId getNextUniqueId()
        {
            lock (uniquelyNumberedObjectMutex)
            {
                return (++nextUniqueId);
            }
        }

        /// <summary>
        /// Checks if this object is the same as another object.
        /// </summary>
        /// <param name="other">The other object to compare against.</param>
        /// <returns>True if the objects have the same unique id, false otherwise.</returns>
        public bool isSameObject(NSFUniquelyNumberedObject other)
        {
            return (uniqueId == other.uniqueId);
        }

        #endregion Public Methods

        #region Private Fields, Events, and Properties

        private static object uniquelyNumberedObjectMutex = new object();
        private static NSFId nextUniqueId = 0;

        private NSFId uniqueId = getNextUniqueId();

        #endregion Private Fields, Events, and Properties
    }

    /// <summary>
    /// Represents an object containing unique id and name
    /// </summary>
    public class NSFTaggedObject : NSFUniquelyNumberedObject, INSFNamedObject
    {
        #region Public Constructors

        /// <summary>
        /// Creates an object with a unique id and a name.
        /// </summary>
        /// <param name="name">The user defined name for the object.</param>
        public NSFTaggedObject(NSFString name)
        {
            Name = name;
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        public NSFString Name { get; set; }

        #endregion Public Fields, Events, and Properties
    }
}