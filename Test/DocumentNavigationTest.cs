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
using System.Text;
using NorthStateSoftware.NorthStateFramework;

namespace NSFTest
{
    /// <summary>
    /// Test NSFDocument loading and navigation.
    /// </summary>
    public class DocumentNavigationTest : ITestInterface
    {
        #region Constructors
        public DocumentNavigationTest(String name, String fileName)
        {
            Name = name;
            FileName = fileName;
        }

        #endregion Constructors

        public String Name { get; set; }

        public String FileName { get; set; }
        #region Methods

        public bool runTest(ref String errorMessage)
        {
            int testValue = 0;
            NSFXMLDocument document = new NSFXMLDocument();

            if (!document.loadFile(FileName))
            {
                errorMessage = "Error loading file";
                return false;
            }

            document.getChildElementValue<int>("VE_1_1", ref testValue);
            if (testValue != 5)
            {
                errorMessage = "Error reading value VE_1_1";
                return false;
            }

            if (!document.jumpToChildElement("CE_1_1"))
            {
                errorMessage = "Error moving to child element CE_1_1";
                return false;
            }

            if (!document.jumpToChildElement("CE_1_1_1"))
            {
                errorMessage = "Error moving to child element CE_1_1_1";
                return false;
            }

            document.getChildElementValue<int>("VE_1_1_1_1", ref testValue);
            if (testValue != 1)
            {
                errorMessage = "Error reading value VE_1_1_1_1";
                return false;
            }

            if (!document.jumpToParentElement())
            {
                errorMessage = "Error moving back up to parent element CE_1_1_1";
                return false;
            }

            if (!document.jumpToParentElement())
            {
                errorMessage = "Error moving back up to parent element CE_1_1";
                return false;
            }

            document.getChildElementValue<int>("VE_1_1", ref testValue);
            if (testValue != 5)
            {
                errorMessage = "Error reading value VE_1_1 second time";
                return false;
            }

            return true;
        }

        #endregion Methods
    }
}
