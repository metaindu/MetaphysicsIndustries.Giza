using System;
using System.Collections.Generic;
using System.Linq;
using MetaphysicsIndustries.Collections;

namespace MetaphysicsIndustries.Giza
{
    public class Spanner2 : ParserBase<InputChar>
    {
        public Spanner2(Definition definition)
            : base(definition)
        {
        }

        public Span[] Process(IInputSource<InputChar> input, ICollection<Error> errors)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (errors == null) throw new ArgumentNullException("errors");

            return Parse(input, errors);
        }
        public NodeMatch<InputChar>[] Match(IInputSource<InputChar> input, List<Error> errors, out bool endOfInput, out InputPosition endOfInputPosition, bool mustUseAllInput=true, int startIndex=0)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (errors == null) throw new ArgumentNullException("errors");

            endOfInput = false;
            endOfInputPosition = new InputPosition();

            return Match(input, errors);
        }

        protected override bool IsBranchTip(NodeMatch<InputChar> cur)
        {
            return (cur.Node is CharNode);
        }

        protected override bool BranchTipMatchesInputElement(NodeMatch<InputChar> branchTip, InputChar inputElement)
        {
            return (branchTip.Node as CharNode).Matches(inputElement.Value);
        }

        protected override bool BranchTipIgnoresInputElement(BranchTip<InputChar> branchTip, InputChar inputElement)
        {
            if (char.IsWhiteSpace(inputElement.Value) &&
                !branchTip.Branch.NodeMatch.Node.ParentDefinition.MindWhitespace)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected override void IgnoreInputElementForBranchTip(InputChar inputElement, BranchTip<InputChar> branchTip, BranchTipsByIndexCollection<InputChar> branchTipsByIndex, EndCandidatesByIndexCollection<InputChar> endCandidatesByIndex)
        {
            var nm = branchTip.Branch.NodeMatch;
            var clone = nm.Clone();
            var index = ((IInputElement)inputElement).IndexOfNextElement;
            clone.AlternateStartPosition =
                new InputPosition(
                    index,
                    nm.AlternateStartPosition.Line,
                    nm.AlternateStartPosition.Column
                );
            branchTipsByIndex[index].Enqueue(
                new BranchTip<InputChar> {
                    Branch = pair(clone, branchTip.Branch.MatchStack),
                    Source = branchTip.Source
                }
            );
        }
    }
}

