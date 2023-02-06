using Fluency.Engine.Tokenization;

namespace Fluency.Engine.PatternSystem.Elements;

public class Disjunction : PatternElement
{
    private readonly List<Pattern> _patterns;

    public Disjunction(List<Pattern> patterns)
    {
        _patterns = patterns;
    }

    public override bool Match(BotInput input, List<string> extracted, Tokenizer tokenizer)
    {
        foreach (var childPattern in _patterns)
        {
            input.Reset();
            var match = MatchChildPattern(input, childPattern, extracted, tokenizer);
            if (match)
            {
                return true;
            }
        }

        return false;
    }

    private bool MatchChildPattern(BotInput input, Pattern childPattern, List<string> extracted, Tokenizer tokenizer)
    {
        var matched = true;
        foreach (var element in childPattern.Elements)
        {
            var currentRuleMatch = false;
            input.Reset();
            while (!currentRuleMatch && input.CanMoveNext())
            {
                currentRuleMatch = element.Match(input, extracted, tokenizer);
            }

            if (!currentRuleMatch)
            {
                matched = false;
                break;
            }
        }

        return matched;
    }
}