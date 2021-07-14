using System.Collections.Generic;
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class ImportTest
    {
        // PreGrammar

        [Test]
        public void ImportingFileAddsImportStatementsToCurrentPreGrammar()
        {
            // given
            const string src = @"import 'file1.txt';";
            var ss = new SupergrammarSpanner();
            var errors = new List<Error>();
            // when
            var result = ss.GetPreGrammar(src, errors);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Definitions.Count);
            Assert.AreEqual(1, result.ImportStatements.Count);
            Assert.AreEqual("file1.txt",
                result.ImportStatements[0].ModuleOrFile);
            Assert.IsNull(result.ImportStatements[0].ImportRefs);
            Assert.IsFalse(result.ImportStatements[0].IsModuleImport);
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
            var result = ss.GetPreGrammar(src, errors);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Definitions.Count);
            Assert.AreEqual(1, result.ImportStatements.Count);
            Assert.AreEqual("file1.txt",
                result.ImportStatements[0].ModuleOrFile);
            Assert.IsNotNull(result.ImportStatements[0].ImportRefs);
            Assert.AreEqual(1, result.ImportStatements[0].ImportRefs.Length);
            var importRef = result.ImportStatements[0].ImportRefs[0];
            Assert.AreEqual("def1", importRef.SourceName);
            Assert.AreEqual("def1", importRef.DestName);
            Assert.IsFalse(result.ImportStatements[0].IsModuleImport);
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
            var result = ss.GetPreGrammar(src, errors);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Definitions.Count);
            Assert.AreEqual(1, result.ImportStatements.Count);
            Assert.AreEqual("file1.txt",
                result.ImportStatements[0].ModuleOrFile);
            Assert.IsNotNull(result.ImportStatements[0].ImportRefs);
            Assert.AreEqual(2, result.ImportStatements[0].ImportRefs.Length);
            var importRef1 = result.ImportStatements[0].ImportRefs[0];
            Assert.AreEqual("def1", importRef1.SourceName);
            Assert.AreEqual("def1", importRef1.DestName);
            var importRef2 = result.ImportStatements[0].ImportRefs[1];
            Assert.AreEqual("def3", importRef2.SourceName);
            Assert.AreEqual("def3", importRef2.DestName);
            Assert.IsFalse(result.ImportStatements[0].IsModuleImport);
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
            var result = ss.GetPreGrammar(src, errors);
            // then
            Assert.AreEqual(0, errors.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Definitions.Count);
            Assert.AreEqual(2, result.ImportStatements.Count);
            var importStmt1 = result.ImportStatements[0];
            Assert.IsFalse(importStmt1.IsModuleImport);
            Assert.AreEqual("file1.txt", importStmt1.ModuleOrFile);
            Assert.IsFalse(importStmt1.ImportAll);
            Assert.IsNotNull(importStmt1.ImportRefs);
            Assert.AreEqual(1, importStmt1.ImportRefs.Length);
            var importRef1 = importStmt1.ImportRefs[0];
            Assert.AreEqual("def1", importRef1.SourceName);
            Assert.AreEqual("def1", importRef1.DestName);

            var importStmt2 = result.ImportStatements[0];
            Assert.IsFalse(importStmt2.IsModuleImport);
            Assert.AreEqual("file1.txt", importStmt2.ModuleOrFile);
            Assert.IsFalse(importStmt2.ImportAll);
            Assert.IsNotNull(importStmt2.ImportRefs);
            Assert.AreEqual(1, importStmt2.ImportRefs.Length);
            var importRef2 = importStmt2.ImportRefs[0];
            Assert.AreEqual("def1", importRef2.SourceName);
            Assert.AreEqual("def1", importRef2.DestName);
        }
    }
}
