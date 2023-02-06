using Fluency.Engine.Tokenization;

namespace Fluency.Engine.PatternSystem.Elements;

public abstract class PatternElement
{
    public PatternElementType PatternElementType { get; set; }

    public abstract bool Match(BotInput input, List<string> extracted, Tokenizer tokenizer);
}