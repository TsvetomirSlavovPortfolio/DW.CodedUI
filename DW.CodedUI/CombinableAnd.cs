using System.Collections.Generic;

namespace DW.CodedUI
{
    public class CombinableAnd : And
    {
        internal CombinableAnd()
        {
            _conditions = new List<AndCondition>();
        }

        private List<AndCondition> _conditions;
        private uint _timeoutMilliseconds;
        private uint _instanceNumber;

        public CombinableAnd And
        {
            get { return this; }
        }

        public new CombinableAnd Timeout(uint milliseconds)
        {
            _timeoutMilliseconds = milliseconds;
            if (!_conditions.Contains(AndCondition.Timeout))
                _conditions.Add(AndCondition.Timeout);
            return this;
        }

        public new CombinableAnd NoTimeout()
        {
            if (!_conditions.Contains(AndCondition.NoTimeout))
                _conditions.Add(AndCondition.NoTimeout);
            return this;
        }

        public new CombinableAnd Assert()
        {
            if (!_conditions.Contains(AndCondition.Assert))
                _conditions.Add(AndCondition.Assert);
            return this;
        }

        public new CombinableAnd NoAssert()
        {
            if (!_conditions.Contains(AndCondition.NoAssert))
                _conditions.Add(AndCondition.NoAssert);
            return this;
        }

        public new CombinableAnd InstanceNumber(uint instanceNumber)
        {
            _instanceNumber = instanceNumber;
            if (_instanceNumber == 0)
                _conditions.Remove(AndCondition.Instance);
            else if (!_conditions.Contains(AndCondition.Instance))
                _conditions.Add(AndCondition.Instance);
            return this;
        }

        internal override List<AndCondition> GetConditions()
        {
            var conditios = new List<AndCondition>();
            conditios.AddRange(_conditions);

            AdjustTimeoutCondition(conditios);
            AdjustAssertCondition(conditios);

            return conditios;
        }

        internal override uint GetTimeout()
        {
            return _timeoutMilliseconds;
        }
        
        internal override uint GetInstanceNumber()
        {
            return _instanceNumber;
        }

        private void AdjustTimeoutCondition(List<AndCondition> conditios)
        {
            if (!conditios.Contains(AndCondition.NoTimeout) && !conditios.Contains(AndCondition.Timeout))
            {
                conditios.Add(AndCondition.Timeout);
                _timeoutMilliseconds = 10000;
            }
            if (conditios.Contains(AndCondition.NoTimeout))
                conditios.Remove(AndCondition.Timeout);
            if (conditios.Contains(AndCondition.Timeout))
                conditios.Remove(AndCondition.NoTimeout);
        }

        private void AdjustAssertCondition(List<AndCondition> conditios)
        {
            if (!conditios.Contains(AndCondition.NoAssert) && !conditios.Contains(AndCondition.Assert))
                conditios.Add(AndCondition.Assert);
            if (conditios.Contains(AndCondition.NoAssert))
                conditios.Remove(AndCondition.Assert);
            if (conditios.Contains(AndCondition.Assert))
                conditios.Remove(AndCondition.NoAssert);
        }
    }
}