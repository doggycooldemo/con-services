﻿using VSS.TRex.TAGFiles.Classes.States;
using VSS.TRex.TAGFiles.Types;

namespace VSS.TRex.TAGFiles.Classes.ValueMatcher
{
    /// <summary>
    /// Handles the machine control system research data flag
    /// </summary>
    public class TAGResearchDataValueMatcher : TAGValueMatcher
    {
        public TAGResearchDataValueMatcher()
        {
        }

        private static readonly string[] valueTypes = { TAGValueNames.kTagResearchData };

        public override string[] MatchedValueTypes() => valueTypes;

        public override bool ProcessUnsignedIntegerValue(TAGValueMatcherState state, TAGProcessorStateBase valueSink,
          TAGDictionaryItem valueType, uint value)
        {
            valueSink.ResearchData = value != 0;

            return true;
        }
    }
}
