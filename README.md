NorthStateFramework-csharp
==========================

The North State Framework (NSF) is an object-oriented framework for implementing UML state machines.  For more information, see the pages <a href="http://northstatesoftware.github.io/NorthStateFramework-csharp/index.html">here.</a>


1. [Compilation](#compilation)
2. [Using NSF](#using-nsf)
3. [Documentation](#documentation)
4. [Contributing](#contributing) 

Compilation
===========

a. Build Requirements
---------------------
* The NSF in C# requires Microsoft .NET 2.0 for compilation.  
* The solution provided is compatible with Visual Studio 9.
* <a href="http://www.doxygen.org/">Doxygen</a> is required to build the documentation. (Optional)

b. Building the Software
------------------------
* Open the file NorthStateFrameworkInC#Development.sln within Visual Studio.
* Choose either Debug or Release build configurations.
* Build the code from the IDE
* Output will be placed in the Build directory
	* Executables will be created for the test project and each of the example projects
	* NorthStateFramework.dll will be created for use in other projects.

c. Building the Documentation
-----------------------------
* If you wish to build the documentation you must first install Doxygen.
* Open the solution
* Right click on the Documentation project and choose build.
* Index.html and other html pages will be generated to the \Documentation\Doxygen\HTML directory.

Using NSF
=========
NSF is a framework contained in a dll for inclusion in projects that require state machine behavior.  To start using  NSF, reference the NorthStateFramework.dll within your project.  See the included examples.  For an in-depth tutorial on using NSF, see the WorkingWithNorthStateFramework document contained in the solution.

Documentation
=============
If you would rather not generate the documentation, you can use the online documentation located at http://northstatesoftware.github.io/NorthStateFramework-csharp/Documentation.

Contributing
============
Before submitting changes to NorthStateFramework-csharp, please review the contribution guidelines at http://northstatesoftware.github.io/NorthStateFramework-csharp/Contributor-FAQ.
