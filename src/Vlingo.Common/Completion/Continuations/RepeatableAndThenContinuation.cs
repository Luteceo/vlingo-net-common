// Copyright (c) 2012-2020 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Common.Completion.Continuations
{
    internal class RepeatableAndThenContinuation<TAntecedentResult, TResult> : RepeatableCompletes<TResult>
    {
        private readonly AtomicReference<RepeatableCompletes<TAntecedentResult>> antecedent = new AtomicReference<RepeatableCompletes<TAntecedentResult>>(default);

        private RepeatableAndThenContinuation(BasicCompletes parent, RepeatableCompletes<TAntecedentResult> antecedent, Optional<TResult> failedOutcomeValue, Delegate function) : base(function, parent)
        {
            this.antecedent.Set(antecedent);
            FailedOutcomeValue = failedOutcomeValue;
        }
        
        internal RepeatableAndThenContinuation(BasicCompletes parent, RepeatableCompletes<TAntecedentResult> antecedent, Delegate function) : this(parent, antecedent, Optional.Empty<TResult>(), function)
        {
        }

        internal override void InnerInvoke(BasicCompletes completedCompletes)
        {
            if (HasFailedValue.Get())
            {
                return;
            }
            
            base.InnerInvoke(completedCompletes);

            if (Action is Func<TAntecedentResult, ICompletes<TResult>> funcCompletes)
            {
                funcCompletes(antecedent.Get()!.Outcome).AndThenConsume(t =>
                {
                    OutcomeValue.Set(t);
                    TransformedResult = t;
                });
                return;
            }

            if (Action is Func<TAntecedentResult, TResult> function)
            {
                OutcomeValue.Set(function(antecedent.Get()!.Outcome));
                TransformedResult = OutcomeValue.Get();
            }
        }

        internal override void UpdateFailure(BasicCompletes previousContinuation)
        {
            if (previousContinuation.HasFailedValue.Get())
            {
                HasFailedValue.Set(true);
                return;
            }
            
            if (previousContinuation is BasicCompletes<TAntecedentResult> completes)
            {
                if (completes.HasOutcome)
                {
                    HasFailedValue.Set(HasFailedValue.Get() || completes.Outcome!.Equals(FailedOutcomeValue.Get()));  
                }
            }
        }
    }
}