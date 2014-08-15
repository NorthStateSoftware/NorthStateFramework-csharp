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
using System.IO;
using System.IO.Compression;

using NSFString = System.String;

using UInt8 = System.Byte;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a simple in-memory xml document editor.
    /// </summary>
    /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    public class NSFXMLDocument
    {
        #region Nested Types

        /// <summary>
        /// Represents xml document error status.
        /// </summary>
        public enum ErrorStatus
        {
            NoError, EmptyDocument, EmptyElement, ParseError, FileError, BadValue
        }

        #endregion Nested Types

        #region Public Constructors

        /// <summary>
        /// Creates an xml document.
        /// </summary>
        public NSFXMLDocument()
        {
        }

        /// <summary>
        /// Creates an xml document.
        /// </summary>
        /// <param name="rootTag">The string for the root element of the document.</param>
        public NSFXMLDocument(NSFString rootTag)
        {
            addRootElement(rootTag);
        }

        /// <summary>
        /// Creates an xml document.
        /// </summary>
        /// <param name="copyDocument">The document to copy.</param>
        public NSFXMLDocument(NSFXMLDocument copyDocument)
        {
            loadDocument(copyDocument);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Adds an element to the back of the current element's child elements
        /// </summary>
        /// <param name="childTag">The tag for the element.</param>
        public void addChildElementBack(NSFString childTag)
        {
            if (rootElement == null)
            {
                rootElement = new NSFXMLElement(childTag);
                currentElement = rootElement;
                bookmarkElement = rootElement;
            }
            else
            {
                currentElement.addChildElementBack(new NSFXMLElement(childTag));
            }
        }

        /// <summary>
        /// Adds an element to the back of the current element's child elements
        /// </summary>
        /// <param name="childTag">The tag for the element.</param>
        /// <param name="childText">The text for the element.</param>
        public void addChildElementBack(NSFString childTag, NSFString childText)
        {
            if (rootElement == null)
            {
                rootElement = new NSFXMLElement(childTag, childText);
                currentElement = rootElement;
                bookmarkElement = rootElement;
            }
            else
            {
                currentElement.addChildElementBack(new NSFXMLElement(childTag, childText));
            }
        }

        /// <summary>
        /// Adds an element to the back of the current element's child elements
        /// </summary>
        /// <param name="childTag">The tag for the element.</param>
        /// <param name="value">The value (converted to string) for the element.</param>
        public void addChildElementBack<ValueType>(NSFString childTag, ValueType value)
        {
            String childText = value.ToString();
            addChildElementBack(childTag, childText);
        }

        /// <summary>
        /// Adds an xml document to the back of the current element's child elements
        /// </summary>
        /// <param name="document">The document to add.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool addChildElementBack(NSFXMLDocument document)
        {
            if (document.rootElement == null)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            if (rootElement == null)
            {
                rootElement = new NSFXMLElement(document.rootElement);
                currentElement = rootElement;
                bookmarkElement = rootElement;
            }
            else
            {
                currentElement.addChildElementBack(new NSFXMLElement(document.rootElement));
            }

            return true;
        }

        /// <summary>
        /// Adds an element to the back of the current element's child elements
        /// </summary>
        /// <param name="childElement">The element to add.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool addChildElementBack(NSFXMLElement childElement)
        {
            if (rootElement == null)
            {
                rootElement = childElement;
                currentElement = rootElement;
                bookmarkElement = rootElement;
                return true;
            }

            currentElement.addChildElementBack(childElement);
            return true;
        }

        /// <summary>
        /// Adds an element to the front of the current element's child elements
        /// </summary>
        /// <param name="childTag">The tag for the element.</param>
        public void addChildElementFront(NSFString childTag)
        {
            if (rootElement == null)
            {
                rootElement = new NSFXMLElement(childTag);
                currentElement = rootElement;
                bookmarkElement = rootElement;
            }
            else
            {
                currentElement.addChildElementFront(new NSFXMLElement(childTag));
            }
        }

        /// <summary>
        /// Adds an element to the front of the current element's child elements
        /// </summary>
        /// <param name="childTag">The tag for the element.</param>
        /// <param name="childText">The text for the element.</param>
        public void addChildElementFront(NSFString childTag, NSFString childText)
        {
            if (rootElement == null)
            {
                rootElement = new NSFXMLElement(childTag, childText);
                currentElement = rootElement;
                bookmarkElement = rootElement;
            }
            else
            {
                currentElement.addChildElementFront(new NSFXMLElement(childTag, childText));
            }
        }

        /// <summary>
        /// Adds an element to the front of the current element's child elements
        /// </summary>
        /// <param name="childTag">The tag for the element.</param>
        /// <param name="value">The value (converted to string) for the element.</param>
        public void addChildElementFront<ValueType>(NSFString childTag, ValueType value)
        {
            String childText = value.ToString();
            addChildElementBack(childTag, childText);
        }

        /// <summary>
        /// Adds an xml document to the front of the current element's child elements
        /// </summary>
        /// <param name="document">The document to add.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool addChildElementFront(NSFXMLDocument document)
        {
            if (document.rootElement == null)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            if (rootElement == null)
            {
                rootElement = new NSFXMLElement(document.rootElement);
                currentElement = rootElement;
                bookmarkElement = rootElement;
            }
            else
            {
                currentElement.addChildElementFront(new NSFXMLElement(document.rootElement));
            }

            return true;
        }

        /// <summary>
        /// Adds an element to the front of the current element's child elements
        /// </summary>
        /// <param name="childElement">The element to add.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool addChildElementFront(NSFXMLElement childElement)
        {
            if (rootElement == null)
            {
                rootElement = childElement;
                currentElement = rootElement;
                bookmarkElement = rootElement;
                return true;
            }

            currentElement.addChildElementFront(childElement);
            return true;
        }

        /// <summary>
        /// Adds a root element and sets the current element and bookmark element to the root element.
        /// </summary>
        public void addRootElement(NSFString rootTag)
        {
            NSFXMLElement newElement = new NSFXMLElement(rootTag, String.Empty);

            if (rootElement != null)
            {
                newElement.addChildElementFront(rootElement);
            }

            rootElement = newElement;
            bookmarkElement = rootElement;
            currentElement = rootElement;
        }

        /// <summary>
        /// Checks if at the bookmark element.
        /// </summary>
        /// <returns>True if at bookmark element, false otherwise.</returns>
        public bool atBookmarkElement()
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            return (currentElement == bookmarkElement);
        }

        /// <summary>
        /// Checks if at the root element.
        /// </summary>
        /// <returns>True if at root element, false otherwise.</returns>
        public bool atRootElement()
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            return (currentElement == rootElement);
        }

        /// <summary>
        /// Checks if the current elements contains a child element.
        /// </summary>
        /// <returns>True if current element contains a child element, false otherwise.</returns>
        public bool containsChildElement()
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            return currentElement.containsChildElement();
        }

        /// <summary>
        /// Checks if the current elements contains a child element with the specified tag.
        /// </summary>
        /// <param name="childTag">The tag for the child element.</param>
        /// <returns>True if current element contains a child element with the specified tag, false otherwise.</returns>
        public bool containsChildElement(NSFString childTag)
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            return currentElement.containsChildElement(childTag);
        }

        /// <summary>
        /// Deletes the current element's last child element.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool deleteChildElementBack()
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            if (bookmarkElement == currentElement.getChildElementBack())
            {
                bookmarkElement = null;
            }

            return currentElement.deleteChildElementBack();
        }

        /// <summary>
        /// Deletes the current element's first child element.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool deleteChildElementFront()
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            if (bookmarkElement == currentElement.getChildElementFront())
            {
                bookmarkElement = null;
            }

            return currentElement.deleteChildElementFront();
        }

        /// <summary>
        /// Deletes the current element.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool deleteCurrentElement()
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            if (currentElement == rootElement)
            {
                rootElement = null;
                bookmarkElement = null;
                currentElement = null;

                return true;
            }

            if (bookmarkElement == currentElement)
            {
                bookmarkElement = null;
            }

            NSFXMLElement deleteElement = currentElement;
            currentElement = currentElement.getParentElement();
            currentElement.deleteChildElement(deleteElement);

            return true;
        }

        /// <summary>
        /// Gets the boolean value of the child element with the specified tag.
        /// </summary>
        /// <param name="childTag">The child element's tag.</param>
        /// <param name="value">The child element's boolean value.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <remarks>
        /// This operation looks for (case insensitive): "true", "t", or "1" for true; "false", "f", or "0" for false.
        /// </remarks>
        public bool getChildElementBoolean(NSFString childTag, ref bool value)
        {
            errorStatus = ErrorStatus.NoError;

            if (jumpToChildElement(childTag))
            {
                getCurrentElementBoolean(ref value);
                jumpToParentElement();
                return true;
            }
            else
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }
        }

        /// <summary>
        /// Gets the boolean value of the child element with the specified tag or the default value if no child found.
        /// </summary>
        /// <param name="childTag">The child element's tag.</param>
        /// <param name="value">The child element's boolean value.</param>
        /// <param name="defaultValue">The default value returned in value argument if child not found.</param>
        /// <remarks>
        /// This operation looks for (case insensitive): "true", "t", or "1" for true; "false", "f", or "0" for false.
        /// </remarks>
        public void getChildElementBoolean(NSFString childTag, ref bool value, bool defaultValue)
        {
            if (!getChildElementBoolean(childTag, ref value))
            {
                value = defaultValue;
            }
        }

        /// <summary>
        /// Gets the text of the child element with the specified tag.
        /// </summary>
        /// <param name="childTag">The child element's tag.</param>
        /// <param name="text">The child element's text.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool getChildElementText(NSFString childTag, NSFString text)
        {
            errorStatus = ErrorStatus.NoError;

            if (jumpToChildElement(childTag))
            {
                getCurrentElementText(text);
                jumpToParentElement();
                return true;
            }
            else
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }
        }

        /// <summary>
        /// Gets the text of the child element with the specified tag or the default text if no child found.
        /// </summary>
        /// <param name="childTag">The child element's tag.</param>
        /// <param name="text">The child element's text.</param>
        /// <param name="defaultText">The default text returned in text argument if child not found.</param>
        public void getChildElementText(NSFString childTag, NSFString text, NSFString defaultText)
        {
            if (!getChildElementText(childTag, text))
            {
                text = defaultText;
            }
        }

        /// <summary>
        /// Gets the numeric value of the child element with the specified tag.
        /// </summary>
        /// <param name="childTag">The child element's tag.</param>
        /// <param name="value">The child element's value.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool getChildElementValue<ValueType>(NSFString childTag, ref ValueType value) where ValueType : IConvertible
        {
            errorStatus = ErrorStatus.NoError;

            if (jumpToChildElement(childTag))
            {
                getCurrentElementValue(ref value);
                jumpToParentElement();
                return true;
            }
            else
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }
        }

        /// <summary>
        /// Gets the numeric value of the child element with the specified tag or the default value if no child found.
        /// </summary>
        /// <param name="childTag">The child element's tag.</param>
        /// <param name="value">The child element's value.</param>
        /// <param name="defaultValue">The default value returned in value argument if child not found.</param>
        public void getChildElementValue<ValueType>(NSFString childTag, ref ValueType value, ValueType defaultValue) where ValueType : IConvertible
        {
            if (!getChildElementValue<ValueType>(childTag, ref value))
            {
                value = defaultValue;
            }
        }

        /// <summary>
        /// Gets the current element's boolean value.
        /// </summary>
        /// <param name="value">The current element's boolean value.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <remarks>
        /// This operation looks for (case insensitive): "true", "t", or "1" for true; "false", "f", or "0" for false.
        /// </remarks>
        public bool getCurrentElementBoolean(ref bool value)
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            NSFString booleanString = currentElement.Text;

            booleanString = booleanString.ToLower();

            if ((booleanString == "true") || (booleanString == "t") || (booleanString == "1"))
            {
                value = true;
                return true;
            }
            else if ((booleanString == "false") || (booleanString == "f") || (booleanString == "0"))
            {
                value = false;
                return true;
            }

            errorStatus = ErrorStatus.BadValue;
            return false;
        }

        /// <summary>
        /// Gets the current element's tag.
        /// </summary>
        /// <param name="tag">The current element's tag.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool getCurrentElementTag(NSFString tag)
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            tag = currentElement.Tag;

            return true;
        }

        /// <summary>
        /// Gets the current element's text.
        /// </summary>
        /// <param name="text">The current element's text.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool getCurrentElementText(NSFString text)
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            text = currentElement.Text;

            return true;
        }

        /// <summary>
        /// Gets the current element's numeric value.
        /// </summary>
        /// <param name="value">The current element's value.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool getCurrentElementValue<ValueType>(ref ValueType value) where ValueType : IConvertible
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            try
            {
                value = (ValueType)Convert.ChangeType(currentElement.Text, typeof(ValueType));
            }
            catch
            {
                errorStatus = ErrorStatus.ParseError;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the error status of the last operation.
        /// </summary>
        /// <remarks>
        /// Boolean methods that return false set the error status of the document.
        /// </remarks>
        public ErrorStatus getErrorStatus()
        {
            return errorStatus;
        }

        /// <summary>
        /// Gets the number of child elements.
        /// </summary>
        /// <param name="numberOfChildElements">The number of child elements.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool getNumberOfChildElements(out int numberOfChildElements)
        {
            numberOfChildElements = 0;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            numberOfChildElements = currentElement.getNumberOfChildElements();
            return true;
        }

        /// <summary>
        /// Moves the current element pointer to the bookmarked element.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        /// <remarks>
        /// The bookmark provides a mechanism for jumping back to a specific element in the document.
        /// </remarks>
        public bool jumpToBookmarkElement()
        {
            errorStatus = ErrorStatus.NoError;

            if (bookmarkElement == null)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            currentElement = bookmarkElement;

            return true;
        }

        /// <summary>
        /// Moves the current element pointer to the first child element with the specified tag.
        /// </summary>
        /// <param name="childTag">The child tag to find.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool jumpToChildElement(NSFString childTag)
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            NSFXMLElement childElement = currentElement.getChildElement(childTag);

            if (childElement == null)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            currentElement = childElement;

            return true;
        }

        /// <summary>
        /// Moves the current element pointer to the last child element of the current element.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool jumpToChildElementBack()
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            NSFXMLElement childElement = currentElement.getChildElementBack();

            if (childElement == null)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            currentElement = childElement;

            return true;
        }

        /// <summary>
        /// Moves the current element pointer to the first child element of the current element.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool jumpToChildElementFront()
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            NSFXMLElement childElement = currentElement.getChildElementFront();

            if (childElement == null)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            currentElement = childElement;

            return true;
        }

        /// <summary>
        /// Moves the current element pointer to the next element at the same level.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool jumpToNextElement()
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            if (currentElement == rootElement)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            NSFXMLElement nextElement = currentElement.getParentElement().getNextElement(currentElement);

            if (nextElement == null)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            currentElement = nextElement;

            return true;
        }

        /// <summary>
        /// Moves the current element pointer to the next element with the specified tag at the same level.
        /// </summary>
        /// <param name="nextTag">The tag to find.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool jumpToNextElement(NSFString nextTag)
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            if (currentElement == rootElement)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            NSFXMLElement nextElement = currentElement.getParentElement().getNextElement(currentElement, nextTag);

            if (nextElement == null)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            currentElement = nextElement;

            return true;
        }

        /// <summary>
        /// Moves the current element pointer to the parent of the current element.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool jumpToParentElement()
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            if (currentElement == rootElement)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            currentElement = currentElement.getParentElement();

            return true;
        }

        /// <summary>
        /// Moves the current element pointer to the parent of the current element with the specified tag.
        /// </summary>
        /// <param name="parentTag">The tag to find.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool jumpToParentElement(NSFString parentTag)
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            NSFXMLElement parentElement = currentElement.getParentElement(parentTag);

            if (parentElement == null)
            {
                errorStatus = ErrorStatus.EmptyElement;
                return false;
            }

            currentElement = parentElement;

            return true;
        }

        /// <summary>
        /// Moves the current element pointer to the root element.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool jumpToRootElement()
        {
            errorStatus = ErrorStatus.NoError;

            currentElement = rootElement;
            return true;
        }

        /// <summary>
        /// Populates the document with xml formatted text in the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to load.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool loadBuffer(NSFString buffer)
        {
            errorStatus = ErrorStatus.NoError;

            rootElement = null;
            bookmarkElement = null;
            currentElement = null;

            rootElement = new NSFXMLElement();

            if (!rootElement.loadBuffer(buffer))
            {
                rootElement = null;
                errorStatus = ErrorStatus.ParseError;
                return false;
            }

            bookmarkElement = rootElement;
            currentElement = rootElement;

            return true;
        }

        /// <summary>
        /// Populates the document with xml from specified xml document.
        /// </summary>
        /// <param name="copyDocument">The file to copy.</param>
        public void loadDocument(NSFXMLDocument copyDocument)
        {
            rootElement = null;
            bookmarkElement = null;
            currentElement = null;

            if (copyDocument.rootElement != null)
            {
                rootElement = new NSFXMLElement(copyDocument.rootElement);
                bookmarkElement = rootElement;
                currentElement = rootElement;
            }
        }

        /// <summary>
        /// Populates the document with xml formatted text in the specified file.
        /// </summary>
        /// <param name="fileName">The file to load.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool loadFile(NSFString fileName)
        {
            errorStatus = ErrorStatus.NoError;

            rootElement = null;
            bookmarkElement = null;
            currentElement = null;

            StreamReader file;
            try
            {
                file = new StreamReader(fileName);
            }
            catch
            {
                errorStatus = ErrorStatus.FileError;
                return false;
            }

            String fileString = file.ReadToEnd();

            bool returnValue = loadBuffer(fileString);

            file.Close();

            return returnValue;
        }

        /// <summary>
        /// Sets the current and bookmark element to the root element.
        /// </summary>
        public void reset()
        {
            bookmarkElement = rootElement;
            currentElement = rootElement;
        }

        /// <summary>
        /// Saves the xml document to the specified file name.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool save(NSFString fileName)
        {
            errorStatus = ErrorStatus.NoError;

            if (rootElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            using (StreamWriter file = new StreamWriter(fileName, false))
            {
                rootElement.save(file);
            }

            return true;
        }

        /// <summary>
        /// Sets the bookmark element to the current element.
        /// </summary>
        /// <remarks>
        /// The bookmark provides a mechanism for jumping back to a specific element in the document.
        /// </remarks>
        public void setBookmarkElement()
        {
            bookmarkElement = currentElement;
        }

        /// <summary>
        /// Sets the text of the specified child element or creates a child element if no match is found.
        /// </summary>
        /// <param name="childTag">The child element's tag.</param>
        /// <param name="text">The child element's text.</param>
        public void setChildElementText(NSFString childTag, NSFString text)
        {
            if (jumpToChildElement(childTag))
            {
                currentElement.Text = text;
                jumpToParentElement();
            }
            else
            {
                addChildElementBack(childTag, text);
            }
        }

        /// <summary>
        /// Sets the text of the specified child element to the specified numeric value, converted to a string, or creates a child element if no match is found.
        /// </summary>
        /// <param name="childTag">The child element's tag.</param>
        /// <param name="value">The child element's value.</param>
        public void setChildElementValue<ValueType>(NSFString childTag, ValueType value) where ValueType : IConvertible
        {
            if (jumpToChildElement(childTag))
            {
                setCurrentElementValue(value);
                jumpToParentElement();
            }
            else
            {
                addChildElementBack(childTag, value);
            }
        }

        /// <summary>
        /// Sets the current element's tag.
        /// </summary>
        /// <param name="tag">The current element's tag.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool setCurrentElementTag(NSFString tag)
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            currentElement.Tag = tag;

            return true;
        }

        /// <summary>
        /// Sets the current element's text.
        /// </summary>
        /// <param name="text">The current element's text.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool setCurrentElementText(NSFString text)
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            currentElement.Text = text;

            return true;
        }

        /// <summary>
        /// Sets the current element's text to the specified value, converted to a string.
        /// </summary>
        /// <param name="value">The current element's value.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool setCurrentElementValue<ValueType>(ValueType value)
        {
            errorStatus = ErrorStatus.NoError;

            if (currentElement == null)
            {
                errorStatus = ErrorStatus.EmptyDocument;
                return false;
            }

            currentElement.Text = value.ToString();

            return true;
        }

        #endregion Public Methods

        #region Private Fields, Events, and Properties

        private NSFXMLElement bookmarkElement;
        private NSFXMLElement currentElement;
        private ErrorStatus errorStatus = ErrorStatus.NoError;
        private NSFXMLElement rootElement;

        #endregion Private Fields, Events, and Properties
    }
}