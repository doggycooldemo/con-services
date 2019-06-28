﻿using System;
using System.Collections;
using System.Collections.Generic;
using Attribute = VSS.Hydrology.WebApi.DXF.Entities.Attribute;

namespace VSS.Hydrology.WebApi.DXF.Collections
{
    /// <summary>
    /// Represents a collection of <see cref="Attribute">Attributes</see>.
    /// </summary>
    public sealed class AttributeCollection :
        IReadOnlyList<Attribute>
    {
        #region private fields

        private readonly List<Attribute> innerArray;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of <c>AttributeCollection</c> with the specified collection of attributes.
        /// </summary>
        public AttributeCollection()
        {
            this.innerArray = new List<Attribute>();
        }

        /// <summary>
        /// Initializes a new instance of <c>AttributeCollection</c> with the specified collection of attributes.
        /// </summary>
        /// <param name="attributes">The collection of attributes from which build the dictionary.</param>
        public AttributeCollection(IEnumerable<Attribute> attributes)
        {
            if (attributes == null)
                throw new ArgumentNullException(nameof(attributes));
            this.innerArray = new List<Attribute>(attributes);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the number of object contained in the collection.
        /// </summary>
        public int Count
        {
            get { return this.innerArray.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public static bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the attribute at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the element to get or set.</param>
        /// <returns>The object at the specified index.</returns>
        public Attribute this[int index]
        {
            get { return this.innerArray[index]; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Determines whether an attribute is in the collection.
        /// </summary>
        /// <param name="item">The attribute to locate in the collection.</param>
        /// <returns>True if attribute is found in the collection; otherwise, false.</returns>
        public bool Contains(Attribute item)
        {
            return this.innerArray.Contains(item);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array"> The one-dimensional System.Array that is the destination of the elements copied from the collection. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Attribute[] array, int arrayIndex)
        {
            this.innerArray.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Searches for the specified attribute and returns the zero-based index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="item">The attribute to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire collection, if found; otherwise, –1.</returns>
        public int IndexOf(Attribute item)
        {
            return this.innerArray.IndexOf(item);
        }

        /// <summary>
        /// Searches for the first occurrence attribute with the specified attribute definition tag within the entire collection
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>The first occurrence of the attribute with the specified attribute definition tag within the entire collection.</returns>
        public Attribute AttributeWithTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return null;
            foreach (Attribute att in this.innerArray)
            {
                if (att.Definition != null)
                    if (string.Equals(tag, att.Tag, StringComparison.OrdinalIgnoreCase))
                        return att;
            }

            return null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<Attribute> GetEnumerator()
        {
            return this.innerArray.GetEnumerator();
        }

        #endregion

        #region private methods

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
