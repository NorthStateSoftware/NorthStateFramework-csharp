// Copyright 2004-2016, North State Software, LLC.  All rights reserved.

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
using System.Collections.Generic;
using System.IO;
using System.Security;

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents an xml element in an xml document.
    /// </summary>
    /// <remarks>
    /// This is a companion class to NSFXMLDocument.
    /// </remarks>
    public class NSFXMLElement
    {
        #region Public Constructors

        /// <summary>
        /// Creates an xml element.
        /// </summary>
        public NSFXMLElement()
        {
        }

        /// <summary>
        /// Creates an xml element.
        /// </summary>
        /// <param name="tag">The tag for the element.</param>
        public NSFXMLElement(NSFString tag)
        {
            Tag = tag;
        }

        /// <summary>
        /// Creates an xml element.
        /// </summary>
        /// <param name="tag">The tag for the element.</param>
        /// <param name="text">The text for the element.</param>
        public NSFXMLElement(NSFString tag, NSFString text)
        {
            Tag = tag;
            // Note: Trace data entries may not be valid XML for several reasons.  One of them
            // involves the use of invalid characters, such as <, >, ", etc, which are handled
            // by SecurityElement.Escape().
            Text = SecurityElement.Escape(text);
        }

        /// <summary>
        /// Creates an xml element.
        /// </summary>
        /// <param name="copyElement">The element to copy.</param>
        public NSFXMLElement(NSFXMLElement copyElement)
        {
            Tag = copyElement.Tag;
            Text = copyElement.Text;

            foreach (NSFXMLElement element in copyElement.childElements)
            {
                addChildElementBack(new NSFXMLElement(element));
            }
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        public NSFString Tag { get; set; }
        public NSFString Text { get; set; }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Adds an element to the back of the child element list.
        /// </summary>
        public void addChildElementBack(NSFXMLElement childElement)
        {
            childElement.parentElement = this;
            childElements.AddLast(childElement);
        }

        /// <summary>
        /// Adds an element to the front of the child element list.
        /// </summary>
        public void addChildElementFront(NSFXMLElement childElement)
        {
            childElement.parentElement = this;
            childElements.AddFirst(childElement);
        }

        /// <summary>
        /// Checks if the element contains any child elements.
        /// </summary>
        /// <returns>True if any child elements, false otherwise.</returns>
        public bool containsChildElement()
        {
            return childElements.Count != 0;
        }

        /// <summary>
        /// Checks if the element contains any child elements with the specified tag.
        /// </summary>
        /// <param name="childTag">The child element tag.</param>
        /// <returns>True if contains, false otherwise.</returns>
        public bool containsChildElement(NSFString childTag)
        {
            foreach (NSFXMLElement element in childElements)
            {
                if (element.Tag == childTag)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Deletes the specified child element.
        /// </summary>
        /// <param name="childElement">The element to delete.</param>
        public void deleteChildElement(NSFXMLElement childElement)
        {
            childElements.Remove(childElement);
        }

        /// <summary>
        /// Deletes the element at the back of the child element list.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool deleteChildElementBack()
        {
            if (childElements.Count == 0)
            {
                return false;
            }

            childElements.RemoveLast();

            return true;
        }

        /// <summary>
        /// Deletes the element at the front of the child element list.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool deleteChildElementFront()
        {
            if (childElements.Count == 0)
            {
                return false;
            }

            childElements.RemoveFirst();

            return true;
        }

        /// <summary>
        /// Gets the first child element with the specified tag.
        /// </summary>
        /// <param name="childTag">The child element to tag.</param>
        /// <returns>The child element, or NULL if none exist.</returns>
        public NSFXMLElement getChildElement(NSFString childTag)
        {
            foreach (NSFXMLElement element in childElements)
            {
                if (element.Tag == childTag)
                {
                    return element;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the last child element.
        /// </summary>
        /// <returns>The child element, or NULL if none exist.</returns>
        public NSFXMLElement getChildElementBack()
        {
            return childElements.Last.Value;
        }

        /// <summary>
        /// Gets the first child element.
        /// </summary>
        /// <returns>The child element, or NULL if none exist.</returns>
        public NSFXMLElement getChildElementFront()
        {
            return childElements.First.Value;
        }

        /// <summary>
        /// Gets the next child element after the specified child element.
        /// </summary>
        /// <param name="childElement">The child element before the returned child element.</param>
        /// <returns>The child element, or NULL if none exist.</returns>
        public NSFXMLElement getNextElement(NSFXMLElement childElement)
        {
            bool returnNext = false;

            foreach (NSFXMLElement element in childElements)
            {
                if (returnNext)
                {
                    return element;
                }

                if (element == childElement)
                {
                    returnNext = true;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the next child element after the specified child element, with the specified tag.
        /// </summary>
        /// <param name="childElement">The child element before the returned child element.</param>
        /// <param name="nextTag">The child element tag.</param>
        /// <returns>The child element, or NULL if none exist.</returns>
        public NSFXMLElement getNextElement(NSFXMLElement childElement, NSFString nextTag)
        {
            bool returnNext = false;

            foreach (NSFXMLElement element in childElements)
            {
                if (returnNext)
                {
                    if (element.Tag == nextTag)
                    {
                        return element;
                    }
                }
                else if (element == childElement)
                {
                    returnNext = true;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the number of child elements
        /// </summary>
        /// <returns>The number of child elements.</returns>
        public int getNumberOfChildElements()
        {
            return childElements.Count;
        }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        /// <returns>The child element, or NULL if none exist.</returns>
        public NSFXMLElement getParentElement()
        {
            return parentElement;
        }

        /// <summary>
        /// Gets the first parent element with the specified tag.
        /// </summary>
        /// <param name="parentTag">The parent element tag.</param>
        /// <returns>The parent element, or NULL if none exist.</returns>
        public NSFXMLElement getParentElement(NSFString parentTag)
        {
            if (parentElement == null)
            {
                return null;
            }

            if (parentElement.Tag == parentTag)
            {
                return parentElement;
            }
            else
            {
                return parentElement.getParentElement(parentTag);
            }
        }

        /// <summary>
        /// Loads the element data from a string buffer.  The buffer must be in valid XML format.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool loadBuffer(NSFString buffer)
        {
            int readPosition = 0;
            return loadBuffer(buffer, ref readPosition);
        }

        /// <summary>
        /// Saves the element to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void save(StreamWriter stream)
        {
            if (containsChildElement())
            {
                stream.WriteLine("<{0}>", Tag);

                foreach (NSFXMLElement element in childElements)
                {
                    element.save(stream);
                }

                stream.WriteLine("</{0}>", Tag);
            }
            else
            {
                stream.WriteLine("<{0}>{1}</{0}>", Tag, Text);
            }
        }

        #endregion Public Methods

        #region Private Fields, Events, and Properties

        private LinkedList<NSFXMLElement> childElements = new LinkedList<NSFXMLElement>();
        private NSFXMLElement parentElement;

        #endregion Private Fields, Events, and Properties

        #region Private Methods

        /// <summary>
        /// Loads the element data from a string.
        /// </summary>
        /// <param name="buffer">The string buffer to load from.</param>
        /// <param name="readPosition">The read position to start from.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <remarks>
        /// The data must be in valid XML format.
        /// </remarks>
        private bool loadBuffer(NSFString buffer, ref int readPosition)
        {
            // Warning: buffer pointer is left modified

            if (!parseStartTag(buffer, ref readPosition))
            {
                return false;
            }

            // Check if start tag is also an end tag
            if (Tag[Tag.Length - 1] == '/')
            {
                Tag = Tag.Remove(Tag.Length - 1, 1);
                return true;
            }

            if (!parseChildElements(buffer, ref readPosition))
            {
                return false;
            }

            if (childElements.Count == 0)
            {
                if (!parseText(buffer, ref readPosition))
                {
                    return false;
                }
            }

            if (!parseEndTag(buffer, ref readPosition))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses an element's attributes.
        /// </summary>
        /// <param name="buffer">The string buffer to parse.</param>
        /// <param name="readPosition">The read position to start from.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <remarks>
        /// This implementation does not support reading attribute values, therefore
        /// this is a dummy operation that simply looks for the end of the start tag
        /// </remarks>
        private bool parseAttributes(NSFString buffer, ref int readPosition)
        {
            // Read from buffer until end of tag
            while (readPosition < buffer.Length)
            {
                // Check for end of tag
                if (buffer[readPosition] == '>')
                {
                    ++readPosition;
                    return true;
                }

                ++readPosition;
            }

            return false;
        }

        /// <summary>
        /// Parses the element's child elements.
        /// </summary>
        /// <param name="buffer">The string buffer to parse.</param>
        /// <param name="readPosition">The read position to start from.</param>
        /// <returns>True if successful, false otherwise.</returns>
        private bool parseChildElements(NSFString buffer, ref int readPosition)
        {
            while (true)
            {
                if (!skipWhitespace(buffer, ref readPosition))
                {
                    return false;
                }

                if (buffer.Length < readPosition + 2)
                {
                    return false;
                }

                // Check for child elements
                if ((buffer[readPosition] == '<') && (buffer[readPosition + 1] != '/'))
                {
                    NSFXMLElement newElement = new NSFXMLElement();

                    if (!newElement.loadBuffer(buffer, ref readPosition))
                    {
                        return false;
                    }

                    addChildElementBack(newElement);

                    continue;
                }

                return true;
            }
        }

        /// <summary>
        /// Parses the element's end tag.
        /// </summary>
        /// <param name="buffer">The string buffer to parse.</param>
        /// <param name="readPosition">The read position to start from.</param>
        /// <returns>True if successful, false otherwise.</returns>
        private bool parseEndTag(NSFString buffer, ref int readPosition)
        {
            if (!skipWhitespace(buffer, ref readPosition))
            {
                return false;
            }

            if (buffer.Length < readPosition + 2)
            {
                return false;
            }

            // Check for start of end tag
            if ((buffer[readPosition] != '<') || (buffer[readPosition + 1] != '/'))
            {
                return false;
            }

            // Increment past start of end tag
            readPosition += 2;

            // Read end tag
            NSFString endTag = "";

            while (true)
            {
                // Check for end of buffer
                if (readPosition >= buffer.Length)
                {
                    return false;
                }

                // Check for end of end tag
                if (buffer[readPosition] == '>')
                {
                    ++readPosition;
                    break;
                }

                endTag += buffer[readPosition];

                ++readPosition;
            }

            // Verify end tag
            if (endTag != Tag)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses an element entity.
        /// </summary>
        /// <param name="buffer">The string buffer to parse.</param>
        /// <param name="readPosition">The read position to start from.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <remarks>
        /// The argument readPosition should be pointing to &amp; at beginning of entity
        /// </remarks>
        private bool parseEntity(NSFString buffer, ref int readPosition)
        {
            NSFString entityBuffer = "";

            //Read in entity, max length is 5
            for (int i = 0; i < 5; ++i)
            {
                // ReadPosition should be pointing to '&' at beginning of entity
                ++readPosition;

                entityBuffer += buffer[readPosition];

                // Check for end of entity
                if (buffer[readPosition] == ';')
                {
                    if (entityBuffer.Substring(0, 2) == "lt")
                    {
                        Text += '<';
                    }
                    else if (entityBuffer.Substring(0, 2) == "#x")
                    {
                        Text += System.Byte.Parse(entityBuffer.Substring(2, 1), System.Globalization.NumberStyles.HexNumber);
                    }
                    else if (entityBuffer.Substring(0, 1) == "#")
                    {
                        Text += System.Byte.Parse(entityBuffer.Substring(2, 1), System.Globalization.NumberStyles.Integer);
                    }
                    else  // Unrecognized entity
                    {
                        return false;
                    }

                    ++readPosition;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Parses an element's starting tag.
        /// </summary>
        /// <param name="buffer">The string buffer to parse.</param>
        /// <param name="readPosition">The read position to start from.</param>
        /// <returns>True if successful, false otherwise.</returns>
        private bool parseStartTag(NSFString buffer, ref int readPosition)
        {
            // Read from buffer until beginning of tag
            while (true)
            {
                // Check for end of buffer
                if (readPosition >= buffer.Length)
                {
                    return false;
                }

                // Check for beginning of tag
                if (buffer[readPosition] == '<')
                {
                    ++readPosition;

                    // Check for declaration or comment
                    if ((buffer[readPosition] == '?') || (buffer[readPosition] == '!'))
                    {
                        ++readPosition;
                        continue;
                    }
                    else  // It's a tag
                    {
                        break;
                    }
                }

                ++readPosition;
            }

            if (!skipWhitespace(buffer, ref readPosition))
            {
                return false;
            }

            while (true)
            {
                // Check for end of buffer
                if (readPosition >= buffer.Length)
                {
                    return false;
                }

                // Check for end of tag
                if (buffer[readPosition] == '>')
                {
                    ++readPosition;
                    return true;
                }

                if (Char.IsWhiteSpace(buffer[readPosition]))
                {
                    // WARNING - Current implementation does not save attributes!
                    if (!parseAttributes(buffer, ref readPosition))
                    {
                        return false;
                    }

                    return true;
                }

                Tag += buffer[readPosition];

                ++readPosition;
            }
        }

        /// <summary>
        /// Parses an element's text.
        /// </summary>
        /// <param name="buffer">The string buffer to parse.</param>
        /// <param name="readPosition">The read position to start from.</param>
        /// <returns>True if successful, false otherwise.</returns>
        private bool parseText(NSFString buffer, ref int readPosition)
        {
            // Warning: buffer pointer is left modified

            if (!skipWhitespace(buffer, ref readPosition))
            {
                return false;
            }

            // Read from buffer until new tag
            while (true)
            {
                // Check for end of buffer
                if (readPosition >= buffer.Length)
                {
                    return false;
                }

                // Check for beginning of end tag
                if (buffer[readPosition] == '<')
                {
                    return true;
                }

                // Check for entity
                // The parser only supports &lt, &#, and &#x
                if (buffer[readPosition] == '&')
                {
                    if (!parseEntity(buffer, ref readPosition))
                    {
                        return false;
                    }

                    continue;
                }

                Text += buffer[readPosition];

                ++readPosition;
            }
        }

        /// <summary>
        /// Skips over an element's whitespace.
        /// </summary>
        /// <param name="buffer">The string buffer to parse.</param>
        /// <param name="readPosition">The read position to start from.</param>
        /// <returns>True if successful, false otherwise.</returns>
        private bool skipWhitespace(NSFString buffer, ref int readPosition)
        {
            while (true)
            {
                // Check for end of buffer
                if (readPosition >= buffer.Length)
                {
                    return false;
                }

                if (!Char.IsWhiteSpace(buffer[readPosition]))
                {
                    return true;
                }

                ++readPosition;
            }
        }

        #endregion Private Methods
    }
}