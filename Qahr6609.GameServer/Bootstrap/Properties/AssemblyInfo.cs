using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("scripthazem")]
[assembly: AssemblyDescription("Conquer Online")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("AymanAlsed")]
[assembly: AssemblyProduct("scripthazem")]
[assembly: AssemblyCopyright("Copyright © scripthazem 2026")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1476c199-9d9e-4a32-bd0f-9fdc3b83f9f9")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
//[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// runtime modules (ServerDataBase\Modules\<Name>\*.cs, compiled at start-up as "QahrModule") may use internal server types,
// like the modules compiled into the server
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("QahrModule")]
