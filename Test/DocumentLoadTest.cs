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
    /// Test loading xml unless there is an exception it will return true
    /// </summary>
    public class DocumentLoadTest : ITestInterface
    {
        #region Constructors
        public DocumentLoadTest(String name, String fileName)
        {
            Name = name;
            FileName = fileName;
        }

        #endregion Constructors

        #region Properties

        public String Name { get; set; }

        public String FileName { get; set; }

        #endregion Properties

        #region Methods

        public bool runTest(ref String errorMessage)
        {
            NSFXMLDocument document = new NSFXMLDocument();

            // As long as there is no exception it is ok
            document.loadFile(FileName);

            return true;
        }

        #endregion Methods
    }
}
