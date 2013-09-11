using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MetaphysicsIndustries.Giza.Test
{
    [TestFixture]
    public class TokenizedGrammarBuilderTest
    {
        [Test]
        public void TestImplicitLiteral()
        {
            // setup
            string testGrammarText = "def = 'value';";
            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
            var explicitDef = grammar.FindDefinitionByName("def");
            var implicitDef = grammar.FindDefinitionByName("$implicit literal value");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(2, grammar.Definitions.Count);

            Assert.IsNotNull(explicitDef);
            Assert.AreEqual(0, explicitDef.Directives.Count);
            Assert.IsFalse(explicitDef.Atomic);
            Assert.IsFalse(explicitDef.IgnoreCase);
            Assert.IsFalse(explicitDef.IsComment);
            Assert.IsFalse(explicitDef.IsTokenized);
            Assert.IsFalse(explicitDef.MindWhitespace);
            Assert.IsNotNull(explicitDef.Nodes);
            Assert.AreEqual(1, explicitDef.Nodes.Count);
            Assert.IsNotNull(explicitDef.StartNodes);
            Assert.AreEqual(1, explicitDef.StartNodes.Count);
            Assert.IsNotNull(explicitDef.EndNodes);
            Assert.AreEqual(1, explicitDef.EndNodes.Count);

            Assert.IsNotNull(implicitDef);
            Assert.AreEqual(2, implicitDef.Directives.Count);
            Assert.Contains(DefinitionDirective.Token, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, implicitDef.Directives.ToArray());
            Assert.IsTrue(implicitDef.Atomic);
            Assert.IsFalse(implicitDef.IgnoreCase);
            Assert.IsFalse(implicitDef.IsComment);
            Assert.IsTrue(implicitDef.IsTokenized);
//            Assert.IsTrue(implicitDef.MindWhitespace);
            Assert.IsNotNull(implicitDef.Nodes);
            Assert.AreEqual(5, implicitDef.Nodes.Count);
            Assert.IsNotNull(implicitDef.StartNodes);
            Assert.AreEqual(1, implicitDef.StartNodes.Count);
            Assert.IsNotNull(implicitDef.EndNodes);
            Assert.AreEqual(1, implicitDef.EndNodes.Count);
        }

        [Test]
        public void TestImplicitCharClass()
        {
            // setup
            string testGrammarText = "def = [\\d];";
            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
            var explicitDef = grammar.FindDefinitionByName("def");
            var implicitDef = grammar.FindDefinitionByName("$implicit char class \\d");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(2, grammar.Definitions.Count);

            Assert.IsNotNull(explicitDef);
            Assert.AreEqual(0, explicitDef.Directives.Count);
            Assert.IsFalse(explicitDef.Atomic);
            Assert.IsFalse(explicitDef.IgnoreCase);
            Assert.IsFalse(explicitDef.IsComment);
            Assert.IsFalse(explicitDef.IsTokenized);
            Assert.IsFalse(explicitDef.MindWhitespace);
            Assert.IsNotNull(explicitDef.Nodes);
            Assert.AreEqual(1, explicitDef.Nodes.Count);
            Assert.IsNotNull(explicitDef.StartNodes);
            Assert.AreEqual(1, explicitDef.StartNodes.Count);
            Assert.IsNotNull(explicitDef.EndNodes);
            Assert.AreEqual(1, explicitDef.EndNodes.Count);

            Assert.IsNotNull(implicitDef);
            Assert.AreEqual(2, implicitDef.Directives.Count);
            Assert.Contains(DefinitionDirective.Token, implicitDef.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, implicitDef.Directives.ToArray());
            Assert.IsTrue(implicitDef.Atomic);
            Assert.IsFalse(implicitDef.IgnoreCase);
            Assert.IsFalse(implicitDef.IsComment);
            Assert.IsTrue(implicitDef.IsTokenized);
//            Assert.IsTrue(implicitDef.MindWhitespace);
            Assert.IsNotNull(implicitDef.Nodes);
            Assert.AreEqual(1, implicitDef.Nodes.Count);
            Assert.IsNotNull(implicitDef.StartNodes);
            Assert.AreEqual(1, implicitDef.StartNodes.Count);
            Assert.IsNotNull(implicitDef.EndNodes);
            Assert.AreEqual(1, implicitDef.EndNodes.Count);
        }
//
//        [Test]
//        public void TestImplicitIgnoreCaseLiteral()
//        {
//        }
//
//        [Test]
//        public void TestImplicitIgnoreCaseCharClass()
//        {
//        }

        [Test]
        public void TestNonTokenWithoutDirectives()
        {
            // setup
            string testGrammarText = "def = token; <token> token = 'token'; ";
            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
            var def = grammar.FindDefinitionByName("def");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(2, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(0, def.Directives.Count);
            Assert.IsFalse(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsFalse(def.IsTokenized);
            Assert.IsFalse(def.MindWhitespace);
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(1, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }

        [Test]
        public void TestTokenDirective()
        {
            // setup
            string testGrammarText = "<token> something = 'value'; ";
            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(2, def.Directives.Count);
            Assert.Contains(DefinitionDirective.Token, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
//            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(5, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }

        [Test]
        public void TestSubtokenDirective()
        {
            // setup
            string testGrammarText = "<subtoken> something = 'value'; ";
            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(2, def.Directives.Count);
            Assert.Contains(DefinitionDirective.Subtoken, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsFalse(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
//            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(5, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }

        [Test]
        public void TestCommentDirective()
        {
            // setup
            string testGrammarText = "<comment> something = 'value'; ";
            var sgs = new SupergrammarSpanner();
            var errors = new List<Error>();
            var dis = sgs.GetExpressions(testGrammarText, errors);
            Assert.IsNotNull(errors);
            Assert.IsEmpty(errors);
            var tgb = new TokenizedGrammarBuilder();


            // action
            var grammar = tgb.BuildTokenizedGrammar(dis);
            var def = grammar.FindDefinitionByName("something");


            // assertions
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Definitions.Count);

            Assert.IsNotNull(def);
            Assert.AreEqual(2, def.Directives.Count);
            Assert.Contains(DefinitionDirective.Comment, def.Directives.ToArray());
            Assert.Contains(DefinitionDirective.Atomic, def.Directives.ToArray());
            Assert.IsTrue(def.Atomic);
            Assert.IsFalse(def.IgnoreCase);
            Assert.IsTrue(def.IsComment);
            Assert.IsTrue(def.IsTokenized);
//            Assert.IsTrue(def.MindWhitespace);
            Assert.IsNotNull(def.Nodes);
            Assert.AreEqual(5, def.Nodes.Count);
            Assert.IsNotNull(def.StartNodes);
            Assert.AreEqual(1, def.StartNodes.Count);
            Assert.IsNotNull(def.EndNodes);
            Assert.AreEqual(1, def.EndNodes.Count);
        }
//
//        [Test]
//        public void TestAtomicDirectiveInNonToken()
//        {
//        }
//
//        [Test]
//        public void TestAtomicDirectiveInToken()
//        {
//        }
//
//        [Test]
//        public void TestAtomicDirectiveInSubtoken()
//        {
//        }
//
//        [Test]
//        public void TestAtomicDirectiveInComment()
//        {
//        }

    }
}

