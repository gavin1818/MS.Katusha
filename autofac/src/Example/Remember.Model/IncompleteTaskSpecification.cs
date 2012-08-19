﻿using System;
using System.Linq;

namespace Remember.Model
{
    public class IncompleteTaskSpecification : Specification<Task>
    {
        public override IQueryable<Task> SatisfyingElementsFrom(IQueryable<Task> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates");

            return from task in candidates
                   where !task.IsComplete
                   select task;
        }
    }
}
