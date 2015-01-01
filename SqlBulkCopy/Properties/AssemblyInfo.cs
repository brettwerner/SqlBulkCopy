/*
    Copyright © 2014 Brett Werner

    This file is part of SQL Bulk Copy.

    SQL Bulk Copy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SQL Bulk Copy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with SQL Bulk Copy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Reflection;
#if DEBUG
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("SQL Bulk Copy")]
[assembly: AssemblyDescription(".NET wrapper for the ODBC Bulk Copy API")]
[assembly: AssemblyProduct("SqlBulkCopy")]
[assembly: AssemblyCopyright("Copyright (c) 2014 Brett Werner")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#if DEBUG
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2,PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]
[assembly: InternalsVisibleTo("SqlBulkCopyTests,PublicKey=0024000004800000940000000602000000240000525341310004000001000100d56998c7694719680d7d8c4b55c72a358dd66fb93e6a705d04b2ac0732227217983ddfe86fc766dcbeb7022b38949067d902858724e7472c12ce6bb10b3613fe26202762e6d80c652c1deee56f07e14217a3673bf20f74130398ba223d62e4468f001f4a70f2460b877d70d3435624ddac46f4989980591e6b995e06517e0b9e")]
#endif