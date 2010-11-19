// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringTableStep.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StringTableStep type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class StringTableStep : StringStep
    {
        private readonly Regex _hasParamsInStep = new Regex(@"\[\w+\]");

        private readonly List<Row> _tableSteps = new List<Row>();

        public StringTableStep(string step, string fromFile, IStringStepRunner stringTableStepRunner)
            : base(step, fromFile, stringTableStepRunner)
        {
        }

        public IEnumerable<Row> TableSteps
        {
            get
            {
                return _tableSteps;
            }
        }

        public void AddTableStep(Row row)
        {
            _tableSteps.Add(row);
        }

        public override void Run()
        {
            var actionStepResult = GetNewActionStepResult();
            var hasParamsInStep = HasParametersInStep();
            foreach (var row in _tableSteps)
            {
                StringStep step = this;
                if (hasParamsInStep)
                {
                    step = InsertParametersToStep(row);
                }

                var result = StringStepRunner.Run(step, row);
                actionStepResult.MergeResult(result.Result);
            }

            StepResult = actionStepResult;
        }

        private ActionStepResult GetNewActionStepResult()
        {
            var fullStep = CreateStepText();
            return new ActionStepResult(fullStep, new Passed());
        }

        private string CreateStepText()
        {
            var step = new StringBuilder(Step + Environment.NewLine);
            step.Append(_tableSteps.First().ColumnNamesToString() + Environment.NewLine);
            foreach (var row in _tableSteps)
            {
                step.Append(row.ColumnValuesToString() + Environment.NewLine);
            }

            RemoveLastNewLine(step);
            return step.ToString();
        }

        private void RemoveLastNewLine(StringBuilder step)
        {
            step.Remove(step.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        }

        private bool HasParametersInStep()
        {
            return _hasParamsInStep.IsMatch(Step);
        }

        private StringStep InsertParametersToStep(Row step)
        {
            var stringStep = Step;
            foreach (var column in step.ColumnValues)
            {
                var replceWithValue = new Regex(string.Format(@"\[{0}\]", column.Key), RegexOptions.IgnoreCase);
                stringStep = replceWithValue.Replace(stringStep, column.Value);
            }

            return new StringStep(stringStep, Source, StringStepRunner);
        }
    }
}