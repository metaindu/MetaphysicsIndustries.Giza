
// MetaphysicsIndustries.Giza - A Parsing System
// Copyright (C) 2008-2021 Metaphysics Industries, Inc.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System.Collections.Generic;
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class ImportTest
    {
        [Test]
        public void ImportingFileAddsImportStatementsToCurrentPreGrammar()
        {
            // given
            const string src = @"import 'file1.txt';";
            var ss = new SupergrammarSpanner();
            var errors = new List<Error>();
            // when
            var result = ss.GetGrammar(src, errors);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(0));
            Assert.That(result.ImportStatements.Count, Is.EqualTo(1));
            Assert.That(result.ImportStatements[0].Filename,
                Is.EqualTo("file1.txt"));
            Assert.IsNull(result.ImportStatements[0].ImportRefs);
            Assert.IsTrue(result.ImportStatements[0].ImportAll);
        }

        [Test]
        public void FromStyleImportingOnlyImportsNamedDefinitions()
        {
            // given
            const string src = @"from 'file1.txt' import def1;";
            var ss = new SupergrammarSpanner();
            var errors = new List<Error>();
            // when
            var result = ss.GetGrammar(src, errors);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(0));
            Assert.That(result.ImportStatements.Count, Is.EqualTo(1));
            Assert.That(result.ImportStatements[0].Filename,
                Is.EqualTo("file1.txt"));
            Assert.IsNotNull(result.ImportStatements[0].ImportRefs);
            Assert.That(result.ImportStatements[0].ImportRefs.Length,
                Is.EqualTo(1));
            var importRef = result.ImportStatements[0].ImportRefs[0];
            Assert.That(importRef.SourceName, Is.EqualTo("def1"));
            Assert.That(importRef.DestName, Is.EqualTo("def1"));
            Assert.IsFalse(result.ImportStatements[0].ImportAll);
        }

        [Test]
        public void FromStyleImportMultipleDefs()
        {
            // given
            const string src = @"from 'file1.txt' import def1, def3;";
            var ss = new SupergrammarSpanner();
            var errors = new List<Error>();
            // when
            var result = ss.GetGrammar(src, errors);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(0));
            Assert.That(result.ImportStatements.Count, Is.EqualTo(1));
            Assert.That(result.ImportStatements[0].Filename,
                Is.EqualTo("file1.txt"));
            Assert.IsNotNull(result.ImportStatements[0].ImportRefs);
            Assert.That(result.ImportStatements[0].ImportRefs.Length,
                Is.EqualTo(2));
            var importRef1 = result.ImportStatements[0].ImportRefs[0];
            Assert.That(importRef1.SourceName, Is.EqualTo("def1"));
            Assert.That(importRef1.DestName, Is.EqualTo("def1"));
            var importRef2 = result.ImportStatements[0].ImportRefs[1];
            Assert.That(importRef2.SourceName, Is.EqualTo("def3"));
            Assert.That(importRef2.DestName, Is.EqualTo("def3"));
            Assert.IsFalse(result.ImportStatements[0].ImportAll);
        }

        [Test]
        public void AliasRenamesImportedDefinition()
        {
            // given
            const string src = "from 'file1.txt' import def1;" +
                               "from 'file2.txt' import def1 as def2;";
            var ss = new SupergrammarSpanner();
            var errors = new List<Error>();
            // when
            var result = ss.GetGrammar(src, errors);
            // then
            Assert.That(errors.Count, Is.EqualTo(0));
            Assert.IsNotNull(result);
            Assert.That(result.Definitions.Count, Is.EqualTo(0));
            Assert.That(result.ImportStatements.Count, Is.EqualTo(2));
            var importStmt1 = result.ImportStatements[0];
            Assert.That(importStmt1.Filename, Is.EqualTo("file1.txt"));
            Assert.IsFalse(importStmt1.ImportAll);
            Assert.IsNotNull(importStmt1.ImportRefs);
            Assert.That(importStmt1.ImportRefs.Length, Is.EqualTo(1));
            var importRef1 = importStmt1.ImportRefs[0];
            Assert.That(importRef1.SourceName, Is.EqualTo("def1"));
            Assert.That(importRef1.DestName, Is.EqualTo("def1"));

            var importStmt2 = result.ImportStatements[0];
            Assert.That(importStmt2.Filename, Is.EqualTo("file1.txt"));
            Assert.IsFalse(importStmt2.ImportAll);
            Assert.IsNotNull(importStmt2.ImportRefs);
            Assert.That(importStmt2.ImportRefs.Length, Is.EqualTo(1));
            var importRef2 = importStmt2.ImportRefs[0];
            Assert.That(importRef2.SourceName, Is.EqualTo("def1"));
            Assert.That(importRef2.DestName, Is.EqualTo("def1"));
        }
    }
}
