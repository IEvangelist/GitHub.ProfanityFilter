using IEvangelist.GitHub.Services.Filters;
using IEvangelist.GitHub.Services.Options;
using IEvangelist.GitHub.Services.Providers;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace IEvangelist.GitHub.Tests
{
    public class ProfanityFilterTests
    {
        readonly IProfanityFilter _sut = 
            new ProfanityFilter(
                new WordReplacerProvider(
                    new IWordReplacer[] 
                    {
                        new GitHubEmojiWordReplacer(),
                        new LintLickerWordReplacer() 
                    },
                    Options.Create(new GitHubOptions())));

        [Fact]
        public void CorrectlyReplacesSimpleTextProfanity()
        {
            var input = "fuck this issue";

            Assert.True(_sut.IsProfane(input));

            var actual = _sut.ApplyFilter(input);

            Assert.False(_sut.IsProfane(actual));
            Assert.False(actual.Contains("fuck", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void CorrectlyReplacesMultilineProfanity()
        {
            var input = @"I'm extremely pissed off by this non-sense. All this project ever does
is fuck around and it's bullshit! Seriously, when are you assholes going to get it together?!";

            Assert.True(_sut.IsProfane(input));

            var actual = _sut.ApplyFilter(input);

            Assert.False(_sut.IsProfane(actual));
            Assert.False(actual.Contains("fuck", StringComparison.OrdinalIgnoreCase));
            Assert.False(actual.Contains("bullshit", StringComparison.OrdinalIgnoreCase));
            Assert.False(actual.Contains("assholes", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void CorrectlyReplacesProfanityWithPlaceHolders()
        {
            var input = "What the fuck! I'm baffled by you basterds - you're a sorry s.o.b.";

            Assert.True(_sut.IsProfane(input));

            var actual = _sut.ApplyFilter(input, '*');

            Assert.False(_sut.IsProfane(actual));
            Assert.True(actual.Contains("****", StringComparison.OrdinalIgnoreCase));
            Assert.True(actual.Contains("******", StringComparison.OrdinalIgnoreCase));
            Assert.True(actual.Contains("********", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void CorrectlyReportsFalseToNegativeSentimentText() => 
            Assert.False(
                _sut.IsProfane(
                    "I don't see how to run this tutorial and stitch together a complete system."));
    }
}