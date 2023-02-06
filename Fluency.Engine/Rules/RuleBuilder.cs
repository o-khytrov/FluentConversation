using System.Text.RegularExpressions;
using Fluency.Engine.PatternSystem;
using Fluency.Engine.PatternSystem.Elements;
using Fluency.Engine.Tokenization;

namespace Fluency.Engine.Rules;

public class RuleBuilder<T>
{
    private static readonly Func<BotInput, bool> DefaultCondition = x => true;
    private readonly BotRule<T> _botRule;
    private Topic<T> _topic;


    public RuleBuilder(BotRule<T> botRule, Topic<T> parent)
    {
        _botRule = botRule;
        _topic = parent;
    }

    /// <summary>
    /// Keep the rule after it has been played
    /// </summary>
    /// <param name="keep"></param>
    /// <returns></returns>
    public RuleBuilder<T> Keep(bool keep = true)
    {
        _botRule.Keep = keep;
        return this;
    }

    /// <summary>
    /// Allow same output multiple times
    /// </summary>
    /// <param name="repeat"></param>
    /// <returns></returns>
    public RuleBuilder<T> Repeat(bool repeat = true)
    {
        _botRule.Repeat = repeat;
        return this;
    }

    public RuleBuilder<T> WithResponder(string output)
    {
        return this;
    }

    public RuleBuilder<T> Output(string output)
    {
        _botRule.RenderOutput = (x) => output;
        return this;
    }

    public RuleBuilder<T> WithRegexPattern(string pattern)
    {
        _botRule.Conditions.Add((c, input) =>
        {
            var regex = new Regex(pattern);
            var matches = regex.Matches(input.RawInput);
            return matches.Count > 0;
        });
        return this;
    }

    public RuleBuilder<T> Pattern(Action<PatternBuilder> patternBuilderAction)
    {
        var builder = new PatternBuilder();
        patternBuilderAction.Invoke(builder);
        _botRule.Pattern = builder.Build();
        return this;
    }


    public RuleBuilder<T> WithRegexPattern(string pattern, Action<MatchCollection> action)
    {
        _botRule.Conditions.Add((context, input) =>
        {
            var regex = new Regex(pattern);
            var matches = regex.Matches(input.RawInput);
            var isMatch = matches.Count > 0;
            if (isMatch)
            {
                action(matches);
            }

            return isMatch;
        });
        return this;
    }


    public RuleBuilder<T> When(Func<T, BotInput, bool> condition)
    {
        _botRule.Conditions.Add(condition);
        return this;
    }

    public RuleBuilder<T> Do(Action<BotInput, T> action)
    {
        _botRule.PreActions.Add(action);
        return this;
    }

    /// <summary>
    /// Set post action, which will be executed after if pattern matches
    /// </summary>
    /// <param name="postAction"></param>
    /// <returns></returns>
    public RuleBuilder<T> Then(Action<T, PatternMatchingResult> postAction)
    {
        _botRule.PostActions.Add(postAction);
        return this;
    }

    public RuleBuilder<T> Output(Func<string> outputRenderer)
    {
        return this;
    }

    public RuleBuilder<T> Output(Func<T, string> outputRenderer)
    {
        _botRule.RenderOutput = outputRenderer;
        return this;
    }

    public RuleBuilder<T> WithTest(string input, string expectedResponse)
    {
        _botRule.Tests.Add(new RuleTest() { Input = input, ExpectedResponse = expectedResponse });
        return this;
    }

    public void Rejoinder(Action action)
    {
        var dependencyContainer = new List<BotRule<T>>();
        // Capture any rules added to the parent validator inside this delegate.
        using (_topic.BotRules.Capture(dependencyContainer.Add))
        {
            action();
        }

        _botRule.Rejoinders.AddRange(dependencyContainer);
    }
}