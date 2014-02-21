using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MetaphysicsIndustries.Giza
{
    public class BaseSpanner
    {
        protected class Backtrack
        {
            public Backtrack(int _i, int _prevState, int[] _nextStates, int _nextStateIndex)
            {
                i = _i;
                currentState = _prevState;
                nextStates = _nextStates;
                nextStateIndex = _nextStateIndex;
            }
            public int i;
            public int currentState;
            public int[] nextStates;
            public int nextStateIndex;
            public List<Span> SubSpans = new List<Span>();
        }

        protected delegate int[] GetValidNextStates(int currentState, char ch, out bool linksToEnd);
        protected delegate string StateTextGetter(int state);
        protected delegate Span SubSpanGetter(string input, ref int i, int nextState);

        protected Span GetItem(
            string input, 
            ref int i, 
            bool ignoreWhitespace, 
            string definitionName, 
            int startState, 
            int endState, 
            GetValidNextStates validNextStatesGetter, 
            //StateTextGetter stateTextGetter, 
            StateTextGetter stateTagGetter, 
            SubSpanGetter subspanGetter)
        {
            int start = i;
            i = start;
            int currentState = startState;
            List<Backtrack> backtracks = new List<Backtrack>();
            List<Span> subspans = new List<Span>();
            int lastValueChar = i;

            for (; i < input.Length; i++)
            {
                if (currentState == endState) break;
                if (ignoreWhitespace && char.IsWhiteSpace(input[i])) continue;

                bool linksToEnd;
                int[] validNextStates;
                Span subspan;

                int nextState = endState;

                do
                {
                    validNextStates = validNextStatesGetter(currentState, input[i], out linksToEnd);

                    nextState = endState;
                    if (validNextStates.Length < 1)
                    {
                        //invalid characters
                        if (linksToEnd)
                        {
                            break;
                        }
                        if (!DoBacktracking(ref i, backtracks, ref currentState, ref nextState))
                        {
                            return null;
                        }

                        //i--;
                        //currentState = nextState;
                        //continue;
                        //break;
                        validNextStates = new int[]{nextState};
                    }
                    else
                    {
                        if (validNextStates.Length > 1)
                        {
                            backtracks.Add(new Backtrack(i, (int)currentState, validNextStates, 0));
                        }

                        nextState = validNextStates[0];
                    }
                } while (validNextStates.Length < 1);
                if (validNextStates.Length < 1 && linksToEnd)
                {
                    break;
                }

                do
                {
                    subspan = subspanGetter(input, ref i, nextState);
                    
                    if (subspan == null)
                    {
                        if (!DoBacktracking(ref i, backtracks, ref currentState, ref nextState))
                        {
                            if (linksToEnd)
                            {
                                break;
                            }
                            return null;
                        }
                    }
                } while (subspan == null);
                if (subspan == null && linksToEnd)
                {
                    break;
                }

                subspan.Definition = definitionName;
                subspan.Tag = stateTagGetter(nextState);

                if (backtracks.Count > 0)
                {
                    backtracks[backtracks.Count - 1].SubSpans.Add(subspan);
                }
                else
                {
                    subspans.Add(subspan);
                }

                currentState = nextState;
                lastValueChar = i;
            }

            foreach (Backtrack bt in backtracks)
            {
                subspans.AddRange(bt.SubSpans);
            }

            i--;

            return new Span(start, lastValueChar - start + 1, input, subspans.ToArray());
        }

        protected bool DoBacktracking(ref int i, List<Backtrack> backtracks, ref int currentState, ref int nextState)
        {
            while (backtracks.Count > 0)
            {
                Backtrack bt = backtracks[backtracks.Count - 1];
                if (bt.nextStateIndex >= bt.nextStates.Length - 1)
                {
                    //backtrack fail
                    //go to the previous backtrack

                    backtracks.RemoveAt(backtracks.Count - 1);
                }
                else
                {
                    i = bt.i;
                    bt.nextStateIndex++;
                    nextState = bt.nextStates[bt.nextStateIndex];
                    currentState = bt.currentState;
                    bt.SubSpans.Clear();

                    break;
                }
            }

            if (backtracks.Count < 1)
            {
                //all backtracks failed
                return false;
            }

            return true;
        }
    }
}
